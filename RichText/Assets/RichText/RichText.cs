
/********************************************************************
created:    2017-08-03
author:     lixianmin

*********************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Text;

namespace Unique.UI
{
    /// 已知问题：
    /// 1. 下划线支持: 不支持超链接与下划线混排（顶点数据获取方式）, 换行bound计算问题，下划线颜色问题

    [ExecuteInEditMode]
    public class RichText : Text, IPointerClickHandler
    {
        protected override void OnEnable ()
        {
            base.alignByGeometry = true;
            base.supportRichText = true;

            _Register();

            base.OnEnable();
        }

        private void _Register ()
        {
            if (_spriteManager == null && canvas != null)
            {
                _spriteManager = GetSpriteManager();

                if (_spriteManager == null)
                {
                    Debug.LogError("InlineSpriteAnimManager is miss");
                }
            }

            if (_spriteManager != null)
            {
                _inlineSprite = _spriteManager.GetComponent<InlineSprite>();
                _inlineSprite.SetAllDirty();

                _ParseText();
                SetVerticesDirty();
                _spriteManager.Register(this);
            }
        }

        /// <summary>
        /// 从自身向上查找，表情图片单独渲染，解决层级问题可以通过增加多个管理器解决（不是很好的解决方案）
        /// </summary>
        /// <returns></returns>
        private InlineSpriteManager GetSpriteManager ()
        {
            Transform current = transform.parent;
            while (null != current) 
            {
                InlineSpriteManager temp = current.GetComponentInChildren<InlineSpriteManager> ();  
                if (temp != null) 
                {
                    return temp;
                }

                current = current.parent;
            }

            return null;
        }

        protected override void OnDisable ()
        {
            if (_spriteManager != null)
            {
                _spriteManager.Unregister(this);
            }

            base.OnDisable();
        }

        protected override void OnDestroy ()
        {
            if (_spriteManager != null)
            {
                _spriteManager.Unregister(this);
            }

            base.OnDestroy();
        }

        public override string text
        {
            get
            {
                return base.text;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (string.IsNullOrEmpty(m_Text))
                    {
                        return;
                    }

                    m_Text = string.Empty;
                    _ParseText();
                    SetVerticesDirty();
                }
                else if (m_Text != value)
                {
                    m_Text = value;
                    _ParseText();
                    SetVerticesDirty();
                    SetLayoutDirty();
                }
            }
        }

        public override void SetVerticesDirty ()
        {
            if (!IsActive())
            {
                return;
            }

            // 处理编辑下文本修改问题，默认TextEditor会绕开override的text实现
            if (Application.isEditor)
            {
                _ParseText();
            }

            _DebugLog("Text SetVerticesDirty");
            base.SetVerticesDirty();
        }

        private void _ParseText ()
        {
            Profiler.BeginSample("RichText.ParseText()");
            {
                _parseOutputText = this.text;

                var leftBracketIndex = _parseOutputText.IndexOf ('[');
                if (leftBracketIndex >= 0 && _parseOutputText.IndexOf(']', leftBracketIndex) > 0)
                {
                    _parseOutputText = _ReplaceSimpleSpriteTags(_parseOutputText);
                }

                _parseOutputText = _ParseHrefTags(_parseOutputText);
                _ParseSpriteTags(_parseOutputText);

                _ResetSpriteInfoList(); 
            }

            Profiler.EndSample();
        }

        //计算在顶点中起始和结束位置，考虑<u></u>的影响，其他标签暂且不考虑
        //归根到底是计算文字在顶点数据中位置方式不太靠谱
        protected string _ParseHrefTags (string strText)
        {
            Profiler.BeginSample("RichText._ParseHrefTags()");

            if (string.IsNullOrEmpty(strText) || strText.IndexOf("href") == -1)
            {
                for (int i = 0; i < _hrefTagInfos.Count; ++i)
                {
                    _hrefTagInfos[i].Reset();
                }

                Profiler.EndSample();
                return strText;
            }

            _sbTextBuilder.Length = 0;

            var indexText = 0;
            int index = 0;
            foreach (Match match in _hrefRegex.Matches(strText))
            {
                _sbTextBuilder.Append(strText.Substring(indexText, match.Index - indexText));

                if (index + 1 > _hrefTagInfos.Count)
                {
                    var temp = new HrefTagInfo();
                    _hrefTagInfos.Add(temp);
                }

                HrefTagInfo hrefInfo = _hrefTagInfos[index];

                hrefInfo.StartIndex = _sbTextBuilder.Length;
                hrefInfo.EndIndex = _sbTextBuilder.Length + match.Groups[2].Length;
                hrefInfo.Name = match.Groups[1].Value;

                _sbTextBuilder.Append(match.Groups[2].Value);
                indexText = match.Index + match.Length;
                index ++;
            }
            _sbTextBuilder.Append(strText.Substring(indexText, strText.Length - indexText));

            if (index < _hrefTagInfos.Count)
            {
                int count = _hrefTagInfos.Count;
                for (int i = index; i < count; ++i)
                {
                    _hrefTagInfos[i].Reset();
                }
            }

            Profiler.EndSample();
            return _sbTextBuilder.ToString();
        }

        private string _ReplaceSimpleSpriteTags (string strText)
        {
            Profiler.BeginSample("RichText._ReplaceSimpleSpriteTags()");

            _sbTextBuilder.Length = 0;
            var indexText = 0;
            foreach (Match match in _constSimpleSpriteTagRegex2.Matches(strText))
            {
                _sbTextBuilder.Append(strText.Substring(indexText, match.Index - indexText));
                string strSprite = "<quad name=" + match.Groups[1].ToString().Trim() + " size=" + fontSize + " width=1.2/>";
                _sbTextBuilder.Append(strSprite);
                indexText = match.Index + match.Length;
            }
            _sbTextBuilder.Append(strText.Substring(indexText, strText.Length - indexText));
            Profiler.EndSample();

            return _sbTextBuilder.ToString();
        }

        private void _ParseSpriteTags (string strText)
        {
            if (_inlineSprite == null)
            {
                return;
            }

            if (_animSpiteTagList == null)
            {
                _animSpiteTagList = new List<SpriteTagInfo>();
            }

            if (string.IsNullOrEmpty(strText) || -1 == strText.IndexOf("quad") )
            {
                for (int i = 0; i < _animSpiteTagList.Count; ++i)
                {
                    _animSpiteTagList[i].Reset();
                }
                return;
            }

            int index = 0;
            foreach (Match match in _constSpriteTagRegex.Matches(strText))
            {
                var matchGroups = match.Groups;
                List<string> names = _inlineSprite.GetSpriteNamesFromPrefix(matchGroups[1].Value);
                if (names != null && names.Count > 0)
                {
                    if (index + 1 > _animSpiteTagList.Count)
                    {
                        SpriteTagInfo tempNew = new SpriteTagInfo();
                        _animSpiteTagList.Add(tempNew);
                    }

                    SpriteTagInfo tempArrayTag = _animSpiteTagList[index];
                    tempArrayTag.Key = _GenerateKey(matchGroups[1].Value, index);
                    tempArrayTag.Names = names;
                    tempArrayTag.VertextIndex = match.Index;
                    float size = float.Parse(matchGroups[2].Value);

                    float width = float.Parse(matchGroups[3].Value);
                    float offset = 0.0f;
                    if (width > 1.0f)
                    {
                        offset = (width - 1.0f) / 2.0f;
                    }

                    tempArrayTag.Size   = new Vector2(size, size);
                    tempArrayTag.Offset = offset;

                    index ++;
                }
            }

            if (index < _animSpiteTagList.Count)
            {
                int count = _animSpiteTagList.Count;
                for (int i = index ; i < count; ++i)
                {
                    _animSpiteTagList[i].Reset();
                }
            }
        }

        private void _ResetSpriteInfoList ()
        {
            if (_animSpiteTagList == null || _animSpiteTagList.Count == 0)
            {
                _animSpriteInfoList = null;
                return;
            }

            if (_animSpriteInfoList == null)
            {
                _animSpriteInfoList = new List<SpriteAnimInfo>(2);
            }

            int validCount = 0;
            for (int i = 0; i < _animSpiteTagList.Count; ++ i)
            {
                if (_animSpiteTagList[i].IsValid())
                {
                    validCount++;
                }
            }

            if (validCount > _animSpriteInfoList.Count)
            {
                int needCount = validCount - _animSpriteInfoList.Count;
                for (int i = 0; i <= needCount; ++i)
                {
                    SpriteAnimInfo infos = new SpriteAnimInfo();
                    _animSpriteInfoList.Add(infos);
                }
            }
            else
            {
                for (int i = validCount; i < _animSpriteInfoList.Count; ++i)
                {
                    _animSpriteInfoList[i].Reset();
                }
            }
        }

        private string _GenerateKey(string name, int pos)
        {
            return name + "_" + gameObject.GetInstanceID() + "_" +pos.ToString();
        }

        private void LateUpdate ()
        {
            if (_spriteManager == null)
            {
                _Register();
            }

            if (rectTransform.hasChanged)
            {
                rectTransform.hasChanged = false;
                _UpdateSpritePos();
            }
        }

        protected override void OnPopulateMesh (VertexHelper toFill)
        {
            if (font == null)
            {
                return;
            }

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;

            _textGenerationSettings = GetGenerationSettings(extents);
            cachedTextGenerator.Populate(_parseOutputText, _textGenerationSettings);

            Rect inputRect = rectTransform.rect;

            // get the text alignment anchor point for the text in local space
            Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
            Vector2 refPoint = Vector2.zero;
            refPoint.x = (textAnchorPivot.x == 1 ? inputRect.xMax : inputRect.xMin);
            refPoint.y = (textAnchorPivot.y == 0 ? inputRect.yMin : inputRect.yMax);

            // Determine fraction of pixel to offset text mesh.
            Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;

            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            //Last 4 verts are always a new line...
            int vertCount = verts.Count - 4;

            toFill.Clear();

            _ClearQuadUv( verts);

            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    _tempVerts[tempVertsIndex] = verts[i];
                    _tempVerts[tempVertsIndex].position *= unitsPerPixel;
                    _tempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    _tempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    if (tempVertsIndex == 3)
                    {
                        toFill.AddUIVertexQuad(_tempVerts);
                    }
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    _tempVerts[tempVertsIndex] = verts[i];
                    _tempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                    {
                        toFill.AddUIVertexQuad(_tempVerts);
                    }
                }
            }
            m_DisableFontTextureRebuiltCallback = false;

            _HandleHerfTag(toFill);
            _HandleSpriteTag(toFill);
        }
            
        private void _UpdateSpritePos ()
        {
            if (_spriteVertPositionList != null)
            {
                _CalcQuadTag(_spriteVertPositionList , true);
                if (_spriteManager != null)
                {
                    _spriteManager.UpdatePositon(this, _animSpriteInfoList);
                }
            }
        }
            
        private void _HandleSpriteTag (VertexHelper toFill)
        {
            if (_animSpriteInfoList.IsNullOrEmptyEx() || _animSpiteTagList.IsNullOrEmptyEx())
            {
                return;
            }

            _spriteVertPositionList = _spriteVertPositionList ?? new List<UIVertex>();

            for (int i = 0; i < _animSpiteTagList.Count; i++)
            {
                SpriteTagInfo tempTagInfo = _animSpiteTagList[i];
                if (!tempTagInfo.IsValid())
                {
                    continue;
                }

                int vertexIndex = ((tempTagInfo.VertextIndex + 1) * 4) - 1;
                if (vertexIndex >= toFill.currentVertCount || vertexIndex < 0)
                {
                    continue;
                }

                if (i+1 > _spriteVertPositionList.Count)
                {
                    int needCount = i + 1 - _spriteVertPositionList.Count;
                    for (int m = 0; m < needCount; ++m)
                    {
                        _spriteVertPositionList.Add(new UIVertex());
                    }
                }

                UIVertex tempVer = new UIVertex();
                toFill.PopulateUIVertex(ref tempVer, vertexIndex);
                _spriteVertPositionList[i] = tempVer;
            }

            _CalcQuadTag(_spriteVertPositionList, false);

            if (_spriteManager != null)
            {
                _spriteManager.UpdateSpriteAnimInfos(this, _animSpriteInfoList);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteVerters">图片位置信息</param>
        /// <param name="onlyUpdatePositon">是否只更新位置</param>
        private void _CalcQuadTag (List<UIVertex> spriteVertices , bool onlyUpdatePositon)
        {
            if (_animSpriteInfoList.IsNullOrEmptyEx())
            {
                return;
            }

            if (_animSpiteTagList.IsNullOrEmptyEx())
            {
                return;
            }

            if (spriteVertices.IsNullOrEmptyEx())
            {
                return;
            }

            //canvas scale 
            Vector3 relativePostion = Vector3.zero;
            if (_spriteManager != null)
            {
                relativePostion = transform.position - _spriteManager.transform.position;
                if (canvas != null)
                {
                    Vector3 scale = canvas.transform.localScale;
                    relativePostion = new Vector3(relativePostion.x / scale.x, relativePostion.y / scale.y, relativePostion.z / scale.z);
                }
            }

            for (int i = 0; i < _animSpiteTagList.Count; i++)
            {
                SpriteTagInfo tempTagInfo = _animSpiteTagList[i];
                if (!tempTagInfo.IsValid())
                {
                    continue;
                }

                SpriteAnimInfo animInfo = _animSpriteInfoList[i];

                animInfo.Key =  tempTagInfo.Key;
                animInfo.Names = tempTagInfo.Names;

                if (i >= spriteVertices.Count)
                {
                    //Debug.LogWarning("SpriteAnim Position is less");
                    continue;
                }

                Vector3 textPos = relativePostion + spriteVertices[i].position;
                float xOffset = tempTagInfo.Offset * tempTagInfo.Size.x;

                animInfo.Vertices[0] = new Vector3(xOffset, 0, 0) + textPos;
                animInfo.Vertices[1] = new Vector3(xOffset + tempTagInfo.Size.x , tempTagInfo.Size.y, 0) + textPos;
                animInfo.Vertices[2] = new Vector3(xOffset + tempTagInfo.Size.x , 0, 0) + textPos;
                animInfo.Vertices[3] = new Vector3(xOffset, tempTagInfo.Size.x, 0) + textPos;

                if (onlyUpdatePositon)
                {
                    continue;
                }

                var names = tempTagInfo.Names;
                for (int j = 0; j < names.Count; j++)
                {
                    Rect newSpriteRect ;
                    SpriteAssetInfo tempSpriteAsset = _inlineSprite.GetSpriteInfo(names[j]);
                    if (tempSpriteAsset != null)
                    {
                        newSpriteRect = tempSpriteAsset.rect;
                    }
                    else
                    {
                        newSpriteRect = _inlineSprite.GetSpriteInfo(0).rect;
                        Debug.LogError("CalcQuadTag Can Find Sprite(name=" + tempTagInfo.Key + ")");
                    }
                    animInfo.Uvs[j] = newSpriteRect;
                }
            }

        }

        //UGUIText不支持<quad/>标签，表现为乱码, 将uv全设置为0
        private void _ClearQuadUv (IList<UIVertex> verts)
        {
            if (_animSpiteTagList == null || _animSpiteTagList.Count == 0)
            {
                return;
            }


            UIVertex tempVertex;

            for (int i = 0; i < _animSpiteTagList.Count; i++)
            {
                SpriteTagInfo temp = _animSpiteTagList[i];
                if (!temp.IsValid())
                {
                    continue;
                }

                int startIndex = temp.VertextIndex * 4;
                int endIndex = startIndex +  4;

                for (int m = startIndex; m < endIndex; m++)
                {
                    if (m >= verts.Count)
                    {
                        continue;
                    }

                    tempVertex = verts[m];
                    tempVertex.uv0 = Vector2.zero;
                    verts[m] = tempVertex;
                }
            }
        }

        //根据起始位置获得包围盒
        private List<Rect> _GetBounds (VertexHelper toFill, int vertexStartIndex, int vertexEndIndex)
        {
            List<Rect> boxs = new List<Rect>();
            if (null == toFill)
            {
                return boxs;
            }

            if (vertexStartIndex < 0 || vertexStartIndex >= toFill.currentVertCount)
            {
                return boxs;
            }

            if (vertexEndIndex < 0 || vertexEndIndex >= toFill.currentVertCount)
            {
                return boxs;
            }

            UIVertex vert = new UIVertex();
            toFill.PopulateUIVertex(ref vert, vertexStartIndex);
            var pos = vert.position;
            var bounds = new Bounds(pos, Vector3.zero);
            for (int i = vertexStartIndex, m = vertexEndIndex; i < m; i++)
            {
                if (i >= toFill.currentVertCount)
                {
                    break;
                }

                toFill.PopulateUIVertex(ref vert, i);
                pos = vert.position;
                if (pos.x < bounds.min.x)      // 换行重新添加包围框     todo
                {
                    boxs.Add(new Rect(bounds.min, bounds.size));
                    bounds = new Bounds(pos, Vector3.zero);
                }
                else                          //扩展包围盒
                {
                    bounds.Encapsulate(pos);
                }
            }

            boxs.Add(new Rect(bounds.min, bounds.size));
            return boxs;
        }

        private void _HandleHerfTag (VertexHelper toFill)
        {
            if (_hrefTagInfos.IsNullOrEmptyEx())
            {
                return;
            }

            var count = _hrefTagInfos.Count;
            for(int i = 0 ; i < count ; ++i)
            {
                HrefTagInfo temp = _hrefTagInfos[i];
                if (!temp.IsValid())
                {
                    continue;
                }

                int vertexStart = temp.StartIndex * 4;
                int vertexEnd = (temp.EndIndex - 1) * 4 + 3;

                _hrefTagInfos[i].Boxes = _GetBounds(toFill, vertexStart, vertexEnd);
            }
        }

        /// 点击事件检测是否点击到超链接文本
        public void OnPointerClick (PointerEventData eventData)
        {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out lp);

            foreach (var hrefInfo in _hrefTagInfos)
            {
                var boxes = hrefInfo.Boxes;
                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(lp))
                    {
                        _onHrefClick.Invoke(hrefInfo.Name);
                        return;
                    }
                }
            }
        }

        [System.Serializable]
        public class SpriteTagInfo
        {
            public string Key;
            public List<string> Names; 
            public int VertextIndex;
            public Vector2 Size;
            public float Offset;

            public void Reset()
            {
                Key = string.Empty;
            }

            public SpriteTagInfo()
            {
                Names = new List<string>();
            }

            public bool IsValid()
            {
                return !string.IsNullOrEmpty(Key);
            }
        }

        /// <summary>
        /// 超链接信息类
        /// </summary>
        private class HrefTagInfo
        {
            public int StartIndex;

            public int EndIndex;

            public string Name;

            public void Reset()
            {
                StartIndex = -1;
                EndIndex = -1;
            }

            public bool IsValid ()
            {
                return StartIndex != -1 && EndIndex != -1;
            }

            public  List<Rect> Boxes = new List<Rect>();
        }

        [System.Serializable]
        public class HrefClickEvent : UnityEvent<string>
        {
            
        }

        private void _DebugLog (string format, params object[] args)
        {
//            Debug.LogFormat(format, args);
        }

        private List<SpriteAnimInfo> _animSpriteInfoList;
        public List<SpriteAnimInfo> AnimSpriteInfoList
        {
            get { return _animSpriteInfoList; }
        }

        /// <summary>
        /// 可通过外部设置避免查找
        /// </summary>
        private InlineSpriteManager _spriteManager;
        public InlineSpriteManager SpriteManager
        {
            get { return _spriteManager; }
            set { _spriteManager = value; }
        }

        private HrefClickEvent _onHrefClick = new HrefClickEvent();
        public HrefClickEvent onHrefClick
        {
            get { return _onHrefClick; }
            set { _onHrefClick = value; }
        }

        private readonly UIVertex[] _tempVerts = new UIVertex[4];

        private TextGenerationSettings _textGenerationSettings;

        private InlineSprite _inlineSprite;
        private List<SpriteTagInfo> _animSpiteTagList;
        private List<UIVertex> _spriteVertPositionList;

        private string _parseOutputText;

        private readonly List<HrefTagInfo> _hrefTagInfos = new List<HrefTagInfo>();

        private static readonly StringBuilder _sbTextBuilder = new StringBuilder();

        private static readonly Regex _constSpriteTagRegex = new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?)\s*/>", RegexOptions.Singleline);
        private static readonly Regex _constSimpleSpriteTagRegex2 = new Regex(@"\[(.+?)\]", RegexOptions.Singleline);
        private static readonly Regex _hrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);
//        private static readonly Regex _underlineRegex = new Regex(@"<u>(.+?)</u>", RegexOptions.Singleline);
    }

}
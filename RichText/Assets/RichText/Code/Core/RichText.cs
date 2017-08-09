
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

namespace Unique.RichText
{
    /// 已知问题：
    /// 1. 下划线支持: 不支持超链接与下划线混排（顶点数据获取方式）, 换行bound计算问题，下划线颜色问题

    [ExecuteInEditMode]
    public partial class RichText : Text, IPointerClickHandler
    {
        [System.Serializable]
        public class HrefClickEvent : UnityEvent<string>
        {

        }

        protected override void OnEnable ()
        {
            base.alignByGeometry = true;
            base.supportRichText = true;

            _ParseText();
            SetVerticesDirty();

            base.OnEnable();
        }

        protected override void OnDisable ()
        {
            base.OnDisable();
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

            base.SetVerticesDirty();
        }

        private void _ParseText ()
        {
            _parseOutputText = this.text;

            var leftBracketIndex = _parseOutputText.IndexOf ('[');
            if (leftBracketIndex >= 0 && _parseOutputText.IndexOf(']', leftBracketIndex) > 0)
            {
                _parseOutputText = _ReplaceSimpleSpriteTags(_parseOutputText);
            }

            _parseOutputText = _ParseHrefTags(_parseOutputText);
            _ParseSpriteTags(_parseOutputText);
        }

        //计算在顶点中起始和结束位置，考虑<u></u>的影响，其他标签暂且不考虑
        //归根到底是计算文字在顶点数据中位置方式不太靠谱
        protected string _ParseHrefTags (string strText)
        {
            if (string.IsNullOrEmpty(strText) || strText.IndexOf("href") == -1)
            {
                for (int i = 0; i < _hrefTagInfos.Count; ++i)
                {
                    _hrefTagInfos[i].Reset();
                }

                return strText;
            }

            _sbTextBuilder.Length = 0;

            var indexText = 0;
            int index = 0;
            foreach (Match match in HrefTag.GetTextMatches(strText))
            {
                var notMatchText = strText.Substring(indexText, match.Index - indexText);
                _sbTextBuilder.Append(notMatchText);

                _hrefTagInfos.EnsureSizeEx(index+1);
                HrefTag hrefInfo = _hrefTagInfos[index] ?? (_hrefTagInfos[index] = new HrefTag());

                var groups = match.Groups;
                hrefInfo.StartIndex = _sbTextBuilder.Length;
                hrefInfo.EndIndex = _sbTextBuilder.Length + groups[2].Length;
                hrefInfo.Name = groups[1].Value;

                _sbTextBuilder.Append(groups[2].Value);
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
            
        private void LateUpdate ()
        {
            if (rectTransform.hasChanged)
            {
                rectTransform.hasChanged = false;
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh (VertexHelper toFill)
        {
            if (null == font)
            {
                return;
            }

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;

            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.Populate(_parseOutputText, settings);

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

            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    _tempVerts[tempVertsIndex] = verts[i];
                    _tempVerts[tempVertsIndex].position *= unitsPerPixel;
                    _tempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    _tempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    _tempVerts[tempVertsIndex].uv1 = new Vector2(1.0f, 0);

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
                    _tempVerts[tempVertsIndex].uv1 = new Vector2(1.0f, 0);

                    if (tempVertsIndex == 3)
                    {
                        toFill.AddUIVertexQuad(_tempVerts);
                    }
                }
            }

            _HandleHrefTag(toFill);
            _HandleSpriteTag(toFill);
            m_DisableFontTextureRebuiltCallback = false;
        }
       
        private void _HandleHrefTag (VertexHelper toFill)
        {
            if (_hrefTagInfos.IsNullOrEmptyEx())
            {
                return;
            }

            var count = _hrefTagInfos.Count;
            for(int i = 0 ; i < count ; ++i)
            {
                HrefTag hrefTag = _hrefTagInfos[i];
                if (!hrefTag.IsValid())
                {
                    continue;
                }

                int vertexStart = hrefTag.StartIndex * 4;
                int vertexEnd = (hrefTag.EndIndex - 1) * 4 + 3;

                _hrefTagInfos[i].CalcBounds(toFill, vertexStart, vertexEnd);
            }
        }

        // 点击事件检测是否点击到超链接文本
        public void OnPointerClick (PointerEventData eventData)
        {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out lp);

            foreach (var hrefInfo in _hrefTagInfos)
            {
                if (hrefInfo.HitTest(lp))
                {
                    _onHrefClick.Invoke(hrefInfo.Name);
                    return;
                }
            }
        }

        public void SetSpriteAsset (SpriteAsset spriteAsset)
        {
            _spriteAsset = spriteAsset;
        }

        private HrefClickEvent _onHrefClick = new HrefClickEvent();
        public HrefClickEvent onHrefClick
        {
            get { return _onHrefClick; }
            set { _onHrefClick = value; }
        }

        private readonly UIVertex[] _tempVerts = new UIVertex[4];
        private SpriteAsset _spriteAsset;
        private string _parseOutputText;

        private readonly List<HrefTag> _hrefTagInfos = new List<HrefTag>();

        private static readonly StringBuilder _sbTextBuilder = new StringBuilder();

        private static readonly Regex _constSimpleSpriteTagRegex2 = new Regex(@"\[(.+?)\]", RegexOptions.Singleline);
//        private static readonly Regex _underlineRegex = new Regex(@"<u>(.+?)</u>", RegexOptions.Singleline);
    }

}
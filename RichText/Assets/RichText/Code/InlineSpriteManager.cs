
/********************************************************************
created:    2017-08-03
author:     lixianmin

*********************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.Match;

namespace Unique.UI
{

    /// <summary>
    /// 表情渲染管理器，定时更新表情数据并绘制
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(InlineSprite))]
    public class InlineSpriteManager : MonoBehaviour
    {
        public void Register (RichText richText)
        {
            if (null == richText)
            {
                return;
            }

            int id = richText.GetInstanceID();
            _activeTextDict[id] = richText;
        }

        public void Unregister (RichText richText)
        {
            if (null == richText)
            {
                return;
            }

            _RemoveSpriteAnimInfos(richText);

            int id = richText.GetInstanceID();
            _activeTextDict.Remove(id);
        }

        private void OnEnable ()
        {
            _totalSpriteAnimDict.Clear();
            _textSpriteAnimKeysDict.Clear();
        }

        private void OnDisable ()
        {
            _textSpriteAnimKeysDict.Clear();
            _activeTextDict.Clear();
            _totalSpriteAnimDict.Clear();
        }

        private void OnDestroy ()
        {
            _tempVertices = null;
            _tempUv = null;
            _tempTriangles = null;
        }

        private void _RemoveSpriteAnimInfos (RichText richText)
        {
            if (richText == null)
            {
                return;
            }

            int id = richText.GetInstanceID();
            if (!_textSpriteAnimKeysDict.ContainsKey(id))
            {
                return;
            }

            int count = _totalSpriteAnimDict.Count;
            List<string> spriteAnimKeys = _textSpriteAnimKeysDict[id];
            for (int i = 0; i < spriteAnimKeys.Count; ++i)
            {
                var animKey = spriteAnimKeys[i];
                _totalSpriteAnimDict.Remove(animKey);
            }
            _textSpriteAnimKeysDict.Remove(id);

            if (count != _totalSpriteAnimDict.Count)
            {
                _UpdateMeshCapacity();
            }
        }

        public void UpdateSpriteAnimInfos (RichText richText, List<SpriteAnimInfo> inputSpriteAnimInfos)
        {
            Profiler.BeginSample("inlineSpriteManager UpdateSpriteAnimInfos ");

            if (richText == null)
            {
                return;
            }

            bool isUpdateMeshData = false;

            int id = richText.GetInstanceID();
            List<string> oldSpriteKeys= null;
            _textSpriteAnimKeysDict.TryGetValue(id, out oldSpriteKeys);

            // input is null
            if (inputSpriteAnimInfos == null)
            {
                if (oldSpriteKeys != null)
                {
                    for (int i = 0; i < oldSpriteKeys.Count; ++i)
                    {
                        _totalSpriteAnimDict.Remove(oldSpriteKeys[i]);
                    }
                    _textSpriteAnimKeysDict.Remove(id);
                    isUpdateMeshData = true;
                }
            }
            else
            {
                int oldCount = _totalSpriteAnimDict.Count;
                if (oldSpriteKeys != null)
                {
                    for (int i = 0; i < oldSpriteKeys.Count; ++i)
                    {
                        _totalSpriteAnimDict.Remove(oldSpriteKeys[i]);
                    }
                }

                List<string> keys = new List<string>();
                for (int i = 0; i < inputSpriteAnimInfos.Count; ++i)
                {
                    SpriteAnimInfo animInfo = inputSpriteAnimInfos[i];
                    if (animInfo != null && animInfo.IsValid())
                    {
                        _totalSpriteAnimDict[animInfo.Key] = animInfo;
                        keys.Add(animInfo.Key);
                    }
                }

                if (keys.Count > 0)
                {
                    _textSpriteAnimKeysDict[id] = keys;
                }
                else
                {
                    if (oldSpriteKeys != null)
                    {
                        _textSpriteAnimKeysDict.Remove(id);
                    }
                }

                if (oldCount != _totalSpriteAnimDict.Count)
                {
                    isUpdateMeshData = true;
                }
            }

            if (isUpdateMeshData)
            {
                _UpdateMeshCapacity();
            }

            Profiler.EndSample ();
        }

        public void UpdatePositon (RichText richText, List<SpriteAnimInfo> inputSpriteAnimInfos)
        {
            UpdateSpriteAnimInfos(richText, inputSpriteAnimInfos);
            _DrawSprite();
        }

        private void LateUpdate ()
        {
            if (_totalSpriteAnimDict == null)
            {
                return;
            }

            var iter = _totalSpriteAnimDict.GetEnumerator();
            while (iter.MoveNext())
            {
                var pair = iter.Current;
                SpriteAnimInfo temp = pair.Value;
                if (!temp.IsValid())
                {
                    continue;
                }

                temp.RuningTime += Time.deltaTime;
                if (temp.RuningTime >= mSpriteAnimTimeGap)
                {
                    temp.RuningTime = 0;
                    temp.Currnt++;

                    if (temp.Currnt >= temp.Names.Count)
                    {
                        temp.Currnt = 0;
                    }
                }
            }

            _DrawSprite();
        }

        //TODO 分配策略 在文本修改比较多的情况下 分配过于频繁
        private void _UpdateMeshCapacity ()
        {
            Profiler.BeginSample("inlineSpriteManager UpdateMeshCapacity ");

            if (_totalSpriteAnimDict == null || _totalSpriteAnimDict.Count == 0)
            {
                _tempVertices = null;
                _tempUv = null;
                _tempTriangles = null;
                return;
            }
     
            int count = _totalSpriteAnimDict.Count;
            _tempUv = _tempUv.SetCapacityEx(count * 4);
            _tempVertices = _tempVertices.SetCapacityEx(count * 4);
            _tempTriangles = _tempTriangles.SetCapacityEx(count * 6);

            Profiler.EndSample ();
        }

        private void _DrawSprite ()
        {
            Profiler.BeginSample("inline SpriteManager DrawSprite");

            if (_totalSpriteAnimDict.Count == 0)
            {
                _UpdateMesh();
                return;
            }

            int index = 0;
            var iter = _totalSpriteAnimDict.GetEnumerator();
            while (iter.MoveNext())
            {
                var pair = iter.Current;
                SpriteAnimInfo animInfo = pair.Value;
                if (animInfo == null || !animInfo.IsValid())
                {
                    continue;
                }

                if (animInfo.Vertices == null)
                {
                    continue;
                }

                Array.Copy(animInfo.Vertices, 0, _tempVertices, index * 4, animInfo.Vertices.Length);
                animInfo.GetUV(ref _tempUv , index * 4);

                int startIndex = index * 6;
                _tempTriangles[startIndex + 0] = 0 + 4 * index;
                _tempTriangles[startIndex + 1] = 1 + 4 * index;
                _tempTriangles[startIndex + 2] = 2 + 4 * index;

                _tempTriangles[startIndex + 3] = 1 + 4 * index;
                _tempTriangles[startIndex + 4] = 0 + 4 * index;
                _tempTriangles[startIndex + 5] = 3 + 4 * index;

                index++;
            }

            Profiler.EndSample ();

            _UpdateMesh();
        }

        private void _UpdateMesh ()
        {
            Profiler.BeginSample("inline SpriteManager DrawSprite UpdateMesh");

            var mesh = new Mesh ();
            mesh.vertices = _tempVertices;
            mesh.uv = _tempUv;
            mesh.triangles = _tempTriangles;
            GetComponent<CanvasRenderer>().SetMesh(mesh);
            GetComponent<InlineSprite>().UpdateMaterial();

            Profiler.EndSample();
        }

        /// <summary>
        /// 所有动画数据，使用前检查Key是否有效
        /// </summary>
        private Dictionary<string, SpriteAnimInfo> _totalSpriteAnimDict = new Dictionary<string, SpriteAnimInfo>();

        /// <summary>
        /// Text对应的表情动画Key值
        /// </summary>
        private Dictionary<int, List<string>> _textSpriteAnimKeysDict = new Dictionary<int, List<string>>();

        /// <summary>
        /// 当前激活中的Text
        /// </summary>
        private Dictionary<int, RichText> _activeTextDict = new Dictionary<int, RichText>();

        private readonly float mSpriteAnimTimeGap = 0.2f;

        //Mesh Data Cache
        private Vector3[] _tempVertices;
        private Vector2[] _tempUv;
        private int[] _tempTriangles;

    }
}
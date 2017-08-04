
/********************************************************************
created:    2017-08-04
author:     lixianmin

*********************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unique.UI
{
    [System.Serializable]
    public class SpriteTagInfo
    {
        public SpriteTagInfo ()
        {
            
        }

        public void PopulateUIVertex (VertexHelper toFill)
        {
            if (null == toFill || !IsValid())
            {
                return;
            }

            int vertexIndex = ((_vertexIndex + 1) * 4) - 1;
            if (vertexIndex >= 0 && vertexIndex < toFill.currentVertCount)
            {
                toFill.PopulateUIVertex(ref _vertex, vertexIndex);
            }
        }

        public void ClearQuadUV (IList<UIVertex> vertices)
        {
            if (null == vertices || !IsValid())
            {
                return;
            }

            int startIndex = _vertexIndex * 4;
            int endIndex = startIndex +  4;

            for (int i = startIndex; i < endIndex; i++)
            {
                if (i >= vertices.Count)
                {
                    continue;
                }

                var tempVertex = vertices[i];
                tempVertex.uv0 = Vector2.zero;
                vertices[i] = tempVertex;
            }
        }

        public void Reset ()
        {
            Key = string.Empty;
        }

        public bool IsValid ()
        {
            return !string.IsNullOrEmpty(Key);
        }

        public UIVertex GetUIVertex ()
        {
            return _vertex;
        }

        public void SetVertexIndex (int index)
        {
            _vertexIndex = index;
        }

        public void SetName (string name)
        {
            _name = name;
        }

        public string GetName ()
        {
            return _name;
        }

        public string Key;
        public Vector2 Size;
        public float Offset;

        private string _name;

        private int _vertexIndex;
        private UIVertex _vertex;
    }
}
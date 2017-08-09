
/********************************************************************
created:    2017-08-04
author:     lixianmin

*********************************************************************/

using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unique.RichText
{
    /// <summary>
    /// 超链接信息类
    /// </summary>
    internal class HrefTag
    {
        public static MatchCollection GetTextMatches (string text)
        {
            return _hrefRegex.Matches(text);
        }

        // 根据起始位置获得包围盒
        public void CalcBounds (VertexHelper toFill, int vertexStartIndex, int vertexEndIndex)
        {
            if (null == toFill)
            {
                return;
            }

            if (vertexStartIndex < 0 || vertexStartIndex >= toFill.currentVertCount)
            {
                return;
            }

            if (vertexEndIndex < vertexStartIndex || vertexEndIndex >= toFill.currentVertCount)
            {
                return;
            }

            List<Rect> boxs = _boxes;
            boxs.Clear();

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
        }

        public bool HitTest (Vector2 lp)
        {
            var boxes = _boxes;
            for (var i = 0; i < boxes.Count; ++i)
            {
                if (boxes[i].Contains(lp))
                {
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            StartIndex = -1;
            EndIndex = -1;
        }

        public bool IsValid ()
        {
            return StartIndex != -1 && EndIndex != -1;
        }

        public int StartIndex;

        public int EndIndex;

        public string Name;

        private readonly List<Rect> _boxes = new List<Rect>();

        private static readonly Regex _hrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);
    }
}
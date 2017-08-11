
/********************************************************************
created:    2017-08-08
author:     lixianmin

*********************************************************************/

using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Unique.RichText
{
    partial class RichText
    {
        private void _ParseSpriteTags (string strText)
        {
            if (string.IsNullOrEmpty(strText) || -1 == strText.IndexOf("quad"))
            {
                _ResetSpriteTags(0);
                return;
            }

            int index = 0;
            foreach (Match match in SpriteTag.GetMatches(strText))
            {
                SpriteTag tag = _GetSpriteTag(index);
                var isOk = tag.SetValue(match);
                if (isOk)
                {
                    ++index;
                }
            }

            _ResetSpriteTags(index);
        }

        private void _HandleSpriteTag (VertexHelper toFill)
        {
            if (null == _spriteData)
            {
                return;
            }

            var spriteTags = _spriteTags;
            var count = spriteTags.Count;
            for (int i = 0; i < count; i++)
            {
                SpriteTag tag = spriteTags[i];
                var name = tag.GetName();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                SpriteItem spriteItem = _spriteData.GetSpriteItem(name);
                if (null == spriteItem)
                {
                    continue;
                }

                UIVertex v = new UIVertex();
                var vertexIndex = tag.GetVertexIndex() * 4;
                var fetchIndex = vertexIndex + 3;
                if (fetchIndex >= toFill.currentVertCount)
                {
                    continue;
                }

                toFill.PopulateUIVertex(ref v, fetchIndex);

                Vector3 textPos = v.position;
                var tagSize = tag.GetSize();
                float xOffset   = tag.GetOffset() * tagSize;
                var rect = spriteItem.rect;

                // pos = (0, 0)
                var position = new Vector3(xOffset, 0, 0) + textPos;
                var uv0 = new Vector2(rect.x, rect.y);
                _SetSpriteVertex(toFill, vertexIndex, position, uv0);

                // pos = (1, 0)
                position = new Vector3(xOffset + tagSize , 0, 0) + textPos;
                uv0 = new Vector2(rect.x + rect.width, rect.y);
                _SetSpriteVertex(toFill, ++vertexIndex, position, uv0);

                // pos = (1, 1)
                position = new Vector3(xOffset + tagSize , tagSize, 0) + textPos;
                uv0 = new Vector2(rect.x + rect.width , rect.y + rect.height);
                _SetSpriteVertex(toFill, ++vertexIndex, position, uv0);

                // pos = (0, 1)
                position = new Vector3(xOffset, tagSize, 0) + textPos;
                uv0 = new Vector2(rect.x, rect.y + rect.height);
                _SetSpriteVertex(toFill, ++vertexIndex, position, uv0);
            }
        }

        private void _SetSpriteVertex (VertexHelper toFill, int vertexIndex, Vector3 position, Vector2 uv0)
        {
            UIVertex v = new UIVertex();
            toFill.PopulateUIVertex(ref v, vertexIndex);
            v.position = position;
            v.uv0 = uv0;
            v.uv1 = new Vector2(0, 1.0f);
            toFill.SetUIVertex(v, vertexIndex);
        }

        private SpriteTag _GetSpriteTag (int index)
        {
            if (index >= 0)
            {
                _spriteTags.EnsureSizeEx(index + 1);
                SpriteTag tag = _spriteTags[index] ?? (_spriteTags[index] = new SpriteTag());
                return tag;
            }

            return null;
        }

        private void _ResetSpriteTags (int startIndex)
        {
            var count = _spriteTags.Count;
            for (int i= startIndex; i< count; ++i)
            {
                var tag = _spriteTags[i];
                tag.Reset();
            }
        }

        public IList<SpriteTag> GetSpriteTags ()
        {
            return _spriteTags;
        }

        private readonly List<SpriteTag> _spriteTags = new List<SpriteTag>();
    }
}
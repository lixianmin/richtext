
/********************************************************************
created:    2017-08-08
author:     lixianmin

*********************************************************************/

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
                SpriteTagManger.Instance.Reset(0);
                return;
            }

            int index = 0;
            foreach (Match match in _constSpriteTagRegex.Matches(strText))
            {
                var groups = match.Groups;
                var name = groups[1].Value;

                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                SpriteTag tag = SpriteTagManger.Instance.GetTag(index);

                tag.SetName(name);
                tag.SetVertexIndex(match.Index);
                float size = float.Parse(groups[2].Value);
                float width = float.Parse(groups[3].Value);

                float offset = 0.0f;
                if (width > 1.0f)
                {
                    offset = (width - 1.0f) * 0.5f;
                }

                tag.Size   = new Vector2(size, size);
                tag.Offset = offset;

                ++index;
            }

            SpriteTagManger.Instance.Reset(index);
        }

        private void _HandleSpriteTag (VertexHelper toFill)
        {
            if (null == _spriteAsset)
            {
                return;
            }

            var spriteTags = SpriteTagManger.Instance.GetTags();
            var count = spriteTags.Count;
            for (int i = 0; i < count; i++)
            {
                SpriteTag tag = spriteTags[i];
                var name = tag.GetName();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                SpriteItemAsset spriteItem = _spriteAsset.GetSpriteItem(name);
                if (null == spriteItem)
                {
                    continue;
                }

                UIVertex v = new UIVertex();
                var vertexIndex = tag.GetVertexIndex() * 4;
                toFill.PopulateUIVertex(ref v, vertexIndex + 3);
                Vector3 textPos = v.position;
                float xOffset   = tag.Offset * tag.Size.x;
                var rect = spriteItem.rect;

                var position = new Vector3(xOffset, 0, 0) + textPos;
                var uv0 = new Vector2(rect.x, rect.y);
                _SetSpriteVertex(toFill, vertexIndex, position, uv0);

                position = new Vector3(xOffset + tag.Size.x , 0, 0) + textPos;
                uv0 = new Vector2(rect.x + rect.width, rect.y);
                _SetSpriteVertex(toFill, ++vertexIndex, position, uv0);

                position = new Vector3(xOffset + tag.Size.x , tag.Size.y, 0) + textPos;
                uv0 = new Vector2(rect.x + rect.width , rect.y + rect.height);
                _SetSpriteVertex(toFill, ++vertexIndex, position, uv0);

                position = new Vector3(xOffset, tag.Size.y, 0) + textPos;
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

        private static readonly Regex _constSpriteTagRegex = new Regex(@"<quad name=(.+?)\s+size=(\d*\.?\d+%?)\s+width=(\d*\.?\d+%?)\s*/>", RegexOptions.Singleline);
    }
}
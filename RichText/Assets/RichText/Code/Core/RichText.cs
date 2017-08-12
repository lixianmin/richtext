
/********************************************************************
created:    2017-08-03
author:     lixianmin

*********************************************************************/

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Unique.RichText
{
    [ExecuteInEditMode]
    public partial class RichText : Text
    {
        protected override void OnEnable ()
        {
            this.supportRichText = true;

            _ParseText();
            SetVerticesDirty();

            base.OnEnable();
        }

        protected override void OnDisable ()
        {
            base.OnDisable();
        }

        protected override void OnDestroy ()
        {
            var manager = MaterialManager.Instance;
            var lastSpriteTexture = manager.GetSpriteTexture(material);
            manager.DetachTexture(this, lastSpriteTexture);

            base.OnDestroy();
        }

        public override string text
        {
            get
            {
                return m_Text;
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

        private void _ParseText ()
        {
            _parseOutputText = text;
            _ParseSpriteTags(_parseOutputText);
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

            _HandleSpriteTag(toFill);
            m_DisableFontTextureRebuiltCallback = false;
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!IsActive())
            {
                return;
            }

            _ParseText();
        }
        #endif

        private readonly UIVertex[] _tempVerts = new UIVertex[4];
        private string _parseOutputText;
    }
}
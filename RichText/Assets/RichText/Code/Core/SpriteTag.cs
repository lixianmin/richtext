
/********************************************************************
created:    2017-08-04
author:     lixianmin

*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace Unique.UI.RichText
{
    [System.Serializable]
    public class SpriteTag
    {
        public enum FillMethod
        {
            None,
            Horizontal,
//            Vertical,
        }

        public SpriteTag (RichText richText)
        {
            if (null == richText)
            {
                throw new ArgumentNullException("richText is null.");
            }

            _richText = richText;
        }

        public static MatchCollection GetMatches (string strText)
        {
            return _spriteTagRegex.Matches(strText);
        }

        public bool SetValue (Match match)
        {
            if (null == match)
            {
                return false;
            }

            var keyCaptures = match.Groups[1].Captures;
            var valCaptures = match.Groups[2].Captures;

            var count = keyCaptures.Count;
            if (count != valCaptures.Count)
            {
                return false;
            }

            for (int i = 0; i < keyCaptures.Count; ++i)
            {
                var key = keyCaptures[i].Value;
                var val = valCaptures[i].Value;
                _CheckSetValue(match, key, val);
            }

            return true;
        }

        private void _CheckSetValue (Match match, string key, string val)
        {
            if (key == "name")
            {
                SetName(val);
                _vertexIndex = match.Index;
            }
            else if (key == "src")
            {
                var path = val;
                var atlas = UIAtlasManager.Instance.Get(path);
                _SetAtlas(atlas);
            }
            else if (key == "width")
            {
                float width;
                float.TryParse(val, out width);
                _size.x = width;
            }
            else if (key == "height")
            {
                float height;
                float.TryParse(val, out height);
                _size.y = height;
            }

//            else if (key == "width")
//            {
//                float width;
//                float.TryParse(val, out width);
//
//                float offset = 0.0f;
//                if (width > 1.0f)
//                {
//                    offset = (width - 1.0f) * 0.5f;
//                }
//
//                _offset = offset;
//            }
        }

        public void Reset ()
        {
            SetName(null);
        }

        public int GetVertexIndex ()
        {
            return _vertexIndex;
        }

        public void SetName (string name)
        {
            _name = name;
        }

        public string GetName ()
        {
            return _name;
        }

        public Vector2 GetSize ()
        {
            return _size;
        }

        public float GetOffset ()
        {
            return _offset;
        }

        public void SetFillMethod (FillMethod fillMethod)
        {
            _fillMethod = fillMethod;
        }

        public FillMethod GetFillMethod ()
        {
            return _fillMethod;
        }

        public void SetFillAmount (float amount)
        {
            amount = Mathf.Clamp01(amount);

            float eps = 0.001f;
            var delta = _fillAmount - amount;
            if (delta > eps || delta < -eps)
            {
                _fillAmount = amount;
                _richText.SetVerticesDirty();
            }
        }

        public float GetFillAmount ()
        {
            return _fillAmount;
        }

        private void _SetAtlas (UIAtlas atlas)
        {
            _atlas = atlas;

            if (null == atlas)
            {
                return;
            }

            var richText = _richText;
            var mat = richText.material;
            var manager = MaterialManager.Instance;
            var lastSpriteTexture = manager.GetSpriteAtlas(mat);
            var spriteTexture = atlas.GetTexture();

            var isTextureChanged = lastSpriteTexture != spriteTexture;
            if (isTextureChanged)
            {
                manager.DetachTexture(richText, lastSpriteTexture);
                manager.AttachTexture(richText, spriteTexture);
            }
        }

        public UIAtlas GetAtlas ()
        {
            return _atlas;
        }

        private RichText _richText;
        private UIAtlas  _atlas;

        private string _name;
        private int _vertexIndex;

        private Vector2 _size;
        private float _offset = 0;

        private float _fillAmount = 1.0f;
        private FillMethod _fillMethod = FillMethod.None;

        // refer: http://blog.useasp.net/archive/2013/06/14/use-regular-expression-to-parse-html-tags-attributes-method-with-csharp.aspx
        private static readonly string _spriteTagPattern = @"<quad(?:\s+(\w+)\s*=\s*(?<quota>['""]?)([\w\/]+)\k<quota>)+\s*\/>";
        private static readonly Regex _spriteTagRegex = new Regex(_spriteTagPattern, RegexOptions.Singleline);
    }
}
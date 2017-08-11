
/********************************************************************
created:    2017-08-04
author:     lixianmin

*********************************************************************/

using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Unique.RichText
{
    [System.Serializable]
    public class SpriteTag
    {
        public SpriteTag ()
        {
            
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

            // name
            var index = 1;
            var groups = match.Groups;
            var name = groups[1].Value;

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            _name = name;
            _vertexIndex = match.Index;

            // size;
            {
                ++index;
                float size;
                float.TryParse(groups[index].Value, out size);
                _size = size;
            }

            // offset
            {
                if (groups.Count <= ++index)
                {
                    return true;       
                }

                float width;
                float.TryParse(groups[index].Value, out width);

                float offset = 0.0f;
                if (width > 1.0f)
                {
                    offset = (width - 1.0f) * 0.5f;
                }

                _offset = offset;
            }

            return true;
        }

        public void Reset ()
        {
            _name = null;
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

        public float GetSize ()
        {
            return _size;
        }

        public float GetOffset ()
        {
            return _offset;
        }

        private string _name;
        private int _vertexIndex;

        private float _size;
        private float _offset;

        private static readonly string _spriteTagPattern = @"<quad name=(.+?)\s+size=(\d*\.?\d+%?)(?:\s+width=(\d*\.?\d+%?))?\s*/>";
        private static readonly Regex _spriteTagRegex = new Regex(_spriteTagPattern, RegexOptions.Singleline);
    }
}
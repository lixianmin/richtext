
/********************************************************************
created:    2017-08-04
author:     lixianmin

*********************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Unique.UI
{
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

        public SpriteTagInfo ()
        {
            Names = new List<string>();
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Key);
        }
    }
}
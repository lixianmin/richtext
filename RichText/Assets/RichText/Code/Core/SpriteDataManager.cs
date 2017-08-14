
/********************************************************************
created:    2017-08-07
author:     lixianmin

*********************************************************************/

using System.Collections;
using UnityEngine;

namespace Unique.UI.RichText
{
    public class SpriteDataManager
    {
        public void Add (string key, SpriteData spriteAsset)
        {
            if (null == key || null == spriteAsset)
            {
                return;
            }

            _spriteMap[key] = spriteAsset;
        }

        public SpriteData Get (string key)
        {
            key = key ?? string.Empty;
            var sprite = _spriteMap[key] as SpriteData;
            return sprite;
        }

        public void Remove (string key)
        {
            key = key ?? string.Empty;
            _spriteMap.Remove(key);
        }

        public void Clear ()
        {
            _spriteMap.Clear();
        }

        private readonly Hashtable _spriteMap = new Hashtable();

        public static readonly SpriteDataManager Instance = new SpriteDataManager();
    }
}
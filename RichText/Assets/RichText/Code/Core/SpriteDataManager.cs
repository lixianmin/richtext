
/********************************************************************
created:    2017-08-07
author:     lixianmin

*********************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

namespace Unique.UI.RichText
{
    public class SpriteDataManager
    {
        public void Add (string key, SpriteAtlas spriteAtlas)
        {
            if (null == key || null == spriteAtlas)
            {
                return;
            }

            _atlasMap[key] = spriteAtlas;
        }

        public SpriteAtlas Get (string key)
        {
            key = key ?? string.Empty;
            var sprite = _atlasMap[key] as SpriteAtlas;
            return sprite;
        }

        public void Remove (string key)
        {
            key = key ?? string.Empty;
            _atlasMap.Remove(key);
        }

        public void Clear ()
        {
            _atlasMap.Clear();
        }

        private readonly Hashtable _atlasMap = new Hashtable();

        public static readonly SpriteDataManager Instance = new SpriteDataManager();
    }
}
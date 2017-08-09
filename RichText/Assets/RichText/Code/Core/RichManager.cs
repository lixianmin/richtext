
/********************************************************************
created:    2017-08-07
author:     lixianmin

*********************************************************************/

using System.Collections;
using UnityEngine;

namespace Unique.RichText
{
    public class RichManager
    {
        public void AddSpriteAsset (string key, SpriteAsset spriteAsset)
        {
            if (null == key || null == spriteAsset)
            {
                return;
            }

            _spriteMap[key] = spriteAsset;
        }

        public SpriteAsset GetSpriteAsset (string key)
        {
            key = key ?? string.Empty;
            var sprite = _spriteMap[key] as SpriteAsset;
            return sprite;
        }

        private readonly Hashtable _spriteMap = new Hashtable();

        public static readonly RichManager Instance = new RichManager();
    }
}
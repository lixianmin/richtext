
/********************************************************************
created:    2017-08-07
author:     lixianmin

*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unique.RichText
{
    public class SpriteAsset
    {
        private SpriteAsset ()
        {
            
        }

        public static SpriteAsset Create (Texture texture, IList<SpriteItemAsset> spriteItems)
        {
            if (null == texture)
            {
                Debug.LogError("texture is null");
                return null;
            }

            var spriteAsset = new SpriteAsset();
            spriteAsset._texture = texture;

            if (null != spriteItems)
            {
                var count = spriteItems.Count;
                for (int i= 0; i< count; ++i)
                {
                    var item = spriteItems[i];
                    spriteAsset._AddSpriteItem(item);
                }
            }

            return spriteAsset;
        }

        private void _AddSpriteItem (SpriteItemAsset item)
        {
            if (null == item || null == item.name)
            {
                return;
            }

            _spriteMap[item.name] = item;
        }

        public SpriteItemAsset GetSpriteItem (string name)
        {
            name = name ?? string.Empty;
            var spriteItem = _spriteMap[name] as SpriteItemAsset;
            return spriteItem ?? _defaultSpriteItem;
        }

        public Texture GetTexture ()
        {
            return _texture;
        }

        private Texture _texture;
        private SpriteItemAsset _defaultSpriteItem = new SpriteItemAsset { name = string.Empty, rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f)};

        private readonly Hashtable _spriteMap = new Hashtable();
    }
}
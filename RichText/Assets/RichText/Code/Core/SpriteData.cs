
/********************************************************************
created:    2017-08-07
author:     lixianmin

*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unique.RichText
{
    public class SpriteData
    {
        private SpriteData ()
        {
            
        }

        public static SpriteData Create (Texture texture, IList<SpriteItem> spriteItems)
        {
            if (null == texture)
            {
                Debug.LogError("texture is null");
                return null;
            }

            var spriteAsset = new SpriteData();
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

        private void _AddSpriteItem (SpriteItem item)
        {
            if (null == item || null == item.name)
            {
                return;
            }

            _spriteMap[item.name] = item;
        }

        public SpriteItem GetSpriteItem (string name)
        {
            name = name ?? string.Empty;
            var spriteItem = _spriteMap[name] as SpriteItem;
            return spriteItem ?? _defaultSpriteItem;
        }

        public SpriteItem GetRandomSpriteItem ()
        {
            var count = _spriteMap.Count;
            if (count > 0)
            {
                var iter = _spriteMap.GetEnumerator();
                var index = UnityEngine.Random.Range(0, count - 1);
                for (int i= 0; i<= index; ++i)
                {
                    iter.MoveNext();
                }

                var spriteItem = iter.Value as SpriteItem;
                return spriteItem;
            }

            return _defaultSpriteItem;
        }

        public Texture GetTexture ()
        {
            return _texture;
        }

        private Texture _texture;
        private SpriteItem _defaultSpriteItem = new SpriteItem { name = string.Empty, rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f)};

        private readonly Hashtable _spriteMap = new Hashtable();
    }
}
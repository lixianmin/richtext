
/********************************************************************
created:    2017-08-07
author:     lixianmin

*********************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

namespace Unique.UI.RichText
{
    public class UIAtlasManager
    {
        public void Add (string key, SpriteAtlas spriteAtlas)
        {
            if (null == key || null == spriteAtlas)
            {
                return;
            }

            _atlasMap[key] = new UIAtlas(spriteAtlas);
        }

        public UIAtlas Get (string key)
        {
            key = key ?? string.Empty;
            var sprite = _atlasMap[key] as UIAtlas;
            return sprite;
        }

        public void Remove (string key)
        {
            key = key ?? string.Empty;
            _atlasMap.Remove(key);
        }

        public void Clear ()
        {
            if (_atlasMap.Count > 0)
            {
                var iter = _atlasMap.GetEnumerator();
                while (iter.MoveNext())
                {
                    var atlas = iter.Value as UIAtlas;
                    if (null != atlas)
                    {
                        atlas.Dispose();
                    }
                }

                _atlasMap.Clear();
            }
        }

        private readonly Hashtable _atlasMap = new Hashtable();
        public static readonly UIAtlasManager Instance = new UIAtlasManager();
    }
}
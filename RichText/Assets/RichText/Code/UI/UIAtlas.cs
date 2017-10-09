
/********************************************************************
created:    2017-10-09
author:     lixianmin

*********************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

namespace Unique.UI
{
    public class UIAtlas : IDisposable
    {
        public UIAtlas (SpriteAtlas spriteAtlas)
        {
            if (null == spriteAtlas)
            {
                throw new ArgumentNullException("spriteAtlas is null.");
            }

            _spriteAtlas = spriteAtlas;
        }

        ~UIAtlas ()
        {
            _DoDispose(false);
        }

        public void Dispose ()
        {
            if (_isDisposed)
            {
                return;
            }

            try
            {
                _DoDispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }

        private void _DoDispose (bool isManualDisposing)
        {
            if (null != _sprites)
            {
                foreach (var sprite in _sprites)
                {
                    GameObject.Destroy(sprite);
                }

                _sprites = null;
            }

            _spritesTable = null;
        }

        public Sprite GetSprite (string name)
        {
            _CheckInit();
            name = name ?? string.Empty;
            var sprite = _spritesTable[name] as Sprite;
            return sprite;
        }

        public Sprite[] GetSprites ()
        {
            _CheckInit();
            return _sprites;
        }

        public Texture GetTexture ()
        {
            _CheckInit();
            return _texture;
        }

        private void _CheckInit ()
        {
            if (null != _spritesTable || _isDisposed)
            {
                return;
            }

            var count = _spriteAtlas.spriteCount;
            _sprites = new Sprite[count];
            _spriteAtlas.GetSprites(_sprites);

            _spritesTable = new Hashtable(count);
            _texture = _sprites[0].texture;

            for (int i= 0; i< count; ++i)
            {
                var sprite = _sprites[i];
                var name = sprite.name;
                // remove the "(Clone)" ending text.
                name = name.Substring(0, name.Length - 7);
                sprite.name = name;

                _spritesTable[name] = sprite;
            }
        }

        private SpriteAtlas _spriteAtlas;
        private bool _isDisposed;

        private Sprite[]    _sprites;
        private Hashtable   _spritesTable;
        private Texture _texture;
    }
}
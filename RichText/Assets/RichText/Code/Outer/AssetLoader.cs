
/********************************************************************
created:    2017-08-11
author:     lixianmin

*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

namespace Unique.UI.RichText
{
    public class AssetLoader
    {
        public void LoadAll ()
        {
            foreach (var assetPath in _spriteAssetPaths)
            {
                _LoadSpriteAtlas(assetPath);
            }    
        }

        private void _LoadSpriteAtlas (string assetPath)
        {
            var spriteAtlas = Resources.Load<SpriteAtlas>(assetPath);
            if (null == spriteAtlas)
            {
                Debug.LogErrorFormat("spriteAtlas=null, assetPath={0}", assetPath);
                return;
            }

            SpriteDataManager.Instance.Add(assetPath, spriteAtlas);
        }

        public string[] GetSpriteAssetPaths ()
        {
            return _spriteAssetPaths;
        }

        private readonly string[] _spriteAssetPaths = new string[] { "emoji/default_emoji", "emoji/fruit", "emoji/bloodbar" };

        public static AssetLoader Instance = new AssetLoader();
    }
}
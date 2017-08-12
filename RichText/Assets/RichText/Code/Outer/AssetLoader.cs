
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


namespace Unique.RichText
{
    public class AssetLoader
    {
        public void LoadAll ()
        {
            foreach (var assetPath in _spriteAssetPaths)
            {
                _LoadSpriteAsset(assetPath);
            }    
        }

        private void _LoadSpriteAsset (string assetPath)
        {
            var inlineSpriteAsset = Resources.Load<SpriteAsset>(assetPath);
            if (null == inlineSpriteAsset)
            {
                Debug.LogErrorFormat("inlineSpriteAsset=null, assetPath={0}", assetPath);
                return;
            }

            var texture = inlineSpriteAsset.texture;
            if (null == texture)
            {
                Debug.LogErrorFormat("texture=null, assetPath={0}", assetPath);
                return;
            }

            var spriteAsset = SpriteData.Create(texture, inlineSpriteAsset.spriteItems);
            SpriteDataManager.Instance.Add(assetPath, spriteAsset);
        }

        public string[] GetSpriteAssetPaths ()
        {
            return _spriteAssetPaths;
        }

        private readonly string[] _spriteAssetPaths = new string[] { "emoji/default_emoji", "emoji/fruit", "emoji/bloodbar" };

        public static AssetLoader Instance = new AssetLoader();
    }
}

/********************************************************************
created:    2017-08-03
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
    public class MBGame : Graphic
    {
        private static MBGame _instance;
        public  static MBGame Instance
        {
            get
            {
                if (_instance == null)
                {
                    #if UNITY_EDITOR
                    _instance = FindObjectOfType<MBGame>() ;
                    #endif

                    if (_instance == null)
                    {
                        _instance = new GameObject ("(Singleton) " + typeof(MBGame).Name).AddComponent<MBGame> ();
                    }
                }

                return _instance;
            }
        }

        protected override void OnEnable ()
        {
            _LoadSpriteAsset();

            var spriteAsset = RichManager.Instance.GetSpriteAsset(_defaultSpriteAssetResPath);
            var richTexts = FindObjectsOfType<RichText>();
            foreach (var richText in richTexts)
            {
                richText.SetSpriteAsset(spriteAsset);
            }
        }

        private void _LoadSpriteAsset ()
        {
            var inlineSpriteAsset = Resources.Load<InlineSpriteAsset>(_defaultSpriteAssetResPath);
            if (null == inlineSpriteAsset)
            {
                Debug.LogErrorFormat("inlineSpriteAsset=null, _defaultSpriteAssetResPath={0}", _defaultSpriteAssetResPath);
                return;
            }

            var texture = inlineSpriteAsset.TextureSource;
            if (null == texture)
            {
                Debug.LogErrorFormat("texture=null, _defaultSpriteAssetResPath={0}", _defaultSpriteAssetResPath);
                return;
            }

            var spriteAsset = SpriteAsset.Create(texture, inlineSpriteAsset.spriteItems);
            RichManager.Instance.AddSpriteAsset(_defaultSpriteAssetResPath, spriteAsset);
        }

        private readonly string _defaultSpriteAssetResPath = "emoji/default_emoji";
    }
}

/********************************************************************
created:    2017-08-03
author:     lixianmin

*********************************************************************/
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Unique.UI
{
    /// <summary>
    /// 表情渲染数据
    /// </summary>
    public class InlineSprite : MaskableGraphic
    {
        [HideInInspector]
        public InlineSpriteAsset inlineSpriteAsset;

        public override Texture mainTexture
        {
            get
            {
                if (inlineSpriteAsset == null)
                {
                    return s_WhiteTexture;
                }

                if (inlineSpriteAsset.TextureSource == null)
                {
                    Debug.LogError("InlineSpriteGraphic: SpriteAsset.texSource is null");
                    return s_WhiteTexture;
                }
                else
                {
                    return inlineSpriteAsset.TextureSource;
                }
            }
        }

        protected override void Awake()
        {
            if (inlineSpriteAsset != null && inlineSpriteAsset.TextureSource != null)
            {
                return;
            }

            RichTextManager.Instance.RebulidSpriteData();

            inlineSpriteAsset = RichTextManager.Instance.InlineSpriteAsset;

            UpdateMaterial();

            transform.localPosition = new Vector3(1000,1000,1000);

            base.Awake();
        }

        public SpriteAssetInfo GetSpriteInfo(int index)
        {
            return RichTextManager.Instance.GetSpriteInfo(index);
        }

        public SpriteAssetInfo GetSpriteInfo(string name)
        {
            return RichTextManager.Instance.GetSpriteInfo(name);
        }

        public List<SpriteAssetInfo> GetSpriteInfosFromPrefix(string namePrefix)
        {
            return RichTextManager.Instance.GetSpriteInfosFromPrefix(namePrefix);
        }

        public List<string> GetSpriteNamesFromPrefix(string namePrefix)
        {
            return RichTextManager.Instance.GetSpriteNamesFromPrefix(namePrefix);
        }

        public new void UpdateMaterial()
        {
            base.UpdateMaterial();
        }
    }
}
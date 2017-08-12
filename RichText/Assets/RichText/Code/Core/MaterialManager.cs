
/********************************************************************
created:    2017-08-12
author:     lixianmin

*********************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Unique.RichText
{ 
    internal class MaterialManager
    {
        private MaterialManager ()
        {

        }

        private MaterialInfo _FetchMaterialInfo (Texture spriteTexture)
        {
            if (null == spriteTexture)
            {
                return null;
            }

            var info = _materialInfos[spriteTexture] as MaterialInfo;
            if (null == info)
            {
                var mat = new Material(_GetRichTextShader());
                mat.name = spriteTexture.name;
                mat.SetTexture(_spriteTextureName, spriteTexture);

                info = new MaterialInfo(mat);
                _materialInfos.Add(spriteTexture, info);
            }

            return info;
        }

        public void AttachTexture (Graphic target, Texture spriteTexture)
        {
            if (null == target || null == spriteTexture)
            {
                return;
            }

            var matInfo = _FetchMaterialInfo(spriteTexture);
            matInfo.Attach(target);
        }

        public void DetachTexture (Graphic target, Texture spriteTexture)
        {
            if (null == target || null == spriteTexture)
            {
                return;
            }

            var info = _materialInfos[spriteTexture] as MaterialInfo;
            if (null != info)
            {
                var mat = info.GetMaterial();
                info.Detach(target);

                var count = info.GetCount();
                if (count == 0)
                {
                    _materialInfos.Remove(spriteTexture);
                    GameObject.DestroyImmediate(mat);
                }
            }
        }

        public Texture GetSpriteTexture (Material mat)
        {
            if (null != mat && mat.shader == _GetRichTextShader())
            {
                var texture = mat.GetTexture(_spriteTextureName);
                return texture;
            }

            return null;
        }

        private Shader _GetRichTextShader ()
        {
            if (null == _richTextShader)
            {
                _richTextShader = Shader.Find("Unique/RichText");
            }

            return _richTextShader;
        }

        private Shader _richTextShader;
        private readonly Hashtable _materialInfos = new Hashtable();

        private static readonly string _spriteTextureName = "_SpriteTex";
        public static readonly MaterialManager Instance = new MaterialManager();
    }
}
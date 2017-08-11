
/********************************************************************
created:    2017-08-10
author:     lixianmin

*********************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Unique.RichText
{
    partial class RichText
    {
        private class MaterialInfo
        {
            public MaterialInfo (Material mat)
            {
                if (null == mat)
                {
                    throw new ArgumentNullException();
                }

                _material = mat;
            }

            public void Attach (Graphic target)
            {
                if (null == target)
                {
                    return;
                }

                target.material = _material;
                _fans.Add(target, this);
            }

            public void Detach (Graphic target)
            {
                if (null == target)
                {
                    return;
                }

                _fans.Remove(target);
            }

            public Material GetMaterial ()
            {
                return _material;
            }

            public int GetCount ()
            {
                return _fans.Count;
            }

            private Material _material;
            private readonly Hashtable _fans = new Hashtable();
        }

        private class MaterialManager
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
}
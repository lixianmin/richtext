
/********************************************************************
created:    2017-08-03
author:     lixianmin

*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Unique.RichText
{
    public static class CreateSpriteAsset
    {
        [MenuItem("Assets/Create/Create SpriteAsset", false, 10)]
        private static void _CreateSpriteAsset ()
        {
            foreach (Texture2D targetTexture in Selection.objects)
            {
                if (null != targetTexture)
                {
                    _CreateSpriteAsset(targetTexture);
                }
            }
        }

        private static void _CreateSpriteAsset (Texture2D targetTexture)
        {
            string filePathWithName = AssetDatabase.GetAssetPath(targetTexture);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePathWithName);
            var exportPath = _targetPath + fileNameWithoutExtension + ".asset";

            var inlineSpriteAsset = ScriptableObject.CreateInstance<SpriteAsset>();
            inlineSpriteAsset.texture = targetTexture;
            inlineSpriteAsset.spriteItems = GetSpriteItems(targetTexture);

            AssetDatabase.CreateAsset(inlineSpriteAsset, exportPath);
            Debug.LogFormat("SpriteAsset: {0} generated successfully", exportPath);
        }

        public static List<SpriteItem> GetSpriteItems (Texture2D texture)
        {
            if (null == texture)
            {
                return  null;
            }

            string filepath = AssetDatabase.GetAssetPath(texture);
            Object[] objects = AssetDatabase.LoadAllAssetsAtPath(filepath);

            Vector2 textureSize = new Vector2(texture.width, texture.height);
            var sprites = new List<SpriteItem>(objects.Length);

            for (int i = 0; i < objects.Length; i++)
            {
                Sprite sprite = objects[i] as Sprite;
                if (null == sprite)
                {
                    continue;
                }

                SpriteItem itemAsset = new SpriteItem();
                itemAsset.name = sprite.name;

                Rect rect = new Rect();
                rect.x = sprite.rect.x / textureSize.x;
                rect.y = sprite.rect.y / textureSize.y;
                rect.width = sprite.rect.width / textureSize.x;
                rect.height = sprite.rect.height / textureSize.y;

                itemAsset.rect = rect;
                sprites.Add(itemAsset);
            }

            return sprites;
        }

        private static string _targetPath = "Assets/Resources/emoji/";
    }
}
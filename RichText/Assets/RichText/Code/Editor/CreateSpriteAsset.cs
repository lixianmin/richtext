
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
        [MenuItem("Assets/Create/Create InlineSpriteAsset", false, 10)]
        private static void _CreateInlineSpriteAsset ()
        {
            Object target = Selection.activeObject;
            var targetTexture = target as Texture2D;
            if (null == targetTexture)
            {
                return;
            }

            string filePathWithName = AssetDatabase.GetAssetPath(targetTexture);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePathWithName);
            var exportPath = _targetPath + fileNameWithoutExtension + ".asset";

            var inlineSpriteAsset = ScriptableObject.CreateInstance<InlineSpriteAsset>();
            inlineSpriteAsset.TextureSource = targetTexture;
            inlineSpriteAsset.spriteItems = GetSpriteItemAssets(targetTexture);

            AssetDatabase.CreateAsset(inlineSpriteAsset, exportPath);
            Debug.LogFormat("InlineSpriteAsset: {0} generated successfully", exportPath);
        }

        public static List<SpriteItemAsset> GetSpriteItemAssets (Texture2D texture)
        {
            if (null == texture)
            {
                return  null;
            }

            string filepath = AssetDatabase.GetAssetPath(texture);
            Object[] objects = AssetDatabase.LoadAllAssetsAtPath(filepath);

            Vector2 textureSize = new Vector2(texture.width, texture.height);
            var sprites = new List<SpriteItemAsset>(objects.Length);

            for (int i = 0; i < objects.Length; i++)
            {
                Sprite sprite = objects[i] as Sprite;
                if (null == sprite)
                {
                    continue;
                }

                SpriteItemAsset itemAsset = new SpriteItemAsset();
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
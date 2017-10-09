
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


namespace Unique.UI.RichText
{
    public class MBDrawCallTest : MonoBehaviour
    {
        protected void Awake ()
        {
            AssetLoader.Instance.LoadAll();

            var canvasArray = FindObjectsOfType<Canvas>();
            var length = canvasArray.Length;
            if (length > 0)
            {
                var currentCanvas = canvasArray[0];
                var spriteAssetPaths = AssetLoader.Instance.GetSpriteAssetPaths();
                for (int i= 0; i< spriteAssetPaths.Length - 1; ++i)
                {
                    var assetPath = spriteAssetPaths[i];
                    var goParent = new GameObject(assetPath);
                    goParent.transform.SetParent(currentCanvas.transform, false);

                    var parent = goParent.transform;
                    _CreateRichText(parent, assetPath);
                }

                StartCoroutine(_CoUpdateSprite()); 
            }
        }

        private void _CreateRichText (Transform parent, string spriteDataPath)
        {
            var count = 1;
            for (int i= 0; i< count; ++i)
            {
                var go = new GameObject();
                var trans = go.AddComponent<RectTransform>();

                var x = UnityEngine.Random.Range(-400, 400);
                var y = UnityEngine.Random.Range(-400, 400);
                trans.position = new Vector2(x, y);
                trans.sizeDelta = new Vector2(450, 200);

                var richText = go.AddComponent<RichText>();
                richText.font =  Resources.GetBuiltinResource<Font>("Arial.ttf");
                richText.text = string.Format("测试用的文字  <quad name=meat_1 src={0} width=96 height=96/> 尾巴", spriteDataPath);
                richText.fontSize = 24;

                go.transform.SetParent(parent, false);
                go.name = "RichText_" + i.ToString();

                _richTextObjects.Add(go);
            }
        }

        protected void OnDestroy()
        {
            foreach (var go in _richTextObjects)
            {
                GameObject.DestroyImmediate(go);    
            }

            _richTextObjects.Clear();
        }

        private Sprite[] _GetAtlasSprites (Hashtable spritesTable, UnityEngine.U2D.SpriteAtlas spriteAtlas)
        {
            var sprites = spritesTable[spriteAtlas] as Sprite[];
            if (null == sprites)
            {
                sprites = new Sprite[spriteAtlas.spriteCount];
                spriteAtlas.GetSprites(sprites);

                foreach (var item in sprites)
                {
                    if (null != item)
                    {
                        var name = item.name;
                        item.name = name.Substring(0, name.Length - 7);
                    }
                }

                spritesTable[spriteAtlas] = sprites;
            }

            return sprites;
        }

        private IEnumerator _CoUpdateSprite ()
        {
            var spritesTable = new Hashtable();

            while (true)
            {
                yield return new WaitForSeconds(0.2f);

                var richTexts = FindObjectsOfType<RichText>();
                foreach (var richText in richTexts)
                {
                    var tags = richText.GetSpriteTags();
                    var count = tags.Count;
                    for (int i= 0; i< count; ++i)
                    {
                        var tag = tags[i];

                        var spriteAtlas = tag.GetSpriteAtlas();
                        var sprites = _GetAtlasSprites(spritesTable, spriteAtlas);

                        var sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
                        tag.SetName(sprite.name);

                        richText.SetVerticesDirty();
                    }
                }
            }
        }

        private readonly List<GameObject> _richTextObjects = new List<GameObject>();
    }
}
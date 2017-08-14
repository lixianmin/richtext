
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
            var count = 200;
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

        private IEnumerator _CoUpdateSprite ()
        {
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
                        var spriteItem = tag.GetSpriteData().GetRandomSpriteItem();
                        tag.SetName(spriteItem.name);

                        richText.SetVerticesDirty();
                    }
                }
            }
        }

        private readonly List<GameObject> _richTextObjects = new List<GameObject>();
    }
}
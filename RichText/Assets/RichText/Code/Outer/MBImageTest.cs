
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
    public class MBImageTest : MonoBehaviour
    {
        private void Awake ()
        {
            AssetLoader.Instance.LoadAll();
            var spriteAssetPaths = AssetLoader.Instance.GetSpriteAssetPaths();

            var richTexts = FindObjectsOfType<RichText>();
            for (int i= 0; i< richTexts.Length; ++i)
            {
                var spriteData = RichManager.Instance.GetSpriteData(spriteAssetPaths[0]);
                richTexts[i].SetSpriteData(spriteData);
            }

        }

        private void Update ()
        {
            
        }
    }
}

/********************************************************************
created:    2017-08-11
author:     lixianmin

*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Unique.UI.RichText
{
    public class MBBloodBarTest : MonoBehaviour
    {
        private void Awake ()
        {
            AssetLoader.Instance.LoadAll();
        }

        private void Update ()
        {
            if (null == foregroundRichText)
            {
                return;
            }

            var tags = foregroundRichText.GetSpriteTags();
            var count = tags.Count;
            for (int i= 0; i< count; ++i)
            {
                var tag = tags[i];
                var amount = Time.time - (int) Time.time;
                tag.SetFillMethod(SpriteTag.FillMethod.Horizontal);
                tag.SetFillAmount(amount);
            }
        }

        public RichText foregroundRichText;
    }
}
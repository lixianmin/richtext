
/********************************************************************
created:    2017-08-03
author:     lixianmin

*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Unique.UI.RichText
{
    [CustomEditor(typeof(SpriteAsset))]
    public class InlineSpriteAssetEditor : Editor
    {
        SpriteAsset inlineSpriteAsset;

        public void OnEnable()
        {
            inlineSpriteAsset = (SpriteAsset)target;
        }
        private Vector2 ve2ScorllView;
        public override void OnInspectorGUI()
        {
            // EditorGUILayout.TextField("Texture:", inlineSpriteAsset.TextureSource.name);

            ve2ScorllView = GUILayout.BeginScrollView(ve2ScorllView);
            GUILayout.Label("UGUI Sprite Asset");
            if (inlineSpriteAsset.spriteItems == null)
            {
                return;
            }

            for (int i = 0; i < inlineSpriteAsset.spriteItems.Count; i++)
            {
                GUILayout.Label("\n");
                //     EditorGUILayout.ObjectField("", InlineSpriteAsset.listSpriteInfor[i].sprite, typeof(Sprite),false);
                //EditorGUILayout.IntField("ID:", inlineSpriteAsset.listSpriteInfor[i].ID);
                EditorGUILayout.TextField("name:", inlineSpriteAsset.spriteItems[i].name);
                //   EditorGUILayout.Vector2Field("povit:", InlineSpriteAsset.listSpriteInfor[i].pivot);
                //     EditorGUILayout.RectField("rect:", inlineSpriteAsset.listSpriteInfor[i].rect);
                //    GUILayout.Label("\n");
            }
            GUILayout.EndScrollView();
        }
    }
}

/********************************************************************
created:    2017-08-10
author:     lixianmin

*********************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

namespace Unique.UI.RichText
{
    public class CreateRichTextMenu
    {
        [MenuItem("GameObject/UI/Rich Text", false, 1901)]
        private static void _AddRichText (MenuCommand menuCommand)
        {
            GameObject go = CreateRichText();
            MenuOptions.PlaceUIElementRoot(go, menuCommand);
        }

        public static GameObject CreateRichText ()
        {
            GameObject go = DefaultControls.CreateUIElementRoot("RichText", s_ThickElementSize);

            var lbl = go.AddComponent<RichText>();
            lbl.text = "New RichText";
            lbl.raycastTarget = false;

            SetDefaultTextValues(lbl);

            return go;
        }

        private static void SetDefaultTextValues (Text lbl)
        {
            // Set text values we want across UI elements in default controls.
            // Don't set values which are the same as the default values for the Text component,
            // since there's no point in that, and it's good to keep them as consistent as possible.
            lbl.color = s_TextColor;

            // Reset() is not called when playing. We still want the default font to be assigned
            lbl.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private const float  kWidth       = 160f;
        private const float  kThickHeight = 30f;
        private static Vector2 s_ThickElementSize       = new Vector2(kWidth, kThickHeight);
        private static Color   s_TextColor              = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);
    }
}
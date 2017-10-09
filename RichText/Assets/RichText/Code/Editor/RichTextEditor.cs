
/********************************************************************
created:    2017-08-07
author:     lixianmin

*********************************************************************/
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace Unique.UI.RichText
{
    [CustomEditor(typeof(RichText))]
    [CanEditMultipleObjects]
    public class TextEditor : GraphicEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            var serializedObject = this.serializedObject;
            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");

            _lpfnParseText = System.Delegate.CreateDelegate(typeof(Action), serializedObject.targetObject, "_ParseText") as Action;
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update();

            var currentTextString = m_Text.stringValue;
            if (_lastTextString != currentTextString)
            {
                _lastTextString = currentTextString;

                var richText = serializedObject.targetObject as RichText;
                if (richText.IsActive() && null != _lpfnParseText)
                {
                    _lpfnParseText();
                }
            }

            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_FontData);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();

//            DrawDefaultInspector();
        }

        private SerializedProperty m_Text;
        private SerializedProperty m_FontData;

        private string _lastTextString;
        private Action _lpfnParseText;
    }
}
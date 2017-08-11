
/********************************************************************
created:    2017-08-07
author:     lixianmin

*********************************************************************/
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace Unique.RichText
{
    [CustomEditor(typeof(RichText))]
    [CanEditMultipleObjects]
    public class TextEditor : GraphicEditor
    {
        SerializedProperty m_Text;
        SerializedProperty m_FontData;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_FontData);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();

//            DrawDefaultInspector();
        }
    }
}
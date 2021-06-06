using System;
using UnityEngine;
using UnityEditor;

namespace Foundation.Editor
{
    [CustomPropertyDrawer(typeof(LocalizedString))]
    public sealed class LocalizedStringDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var options = LocalizationData.EditorGetLocalizationIDs();

            var idProperty = property.FindPropertyRelative("LocalizationID");
            var oldID = idProperty.stringValue;
            int oldSelectedIndex = Array.IndexOf(options, oldID);

            int newSelectedIndex = EditorGUI.Popup(position, label.text, oldSelectedIndex, options);
            if (newSelectedIndex != oldSelectedIndex)
                idProperty.stringValue = options[newSelectedIndex];
        }
    }
}

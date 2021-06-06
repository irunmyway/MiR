using UnityEngine;
using UnityEditor;

namespace Foundation.Editor
{
    [CustomEditor(typeof(LevelUpThresholds))]
    public sealed class LevelUpThresholdsEditor : UnityEditor.Editor
    {
        SerializedProperty levelsProperty;
        SerializedProperty multiplierProperty;
        SerializedProperty level2ExperienceProperty;
        SerializedProperty maxLevelProperty;

        void OnEnable()
        {
            levelsProperty = serializedObject.FindProperty("ExperienceLevels");
            multiplierProperty = serializedObject.FindProperty("multiplier");
            level2ExperienceProperty = serializedObject.FindProperty("level2Experience");
            maxLevelProperty = serializedObject.FindProperty("maxLevel");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(levelsProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("=== Generator ===", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(maxLevelProperty);
            EditorGUILayout.PropertyField(level2ExperienceProperty);
            EditorGUILayout.PropertyField(multiplierProperty);

            if (GUILayout.Button("Generate")) {
                int n = maxLevelProperty.intValue - 1;
                levelsProperty.arraySize = n;

                int exp = level2ExperienceProperty.intValue;
                for (int i = 0; i < n; i++) {
                    levelsProperty.GetArrayElementAtIndex(i).intValue = exp;
                    exp = Mathf.FloorToInt(exp * multiplierProperty.floatValue);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

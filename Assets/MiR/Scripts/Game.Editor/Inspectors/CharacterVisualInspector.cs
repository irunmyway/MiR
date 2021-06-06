using UnityEngine;
using UnityEditor;

namespace Game.Editor
{
    [CustomEditor(typeof(CharacterVisual))]
    public sealed class CharacterVisualInspector : UnityEditor.Editor
    {
        SerializedProperty selectedHeadProperty;
        SerializedProperty selectedHairProperty;
        SerializedProperty selectedBeardProperty;
        SerializedProperty selectedJacketProperty;
        SerializedProperty selectedPantsProperty;
        SerializedProperty selectedBootsProperty;

        SerializedProperty headsProperty;
        SerializedProperty hairsProperty;
        SerializedProperty beardsProperty;
        SerializedProperty jacketsProperty;
        SerializedProperty pantsProperty;
        SerializedProperty bootsProperty;

        void OnEnable()
        {
            selectedHeadProperty = serializedObject.FindProperty("SelectedHead");
            selectedHairProperty = serializedObject.FindProperty("SelectedHair");
            selectedBeardProperty = serializedObject.FindProperty("SelectedBeard");
            selectedJacketProperty = serializedObject.FindProperty("SelectedJacket");
            selectedPantsProperty = serializedObject.FindProperty("SelectedPants");
            selectedBootsProperty = serializedObject.FindProperty("SelectedBoots");

            headsProperty = serializedObject.FindProperty("Heads");
            hairsProperty = serializedObject.FindProperty("Hairs");
            beardsProperty = serializedObject.FindProperty("Beards");
            jacketsProperty = serializedObject.FindProperty("Jackets");
            pantsProperty = serializedObject.FindProperty("Pants");
            bootsProperty = serializedObject.FindProperty("Boots");
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Randomize")) {
                Undo.RecordObject(target, "Randomize character");
                ((CharacterVisual)target).Randomize();
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                EditorUtility.SetDirty(target);
            }

            serializedObject.Update();

            Slider(selectedHeadProperty, headsProperty, false);
            Slider(selectedHairProperty, hairsProperty, true);
            Slider(selectedBeardProperty, beardsProperty, true);
            Slider(selectedJacketProperty, jacketsProperty, false);
            Slider(selectedPantsProperty, pantsProperty, false);
            Slider(selectedBootsProperty, bootsProperty, false);

            EditorGUILayout.PropertyField(headsProperty);
            EditorGUILayout.PropertyField(hairsProperty);
            EditorGUILayout.PropertyField(beardsProperty);
            EditorGUILayout.PropertyField(jacketsProperty);
            EditorGUILayout.PropertyField(pantsProperty);
            EditorGUILayout.PropertyField(bootsProperty);

            serializedObject.ApplyModifiedProperties();
        }

        void Slider(SerializedProperty valueProperty, SerializedProperty arrayProperty, bool optional)
        {
            int arraySize = arrayProperty.arraySize;
            if (arraySize == 0) {
                GUI.enabled = false;
                EditorGUILayout.IntSlider(valueProperty.displayName, 0, 0, 0);
                GUI.enabled = true;
            } else {
                int min = (optional ? -1 : 0);
                int max = arraySize - 1;

                int oldValue = valueProperty.intValue;
                int newValue = EditorGUILayout.IntSlider(valueProperty.displayName, oldValue, min, max);
                if (newValue != oldValue)
                    valueProperty.intValue = newValue;
            }
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public sealed class LocalizationData : ScriptableObject, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct StringInfo
        {
            public string Key;
            public Language[] Languages;
            public string[] Strings;
        }

        [SerializeField] [HideInInspector] List<StringInfo> Strings;
        public Dictionary<string, Dictionary<Language, string>> TranslatedStrings;

        public string GetLocalization(LocalizedString str, Language language)
        {
            if (TranslatedStrings == null)
                return str.LocalizationID;

            if (!TranslatedStrings.TryGetValue(str.LocalizationID, out var dict))
                return str.LocalizationID;

            if (!dict.TryGetValue(language, out var localized))
                return str.LocalizationID;

            return localized;
        }

      #if UNITY_EDITOR
        public const string AssetPath = "Assets/Config/LocalizationData.asset";

        static LocalizationData editorInstance;
        static string[] editorStringIds;

        public static void EditorInvalidateCache()
        {
            editorInstance = null;
            editorStringIds = null;
        }

        public static string EditorGetLocalization(LocalizedString str, Language language = Language.English)
        {
            if (editorInstance == null) {
                editorInstance = UnityEditor.AssetDatabase.LoadAssetAtPath<LocalizationData>(AssetPath);
                if (editorInstance == null)
                    return str.LocalizationID;
            }

            return editorInstance.GetLocalization(str, language);
        }

        public static string[] EditorGetLocalizationIDs()
        {
            if (editorStringIds != null)
                return editorStringIds;

            if (editorInstance == null) {
                editorInstance = UnityEditor.AssetDatabase.LoadAssetAtPath<LocalizationData>(AssetPath);
                if (editorInstance == null) {
                    editorStringIds = new string[0];
                    return editorStringIds;
                }
            }

            if (editorInstance.TranslatedStrings == null) {
                editorStringIds = new string[0];
                return editorStringIds;
            }

            var ids = editorInstance.TranslatedStrings.Keys.ToList();
            ids.Sort();
            editorStringIds = ids.ToArray();

            return editorStringIds;
        }
      #endif

        public void OnBeforeSerialize()
        {
            Strings = new List<StringInfo>();

            if (TranslatedStrings == null)
                return;

            foreach (var it in TranslatedStrings) {
                var languages = new Language[it.Value.Count];
                var strings = new string[it.Value.Count];

                int index = 0;
                foreach (var jt in it.Value) {
                    languages[index] = jt.Key;
                    strings[index] = jt.Value;
                    ++index;
                }

                Strings.Add(new StringInfo{ Key = it.Key, Languages = languages, Strings = strings });
            }
        }

        public void OnAfterDeserialize()
        {
            TranslatedStrings = new Dictionary<string, Dictionary<Language, string>>();

            if (Strings == null)
                return;

            foreach (var it in Strings) {
                var dict = new Dictionary<Language, string>();
                for (int i = 0; i < it.Languages.Length; i++)
                    dict[it.Languages[i]] = it.Strings[i];
                TranslatedStrings[it.Key] = dict;
            }
        }
    }
}

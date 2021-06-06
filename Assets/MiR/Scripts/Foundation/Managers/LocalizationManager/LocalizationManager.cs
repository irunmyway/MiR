using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Zenject;

namespace Foundation
{
    public sealed class LocalizationManager : AbstractService<ILocalizationManager>, ILocalizationManager
    {
        public LocalizationData LocalizationData;

        public ObserverList<IOnLanguageChanged> OnLanguageChanged { get; } = new ObserverList<IOnLanguageChanged>();

        Language currentLanguage;
        public Language CurrentLanguage { get {
                return currentLanguage;
            } set {
                if (currentLanguage != value) {
                    currentLanguage = value;
                    PlayerPrefs.SetString("Language", currentLanguage.ToString());
                    PlayerPrefs.Save();
                    foreach (var notify in OnLanguageChanged.Enumerate())
                        notify.Do();
                }
            } }

        void Awake()
        {
            string lang = PlayerPrefs.GetString("Language", "");
            if (!string.IsNullOrEmpty(lang)) {
                if (Enum.TryParse<Language>(lang, out var language)) {
                    currentLanguage = language;
                    return;
                }
                DebugOnly.Error($"Invalid language in settings: {lang}");
            }

            var sysLang = Application.systemLanguage;
            switch (sysLang) {
                case SystemLanguage.Russian: currentLanguage = Language.Russian; return;
                case SystemLanguage.English: currentLanguage = Language.English; return;
            }

            DebugOnly.Error($"Unsupported system language \"{sysLang}\", using English instead.");
            currentLanguage = Language.English;
        }

        public string GetString(LocalizedString str)
        {
            return LocalizationData.GetLocalization(str, currentLanguage);
        }
    }
}

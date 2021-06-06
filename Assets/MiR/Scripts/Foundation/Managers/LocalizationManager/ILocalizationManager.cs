namespace Foundation
{
    public interface ILocalizationManager
    {
        ObserverList<IOnLanguageChanged> OnLanguageChanged { get; }

        Language CurrentLanguage { get; set; }

        string GetString(LocalizedString str);
    }
}

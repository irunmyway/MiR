namespace Foundation
{
    public interface IExperienceManager
    {
        ObserverList<IOnExperienceGained> OnExperienceGained { get; }
        ObserverList<IOnExperienceChanged> OnExperienceChanged { get; }
        ObserverList<IOnLevelReached> OnLevelReached { get; }
        ObserverList<IOnLevelChanged> OnLevelChanged { get; }

        void ResetAllPlayers();

        void AddExperience(int player, int experience);

        int GetPlayerLevel(int player);
        bool IsPlayerMaxLevel(int player);

        int GetPlayerExperience(int player);
        int GetPlayerExperienceForNextLevel(int player);
    }
}

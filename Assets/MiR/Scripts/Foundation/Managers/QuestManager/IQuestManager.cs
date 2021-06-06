namespace Foundation
{
    public interface IQuestManager
    {
        // 1 классификация:
        //  - никогда не стартовал (NotStarted)
        //  - стартовал (Started) и возможно был завершен

        // 2 классификация:
        //  - не активен
        //  - активен сейчас (Active)
        //  - завершен (Completed)
        //  - провален (Failed)

        ObserverList<IOnQuestStarted> OnQuestStarted { get; }
        ObserverList<IOnQuestCompleted> OnQuestCompleted { get; }
        ObserverList<IOnQuestFailed> OnQuestFailed { get; }

        void StartQuest(Quest quest);
        void CompleteQuest(Quest quest);
        void FailQuest(Quest quest);

        bool QuestStarted(Quest quest);
        bool QuestActive(Quest quest);
        bool QuestCompleted(Quest quest);
        bool QuestFailed(Quest quest);
    }
}

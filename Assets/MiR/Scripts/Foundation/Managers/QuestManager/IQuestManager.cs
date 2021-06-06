namespace Foundation
{
    public interface IQuestManager
    {
        // 1 �������������:
        //  - ������� �� ��������� (NotStarted)
        //  - ��������� (Started) � �������� ��� ��������

        // 2 �������������:
        //  - �� �������
        //  - ������� ������ (Active)
        //  - �������� (Completed)
        //  - �������� (Failed)

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

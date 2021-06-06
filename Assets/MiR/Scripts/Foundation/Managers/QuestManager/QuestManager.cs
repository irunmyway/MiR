using System.Collections.Generic;
using Zenject;

namespace Foundation
{
    public sealed class QuestManager : AbstractService<IQuestManager>, IQuestManager
    {
        public ObserverList<IOnQuestStarted> OnQuestStarted { get; } = new ObserverList<IOnQuestStarted>();
        public ObserverList<IOnQuestCompleted> OnQuestCompleted { get; } = new ObserverList<IOnQuestCompleted>();
        public ObserverList<IOnQuestFailed> OnQuestFailed { get; } = new ObserverList<IOnQuestFailed>();

        List<Quest> activeQuests = new List<Quest>();
        List<Quest> completedQuests = new List<Quest>();
        List<Quest> failedQuests = new List<Quest>();

        HashSet<Quest> activeQuestsSet = new HashSet<Quest>();
        HashSet<Quest> completedQuestsSet = new HashSet<Quest>();
        HashSet<Quest> failedQuestsSet = new HashSet<Quest>();

        public LocalizedString QuestStartedMessage;
        public LocalizedString QuestCompletedMessage;
        public LocalizedString QuestFailedMessage;

        [Inject] INotificationManager notificationManager = default;
        [Inject] ILocalizationManager localizationManager = default;

        [Inject] public IPlayerManager playerManager { get; private set; } = default;

        public void StartQuest(Quest quest)
        {
            if (QuestStarted(quest)) {
                DebugOnly.Error($"Attempting to start quest {quest.name} again.");
                return;
            }

            activeQuests.Add(quest);
            activeQuestsSet.Add(quest);

            foreach (var it in OnQuestStarted.Enumerate())
                it.Do(quest);

            string questName = localizationManager.GetString(quest.Name);
            notificationManager.DisplayMessage(string.Format(localizationManager.GetString(QuestStartedMessage), questName));
        }

        public void CompleteQuest(Quest quest)
        {
            if (!activeQuestsSet.Contains(quest))
                DebugOnly.Error($"Attempted to complete non-started quest {quest.name}.");
            else {
                activeQuests.Remove(quest);
                activeQuestsSet.Remove(quest);
                AddCompletedQuest(quest);
            }
        }

        public void FailQuest(Quest quest)
        {
            if (!activeQuestsSet.Contains(quest))
                DebugOnly.Error($"Attempted to complete non-started quest {quest.name}.");
            else {
                activeQuests.Remove(quest);
                activeQuestsSet.Remove(quest);
                AddFailedQuest(quest);
            }
        }

        public bool QuestStarted(Quest quest)
        {
            return (activeQuestsSet.Contains(quest) || completedQuestsSet.Contains(quest) || failedQuestsSet.Contains(quest));
        }

        public bool QuestActive(Quest quest)
        {
            return activeQuestsSet.Contains(quest);
        }

        public bool QuestCompleted(Quest quest)
        {
            return completedQuestsSet.Contains(quest);
        }

        public bool QuestFailed(Quest quest)
        {
            return failedQuestsSet.Contains(quest);
        }

        void Update()
        {
            int n = activeQuests.Count;
            while (n-- > 0) {
                var quest = activeQuests[n];
                switch (quest.GetState(this)) {
                    case Quest.State.Unknown:
                        break;

                    case Quest.State.Success:
                        activeQuests.RemoveAt(n);
                        activeQuestsSet.Remove(quest);
                        AddCompletedQuest(quest);
                        break;

                    case Quest.State.Failure:
                        activeQuests.RemoveAt(n);
                        activeQuestsSet.Remove(quest);
                        AddFailedQuest(quest);
                        break;
                }
            }
        }

        void AddCompletedQuest(Quest quest)
        {
            completedQuests.Add(quest);
            completedQuestsSet.Add(quest);

            foreach (var it in OnQuestCompleted.Enumerate())
                it.Do(quest);

            string questName = localizationManager.GetString(quest.Name);
            notificationManager.DisplayMessage(string.Format(localizationManager.GetString(QuestCompletedMessage), questName));
        }

        void AddFailedQuest(Quest quest)
        {
            failedQuests.Add(quest);
            failedQuestsSet.Add(quest);

            foreach (var it in OnQuestFailed.Enumerate())
                it.Do(quest);

            string questName = localizationManager.GetString(quest.Name);
            notificationManager.DisplayMessage(string.Format(localizationManager.GetString(QuestFailedMessage), questName));
        }
    }
}

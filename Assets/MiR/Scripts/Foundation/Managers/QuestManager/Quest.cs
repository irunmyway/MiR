using System;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    [CreateAssetMenu(menuName = "OTUS/Quest")]
    public sealed class Quest : ScriptableObject
    {
        public enum State
        {
            Unknown,
            Success,
            Failure,
        }

        public LocalizedString Name;
        public List<QuestCondition> SuccessConditions; // "И", все должны быть выполнены
        public List<QuestCondition> FailureConditions; // "ИЛИ", хотя бы одно должно быть выполнено

        public State GetState(QuestManager questManager)
        {
            if (FailureConditions != null) {
                foreach (var condition in FailureConditions) {
                    if (condition.IsTrue(questManager))
                        return State.Failure;
                }
            }

            if (SuccessConditions != null) {
                bool success = true;
                foreach (var condition in SuccessConditions) {
                    if (!condition.IsTrue(questManager)) {
                        success = false;
                        break;
                    }
                }

                return (success ? State.Success : State.Unknown);
            }

            return (FailureConditions == null ? State.Success : State.Unknown);
        }
    }
}

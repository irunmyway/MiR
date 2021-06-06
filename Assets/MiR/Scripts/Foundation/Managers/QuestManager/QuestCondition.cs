using UnityEngine;

namespace Foundation
{
    public abstract class QuestCondition : ScriptableObject
    {
        public abstract bool IsTrue(QuestManager questManager);
    }
}

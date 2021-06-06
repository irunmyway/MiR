using UnityEngine;

namespace Foundation
{
    [CreateAssetMenu(menuName="OTUS/Level Up Thresholds")]
    public sealed class LevelUpThresholds : ScriptableObject
    {
        public int[] ExperienceLevels;

        // Для использования в редакторе
        [SerializeField] int maxLevel;
        [SerializeField] int level2Experience;
        [SerializeField] float multiplier;
    }
}

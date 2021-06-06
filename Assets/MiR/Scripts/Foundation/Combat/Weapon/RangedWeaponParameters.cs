using System;
using UnityEngine;

namespace Foundation
{
    [Serializable]
    public struct RangedWeaponParameters
    {
        // Количество выстрелов
        public int Count;

        // Максимальное расстояние выстрела
        public float MaxDistance;

        // Минимальный и максимальный угол точности.
        // Чем выше получившийся угол, тем меньше точность.
        public float MinTargetAngle;
        public float MaxTargetAngle;

        // Минимальный и максимальный угол группировки.
        // Чем выше получившийся угол, тем больше разброс.
        public float MinGroupingAngle;
        public float MaxGroupingAngle;
    }
}

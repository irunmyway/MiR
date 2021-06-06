using System.Collections;
using UnityEngine;

namespace Foundation
{
    [CreateAssetMenu(menuName = "OTUS/Character Effects/Healing")]
    public sealed class CharacterHealingEffect : AbstractCharacterEffect
    {
        public override Mode ApplyMode => Mode.Prolong;

        public float Duration;
        public float HealPerSec;

        public override IEnumerator Apply(ICharacterEffectManager manager, ICharacterEffectState state)
        {
            float time = 0.0f;
            do {
                yield return null;

                if (manager.Health != null)
                    manager.Health.Heal(state, HealPerSec * state.TimeDelta);

                time += state.TimeDelta;
            } while (time < Duration);
        }
    }
}

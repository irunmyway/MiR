using System.Collections;
using UnityEngine;

namespace Foundation
{
    [CreateAssetMenu(menuName = "OTUS/Character Effects/Burning")]
    public sealed class CharacterBurningEffect : AbstractCharacterEffect
    {
        public override Mode ApplyMode => Mode.Prolong;

        public float Duration;
        public float DamagePerSec;

        public override IEnumerator Apply(ICharacterEffectManager manager, ICharacterEffectState state)
        {
            float time = 0.0f;
            do {
                yield return null;

                if (manager.Health != null)
                    manager.Health.Damage(state, DamagePerSec * state.TimeDelta);

                time += state.TimeDelta;
            } while (time < Duration);
        }
    }
}

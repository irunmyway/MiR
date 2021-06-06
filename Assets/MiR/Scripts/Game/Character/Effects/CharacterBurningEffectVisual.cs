using UnityEngine;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class CharacterBurningEffectVisual : AbstractBehaviour, IOnCharacterEffectStarted, IOnCharacterEffectEnded
    {
        ParticleSystem particles;
        [Inject] ICharacterEffectManager effectManager = default;

        void Awake()
        {
            particles = GetComponent<ParticleSystem>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(effectManager.OnEffectStarted);
            Observe(effectManager.OnEffectEnded);
        }

        void IOnCharacterEffectStarted.Do(AbstractCharacterEffect effect)
        {
            if (effect is CharacterBurningEffect)
                particles.Play();
        }

        void IOnCharacterEffectEnded.Do(AbstractCharacterEffect effect)
        {
            if (effect is CharacterBurningEffect)
                particles.Stop();
        }
    }
}

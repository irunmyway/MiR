using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class ExplodeOnContact : AbstractBehaviour, IAttacker
    {
        [InjectOptional] IPlayer player = default;
        public IPlayer Player => player;

        [InjectOptional] IEnemy enemy = default;
        public IEnemy Enemy => enemy;

        public AbstractCharacterEffect Effect => null;

        public float Radius = 1.0f;
        public float Damage = 1.0f;
        public LayerMask LayerMask;
        public ParticleSystem[] Effects;

        void OnCollisionEnter(Collision c)
        {
            if (Effects != null) {
                foreach (var effect in Effects) {
                    effect.transform.SetParent(null);
                    effect.Play();
                }
            }

            var r = Physics.OverlapSphere(transform.position, Radius, LayerMask.value, QueryTriggerInteraction.Collide);
            foreach (var collider in r) {
                var context = collider.GetComponentInParent<Context>();
                if (context != null) {
                    var health = context.Container.TryResolve<ICharacterHealth>();
                    if (health != null)
                        health.Damage(this, Damage);
                }
            }
        }

        public void Revert()
        {
            foreach (var effect in Effects) {
                effect.Stop();
                effect.transform.SetParent(transform);
            }
        }
    }
}

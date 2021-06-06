using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public abstract class AbstractProjectile : AbstractBehaviour, IPoolable<Transform, IMemoryPool>
    {
        public sealed class Factory : PlaceholderFactory<Transform, AbstractProjectile>
        {
        }

        IMemoryPool pool;
        Transform originalParent;

        public float Damage { get; private set; }
        public bool Launched { get; private set; }

        public ExplodeOnContact ExplodeOnContact;
        public ParticleSystem[] Effects;

        void Start()
        {
            originalParent = transform.parent;
            EnableEffects(false);
        }

        void EnableEffects(bool flag)
        {
            if (Effects != null) {
                foreach (var effect in Effects) {
                    if (flag)
                        effect.Play();
                    else
                        effect.Stop();
                }
            }
        }

        public virtual void Launch(float damage)
        {
            Launched = true;
            Damage = damage;
            transform.SetParent(null);
            EnableEffects(true);

            if (ExplodeOnContact != null)
                ExplodeOnContact.Damage = damage;
        }

        public void OnSpawned(Transform where, IMemoryPool pool)
        {
            this.pool = pool;

            if (ExplodeOnContact != null)
                ExplodeOnContact.Revert();

            transform.SetParent(where);
            transform.position = where.position;
            transform.rotation = where.rotation;

            gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            pool = null;
            Launched = false;
            gameObject.SetActive(false);
            transform.SetParent(originalParent);
            EnableEffects(false);
        }

        void OnCollisionEnter(Collision c)
        {
            /*
            if (Launched) {
                if (pool != null)
                    pool.Despawn(this);
            }
            */
        }
    }
}

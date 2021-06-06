using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class Player : AbstractService<IPlayer>, IPlayer, IOnCharacterHealed, IOnCharacterDamaged, IOnCharacterDied
    {
        int index = -1;
        public int Index => index;

        public Vector3 Position => transform.position;

        [InjectOptional] ICharacterHealth health = default;
        public ICharacterHealth Health => health;

        [InjectOptional] ICharacterAgent agent = default;
        public ICharacterAgent Agent => agent;

        [InjectOptional] ICharacterVehicle vehicle = default;
        public ICharacterVehicle Vehicle => vehicle;

        [InjectOptional] ICharacterRain rain = default;
        public ICharacterRain Rain => rain;

        [InjectOptional] IInventory inventory = default;
        public IInventory Inventory => inventory;

        [Inject] IPlayerManager playerManager = default;

        [SerializeField] Sprite portrait;
        public Sprite Portrait => portrait;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (health != null) {
                Observe(health.OnHealed);
                Observe(health.OnDamaged);
                Observe(health.OnDied);
            }

            playerManager.AddPlayer(this, out index);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            playerManager.RemovePlayer(this);
            index = -1;
        }

        void IOnCharacterHealed.Do(ICharacterHealth health, IAttacker attacker, float amount, float newHealth)
        {
            foreach (var it in playerManager.OnPlayerHealed.Enumerate())
                it.Do(index, attacker, amount, newHealth);
        }

        void IOnCharacterDamaged.Do(ICharacterHealth health, IAttacker attacker, float amount, float newHealth)
        {
            foreach (var it in playerManager.OnPlayerDamaged.Enumerate())
                it.Do(index, attacker, amount, newHealth);
        }

        void IOnCharacterDied.Do(ICharacterHealth health, IAttacker attacker)
        {
            foreach (var it in playerManager.OnPlayerDied.Enumerate())
                it.Do(index, attacker);
        }
    }
}

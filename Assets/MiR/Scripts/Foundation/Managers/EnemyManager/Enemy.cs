using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class Enemy : AbstractService<IEnemy, ICharacterCrouchInput>, IEnemy, IOnUpdate
    {
        public const int MinimumPriority = 0;

        [InjectOptional] ICharacterAgent agent = default;
        public ICharacterAgent Agent => agent;

        [InjectOptional] ICharacterHealth health = default;
        public ICharacterHealth Health => health;

        [InjectOptional] ICharacterWeapon weapon = default;
        public ICharacterWeapon Weapon => weapon;

        public Vector3 Position => transform.position;

        public bool Crouching => (activeBehaviour != null ? activeBehaviour.Crouching : false);

        public ObserverList<IOnEnemySeenPlayer> OnSeenPlayer { get; } = new ObserverList<IOnEnemySeenPlayer>();
        public ObserverList<IOnEnemyLostPlayer> OnLostPlayer { get; } = new ObserverList<IOnEnemyLostPlayer>();
        public ObserverList<IOnEnemyEnterAlertState> OnEnterAlertState { get; } = new ObserverList<IOnEnemyEnterAlertState>();
        public ObserverList<IOnEnemyLeaveAlertState> OnLeaveAlertState { get; } = new ObserverList<IOnEnemyLeaveAlertState>();
        public ObserverList<IOnEnemyDidAttackPlayer> OnDidAttackPlayer { get; }  = new ObserverList<IOnEnemyDidAttackPlayer>();

        [InjectOptional] IPlayerDetector playerDetector = default;

        [Inject] ISceneState sceneState = default;
        [Inject] IEnemyManager enemyManager = default;

        [SerializeField] List<EnemyBehaviour> defaultBehaviours;
        List<(EnemyBehaviour behaviour, int priority)> behaviours = new List<(EnemyBehaviour, int)>();

        public bool IsAlert { get; private set; }
        public IPlayer SeenPlayer { get; private set; }

        [SerializeField] [ReadOnly] EnemyBehaviour activeBehaviour;

        void Awake()
        {
            if (defaultBehaviours != null) {
                int priority = MinimumPriority - defaultBehaviours.Count;
                foreach (var b in defaultBehaviours)
                    behaviours.Insert(0, (b, priority++));
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
            enemyManager.AddEnemy(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            enemyManager.RemoveEnemy(this);
        }

        void IOnUpdate.Do(float deltaTime)
        {
            if (health != null && health.IsDead) {
                if (activeBehaviour != null) {
                    activeBehaviour.DeactivateAI();
                    activeBehaviour = null;
                }
                return;
            }

            IPlayer seesPlayer = null;
            if (playerDetector != null)
                seesPlayer = playerDetector.FindTargetPlayer();

            if (seesPlayer != SeenPlayer) {
                if (SeenPlayer != null) {
                    foreach (var it in OnLostPlayer.Enumerate())
                        it.Do(SeenPlayer);
                }

                SeenPlayer = seesPlayer;

                if (SeenPlayer != null) {
                    foreach (var it in OnSeenPlayer.Enumerate())
                        it.Do(SeenPlayer);
                }
            }

            EnemyBehaviour selectedBehaviour = null;
            foreach (var it in behaviours) {
                if (it.behaviour.CheckUpdateAI(deltaTime)) {
                    if (selectedBehaviour == null)
                        selectedBehaviour = it.behaviour;
                }
            }

            if (selectedBehaviour != activeBehaviour) {
                if (activeBehaviour != null)
                    activeBehaviour.DeactivateAI();

                activeBehaviour = selectedBehaviour;

                if (activeBehaviour != null)
                    activeBehaviour.ActivateAI();
            }

            if (selectedBehaviour != null)
                selectedBehaviour.UpdateAI(deltaTime);
        }

        public void EnterAlertState()
        {
            if (!IsAlert) {
                IsAlert = true;
                foreach (var it in OnEnterAlertState.Enumerate())
                    it.Do();
            }
        }

        public void LeaveAlertState()
        {
            if (IsAlert) {
                IsAlert = false;
                foreach (var it in OnLeaveAlertState.Enumerate())
                    it.Do();
            }
        }

        public bool CanAttackPlayer(IPlayer target)
        {
            if (target == null || Weapon == null)
                return false;

            if (!enemyManager.EnemyCanAttack(this))
                return false;

            return Weapon.CanAttack();
        }

        public bool TryAttackPlayer(IPlayer target)
        {
            if (target == null)
                return false;

            if (Weapon != null && Weapon.CanAttack()) {
                Weapon.Attack();
                foreach (var it in OnDidAttackPlayer.Enumerate())
                    it.Do(this);
                return true;
            }

            return false;
        }

        public void AddBehaviour(EnemyBehaviour behaviour, int priority)
        {
            if (priority < MinimumPriority)
                priority = MinimumPriority;

            bool found = false;
            int n = behaviours.Count;
            while (n-- > 0) {
                var b = behaviours[n];
                if (b.behaviour == behaviour) {
                    if (b.priority == priority)
                        return;
                    b.priority = priority;
                    behaviours[n] = b;
                    found = true;
                    break;
                }
            }

            if (!found)
                behaviours.Add((behaviour, priority));

            behaviours.Sort((a, b) => {
                    if (a.priority < b.priority)
                        return 1;
                    else if (a.priority > b.priority)
                        return -1;
                    else
                        return 0;
                });
        }

        public void RemoveBehaviour(EnemyBehaviour behaviour)
        {
            behaviours.RemoveAll(x => x.behaviour == behaviour);
        }
    }
}

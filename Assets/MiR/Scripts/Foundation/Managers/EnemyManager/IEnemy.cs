using UnityEngine;

namespace Foundation
{
    public interface IEnemy : ICharacterCrouchInput
    {
        ObserverList<IOnEnemySeenPlayer> OnSeenPlayer { get; }
        ObserverList<IOnEnemyLostPlayer> OnLostPlayer { get; }

        ObserverList<IOnEnemyEnterAlertState> OnEnterAlertState { get; }
        ObserverList<IOnEnemyLeaveAlertState> OnLeaveAlertState { get; }

        ObserverList<IOnEnemyDidAttackPlayer> OnDidAttackPlayer { get; }

        public ICharacterAgent Agent { get; }
        public ICharacterHealth Health { get; }
        public ICharacterWeapon Weapon { get; }

        public Vector3 Position { get; }

        bool IsAlert { get; }
        IPlayer SeenPlayer { get; }

        void EnterAlertState();
        void LeaveAlertState();

        bool CanAttackPlayer(IPlayer target);
        bool TryAttackPlayer(IPlayer target);

        void AddBehaviour(EnemyBehaviour behaviour, int priority);
        void RemoveBehaviour(EnemyBehaviour behaviour);
   }
}

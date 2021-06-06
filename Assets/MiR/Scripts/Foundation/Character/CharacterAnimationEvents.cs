namespace Foundation
{
    public sealed class CharacterAnimationEvents : AbstractService<CharacterAnimationEvents>
    {
        public readonly ObserverList<IOnAttackAnimationEnded> OnAttackEnded = new ObserverList<IOnAttackAnimationEnded>();
        public readonly ObserverList<IOnEnteringCarAnimationEnded> OnEnteringCarAnimationEnded = new ObserverList<IOnEnteringCarAnimationEnded>();
        public readonly ObserverList<IOnExitingCarAnimationEnded> OnExitingCarAnimationEnded = new ObserverList<IOnExitingCarAnimationEnded>();
        public readonly ObserverList<IOnOpenDoorAnimation> OnOpenDoorAnimation = new ObserverList<IOnOpenDoorAnimation>();
        public readonly ObserverList<IOnCloseDoorAnimation> OnCloseDoorAnimation = new ObserverList<IOnCloseDoorAnimation>();

        void AttackEnded()
        {
            foreach (var it in OnAttackEnded.Enumerate())
                it.Do();
        }

        void DoneEnteringCar()
        {
            foreach (var it in OnEnteringCarAnimationEnded.Enumerate())
                it.Do();
        }

        void DoneExitingCar()
        {
            foreach (var it in OnExitingCarAnimationEnded.Enumerate())
                it.Do();
        }

        void OpenDoor()
        {
            foreach (var it in OnOpenDoorAnimation.Enumerate())
                it.Do();
        }

        void CloseDoor()
        {
            foreach (var it in OnCloseDoorAnimation.Enumerate())
                it.Do();
        }
    }
}

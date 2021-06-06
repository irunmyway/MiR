using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class CharacterVehicle : AbstractService<ICharacterVehicle>, ICharacterVehicle,
        IOnEnteringCarAnimationEnded, IOnExitingCarAnimationEnded, IOnOpenDoorAnimation, IOnCloseDoorAnimation
    {
        public IVehicle CurrentVehicle { get; private set; }
        public VehicleEntrance VehicleEntrance { get; private set; }
        public CharacterVehicleState State { get; private set; } = CharacterVehicleState.NotInVehicle;
        public IPlayer Player => player;

        Transform characterOriginalParent;
        public Transform CharacterTransform;

        [Inject] CharacterAnimationEvents events = default;
        [InjectOptional] ICharacterRigidbody rigidBody = default;

        // FIXME
        //[InjectOptional] IPlayer player = default;
        IPlayer player = default;

        void Awake()
        {
            player = GetComponentInParent<Player>(); // FIXME: использовать Zenject
            characterOriginalParent = CharacterTransform.parent;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(events.OnEnteringCarAnimationEnded);
            Observe(events.OnExitingCarAnimationEnded);
            Observe(events.OnOpenDoorAnimation);
            Observe(events.OnCloseDoorAnimation);
        }

        public bool TryEnterVehicle(VehicleEntrance entrance)
        {
            DebugOnly.Check(entrance != null, "NULL entrance.");
            DebugOnly.Check(entrance.Car != null, "NULL car.");

            if (CurrentVehicle != null)
                return false;
            if (entrance.CharacterVehicle != null)
                return false;

            VehicleEntrance = entrance;
            CurrentVehicle = entrance.Car;
            State = CharacterVehicleState.Entering;

            CharacterTransform.SetParent(entrance.Seat, false);
            CharacterTransform.position = entrance.transform.position;
            CharacterTransform.rotation = entrance.transform.rotation;
            VehicleEntrance.CharacterVehicle = this;
            rigidBody.Enabled = false;

            return true;
        }

        public bool TryExitVehicle()
        {
            if (CurrentVehicle == null || State == CharacterVehicleState.Exiting)
                return false;

            State = CharacterVehicleState.Exiting;

            return true;
        }

        void IOnEnteringCarAnimationEnded.Do()
        {
            if (VehicleEntrance.DriverSeat)
                State = CharacterVehicleState.Driving;
            else
                State = CharacterVehicleState.Sitting;

            CharacterTransform.position = VehicleEntrance.Seat.transform.position;
            CharacterTransform.rotation = VehicleEntrance.Seat.transform.rotation;
        }

        void IOnExitingCarAnimationEnded.Do()
        {
            State = CharacterVehicleState.NotInVehicle;
            VehicleEntrance.CharacterVehicle = null;
            CurrentVehicle = null;
            VehicleEntrance = null;
            CharacterTransform.SetParent(characterOriginalParent);
            rigidBody.Enabled = true;
        }

        void IOnOpenDoorAnimation.Do()
        {
            var door = VehicleEntrance.Door;
            door.DOLocalRotate(VehicleEntrance.openDoorAngles, 1.0f);
        }

        void IOnCloseDoorAnimation.Do()
        {
            var door = VehicleEntrance.Door;
            door.DOLocalRotate(VehicleEntrance.closedDoorAngles, 1.0f);
        }
    }
}

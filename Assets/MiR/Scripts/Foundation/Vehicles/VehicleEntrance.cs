using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public sealed class VehicleEntrance : MonoBehaviour
    {
        public bool DriverSeat;
        public bool LeftSide;
        public Car Car;
        public Transform Door;
        public Transform Seat;
        public ICharacterVehicle CharacterVehicle;
        public int DoorOpenAngle = 60;

        public Vector3 closedDoorAngles { get; private set; }
        public Vector3 openDoorAngles { get; private set; }

        void Awake()
        {
            closedDoorAngles = Door.localEulerAngles;
            openDoorAngles = closedDoorAngles + (LeftSide ? 1.0f : -1.0f) * new Vector3(0, 0, DoorOpenAngle);
        }
    }
}

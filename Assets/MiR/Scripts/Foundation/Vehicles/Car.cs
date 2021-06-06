using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class Car : AbstractService<IVehicle>, IVehicle, IOnFixedUpdate, IOnLateUpdate
    {
        public enum Drive
        {
            Front,
            Back,
            FourWheel,
        };

        public Drive Transmission;
        public float Acceleration;
        public float BrakesPower;
        public float MaxSteeringAngle;
        public Wheel[] FrontWheels;
        public Wheel[] BackWheels;
        public VehicleEntrance[] Entrances;
        public Camera MirrorCamera;
        public GameObject Mirror;
        public Transform CenterOfMass;

        [Range(-1, 1)] [SerializeField] float forward;
        public float Forward { get { return forward; } set { forward = value; } }

        [Range(-1, 1)] [SerializeField] float turn;
        public float Turn { get { return turn; } set { turn = value; } }

        [Range(-1, 1)] [SerializeField] float brakes;
        public float Brakes { get { return brakes; } set { brakes = value; } }

        [ReadOnly] [SerializeField] float speedKmh;
        public float SpeedKmh => speedKmh;

        public Vector3 Position { get { return transform.position; } set { transform.position = value; } }
        public Quaternion Rotation { get { return transform.rotation; } set { transform.rotation = value; } }

        Rigidbody rigidBody;
        bool hasPassengers;

        [Inject] ISceneState state = default;

        void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.centerOfMass = CenterOfMass.localPosition;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(state.OnFixedUpdate);
            Observe(state.OnLateUpdate);
        }

        void IOnFixedUpdate.Do()
        {
            float torque = -forward * Acceleration;
            if (Transmission == Drive.Front || Transmission == Drive.FourWheel) {
                foreach (var wheel in FrontWheels)
                    wheel.collider.motorTorque = torque;
            }
            if (Transmission == Drive.Back || Transmission == Drive.FourWheel) {
                foreach (var wheel in BackWheels)
                    wheel.collider.motorTorque = torque;
            }

            float breakTorque = brakes * BrakesPower;
            foreach (var wheel in BackWheels)
                wheel.collider.brakeTorque = breakTorque;

            float steering = turn * MaxSteeringAngle;
            foreach (var wheel in FrontWheels)
                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, steering, 0.5f);
        }

        void IOnLateUpdate.Do(float timeDelta)
        {
            speedKmh = rigidBody.velocity.magnitude * 3.6f;     // m/s => km/h

            bool newHasPassengers = false;
            foreach (var entrance in Entrances) {
                if (entrance.CharacterVehicle != null) {
                    newHasPassengers = true;
                    break;
                }
            }

            if (newHasPassengers != hasPassengers) {
                hasPassengers = newHasPassengers;
                MirrorCamera.gameObject.SetActive(hasPassengers);
                Mirror.SetActive(hasPassengers);
            }
        }

        public void FullStop()
        {
            forward = 0.0f;
            turn = 0.0f;
            brakes = 1.0f;
            rigidBody.velocity = Vector3.zero;
        }

        public VehicleEntrance[] GetEntrances()
        {
            return Entrances;
        }
    }
}

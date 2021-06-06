using UnityEngine;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class FloatingObject : AbstractBehaviour, IOnFixedUpdate
    {
        public float buoyancy = 3.0f;
        public float waveLength = 2.0f;
        public float waveAmplitude = 0.1f;
        public float waveSpeed = 1.0f;
        public float waterLevel = 0.0f;
        public float dragInAir = 0.0f;
        public float dragInWater = 5.0f;
        public Transform[] buoyancyPoints;

        [Inject] ISceneState sceneState = default;

        Rigidbody rigidBody;
        float waveOffset;

        void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnFixedUpdate);
        }

        void IOnFixedUpdate.Do()
        {
            waveOffset += waveSpeed * Time.fixedDeltaTime;

            float waterLine = 0.0f;
            int n = (buoyancyPoints != null ? buoyancyPoints.Length : 0);
            if (n == 0)
                DoPoint(transform.position, 1, ref waterLine);
            else {
                foreach (var point in buoyancyPoints)
                    DoPoint(point.position, n, ref waterLine);
            }

            bool inWater = transform.position.y + rigidBody.centerOfMass.y < waterLine;
            rigidBody.drag = (inWater ? dragInWater : dragInAir);
            rigidBody.angularDrag = (inWater ? dragInWater : dragInAir);
        }

        void DoPoint(Vector3 position, int numPoints, ref float waterLine)
        {
            rigidBody.AddForceAtPosition(Physics.gravity / numPoints, position, ForceMode.Acceleration);

            float waveHeight = waterLevel + waveAmplitude * Mathf.Sin(position.x / waveLength + waveOffset);
            waterLine += waveHeight / numPoints;

            float pointHeight = position.y;
            if (pointHeight < waveHeight) {
                float coeff = Mathf.Clamp01((waveHeight - pointHeight) / waveAmplitude) * buoyancy;
                rigidBody.AddForceAtPosition(-Physics.gravity * coeff, position, ForceMode.Acceleration);
            }
        }
    }
}

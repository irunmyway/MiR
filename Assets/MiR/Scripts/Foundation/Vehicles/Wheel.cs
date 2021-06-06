using UnityEngine;

namespace Foundation
{
    public sealed class Wheel : MonoBehaviour
    {
        public new WheelCollider collider;

        void LateUpdate()
        {
            collider.GetWorldPose(out Vector3 pos, out Quaternion rotation);
            transform.position = pos;
            transform.rotation = rotation;
        }
    }
}

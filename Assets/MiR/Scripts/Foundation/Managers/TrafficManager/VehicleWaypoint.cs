using UnityEngine;

namespace Foundation
{
    public sealed class VehicleWaypoint : AbstractBehaviour
    {
        public VehicleWaypoint Next;
        public TrafficLights TrafficLights;
        public bool ForRacing;

        public int VehicleCount { get; set; }

        void OnDrawGizmos()
        {
            if (Next != null) {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, Next.transform.position);
            }

            if (TrafficLights != null) {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, TrafficLights.transform.position);
            }
        }
    }
}

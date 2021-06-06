using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public sealed class PedestrianWaypoint : AbstractBehaviour
    {
        public PedestrianWaypoint Next;
        public PedestrianWaypoint Prev { get; private set; }
        public PedestrianWaypoint AlternativePrev { get; private set; }
        public PedestrianWaypoint AlternativeNext;

        public int PedestrianCount { get; set; }

        void Awake()
        {
            if (Next != null)
                Next.Prev = this;
            if (AlternativeNext != null)
                AlternativeNext.AlternativePrev = this;
        }

        void OnDrawGizmos()
        {
            if (Next != null) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, Next.transform.position);
            }

            if (AlternativeNext != null) {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, AlternativeNext.transform.position);
            }
        }
    }
}

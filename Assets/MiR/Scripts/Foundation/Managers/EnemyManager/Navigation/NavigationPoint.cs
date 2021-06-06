using Roy_T.AStar.Graphs;
using UnityEngine;

namespace Foundation
{
    public class NavigationPoint : MonoBehaviour
    {
        public NavigationPoint[] Connected;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            if (Connected != null) {
                Vector3 pos = transform.position;
                foreach (var point in Connected) {
                    if (point != null)
                        Gizmos.DrawLine(pos, point.transform.position);
                }
            }
        }
    }
}

using System;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class PatrollingBehaviour : EnemyBehaviour
    {
        [Inject] IEnemy enemy = default;
        [Inject] ICharacterAgent agent = default;

        [SerializeField] WayPoint[] waypoints;
        int currentWaypoint;
        float waitTimeLeft;
        bool waiting;

        void Start()
        {
            ArrivedToWaypoint();
        }

        void ArrivedToWaypoint()
        {
            waiting = true;
            waitTimeLeft = waypoints[currentWaypoint].WaitTime;
            agent.Stop();
        }

        void GoToCurrentWaypoint()
        {
            var pos = waypoints[currentWaypoint].transform.position;
            agent.NavigateTo(new Vector2(pos.x, pos.z));
        }

        public override bool CheckUpdateAI(float deltaTime)
        {
            return enabled;
        }

        public override void ActivateAI()
        {
            if (!waiting)
                GoToCurrentWaypoint();
        }

        public override void DeactivateAI()
        {
            agent.Stop();
        }

        public override void UpdateAI(float deltaTime)
        {
            if (waiting) {
                agent.Stop();
                waitTimeLeft -= deltaTime;
                if (waitTimeLeft > 0.0f)
                    return;
                waiting = false;
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                GoToCurrentWaypoint();
            }

            float distance = Vector3.Distance(enemy.Position, waypoints[currentWaypoint].transform.position);
            if (distance < 0.1f)
                ArrivedToWaypoint();
        }
    }
}

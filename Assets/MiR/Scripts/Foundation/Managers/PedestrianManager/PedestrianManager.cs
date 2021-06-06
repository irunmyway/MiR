using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class PedestrianManager : AbstractService<IPedestrianManager>, IPedestrianManager, IOnUpdate
    {
        public Pedestrian PedestrianPrefab;
        public int MaxPedestrians;

        public float VisibilityDistance = 10.0f;

        [Inject] ISceneState sceneState = default;
        [Inject] IPlayerManager playerManager = default;
        [Inject] DiContainer container = default;

        Stack<Pedestrian> availablePedestrians;
        List<Pedestrian> activePedestrians;
        PedestrianWaypoint[] waypoints;

        public override void Start()
        {
            base.Start();

            waypoints = FindObjectsOfType<PedestrianWaypoint>();

            activePedestrians = new List<Pedestrian>(MaxPedestrians);
            availablePedestrians = new Stack<Pedestrian>(MaxPedestrians);
            for (int i = 0; i < MaxPedestrians; i++) {
                var pedestrianObject = container.InstantiatePrefab(PedestrianPrefab);
                pedestrianObject.SetActive(false);
                availablePedestrians.Push(pedestrianObject.GetComponent<Pedestrian>());
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
        }

        int frameCounter;

        void IOnUpdate.Do(float timeDelta)
        {
            ++frameCounter;
            if (frameCounter < 5)
                return;
            frameCounter = 0;

            IPlayer player = playerManager.GetPlayer(0);
            Vector3 playerPos = player.Position;

            float distanceSqr = VisibilityDistance * VisibilityDistance;

            if (availablePedestrians.Count > 0) {
                foreach (var waypoint in waypoints) {
                    Vector3 pos = waypoint.transform.position;
                    if ((pos - playerPos).sqrMagnitude <= distanceSqr) {
                        if (waypoint.PedestrianCount < 1) {
                            var pedestrian = availablePedestrians.Pop();
                            pedestrian.transform.position = pos;
                            pedestrian.gameObject.SetActive(true);
                            pedestrian.StartFromWaypoint(waypoint);
                            activePedestrians.Add(pedestrian);

                            if (availablePedestrians.Count == 0)
                                break;
                        }
                    }
                }
            }

            int n = activePedestrians.Count;
            int i = n;
            while (i-- > 0) {
                Pedestrian pedestrian = activePedestrians[i];
                Vector3 dir = pedestrian.transform.position - playerPos;
                if (dir.sqrMagnitude > distanceSqr) {
                    if (i < n - 1)
                        activePedestrians[i] = activePedestrians[n - 1];
                    activePedestrians.RemoveAt(n - 1);
                    --n;

                    pedestrian.gameObject.SetActive(false);
                    availablePedestrians.Push(pedestrian);
                }
            }
        }
    }
}

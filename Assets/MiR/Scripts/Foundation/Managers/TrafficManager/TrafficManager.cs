using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class TrafficManager : AbstractService<ITrafficManager>, ITrafficManager, IOnUpdate
    {
        public VehicleAI[] VehiclePrefabs;
        public int MaxVehicles = 30;

        public float VisibilityDistance = 50.0f;

        [Inject] ISceneState sceneState = default;
        [Inject] IPlayerManager playerManager = default;
        [Inject] DiContainer container = default;

        Stack<VehicleAI> availableVehicles;
        List<VehicleAI> activeVehicles;
        VehicleWaypoint[] waypoints;

        public override void Start()
        {
            base.Start();

            var allWaypoints = FindObjectsOfType<VehicleWaypoint>();
            List<VehicleWaypoint> nonRacingWaypoints = new List<VehicleWaypoint>(allWaypoints.Length);
            foreach (var waypoint in allWaypoints) {
                if (!waypoint.ForRacing)
                    nonRacingWaypoints.Add(waypoint);
            }
            waypoints = nonRacingWaypoints.ToArray();

            activeVehicles = new List<VehicleAI>(MaxVehicles);
            availableVehicles = new Stack<VehicleAI>(MaxVehicles);
            for (int i = 0; i < MaxVehicles; i++) {
                var prefab = VehiclePrefabs[Random.Range(0, VehiclePrefabs.Length)];
                var vehicleObject = container.InstantiatePrefab(prefab);
                vehicleObject.SetActive(false);
                availableVehicles.Push(vehicleObject.GetComponent<VehicleAI>());
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
        }

        void IOnUpdate.Do(float timeDelta)
        {
            IPlayer player = playerManager.GetPlayer(0);
            Vector3 playerPos = player.Position;

            float distanceSqr = VisibilityDistance * VisibilityDistance;

            if (availableVehicles.Count > 0) {
                foreach (var waypoint in waypoints) {
                    Vector3 pos = waypoint.transform.position;
                    if ((pos - playerPos).sqrMagnitude <= distanceSqr) {
                        if (waypoint.VehicleCount < 1) {
                            var vehicle = availableVehicles.Pop();
                            vehicle.transform.position = pos;
                            vehicle.transform.rotation = waypoint.transform.rotation;
                            vehicle.gameObject.SetActive(true);
                            vehicle.StartFromWaypoint(waypoint);
                            activeVehicles.Add(vehicle);

                            if (availableVehicles.Count == 0)
                                break;
                        }
                    }
                }
            }

            int n = activeVehicles.Count;
            int i = n;
            while (i-- > 0) {
                VehicleAI vehicle = activeVehicles[i];
                Vector3 dir = vehicle.transform.position - playerPos;
                if (dir.sqrMagnitude > distanceSqr) {
                    if (i < n - 1)
                        activeVehicles[i] = activeVehicles[n - 1];
                    activeVehicles.RemoveAt(n - 1);
                    --n;

                    vehicle.gameObject.SetActive(false);
                    availableVehicles.Push(vehicle);
                }
            }
        }
    }
}

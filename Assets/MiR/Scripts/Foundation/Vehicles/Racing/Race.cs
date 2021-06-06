using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class Race : AbstractBehaviour, IOnUpdate
    {
        public RaceTrigger Trigger;
        public int Laps = 2;
        public VehicleAI[] Enemies;
        public VehicleWaypoint FirstWaypoint;
        public int Counter = 3;

        List<VehicleWaypoint> waypoints;
        IPlayer racingPlayer;
        int playerNextWaypoint;

        float counterTimer;

        [Inject] IInputManager inputManager = default;
        [Inject] INotificationManager notificationManager = default;
        [Inject] ISceneState sceneState = default;

        public void Start()
        {
            foreach (var enemy in Enemies) {
                enemy.Race = this;
                enemy.enabled = false;
            }

            waypoints = new List<VehicleWaypoint>();
            VehicleWaypoint wp = FirstWaypoint;
            do {
                waypoints.Add(wp);
                wp = wp.Next;
            } while (wp != FirstWaypoint);
        }

        public VehicleWaypoint FindClosestWaypoint(Vector3 target)
        {
            float closestDistance = float.MaxValue;
            int closestWaypoint = -1;

            for (int i = 0; i < waypoints.Count; i++) {
                float distance = Vector3.Distance(waypoints[i].transform.position, target);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestWaypoint = i;
                }
            }

            return waypoints[closestWaypoint];
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
        }

        public void OnPlayerEnterTrigger(IPlayer player)
        {
            if (racingPlayer == null) {
                //player.Vehicle.CurrentVehicle.FullStop();
                player.Vehicle.CurrentVehicle.Position = Trigger.transform.position;
                player.Vehicle.CurrentVehicle.Rotation = Trigger.transform.rotation;

                playerNextWaypoint = waypoints.IndexOf(FirstWaypoint);
                Trigger.transform.position = FirstWaypoint.transform.position;

                inputManager.OverrideInputForPlayer(player.Index, DummyInputSource.Instance);

                racingPlayer = player;
                counterTimer = 0.0f;
                UpdateCounterTimer(0.0f);

                return;
            }

            if (racingPlayer != player)
                return;

            playerNextWaypoint = (playerNextWaypoint + 1) % waypoints.Count;
            Trigger.transform.position = waypoints[playerNextWaypoint].transform.position;

            if (waypoints[playerNextWaypoint] == FirstWaypoint) {
                --Laps;
                if (Laps > 0)
                    notificationManager.DisplayMessage($"{Laps} lap(s) left..."); // FIXME: localization
                else {
                    racingPlayer = null;
                    foreach (var enemy in Enemies)
                        enemy.gameObject.SetActive(false);
                    Trigger.gameObject.SetActive(false);
                    notificationManager.DisplayMessage("Race Complete!"); // FIXME: localization
                }
            }
        }

        void IOnUpdate.Do(float deltaTime)
        {
            if (racingPlayer == null)
                return;

            if (counterTimer > 0.0f)
                UpdateCounterTimer(deltaTime);

            // FIXME: выделение памяти каждый кадр - это плохо
            var positions = new List<(float distance, RacePosition positionComponent)>();
            foreach (var enemy in Enemies)
                positions.Add((DeterminePosition(enemy.nextWaypoint, enemy.transform.position), enemy.GetComponent<RacePosition>()));

            var nextWaypoint = waypoints[playerNextWaypoint];
            positions.Add((DeterminePosition(nextWaypoint, racingPlayer.Position), ((Player)racingPlayer).GetComponent<RacePosition>()));

            // Сортируем в обратном порядке
            positions.Sort((a, b) => {
                    if (a.distance > b.distance)
                        return -1;
                    if (a.distance < b.distance)
                        return 1;
                    return 0;
                });

            int index = 1;
            foreach (var it in positions)
                it.positionComponent.Position = index++;
        }

        float DeterminePosition(VehicleWaypoint nextWaypoint, Vector3 position)
        {
            VehicleWaypoint prevWaypoint;
            int segmentIndex;

            // FIXME: использовать более эффективный способ, чем линейный поиск
            int index = waypoints.IndexOf(nextWaypoint);
            if (index < 0)
                return 0.0f;

            if (index == 0) {
                prevWaypoint = waypoints[waypoints.Count - 1];
                segmentIndex = waypoints.Count - 1;
            } else {
                prevWaypoint = waypoints[index - 1];
                segmentIndex = index - 1;
            }

            float step = 1.0f / waypoints.Count;
            float start = (float)segmentIndex * step;
            float end = start + step;

            Vector3 p1 = prevWaypoint.transform.position;
            Vector3 p2 = nextWaypoint.transform.position;
            float delta = MathUtility.FindNearestPointOnLine(p1, p2, position);

            return start + (end - start) * delta; // + lapsCount
        }

        void StartRace()
        {
            notificationManager.DisplayMessage("GO !"); // FIXME: localization

            inputManager.OverrideInputForPlayer(racingPlayer.Index, null);

            foreach (var enemy in Enemies) {
                enemy.enabled = true;
                enemy.StartFromWaypoint(FirstWaypoint);
            }
        }

        void UpdateCounterTimer(float deltaTime)
        {
            counterTimer -= deltaTime;
            if (counterTimer <= 0.0f) {
                if (Counter == 0)
                    StartRace();
                else {
                    notificationManager.DisplayMessage($"{Counter}");
                    Counter--;
                    counterTimer = 1.0f;
                }
            }
        }
    }
}

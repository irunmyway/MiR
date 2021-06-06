using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class Pedestrian : AbstractBehaviour, IOnUpdate
    {
        [Inject] ISceneState sceneState = default;
        [Inject] ICharacterVisual characterVisual = default;
        [Inject] ICharacterAgent characterAgent = default;

        PedestrianWaypoint nextWaypoint;
        PedestrianWaypoint prevWaypoint;
        bool movingForward;

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (nextWaypoint != null) {
                nextWaypoint.PedestrianCount--;
                nextWaypoint = null;
            }

            if (prevWaypoint != null) {
                prevWaypoint.PedestrianCount--;
                prevWaypoint = null;
            }
        }

        public void StartFromWaypoint(PedestrianWaypoint waypoint)
        {
            movingForward = (Random.Range(0, 2) == 1);
            nextWaypoint = waypoint;
            nextWaypoint.PedestrianCount++;
            characterVisual.Randomize();
            ChooseNextWaypoint();
        }

        void ChooseNextWaypoint()
        {
            PedestrianWaypoint chosenWaypoint = null;
            if (movingForward) {
                if (nextWaypoint.AlternativeNext == null)
                    chosenWaypoint = nextWaypoint.Next;
                else if (Random.Range(0, 2) == 0)
                    chosenWaypoint = nextWaypoint.Next;
                else
                    chosenWaypoint = nextWaypoint.AlternativeNext;

                if (chosenWaypoint == null) {
                    movingForward = false;
                    chosenWaypoint = nextWaypoint.Prev;
                }
            } else {
                if (nextWaypoint.AlternativePrev == null || Random.Range(0, 2) == 0)
                    chosenWaypoint = nextWaypoint.Prev;
                else
                    chosenWaypoint = nextWaypoint.AlternativePrev;

                if (chosenWaypoint == null) {
                    movingForward = true;
                    chosenWaypoint = nextWaypoint.Next;
                }
            }

            if (prevWaypoint != null)
                prevWaypoint.PedestrianCount--;

            prevWaypoint = nextWaypoint;
            nextWaypoint = chosenWaypoint;

            if (chosenWaypoint != null) {
                nextWaypoint.PedestrianCount++;
                Vector3 pos = chosenWaypoint.transform.position;
                characterAgent.NavigateTo(new Vector2(pos.x, pos.z));
            }
        }

        void IOnUpdate.Do(float timeDelta)
        {
            if (nextWaypoint == null)
                return;

            float distance = (transform.position - nextWaypoint.transform.position).sqrMagnitude;
            if (distance < 0.1f)
                ChooseNextWaypoint();
        }
    }
}

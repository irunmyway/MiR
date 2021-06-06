using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class RaceTrigger : AbstractBehaviour
    {
        public Race Race;

        private void OnTriggerEnter(Collider other)
        {
            var context = other.GetComponentInParent<Context>();
            if (context != null) {
                var vehicle = context.Container.TryResolve<IVehicle>();
                if (vehicle != null) {
                    foreach (var entrance in vehicle.GetEntrances()) {
                        if (entrance.DriverSeat) {
                            if (entrance.CharacterVehicle != null && entrance.CharacterVehicle.Player != null)
                                Race.OnPlayerEnterTrigger(entrance.CharacterVehicle.Player);
                            break;
                        }
                    }
                }
            }
        }
    }
}

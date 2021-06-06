using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public interface IVehicle
    {
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }

        float Forward { get; set; }
        float Brakes { get; set; }
        float Turn { get; set; }
        float SpeedKmh { get; }

        void FullStop();
        VehicleEntrance[] GetEntrances();
    }
}

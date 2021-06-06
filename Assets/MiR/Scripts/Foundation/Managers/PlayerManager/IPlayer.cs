using UnityEngine;

namespace Foundation
{
    public interface IPlayer
    {
        int Index { get; }
        ICharacterHealth Health { get; }
        ICharacterAgent Agent { get; }
        ICharacterVehicle Vehicle { get; }
        ICharacterRain Rain { get; }
        IInventory Inventory { get; } 
        Sprite Portrait { get; }
        Vector3 Position { get; }
    }
}

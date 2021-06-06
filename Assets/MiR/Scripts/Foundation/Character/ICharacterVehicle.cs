namespace Foundation
{
    public interface ICharacterVehicle
    {
        IVehicle CurrentVehicle { get; }
        VehicleEntrance VehicleEntrance { get; }
        CharacterVehicleState State { get; }
        IPlayer Player { get; }

        bool TryEnterVehicle(VehicleEntrance vehicleEntrance);
        bool TryExitVehicle();
    }
}

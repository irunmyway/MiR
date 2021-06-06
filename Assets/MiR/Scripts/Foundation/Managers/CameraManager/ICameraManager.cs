using System.Collections.Generic;

namespace Foundation
{
    public interface ICameraManager
    {
        ICollection<ICamera> AllCameras { get; }

        void ToggleFirstThirdPersonCamera(int playerIndex);

        void AddFirstPersonCamera(IFirstPersonCamera camera);
        void RemoveFirstPersonCamera(IFirstPersonCamera camera);

        void AddThirdPersonCamera(IThirdPersonCamera camera);
        void RemoveThirdPersonCamera(IThirdPersonCamera camera);
    }
}

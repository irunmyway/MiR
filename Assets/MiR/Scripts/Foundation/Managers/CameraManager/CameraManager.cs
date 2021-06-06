using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class CameraManager : AbstractService<ICameraManager>, ICameraManager, IOnPlayerAdded, IOnPlayerRemoved
    {
        [Serializable]
        sealed class PerPlayer
        {
            readonly CameraManager manager;
            public bool FirstPerson;

            public ICamera CurrentCamera;
            public List<IFirstPersonCamera> FirstPersonCameras = new List<IFirstPersonCamera>();
            public List<IThirdPersonCamera> ThirdPersonCameras = new List<IThirdPersonCamera>();

            public PerPlayer(CameraManager manager, int playerIndex)
            {
                this.manager = manager;

                foreach (var camera in manager.AllCameras) {
                    if (camera is IFirstPersonCamera firstPerson && firstPerson.PlayerIndex == playerIndex)
                        FirstPersonCameras.Add(firstPerson);
                    if (camera is IThirdPersonCamera thirdPerson && thirdPerson.PlayerIndex == playerIndex)
                        ThirdPersonCameras.Add(thirdPerson);
                }
            }

            ICamera ChooseCamera()
            {
                if (FirstPerson) {
                    if (FirstPersonCameras.Count > 0)
                        return FirstPersonCameras[0];
                    else
                        DebugOnly.Error("No first person cameras in the scene.");
                } else {
                    if (ThirdPersonCameras.Count > 0)
                        return ThirdPersonCameras[0];
                    else
                        DebugOnly.Error("No third person cameras in the scene.");
                }

                return null;
            }

            public bool UpdateCamera()
            {
                ICamera wantedCamera = ChooseCamera();
                if (wantedCamera != CurrentCamera) {
                    CurrentCamera = wantedCamera;
                    return true;
                }
                return false;
            }
        }

        [Inject] IPlayerManager playerManager = default;

        List<ICamera> allCameras = new List<ICamera>();
        List<PerPlayer> perPlayer = new List<PerPlayer>();

        public ICollection<ICamera> AllCameras => allCameras;

        PerPlayer Player(int index)
        {
            if (index < 0 || index >= perPlayer.Count) {
                DebugOnly.Error("Invalid player index.");
                return null;
            }

            return perPlayer[index];
        }

        void IOnPlayerAdded.Do(int playerIndex)
        {
            if (playerIndex < 0) {
                DebugOnly.Error("Invalid player index.");
                return;
            }

            while (playerIndex >= perPlayer.Count)
                perPlayer.Add(null);

            var player = perPlayer[playerIndex];
            if (player == null) {
                player = new PerPlayer(this, playerIndex);
                perPlayer[playerIndex] = player;
            }

            UpdateCameras(true);
        }

        void IOnPlayerRemoved.Do(int playerIndex)
        {
            if (playerIndex >= 0 && playerIndex < perPlayer.Count)
                perPlayer[playerIndex] = null;

            UpdateCameras(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(playerManager.OnPlayerAdded);
            Observe(playerManager.OnPlayerRemoved);

            int n = playerManager.NumPlayers;
            for (int i = 0; i < n; i++) {
                if (i >= perPlayer.Count)
                    ((IOnPlayerAdded)this).Do(i);
            }

            UpdateCameras(true);
        }

        void Update()
        {
            UpdateCameras();
        }

        void UpdateCamera(ICamera camera)
        {
            int playerIndex = camera.PlayerIndex;
            if (playerIndex < 0)
                camera.GameObject.SetActive(false);
            else {
                PerPlayer player = Player(playerIndex);
                camera.GameObject.SetActive(player != null && camera == player.CurrentCamera);
            }
        }

        void UpdateCameras(bool force = false)
        {
            bool changed = force;
            foreach (var player in perPlayer) {
                if (player.UpdateCamera())
                    changed = true;
            }

            if (changed) {
                foreach (var camera in allCameras)
                    UpdateCamera(camera);
            }
        }

        public void ToggleFirstThirdPersonCamera(int playerIndex)
        {
            PerPlayer player = Player(playerIndex);
            if (player != null)
                player.FirstPerson = !player.FirstPerson;
        }

        public void AddFirstPersonCamera(IFirstPersonCamera camera)
        {
            DebugOnly.Check(!allCameras.Contains(camera), "Attempted to add camera multiple times.");

            if (camera.PlayerIndex >= 0) {
                PerPlayer player = Player(camera.PlayerIndex);
                if (player != null)
                    player.FirstPersonCameras.Add(camera);
            }

            allCameras.Add(camera);
            UpdateCamera(camera);
        }

        public void RemoveFirstPersonCamera(IFirstPersonCamera camera)
        {
            if (camera.PlayerIndex >= 0) {
                PerPlayer player = Player(camera.PlayerIndex);
                if (player != null)
                    player.FirstPersonCameras.Remove(camera);
            }

            allCameras.Remove(camera);
            UpdateCameras();
        }

        public void AddThirdPersonCamera(IThirdPersonCamera camera)
        {
            DebugOnly.Check(!allCameras.Contains(camera), "Attempted to add camera multiple times.");

            if (camera.PlayerIndex >= 0) {
                PerPlayer player = Player(camera.PlayerIndex);
                if (player != null)
                    player.ThirdPersonCameras.Add(camera);
            }

            allCameras.Add(camera);
            UpdateCamera(camera);
        }

        public void RemoveThirdPersonCamera(IThirdPersonCamera camera)
        {
            if (camera.PlayerIndex >= 0) {
                PerPlayer player = Player(camera.PlayerIndex);
                if (player != null)
                    player.ThirdPersonCameras.Remove(camera);
            }

            allCameras.Remove(camera);
            UpdateCameras();
        }
    }
}

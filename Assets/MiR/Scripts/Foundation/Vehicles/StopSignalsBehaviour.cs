using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class StopSignalsBehaviour : AbstractBehaviour, IOnUpdate
    {
        [Inject] IVehicle vehicle = default;
        [Inject] ISceneState sceneState = default;

        public int SubmeshIndex;
        public Material NormalMaterial;
        public Material ActiveMaterial;

        MeshRenderer meshRenderer;

        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
        }

        void IOnUpdate.Do(float timeDelta)
        {
            var materials = meshRenderer.sharedMaterials;
            if (vehicle.Forward < 0.0f)
                materials[SubmeshIndex] = ActiveMaterial;
            else
                materials[SubmeshIndex] = NormalMaterial;
            meshRenderer.sharedMaterials = materials;
        }
    }
}

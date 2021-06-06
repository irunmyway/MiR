using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Zenject;

namespace Foundation
{
    public sealed class WeatherManager : AbstractService<IWeatherManager>, IWeatherManager, IOnUpdate
    {
        [SerializeField] bool raining;
        public bool Raining { get { return raining; } set { raining = value; } }

        [Inject] IPlayerManager playerManager = default;
        [Inject] ISceneState sceneState = default;

        Puddle[] puddles;
        bool isRaining;

        public override void Start()
        {
            base.Start();
            puddles = FindObjectsOfType<Puddle>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
        }

        void IOnUpdate.Do(float deltaTime)
        {
            if (isRaining != raining) {
                isRaining = raining;

                foreach (var player in playerManager.EnumeratePlayers()) {
                    if (player.Rain != null)
                        player.Rain.Enabled = raining;
                }

                if (raining) {
                    foreach (var puddle in puddles)
                        puddle.Appear();
                } else {
                    foreach (var puddle in puddles)
                        puddle.Disappear();
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Zenject;

namespace Foundation
{
    public sealed class DayTimeManager : AbstractService<IDayTimeManager>, IDayTimeManager, IOnUpdate
    {
        public float TimeSpeed;
        public float LightNightY;
        public float NightLightsThreshold = 0.1f;
        public AnimationCurve DayNightCurve;

        public Color ambientDay;
        public Color ambientNight;

        public Light secondLight;
        public float secondLightIntensityDay;
        public float secondLightIntensityNight;

        public PostProcessVolume dayVolume;
        public PostProcessVolume nightVolume;

        public Material[] emissionMaterials;

        Quaternion initialRotation;
        NightLight[] nightLights;
        bool lightsEnabled;

        [ReadOnly] [SerializeField] float dayTime;
        [Inject] ISceneState sceneState = default;

        public override void Start()
        {
            base.Start();

            dayTime = 0.0f;

            nightLights = FindObjectsOfType<NightLight>();
            foreach (var light in nightLights)
                light.Light.enabled = false;
            foreach (var material in emissionMaterials)
                material.DisableKeyword("_EMISSION");
            lightsEnabled = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
            initialRotation = RenderSettings.sun.transform.localRotation;
        }

        void IOnUpdate.Do(float deltaTime)
        {
            dayTime = (dayTime + TimeSpeed * deltaTime) % 24.0f;

            float angleX = LightNightY + 360.0f * dayTime / 24.0f;
            var r = RenderSettings.sun.transform.localRotation;
            r = initialRotation * Quaternion.AngleAxis(angleX, Vector3.right);
            RenderSettings.sun.transform.localRotation = r;

            float dayOrNight = DayNightCurve.Evaluate(dayTime / 24.0f);
            RenderSettings.ambientLight = Color.Lerp(ambientNight, ambientDay, dayOrNight);
            secondLight.intensity = Mathf.Lerp(secondLightIntensityNight, secondLightIntensityDay, dayOrNight);
            dayVolume.weight = dayOrNight;
            nightVolume.weight = 1.0f - dayOrNight;

            bool needLights = dayOrNight < NightLightsThreshold;
            if (needLights != lightsEnabled) {
                lightsEnabled = needLights;
                foreach (var light in nightLights)
                    light.Light.enabled = needLights;
                if (lightsEnabled) {
                    foreach (var material in emissionMaterials)
                        material.EnableKeyword("_EMISSION");
                } else {
                    foreach (var material in emissionMaterials)
                        material.DisableKeyword("_EMISSION");
                }
            }
        }
    }
}

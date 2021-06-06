using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Foundation
{
    public sealed class TimeScaleMixerBehaviour : PlayableBehaviour
    {
        TimeScaleHandle handle = new TimeScaleHandle();
        TimeScaleManager currentManager;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
                return;

            TimeScaleManager manager = playerData as TimeScaleManager;
            float scale = 1.0f;

            if (manager == null)
                return;

            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; i++) {
                float weight = playable.GetInputWeight(i);
                ScriptPlayable<TimeScaleBehaviour> input = (ScriptPlayable<TimeScaleBehaviour>)playable.GetInput(i);
                TimeScaleBehaviour behaviour = input.GetBehaviour();
                if (Mathf.Approximately(weight, 1.0f))
                    scale *= behaviour.Scale;
            }

            if (handle.IsValid)
                manager.EndTimeScale(handle);

            manager.BeginTimeScale(handle, scale);
            currentManager = manager;
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
                return;
            if (!handle.IsValid || currentManager == null)
                return;

            var duration = (float)playable.GetDuration();
            var time = (float)playable.GetTime();
            var count = time + info.deltaTime;
            if ((info.effectivePlayState == PlayState.Paused && count > duration) || Mathf.Approximately(time, duration))
                currentManager.EndTimeScale(handle);
        }
    }
}

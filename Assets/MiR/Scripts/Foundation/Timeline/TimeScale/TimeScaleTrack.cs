using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Foundation
{
    [TrackClipType(typeof(TimeScaleAsset))]
    [TrackBindingType(typeof(TimeScaleManager))]
    public sealed class TimeScaleTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<TimeScaleMixerBehaviour>.Create(graph, inputCount);
        }
    }
}

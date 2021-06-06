using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Foundation
{
    public sealed class TimeScaleAsset : PlayableAsset
    {
        public float Scale;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TimeScaleBehaviour>.Create(graph);

            var behaviour = playable.GetBehaviour();
            behaviour.Scale = Scale;

            return playable;
        }
    }
}

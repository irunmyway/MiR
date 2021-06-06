using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Foundation
{
    public sealed class CharacterShootingAsset : PlayableAsset
    {
        //public ExposedReference<CharacterWeapon> CharacterWeapon;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CharacterShootingBehaviour>.Create(graph);

            var behaviour = playable.GetBehaviour();
            //behaviour.CharacterWeapon = CharacterWeapon.Resolve(graph.GetResolver());

            return playable;
        }
    }
}

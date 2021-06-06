using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Foundation
{
    [TrackClipType(typeof(CharacterShootingAsset))]
    [TrackBindingType(typeof(CharacterWeapon))]
    public sealed class CharacterShootingTrack : TrackAsset
    {
    }
}

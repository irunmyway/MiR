using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Foundation
{
    public sealed class CharacterShootingBehaviour : PlayableBehaviour
    {
        //public CharacterWeapon CharacterWeapon;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            /*
            if (CharacterWeapon != null)
                CharacterWeapon.Attack();
            */

            if (playerData is CharacterWeapon weapon)
                weapon.Attack();
        }
    }
}

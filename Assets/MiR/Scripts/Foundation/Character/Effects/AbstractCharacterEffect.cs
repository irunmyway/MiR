using System.Collections;
using UnityEngine;

namespace Foundation
{
    public abstract class AbstractCharacterEffect : ScriptableObject
    {
        public enum Mode
        {
            Single,
            Prolong,
            Stack,
        }

        public abstract Mode ApplyMode { get; }
        public abstract IEnumerator Apply(ICharacterEffectManager manager, ICharacterEffectState state);
    }
}

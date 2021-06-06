using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public sealed class CharacterDialogs : AbstractService<ICharacterDialogs>, ICharacterDialogs
    {
        [SerializeField] Sprite portrait;
        public Sprite Portrait => portrait;

        [SerializeField] List<Dialog> dialogs;
        public List<Dialog> Dialogs => dialogs;
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public interface ICharacterDialogs
    {
        Sprite Portrait { get; }
        List<Dialog> Dialogs { get; }
    }
}

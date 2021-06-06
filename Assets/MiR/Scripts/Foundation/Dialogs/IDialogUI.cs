using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public interface IDialogUI
    {
        void DisplayDialogs(IPlayer player, Sprite portrait, List<Dialog> dialogs);
    }
}

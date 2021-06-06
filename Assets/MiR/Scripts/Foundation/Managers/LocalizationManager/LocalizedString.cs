using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Foundation
{
    [Serializable]
    public struct LocalizedString
    {
        public string LocalizationID;

        public LocalizedString(string id)
        {
            LocalizationID = id;
        }
    }
}

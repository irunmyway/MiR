using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Foundation;

namespace Foundation
{
    public sealed class SaveGameButton : MonoBehaviour
    {
        public static int counter = 0;
        [Inject] ILoadSaveManager loadSaveManager = default;

        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => loadSaveManager.SaveNew($"Save {counter++}"));
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(Button))]
    public sealed class PushStateButton : MonoBehaviour
    {
        [Inject] ISceneStateManager manager = default;
        public SceneState State;

        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => manager.Push(State));
        }
    }
}

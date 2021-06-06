using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Foundation
{
    public sealed class TutorialState : AbstractBehaviour, IOnTutorialStarted, IOnTutorialEnded
    {
        [Inject] ISceneState sceneState = default;
        [Inject] ISceneStateManager sceneStateManager = default;
        [Inject] ITutorialManager tutorialManager = default;

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(tutorialManager.OnTutorialStarted);
            Observe(tutorialManager.OnTutorialEnded);
        }

        void IOnTutorialStarted.Do()
        {
            sceneStateManager.PushTopmost(sceneState);
        }

        void IOnTutorialEnded.Do()
        {
            sceneStateManager.Pop(sceneState);
        }
    }
}

using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class ShowMessageTutorialStep : TutorialStep, IOnTutorialOverlayClicked
    {
        [Inject] ITutorialManager tutorialManager;

        ObserverHandle clickObserver;
        bool buttonClicked;

        [SerializeField] [TextArea] string message;
        public override string Message => message;

        public override void OnBegin()
        {
            buttonClicked = false;
            Observe(ref clickObserver, tutorialManager.OnTutorialOverlayClicked);
        }

        public override void OnEnd()
        {
            Unobserve(clickObserver);
        }

        void IOnTutorialOverlayClicked.Do()
        {
            buttonClicked = true;
        }

        public override bool IsComplete()
        {
            return buttonClicked;
        }
    }
}

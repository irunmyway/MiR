using UnityEngine;

namespace Foundation
{
    public interface ITutorialManager
    {
        Tutorial CurrentTutorial { get; }

        ObserverList<IOnTutorialStarted> OnTutorialStarted { get; }
        ObserverList<IOnTutorialEnded> OnTutorialEnded { get; }
        ObserverList<IOnTutorialOverlayClicked> OnTutorialOverlayClicked { get; }

        void StartTutorial(Tutorial tutorial);
    }
}

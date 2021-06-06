using UnityEngine;

namespace Foundation
{
    public sealed class TutorialManager : AbstractService<ITutorialManager>, ITutorialManager
    {
        public TutorialOverlay TutorialOverlay;

        public Tutorial CurrentTutorial { get; private set; }

        public ObserverList<IOnTutorialStarted> OnTutorialStarted { get; } = new ObserverList<IOnTutorialStarted>();
        public ObserverList<IOnTutorialEnded> OnTutorialEnded { get; } = new ObserverList<IOnTutorialEnded>();
        public ObserverList<IOnTutorialOverlayClicked> OnTutorialOverlayClicked { get; } = new ObserverList<IOnTutorialOverlayClicked>();

        public void StartTutorial(Tutorial tutorial)
        {
            if (CurrentTutorial != null) {
                DebugOnly.Error("Can't start tutorial: another tutorial already active.");
                return;
            }

            if (tutorial.WasShown && tutorial.Once)
                return;

            CurrentTutorial = tutorial;

            foreach (var it in OnTutorialStarted.Enumerate())
                it.Do();

            tutorial.Restart();

            TutorialOverlay.DisableMessage();
            TutorialOverlay.DisableHole();
        }

        void Update()
        {
            if (CurrentTutorial == null)
                return;

            if (!CurrentTutorial.UpdateCurrentStep(TutorialOverlay)) {
                if (CurrentTutorial.Next != null) {
                    CurrentTutorial = CurrentTutorial.Next;
                    CurrentTutorial.Restart();
                } else {
                    CurrentTutorial = null;
                    foreach (var it in OnTutorialEnded.Enumerate())
                        it.Do();
                }
            }
        }
    }
}

using Zenject;

namespace Foundation
{
    public sealed class TutorialOnStateActivate : Tutorial, IOnStateBecomeTopmost
    {
        [Inject] ISceneState state = default;
        [Inject] ITutorialManager tutorialManager = default;

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(state.OnBecomeTopmost);
        }

        void IOnStateBecomeTopmost.Do()
        {
            tutorialManager.StartTutorial(this);
        }
    }
}

using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Zenject;

namespace Foundation
{
    public sealed class FadeCanvasAnimator : AbstractService<ICanvasAnimator>, ICanvasAnimator
    {
        public float AppearDuration = 1.0f;
        public float DisappearDuration = 1.0f;

        public void AnimateAppear(Canvas canvas, CanvasGroup canvasGroup)
        {
            canvasGroup.DOFade(1.0f, AppearDuration);
        }

        public void AnimateDisappear(Canvas canvas, CanvasGroup canvasGroup)
        {
            canvasGroup.DOFade(0.0f, DisappearDuration);
        }
    }
}

using UnityEngine;

namespace Foundation
{
    public interface ICanvasAnimator
    {
        void AnimateAppear(Canvas canvas, CanvasGroup canvasGroup);
        void AnimateDisappear(Canvas canvas, CanvasGroup canvasGroup);
    }
}

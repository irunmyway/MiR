using UnityEngine;

namespace Foundation
{
    public abstract class TutorialStep : AbstractBehaviour
    {
        public virtual string Message => null;
        public virtual RectTransform FingerTarget => null;

        public virtual void OnBegin()
        {
        }

        public virtual void OnEnd()
        {
        }

        public virtual bool IsComplete()
        {
            return true;
        }
    }
}

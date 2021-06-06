using UnityEngine;

namespace Foundation
{
    public abstract class EnemyBehaviour : AbstractBehaviour
    {
        public virtual bool Crouching => false;

        public virtual void ActivateAI() {}
        public virtual void DeactivateAI() {}
        public virtual bool CheckUpdateAI(float deltaTime) { return true; }
        public virtual void UpdateAI(float deltaTime) {}
    }
}

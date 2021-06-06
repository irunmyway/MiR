namespace Foundation
{
    public abstract class AbstractWeaponAttack : AbstractBehaviour, IWeaponAttack
    {
        public abstract void EndAttack();

        public virtual void EndCooldown()
        {
        }
    }
}

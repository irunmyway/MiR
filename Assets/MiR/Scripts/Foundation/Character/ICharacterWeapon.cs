namespace Foundation
{
    public interface ICharacterWeapon
    {
        AbstractWeapon CurrentWeapon { get; }
        AbstractWeapon AttackingWeapon { get; }
        ObserverList<IOnCharacterAttack> OnAttack { get; }
        void SetCurrentWeapon(AbstractWeapon weapon);
        bool CanAttack();
        bool Attack();
        void EndAttack(bool applyCooldown);
    }
}

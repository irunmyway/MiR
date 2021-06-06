namespace Foundation
{
    public interface IOnCharacterDamaged
    {
        void Do(ICharacterHealth health, IAttacker attacker, float amount, float newHealth);
    }
}

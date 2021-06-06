namespace Foundation
{
    public interface IOnCharacterHealed
    {
        void Do(ICharacterHealth health, IAttacker attacker, float amount, float newHealth);
    }
}

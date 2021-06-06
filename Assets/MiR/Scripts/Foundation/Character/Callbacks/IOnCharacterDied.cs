namespace Foundation
{
    public interface IOnCharacterDied
    {
        void Do(ICharacterHealth health, IAttacker attacker);
    }
}

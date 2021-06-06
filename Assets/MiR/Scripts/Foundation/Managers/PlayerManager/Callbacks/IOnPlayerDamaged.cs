namespace Foundation
{
    public interface IOnPlayerDamaged
    {
        void Do(int playerIndex, IAttacker attacker, float amount, float newHealth);
    }
}

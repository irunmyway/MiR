namespace Foundation
{
    public interface IOnPlayerHealed
    {
        void Do(int playerIndex, IAttacker attacker, float amount, float newHealth);
    }
}

namespace Foundation
{
    public interface IOnPlayerDied
    {
        void Do(int playerIndex, IAttacker attacker);
    }
}

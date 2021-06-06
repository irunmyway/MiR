namespace Foundation
{
    public interface ICharacterEffectManager
    {
        ObserverList<IOnCharacterEffectStarted> OnEffectStarted { get; }
        ObserverList<IOnCharacterEffectEnded> OnEffectEnded { get; }

        public ICharacterHealth Health { get; }

        public void AddEffect(IAttacker attacker, AbstractCharacterEffect effect);
    }
}

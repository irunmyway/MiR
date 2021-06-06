using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Foundation
{
    public sealed class CharacterEffectManager : AbstractService<ICharacterEffectManager>, ICharacterEffectManager, IOnUpdate
    {
        sealed class EffectState : ICharacterEffectState, IAttacker
        {
            public float TimeDelta { get; set; }
            public AbstractCharacterEffect Effect { get; set; }
            public IEnumerator Enumerator;
            public IAttacker OriginalAttacker;
            public IPlayer Player => OriginalAttacker.Player;
            public IEnemy Enemy => OriginalAttacker.Enemy;
        }

        public ObserverList<IOnCharacterEffectStarted> OnEffectStarted { get; } = new ObserverList<IOnCharacterEffectStarted>();
        public ObserverList<IOnCharacterEffectEnded> OnEffectEnded { get; } = new ObserverList<IOnCharacterEffectEnded>();

        [Inject] ISceneState sceneState = default;
        [InjectOptional] public ICharacterHealth Health { get; private set; }

        Stack<EffectState> statePool = new Stack<EffectState>();
        List<EffectState> activeEffects = new List<EffectState>();

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(sceneState.OnUpdate);
        }

        void IOnUpdate.Do(float timeDelta)
        {
            int n = activeEffects.Count;
            while (n-- > 0) {
                var state = activeEffects[n];
                state.TimeDelta = timeDelta;
                if (state.Enumerator == null || !state.Enumerator.MoveNext()) {
                    var effect = state.Effect;

                    state.Enumerator = null;
                    state.Effect = null;
                    state.OriginalAttacker = null;
                    activeEffects.RemoveAt(n);
                    statePool.Push(state);

                    foreach (var it in OnEffectEnded.Enumerate())
                        it.Do(effect);
                }
            }
        }

        public void AddEffect(IAttacker attacker, AbstractCharacterEffect effect)
        {
            if (effect.ApplyMode != AbstractCharacterEffect.Mode.Stack) {
                foreach (var st in activeEffects) {
                    if (st.Effect == effect) {
                        switch (effect.ApplyMode) {
                            case AbstractCharacterEffect.Mode.Single:
                                return;
                            case AbstractCharacterEffect.Mode.Prolong:
                                st.Enumerator = effect.Apply(this, st);
                                return;
                        }
                    }
                }
            }

            EffectState state;
            if (statePool.Count > 0)
                state = statePool.Pop();
            else
                state = new EffectState();

            state.TimeDelta = 0.0f;
            state.Effect = effect;
            state.OriginalAttacker = attacker;
            state.Enumerator = effect.Apply(this, state);
            activeEffects.Add(state);

            foreach (var it in OnEffectStarted.Enumerate())
                it.Do(effect);
        }
    }
}

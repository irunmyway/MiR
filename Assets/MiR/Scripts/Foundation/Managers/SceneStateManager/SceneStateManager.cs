using UnityEngine;
using System.Collections.Generic;

namespace Foundation
{
    sealed class SceneStateManager : AbstractService<ISceneStateManager>, ISceneStateManager
    {
        public List<SceneState> InitialStates;

        ISceneState currentState;
        ISceneState currentTopmostState;
        public ISceneState CurrentState => currentState;
        public ISceneState CurrentTopmostState => currentTopmostState;

        readonly List<ISceneState> states = new List<ISceneState>();
        readonly List<ISceneState> topmostStates = new List<ISceneState>();
        readonly List<ISceneState> statesCache = new List<ISceneState>();
        bool statesListChanged;

        public override void Start()
        {
            base.Start();
            foreach (var state in InitialStates)
                Push(state);
        }

        public void Push(ISceneState state)
        {
            DebugOnly.Check(!states.Contains(state) && !topmostStates.Contains(state),
                $"GameState is already on the stack.");

            states.Add(state);
            statesListChanged = true;
            (state as ISceneStateInternal)?.InternalActivate();
        }

        public void PushTopmost(ISceneState state)
        {
            DebugOnly.Check(!states.Contains(state) && !topmostStates.Contains(state),
                $"GameState is already on the stack.");

            topmostStates.Add(state);
            statesListChanged = true;
            (state as ISceneStateInternal)?.InternalActivate();
        }

        public void Pop(ISceneState state)
        {
            statesListChanged = true;

            states.Remove(state);
            topmostStates.Remove(state);

            if (currentState == state) {
                (currentState as ISceneStateInternal)?.InternalResignTopmost();
                currentState = null;
            }

            if (currentTopmostState == state) {
                (currentTopmostState as ISceneStateInternal)?.InternalResignTopmost();
                currentTopmostState = null;
            }

            (state as ISceneStateInternal)?.InternalDeactivate();

            DebugOnly.Check(states.Count != 0 || topmostStates.Count != 0, "GameState stack is empty.");
        }

        void UpdateTopmostState(ref ISceneState currentState, List<ISceneState> states)
        {
            int n = states.Count;
            if (n == 0) {
                if (currentState != null) {
                    (currentState as ISceneStateInternal)?.InternalResignTopmost();
                    currentState = null;
                }
            } else {
                if (currentState != states[n - 1]) {
                    var oldState = currentState;
                    currentState = states[n - 1];
                    (currentState as ISceneStateInternal)?.InternalBecomeTopmost();
                    if (oldState != null)
                        (oldState as ISceneStateInternal)?.InternalResignTopmost();
                }
            }
        }

        IEnumerable<ISceneState> CachedGameStates()
        {
            int n;
            if (!statesListChanged)
                n = statesCache.Count;
            else {
                statesListChanged = false;
                statesCache.Clear();
                statesCache.AddRange(states);
                statesCache.AddRange(topmostStates);
                n = statesCache.Count;

                UpdateTopmostState(ref currentState, states);
                UpdateTopmostState(ref currentTopmostState, topmostStates);

                int index = 0;
                foreach (var it in statesCache)
                    (it as ISceneStateInternal)?.InternalSetSortingOrder(index++);
            }

            while (n-- > 0) {
                var state = statesCache[n];
                yield return state;
            }
        }

        void Update()
        {
            float timeDelta = Time.deltaTime;
            bool update = true;

            foreach (var state in CachedGameStates()) {
                if (update) {
                    foreach (var ticker in state.OnUpdate.Enumerate())
                        ticker.Do(timeDelta);
                } else {
                    foreach (var ticker in state.OnUpdateDuringPause.Enumerate())
                        ticker.Do(timeDelta);
                }

                update = update && state.UpdateParentState;
            }
        }

        void FixedUpdate()
        {
            foreach (var state in CachedGameStates()) {
                foreach (var ticker in state.OnFixedUpdate.Enumerate())
                    ticker.Do();
                if (!state.UpdateParentState)
                    break;
            }
        }

        void LateUpdate()
        {
            float timeDelta = Time.deltaTime;
            foreach (var state in CachedGameStates()) {
                foreach (var ticker in state.OnLateUpdate.Enumerate())
                    ticker.Do(timeDelta);
                if (!state.UpdateParentState)
                    break;
            }
        }
    }
}

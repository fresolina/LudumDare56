using System;
using System.Collections.Generic;

namespace statemachine {
    public interface ITransition {
        IState To { get; }
        Func<bool> Condition { get; }
    }

    public class Transition : ITransition {
        public IState To { get; }
        public Func<bool> Condition { get; }

        public Transition(IState to, Func<bool> condition) {
            To = to;
            Condition = condition;
        }
    }

    public class StateMachine {
        public bool Enabled { get; set; } = true;
        public event Action<IState, IState> StateChanged;

        IState _current;
        readonly HashSet<ITransition> _anyTransitions = new(); // anyState.Transitions

        public void Update() {
            if (!Enabled) return;

            if (TryGetTransition(out ITransition transition))
                ChangeState(transition.To);

            _current?.Update();
        }

        public void FixedUpdate() => _current?.FixedUpdate();

        public void SetState(IState state) => ChangeState(state);

        void ChangeState(IState nextState) {
            if (nextState == _current) return;

            _current?.OnExit();
            nextState?.OnEnter(); // TODO: Warn if setting nextState to null?
            StateChanged?.Invoke(_current, nextState);
            _current = nextState;
        }

        bool TryGetTransition(out ITransition outTransition) {
            foreach (ITransition transition in _current.Transitions)
                if (transition.Condition()) {
                    outTransition = transition;
                    return true;
                }

            foreach (ITransition transition in _anyTransitions)
                if (transition.Condition()) {
                    outTransition = transition;
                    return true;
                }

            outTransition = null;

            return false;
        }

        public void AddTransition(IState from, IState to, Func<bool> condition) {
            from.Transitions.Add(new Transition(to, condition));
        }

        public void AddAnyTransition(IState to, Func<bool> condition) {
            _anyTransitions.Add(new Transition(to, condition));
        }
    }
}

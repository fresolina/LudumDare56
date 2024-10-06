using System.Collections.Generic;

namespace statemachine {
    public interface IState {
        void OnEnter();
        void Update();
        void FixedUpdate();
        void OnExit();
        HashSet<ITransition> Transitions { get; }
    }
    public class State : IState {
        public HashSet<ITransition> Transitions { get; private set; }
        bool _enabled = false;

        public State() {
            Init();
        }

        public virtual void Init() {
            Transitions = new HashSet<ITransition>();
        }

        public virtual void FixedUpdate() {
            // Debug.Log(GetType().Name + " FixedUpdate");
        }
        public virtual void OnEnter() {
            // Debug.Log(GetType().Name + " OnEnter");
            _enabled = true;
        }
        public virtual void OnExit() {
            // Debug.Log(GetType().Name + " OnExit");
            _enabled = false;
        }
        public virtual void Update() { }
    }
}

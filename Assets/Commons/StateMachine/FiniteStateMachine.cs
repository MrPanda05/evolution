using System;
using System.Collections.Generic;
using UnityEngine;

namespace Evolution.Commons.StateMachine
{
    public class FiniteStateMachine : MonoBehaviour
    {
        [SerializeField]
        private State _initialState;
        private State _currentState;
        private Dictionary<string, State> _states = new Dictionary<string, State>();
        [field: SerializeField]
        public bool AutoEnterInitialState { get; private set; } = true;
        public event Action OnStateChanged;
        public Action<string, string> OnStateChangedTo;
        public State CurrentState => _currentState;

        private void Awake()
        {
            foreach (State state in GetComponentsInChildren<State>())
            {
                _states.Add(state.StateName, state);
                state.SetStateMachine(this);
                state.Ready();
            }
            if (_initialState != null)
            {
                _currentState = _initialState;
            }
            print("Fsm is awake");
        }
        private void Start()
        {
            foreach (State state in _states.Values)
            {
                state.OnStart();
            }
            if (_initialState != null)
            {
                _currentState.OnStart();
                if (AutoEnterInitialState)
                {
                    _currentState.Enter();
                }
            }
            print("Fsm is start");
        }
        private void Update()
        {
            if (_currentState != null)
            {
                _currentState.Process();
            }
        }
        private void FixedUpdate()
        {
            if (_currentState != null)
            {
                _currentState.FixedProcess();
            }
        }
        public void ChangeState(string newStateName)
        {
            var stateExist = _states.TryGetValue(newStateName, out State newState);
            var oldStateName = "";
            if (!stateExist || newState == null) return;
            if (newState == _currentState) return;
            if (_currentState != null)
            {
                oldStateName = _currentState.StateName;
                _currentState.Exit();
            }
            _currentState = newState;
            _currentState.Enter();
            OnStateChanged?.Invoke();
            OnStateChangedTo?.Invoke(oldStateName, newStateName);
        }
        public void PrintAllStates()
        {
            foreach (var state in _states)
            {
                print("State name: " + state.Key);
            }
        }
    }
}

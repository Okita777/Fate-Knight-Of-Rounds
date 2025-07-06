using System;
using System.Collections.Generic;
using UnityEngine;

namespace okita.CharacterStateMachine
{
    [System.Serializable]
    public class StateMachine
    {
        [SerializeField] private List<State> states;

        public CharacterController CharacterController;

        private Dictionary<string, State> stateDictionary = new Dictionary<string, State>();

        private State defaultState;

        private State currentState;

        private State previousState;

        public List<State> States
        {
            get => states;
            private set => states = value;
        }

        public State DefaultState
        {
            get => defaultState;
            private set => defaultState = value;
        }

        public State CurrentState
        {
            get => currentState;
            private set => currentState = value;
        }

        public State PreviousState
        {
            get => previousState;
            private set => previousState = value;
        }
        public StateMachine()
        {
            states = new List<State>();
            defaultState = null; // No default state initially
            stateDictionary = new Dictionary<string, State>();
        }

        public StateMachine(List<State> states)
        {
            if (states == null || states.Count == 0)
            {
                throw new ArgumentException("States cannot be null or empty", nameof(states));
            }

            this.states = states;

            stateDictionary = new Dictionary<string, State>();
            foreach (var state in states)
            {
                if (stateDictionary.ContainsKey(state.StateName))
                {
                    throw new ArgumentException($"State with name {state.StateName} already exists", nameof(states));
                }
                stateDictionary[state.StateName] = state;
            }
        }

        public void AddState(State state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state), "State cannot be null");
            }

            if (states == null)
            {
                states = new List<State>();
            }

            states.Add(state);

            if (stateDictionary.ContainsKey(state.StateName))
            {
                throw new ArgumentException($"State with name {state.StateName} already exists", nameof(state));
            }
            stateDictionary[state.StateName] = state;
        }

        public void SetDefaultState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                throw new ArgumentException("State name cannot be null or empty", nameof(stateName));
            }

            if (!stateDictionary.ContainsKey(stateName))
            {
                throw new KeyNotFoundException($"State with name {stateName} does not exist");
            }

            defaultState = stateDictionary[stateName];
        }

        public void StartStateMachine()
        {
            if (defaultState == null)
            {
                throw new InvalidOperationException("Default state is not set. Please set a default state before starting the state machine.");
            }

            ChangeState(defaultState);
        }

        public void UpdateState(float deltaTime)
        {
            if (currentState != null)
            {
                string runState = currentState.OnUpdateState(deltaTime);
                if ((runState != null) & (runState != currentState.StateName) && stateDictionary.ContainsKey(runState))
                {
                    // If the runState is not null, not the same as currentState, and exists in the state dictionary
                    ChangeState(stateDictionary[runState]);
                }
            }
        }

        public void LateUpdateState(float deltaTime)
        {
            if (currentState != null)
            {
                currentState.OnLateUpdateState(deltaTime);
            }
        }

        public void ChangeState(State newState)
        {
            if (newState == null)
            {
                throw new ArgumentNullException(nameof(newState), "New state cannot be null");
            }

            if (currentState != null)
            {
                currentState.OnExitState();
                previousState = currentState;
            }

            currentState = newState;
            currentState.OnEnterState();
        }
    }

}
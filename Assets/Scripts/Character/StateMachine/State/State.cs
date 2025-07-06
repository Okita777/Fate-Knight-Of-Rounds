using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace okita.CharacterStateMachine
{
    [System.Serializable]
    public class State
    {
        [SerializeField] protected string stateName;

        [SerializeField] protected float duration;

        protected float elapsedTime = 0;

        protected StateMachine StateMachine;

        public string StateName
        {
            get => stateName;
            private set => stateName = value;
        }

        public float ElapsedTime
        {
            get => elapsedTime;
            private set => elapsedTime = value;
        }

        public float Duration
        {
            get => duration;
            private set => duration = value;
        }

        public State(StateMachine StateMachine)
        {
            this.StateMachine = StateMachine ?? throw new ArgumentNullException(nameof(StateMachine), "StateMachine cannot be null");
            stateName = "DefaultState";
            duration = 0f;
            elapsedTime = 0f;
        }

        public State(StateMachine StateMachine, float duration)
        {
            this.StateMachine = StateMachine ?? throw new ArgumentNullException(nameof(StateMachine), "StateMachine cannot be null");
            stateName = "DefaultState";
            this.duration = duration;
            elapsedTime = 0f;
        }

        public virtual void OnEnterState()
        {
            Debug.Log($"Entering state: {stateName}");
        }

        public virtual string OnUpdateState(float deltaTime)
        {
            return "";
        }

        public virtual void OnLateUpdateState(float deltaTime)
        {

        }

        public virtual void OnExitState()
        {

        }
    }

}
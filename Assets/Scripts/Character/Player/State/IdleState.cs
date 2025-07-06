using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using okita.CharacterStateMachine;
using UnityEngine;

namespace okita.Player
{
    [System.Serializable]
    public class IdleState : State
    {
        public IdleState(StateMachine stateMachine) : base(stateMachine)
        {
            stateName = "IdleState";
            duration = 0f; // Idle state typically has no duration
        }

        public IdleState(StateMachine stateMachine, float duration) : base(stateMachine, duration)
        {
            stateName = "IdleState";
        }

        public override void OnEnterState()
        {

        }

        public override string OnUpdateState(float deltaTime)
        {
            return "";
        }

        public override void OnLateUpdateState(float deltaTime)
        {
            // 在空闲状态下，通常不需要执行任何操作
        }

        public override void OnExitState()
        {
            // 在退出空闲状态时，可以执行一些清理操作
            // 例如，重置某些变量或状态
        }
    }

}
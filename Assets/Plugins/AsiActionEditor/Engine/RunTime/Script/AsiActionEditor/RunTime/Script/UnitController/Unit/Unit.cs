using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AsiActionEngine.RunTime
{
    [RequireComponent(typeof(Animator))]
    public class Unit : MonoBehaviour
    {
        private ActionStateMachine m_ActionStateMachine;
        public int UnitID;//单位ID  生成时赋值
        // public string ActionGroupName;//行为列表组名称
        public string CameraObject { get; set; }
        public Transform RootTarget{ get; set; }
        public ActionStateMachine ActionStateMachine => m_ActionStateMachine;

        private Vector3 deltaPos;

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
            if (RootTarget == null) RootTarget = transform;
        }

        public virtual void OnUpdate(float _time)
        {
            m_ActionStateMachine.OnUpdate(_time);
        }

        public virtual void OnLateUpdate(float _time)
        {
            m_ActionStateMachine.OnLateUpdate(_time);
        }

        public void SetActionStateMachine(ActionStateMachine part)
        {
            m_ActionStateMachine = part;
        }

        public void OnAnimatorMove()
        {
            if (!ActionStateMachine.IsLocalClient) return;
            if (m_ActionStateMachine.TryGetComponent(out Animator animator))
            {
                if (m_ActionStateMachine.TryGetComponent(out CharacterController characterController, RootTarget))
                {
                    deltaPos.x = animator.deltaPosition.x * m_ActionStateMachine.RootWeight.x;
                    deltaPos.y = animator.deltaPosition.y * m_ActionStateMachine.RootWeight.y;
                    deltaPos.z = animator.deltaPosition.z * m_ActionStateMachine.RootWeight.z;
                    
                    characterController.Move(deltaPos);
                    Quaternion _targetRota = animator.deltaRotation * characterController.transform.rotation;
                    characterController.transform.rotation = _targetRota;
                }
            }
        }
    }
}

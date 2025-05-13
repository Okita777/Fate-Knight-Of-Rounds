using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    public class Ex_Update_TargetingMove : ActionLogics
    {
        private float m_LerpTime = -1;
        private float m_LerpTimeD = -1;
        private bool m_IsActive = false;
        private Transform m_Target;
        private Transform m_ReferBone;
        private Vector3 m_ScriptLookDir;
        private byte m_Type;
        private Event_TargetingMove m_EventTargetingMove;

        public Vector3 ScriptLookDir
        {
            get { return m_ScriptLookDir; }
            set { m_ScriptLookDir = value; }
        }

        public void OnSetRot(Event_TargetingMove _EventTargetingMove, Transform _target, Transform _referBone)
        {
            m_EventTargetingMove = _EventTargetingMove;
            if (!m_EventTargetingMove.FreeToX && !m_EventTargetingMove.FreeToY) return;
            m_IsActive = true;
            m_Target = _target;
            m_ReferBone = _referBone;
            if (m_EventTargetingMove.EnterTime > 0)
            {
                m_LerpTime = m_EventTargetingMove.EnterTime;
                m_LerpTimeD = m_EventTargetingMove.EnterTime;
            }
            else
            {
                m_LerpTime = -1;
                m_LerpTimeD = -1;
            }
        }

        public void OnExit(float _enterTime = 0)
        {
            m_IsActive = false;
            if (_enterTime > 0)
            {
                m_LerpTime = _enterTime;
                m_LerpTimeD = _enterTime;
            }
            else
            {
                m_LerpTime = -1;
                m_LerpTimeD = -1;
            }
        }
        
        public override void LateUpdate(ActionStateMachine _actionState, float _deltaTime)
        {
            if (m_LerpTime > 0)
            {
                float percentage = m_LerpTime / m_LerpTimeD;
                if (m_IsActive)
                {
                    OnSetRotUpdate(m_ReferBone.forward, GetTargetDir(_actionState), 1 - percentage);
                }
                else
                {
                    OnSetRotUpdate(m_ReferBone.forward, GetTargetDir(_actionState), percentage);
                }
                m_LerpTime -= _deltaTime;
                return;
            }
            if(!m_IsActive)return;
            OnSetRotUpdate(m_ReferBone.forward, GetTargetDir(_actionState), 1);
        }

        private Vector3 GetTargetDir(ActionStateMachine _actionState)
        {
            if (m_Type == 0) return _actionState.GetCamRot() * Vector3.forward;
            if (m_Type == 1) return (_actionState.LockTransform.position - _actionState.CurUnit.transform.position);
            if (m_Type == 2) return m_ScriptLookDir;
            if (m_Type == 3) return Quaternion.Euler(0, m_EventTargetingMove.WorldAngle.value, 0) * Vector3.forward;

            return Vector3.forward;
        }
        
        private void OnSetRotUpdate(Vector3 _referDir, Vector3 _targetDir, float _weight)
        {
            //偏移角度
            _targetDir = Quaternion.Euler(0, m_EventTargetingMove.WorldAngle.value, 0) * _targetDir;
            //均分的朝向
            Vector3 _lookDir = Vector3.Slerp(_referDir, _targetDir, ((float)1 / (m_EventTargetingMove.TargetBoneLinks + 1)) * _weight);
            //目标旋转矩阵
            Quaternion _quat = Quaternion.FromToRotation(_referDir, _lookDir);
            if (!m_EventTargetingMove.FreeToX) _quat = Quaternion.Euler(_quat.eulerAngles.x, 0, 0);
            if (!m_EventTargetingMove.FreeToY) _quat = Quaternion.Euler(0, _quat.eulerAngles.y, 0);

            Transform _nowSetTarget = m_Target;
            _nowSetTarget.rotation = _quat * _nowSetTarget.rotation;
            //关系链
            for (int i = 0; i < m_EventTargetingMove.TargetBoneLinks; i++)
            {
                _nowSetTarget = _nowSetTarget.parent;
                _nowSetTarget.rotation = _quat * _nowSetTarget.rotation;
            }
        }
    }
}
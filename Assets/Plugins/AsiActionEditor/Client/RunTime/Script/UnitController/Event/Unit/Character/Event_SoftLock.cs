using System;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_SoftLock : IActionEventData
    {
        [SerializeField] protected float m_Radius = 6;
        [SerializeField] protected float m_SelfAngle = 120;
        [SerializeField] protected float m_LerpSpeed = 12;
        [SerializeField] protected int m_CheckLayer;
        [SerializeField] protected int m_Priority = 1;
        [SerializeField] protected bool m_ReferToCam = true;

        #region Property
        [EditorProperty("锁定层级: ", EditorPropertyType.EEPT_LayerMask)]
        public int CheckLayer
        {
            get { return m_CheckLayer; }
            set { m_CheckLayer = value; }
        }
        [EditorProperty("最大半径: ", EditorPropertyType.EEPT_Float)]
        public float Radius
        {
            get { return m_Radius; }
            set { m_Radius = value; }
        }
        [EditorProperty("最大角度差: ", EditorPropertyType.EEPT_Float)]
        public float SelfAngle
        {
            get { return m_SelfAngle; }
            set { m_SelfAngle = value; }
        }        
        [EditorProperty("参考锁定方向至相机: ", EditorPropertyType.EEPT_Bool)]
        public bool ReferToCam
        {
            get { return m_ReferToCam; }
            set { m_ReferToCam = value; }
        }   
        [EditorProperty("旋转速度: ", EditorPropertyType.EEPT_Float)]
        public float LerpSpeed
        {
            get { return m_LerpSpeed; }
            set { m_LerpSpeed = value; }
        }
        [EditorProperty("旋转优先级: ", EditorPropertyType.EEPT_Int)]
        public int Priority
        {
            get { return m_Priority; }
            set { m_Priority = value; }
        }
        #endregion
        public int GetEvenType() => (int)EEvenType.EET_SoftLock;
        public IActionEventData Creact() => new Event_SoftLock();

        [NonSerialized] private Transform m_LockTarget = null;
        [NonSerialized] private bool m_OnLock = false;

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            // EngineDebug.Log("软锁定");
            // return;
            m_OnLock = false;
            ActionStateMachine _actionStateMachine = _actionState.ActionStateMachine;

            if (_actionStateMachine.IsLock)
            {
                m_OnLock = true;
                m_LockTarget = _actionStateMachine.LockTransform;
                return;
            }
            
            
            Transform _center = _actionStateMachine.CurUnit.transform;
            Collider[] _colliders = Physics.OverlapSphere(_center.position, m_Radius, _actionStateMachine.GetLayer(m_CheckLayer) , QueryTriggerInteraction.Ignore);

            if (m_ReferToCam)
            {
                if (_actionStateMachine.TryGetComponent(out CharacterConfig _config))
                {
                    if (!_config.HelpPointDic.TryGetValue(ECharacteLimbType.Cam_Main, out _center))
                    {
                        EngineDebug.LogWarning("软锁定运行错误：角色未配置相机挂点");
                    }
                }
                else
                {
                    EngineDebug.LogWarning("软锁定运行错误：角色未配置相机挂点");
                }  
            }

            Transform _findTarget = null;
            float _findMinAngle = 360;
            foreach (var _collider in _colliders)
            {
                Transform _transform = _collider.transform;
                if (_center != _transform)
                {
                    Vector3 _transDir = _transform.position - _center.position;
                    float _angleOffset = Vector3.Angle(_center.forward, _transDir.normalized);
                    if (_angleOffset < _findMinAngle)
                    {
                        _findMinAngle = _angleOffset;
                        _findTarget = _transform;
                    }
                }
                // EngineDebug.Log("成功软锁定");
            }

            if (_findMinAngle < m_SelfAngle * 0.5f)
            {
                m_OnLock = true;
                m_LockTarget = _findTarget;
            }
        }

        public void Update(ActionStatePart _actionState, ActionMachineTime _actionTime)
        {
            if(!m_OnLock)return;
            // EngineDebug.Log("成功软锁定");
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            if(_stateMachine.TryGetLogic(out Ex_Update_CharacterControl _characterControl))
            {
                Transform _transform = _stateMachine.CurUnit.transform;
                Vector3 _lockDir = m_LockTarget.position - _transform.position;
                _lockDir.y = 0;
                if (m_LerpSpeed > 0)
                {
                    Quaternion _lerpRot = Quaternion.Lerp(_stateMachine.CurUnit.transform.rotation,
                        Quaternion.LookRotation(_lockDir.normalized), _actionTime.Deltatime * m_LerpSpeed);
                    _characterControl.SetRot(_lerpRot, m_Priority);
                }
                else
                {
                    _characterControl.SetRot(Quaternion.LookRotation(_lockDir.normalized), m_Priority);
                }
            }
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_SoftLock _event = _eventData as Event_SoftLock;

            _event.Radius = m_Radius;
            _event.SelfAngle = m_SelfAngle;
            _event.LerpSpeed = m_LerpSpeed;
            _event.CheckLayer = m_CheckLayer;
            _event.ReferToCam = m_ReferToCam;
            _event.Priority = m_Priority;

            return _event;
        }
    }
}
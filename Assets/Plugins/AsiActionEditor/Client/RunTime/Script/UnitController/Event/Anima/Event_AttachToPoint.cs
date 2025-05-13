using System;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_AttachToPoint : IActionEventData
    {
        [SerializeField] protected int m_HelpPointID;
        [SerializeField] protected GPoint m_PointData = new GPoint();
        [SerializeField] protected EVector3 m_Position = new EVector3();
        [SerializeField] protected EVector3 m_Rotation = new EVector3();


        #region Property
        [EditorProperty("对齐的点", EditorPropertyType.EEPT_CharacteLimbType)]
        public int HelpPointID
        {
            get { return m_HelpPointID; }
            set { m_HelpPointID = value; }
        }
        [EditorProperty("对齐的点", EditorPropertyType.EEPT_GPoint)]
        public GPoint PointData
        {
            get { return m_PointData; }
            set { m_PointData = value; }
        }
        [EditorProperty("相对偏移位置", EditorPropertyType.EEPT_Vector3)]
        public EVector3 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }
        [EditorProperty("相对偏移角度", EditorPropertyType.EEPT_Vector3)]
        public EVector3 Rotation
        {
            get { return m_Rotation; }
            set { m_Rotation = value; }
        }
        #endregion

        public int GetEvenType() => 1;
        public IActionEventData Creact() => new Event_AttachToPoint();

        [NonSerialized] private bool m_IsValid = false;
        [NonSerialized] private Transform m_ReferTransform;
        [NonSerialized] private Matrix4x4 m_Matrix;
        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            m_IsValid = false;
            ActionStateMachine _actionStateMachine = _actionState.ActionStateMachine;
            m_PointData.Init(_actionStateMachine);

            if (_actionStateMachine.TryGetComponent(out CharacterConfig _characterConfig))
            {
                if (_characterConfig.HelpPointDic.TryGetValue((ECharacteLimbType)m_HelpPointID, out Transform _transform))
                {
                    m_ReferTransform = _transform;
                    m_IsValid = m_PointData.IsValid;
                }
            }
        }

        public void Update(ActionStatePart _actionState, ActionMachineTime _actionTime)
        {
            
        }

        public void Exit(ActionStatePart _actionState, bool _isSingle)
        {
            
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_AttachToPoint _event = (Event_AttachToPoint)_eventData;
            _event.PointData = (GPoint)m_PointData.Clone();
            _event.Position = m_Position;
            _event.Rotation = m_Rotation;
            return _event;
        }
    }
}
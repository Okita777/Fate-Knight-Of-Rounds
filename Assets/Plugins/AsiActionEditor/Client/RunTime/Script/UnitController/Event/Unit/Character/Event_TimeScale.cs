using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_TimeScale : IActionEventData
    {
        [SerializeField] protected float m_TimeScale = 1;

        #region Property

        [EditorProperty("时间缩放: ", EditorPropertyType.EEPT_Float)]
        public float TimeScale
        {
            get { return m_TimeScale; }
            set { m_TimeScale = value; }
        }
        #endregion
        public int GetEvenType() => (int)EEvenType.EET_TimeScale;
        public IActionEventData Creact() => new Event_TimeScale();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            _stateMachine.TimeScale = m_TimeScale;
            _stateMachine.CurAnimator.speed = m_TimeScale;
        }

        public void Exit(ActionStatePart _actionState, bool _interruot)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            _stateMachine.TimeScale = 1;
            _stateMachine.CurAnimator.speed = 1;
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_TimeScale _event = _eventData as Event_TimeScale;
            
            _event.TimeScale = m_TimeScale;
            
            return _event;
        }
    }
}
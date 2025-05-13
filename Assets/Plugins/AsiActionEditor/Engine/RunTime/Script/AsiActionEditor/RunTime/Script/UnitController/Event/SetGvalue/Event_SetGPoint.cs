using AsiActionEngine.RunTime.GraphVal;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class Event_SetGPoint : IActionEventData
    {
        [SerializeField] protected GraphEvent_NoValue_Point m_GraphEvent = new GraphEvent_NoValue_Point();
        [SerializeField] protected GValue_SetPoint m_SetFloat = new GValue_SetPoint();
        #region property
        [EditorProperty("设置逻辑", EditorPropertyType.EEPT_GraphValue)]
        public GraphEvent_NoValue_Point GraphEvent
        {
            get { return m_GraphEvent; }
            set { m_GraphEvent = value; } 
            
        }
        [EditorProperty("设置对象", EditorPropertyType.EEPT_SetGPoint)]
        public GValue_SetPoint SetFloat
        {
            get { return m_SetFloat; }
            set { m_SetFloat = value; } 
            
        }
        #endregion
        public int GetEvenType() => -(int)EEvenTypeInternal.EET_SetGPoint;

        public IActionEventData Creact() => new Event_SetGPoint();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            if (_isSingle)
            {
                PointData val = GraphEvent.value(_actionState, new ActionMachineTime(0, 0, 0, 0));
                m_SetFloat.Init(_actionState.ActionStateMachine);
                if (m_SetFloat.m_IsSet)
                {
                    m_SetFloat.Set(val);
                }
            }
        }

        public void Update(ActionStatePart _actionState, ActionMachineTime _actionTime)
        {
            if (m_SetFloat.m_IsSet)
            {
                m_SetFloat.Init(_actionState.ActionStateMachine);
                m_SetFloat.Set(GraphEvent.value(_actionState, _actionTime)); 
            }
        }


        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_SetGPoint eventCast = _eventData as Event_SetGPoint;
            eventCast.m_GraphEvent = m_GraphEvent.Clone();
            eventCast.m_SetFloat = m_SetFloat.Clone();
            return eventCast;
        }
    }
}
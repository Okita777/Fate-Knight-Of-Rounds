using AsiActionEngine.RunTime.GraphVal;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class Event_SetGFloat : IActionEventData
    {
        [SerializeField] protected GraphEvent_NoValue_Float m_GraphEvent = new GraphEvent_NoValue_Float();
        [SerializeField] protected GValue_SetFloat m_SetFloat = new GValue_SetFloat();
        #region property
        [EditorProperty("设置逻辑", EditorPropertyType.EEPT_GraphValue)]
        public GraphEvent_NoValue_Float GraphEvent
        {
            get { return m_GraphEvent; }
            set { m_GraphEvent = value; } 
            
        }
        [EditorProperty("设置对象", EditorPropertyType.EEPT_SetGFloat)]
        public GValue_SetFloat SetFloat
        {
            get { return m_SetFloat; }
            set { m_SetFloat = value; } 
            
        }
        #endregion
        public int GetEvenType() => -(int)EEvenTypeInternal.EET_SetGFloat;

        public IActionEventData Creact() => new Event_SetGFloat();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            if (_isSingle)
            {
                float val = GraphEvent.value(_actionState, new ActionMachineTime(0, 0, 0, 0));
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
                // EngineDebug.Log($"Gvalue引擎: {m_SetFloat.m_Value.mStateMachine.GetHashCode()}   当前引擎: {_actionState.ActionStateMachine.GetHashCode()}");
                m_SetFloat.Init(_actionState.ActionStateMachine);
                m_SetFloat.Set(GraphEvent.value(_actionState, _actionTime));
            }
        }


        public IActionEventData Clone(IActionEventData _eventData)
        {
            // EngineDebug.LogError($"克隆: {this.GetHashCode()}");
            Event_SetGFloat eventCast = _eventData as Event_SetGFloat;
            eventCast.m_GraphEvent = m_GraphEvent.Clone();
            eventCast.m_SetFloat = m_SetFloat.Clone();
            return eventCast;
        }
    }
}
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_SetGValue : IActionEventData
    {
        [SerializeField] protected GValue_Setting mGValueSet = new GValue_Setting();
        [SerializeField] protected GValue_Setting mExitGValueSet = new GValue_Setting();

        [EditorProperty("设置GValue", EditorPropertyType.EEPT_GValueSetting)]
        public GValue_Setting GValueSet
        {
            get { return mGValueSet; }
            set { mGValueSet = value; }
        }
        [EditorProperty("退出时设置GValue", EditorPropertyType.EEPT_GValueSetting)]
        public GValue_Setting ExitGValueSet
        {
            get { return mExitGValueSet; }
            set { mExitGValueSet = value; }
        }
        public int GetEvenType()=>(int)EEvenType.EET_SetGValue;
        public IActionEventData Creact() => new Event_SetGValue();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            mGValueSet.OnSet(_stateMachine);
        }

        public void Exit(ActionStatePart _actionState, bool _interruot)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            mExitGValueSet.OnSet(_stateMachine);
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_SetGValue _event = _eventData as Event_SetGValue;
            _event.GValueSet = mGValueSet;
            _event.ExitGValueSet = mExitGValueSet;
            return _event;
        }
    }
}
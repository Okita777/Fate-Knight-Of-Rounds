using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_CharacterGravity : IActionEventData
    {
        [SerializeField] private float mGravity = 1f;

        #region property

        [EditorProperty("重力: ", EditorPropertyType.EEPT_Float)]
        public float Gravity
        {
            get { return mGravity; }
            set { mGravity = value; }
        }

        #endregion
        public int GetEvenType() => (int)EEvenType.EET_CharacterGravity;
        public IActionEventData Creact() => new Event_CharacterGravity();

        private float m_Ground;
        
        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            if (_stateMachine.TryGetLogic(out Ex_Update_CharacterControl _characterControl))
            {
                m_Ground = _characterControl.CharacterGravity;
                _characterControl.CharacterGravity = mGravity;
            }
            // Debug.Log("设置重力: " + mGravity);
        }

        public void Exit(ActionStatePart _actionState, bool _interruot)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            if (_stateMachine.TryGetLogic(out Ex_Update_CharacterControl _characterControl))
            {
                _characterControl.CharacterGravity = m_Ground;
            }
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_CharacterGravity _characterGravity = _eventData as Event_CharacterGravity;
            
            _characterGravity.Gravity = mGravity;
            
            return _characterGravity;
        }
    }
}
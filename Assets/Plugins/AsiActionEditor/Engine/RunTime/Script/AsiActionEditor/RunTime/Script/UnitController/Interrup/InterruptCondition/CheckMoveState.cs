using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class CheckMoveState : IInterruptCondition
    {
        [SerializeField]private bool mIsMove = true;
        [SerializeField]private bool mIsMovePre = false;

        #region Property
        [EditorProperty("预输入", EditorPropertyType.EEPT_Bool)]
        public bool IsMovePre
        {
            get { return mIsMovePre; }
            set { mIsMovePre = value; }
        }
        [EditorProperty("移动输入中", EditorPropertyType.EEPT_Bool)]
        public bool IsMove
        {
            get { return mIsMove; }
            set { mIsMove = value; }
        }
        #endregion

        public int InterruptType => -(int)EInterruptTypeInternal.EIT_CheckMove;

        public bool CheckInterrupt(Unit unit, ActionStatePart actionStatePart)
        {
            return mIsMovePre? actionStatePart.ActionStateMachine.IsMoveInputPre : actionStatePart.ActionStateMachine.IsMoveInput == mIsMove;
        }
        
        public IInterruptCondition Clone()
        {
            CheckMoveState _CheckMoveState = new CheckMoveState();
            _CheckMoveState.IsMove = IsMove;
            _CheckMoveState.mIsMovePre = mIsMovePre;
            return _CheckMoveState;
        }
    }
}
using System;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class Event_PlayAnim : IActionEventData
    {
        [SerializeField] private string mAnimName = string.Empty;

        #region property
        public string AnimName
        {
            get { return mAnimName; }
            set { mAnimName = value; }
        }
        #endregion


        public int GetEvenType() => -(int)EEvenTypeInternal.EET_DTD_PlayAnim;
        public IActionEventData Creact() => new Event_PlayAnim();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            float _doubling = MotionEngineConst.TimeDoubling;
            if (_actionState.MixTime < 0)
            {
                if (_actionState.CurrentActionState_Last == null)
                {
                    EngineDebug.LogWarning("CurrentActionState_Last");
                }
                float _offsetTime = (float)_actionState.ElapsedTime_last / _actionState.CurrentActionState_Last.TotalTime;
                _offsetTime *= _actionState.CurrentActionState.TotalTime;
                _actionState.ElapsedTime = _offsetTime;

                float _jumpMix = (float)_actionState.MixTime / MotionEngineConst.TimeDoubling * -1;
                if (_offsetTime + _jumpMix * _doubling > _actionState.CurrentActionState.TotalTime)
                {
                    _actionState.IsJumpEnd = true;
                }//这次跳过结尾循环动画跳转
                _actionState.PlayAnim(mAnimName, _jumpMix,
                    _actionState.AnimaLayer, _offsetTime / _doubling);
            }
            else
            {
                int _startTime = _actionState.OffsetTime - _actionState.CurrentActionState.AnimEvent.TriggerTime;
                _actionState.PlayAnim(mAnimName, _actionState.MixTime / _doubling,
                    _actionState.AnimaLayer, _startTime / _doubling);
            }
            // _actionState.ActionStateMachine.CurAnimator.CrossFadeInFixedTime();
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_PlayAnim _event = _eventData as Event_PlayAnim;

            _event.mAnimName = mAnimName;
            
            return _event;
        }
    }
}
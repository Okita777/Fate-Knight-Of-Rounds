using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public partial class ActionStatePart
    {
        public bool IsJumpEnd = false;//仅在这次跳过结尾动画跳转逻辑
        // private int mFinishLayer = 0;
        private bool ActionStateCheckEnd()
        {
            if (ElapsedTime > CurrentActionState.TotalTime)
            {
                if (!string.IsNullOrEmpty(CurrentActionState.DefaultAction))
                {
                    // Debug.Log($"尝试跳转至: {CurrentActionState.DefaultAction}");
                    return OnChangeState(CurrentActionState.DefaultAction, CurrentActionState.MixTime,
                        CurrentActionState.OffsetTime);
                }
                else
                {
                    //刷新事件
                    OnChangeEvent(CurrentActionState.EventList,ElapsedTime,0);

                    ElapsedTime -= CurrentActionState.TotalTime;
                    MixTime = 0;
                    OffsetTime = 0;

                    if (!IsJumpEnd)
                    {
                        if (CurrentActionState.AnimEvent is not null && CurrentActionState.AnimEvent.EventData is not null)
                            CurrentActionState.AnimEvent.EventData.Enter(this, false); //执行动画轨
                    }
                    else
                    {
                        IsJumpEnd = false;
                    }
                }
            }

            return false;
        }
    }
}
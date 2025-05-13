using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public partial class ActionStatePart
    {
        private Animator mCurAnimator => mActionStateMachine.CurAnimator;
        private void OnPlayAnim(string _name, float _mixTime, int _layer, float _offsetTime)
        {
            
#if UNITY_EDITOR
            if (!mCurAnimator.HasState(_layer, Animator.StringToHash(_name)))
            {
                EngineDebug.Log($"在 {_layer} 层下不存在 [{_name}] 动画,我是由 [{mCurActionLayer}] 执行");
                return;
            }
#endif
            
            mCurAnimator.CrossFadeInFixedTime(_name, _mixTime, _layer, _offsetTime);
        }
    }
}
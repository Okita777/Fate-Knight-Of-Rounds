using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public partial class ActionStateMachine
    {
        private Quaternion mMouseXY { get; set; }
        private Quaternion mCamRot{ get; set; }

        private float OnGetAnimatorFloat(string _name)
        {
            return CurAnimator.GetFloat(_name);
            // return 0;
        }
        private int OnGetAnimatorInt(string _name)
        {
            return CurAnimator.GetInteger(_name);
            // return 0;
        }
        private void OnSetAnimatorFloat(string _name, float _value)
        {
            CurAnimator.SetFloat(_name, _value);
        }
        private void OnSetAnimatorInt(string _name, int _value)
        {
            CurAnimator.SetInteger(_name, _value);
        }
    }
}
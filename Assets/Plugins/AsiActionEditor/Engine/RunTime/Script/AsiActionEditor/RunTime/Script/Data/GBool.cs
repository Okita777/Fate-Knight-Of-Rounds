using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GBool : GValue
    {
        [SerializeField] public bool mSerValue;
        public GBool(bool _value = false, bool _mType = false)
        {
            mSerValue = _value;
            mType = _mType;
        }
        public GBool(bool _mType)
        {
            mType = _mType;
        }
        public bool value
        {
            get
            {
                if (mType) return mStateMachine.GValue.mEngineBool[mValueIndex];
                return mSerValue;
            }
            set
            {
                if (mType) mStateMachine.GValue.mEngineBool[mValueIndex] = value;
                else mSerValue = value;
            }
        }

        public override GValue Clone()
        {
#if UNITY_EDITOR
            GBool n = new GBool(mSerValue);
            n.mType = mType;
            n.mValueIndex = mValueIndex;
            return n;
#endif
            return this;

        }
    }
}
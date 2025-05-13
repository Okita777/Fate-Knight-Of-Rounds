using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GEnum : GValue
    {
        [SerializeField] public byte mSerValue;
        public GEnum(byte _value = 0, bool _type = false)
        {
            mSerValue = _value;
            mType = _type;
        }
        public byte value
        {
            get
            {
                if (mType) return mStateMachine.GValue.mEngineEnum[mValueIndex];
                return mSerValue;
            }
            set
            {
                if (mType) mStateMachine.GValue.mEngineEnum[mValueIndex] = value;
                else mSerValue = value;
            }
        }
        public override GValue Clone()
        {

#if UNITY_EDITOR
            GEnum n = new GEnum(mSerValue);
            n.mType = mType;
            n.mValueIndex = mValueIndex;
            return n;
#endif
            return this;
        }
    }
}
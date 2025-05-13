using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GTransform : GValue
    {
        [SerializeField] public byte mSerValue;
        public GTransform(byte _value = 0)
        {
            mSerValue = _value;
            mType = true;
        }
        public GTransform(bool _Type)
        {
            mType = _Type;
        }
        public Transform value
        {
            get
            {
                int _index = mStateMachine.GValue.mEngineTransform[mValueIndex];
                _index += mSerValue * 1000;
                return mStateMachine.GetTransform(_index);
            }
            set
            {
                int _index = mStateMachine.GValue.mEngineTransform[mValueIndex];
                _index += mSerValue * 1000;
                mStateMachine.SetTransform(_index, value);
            }
        }

        public bool isValid
        {
            get
            {
                int _index = mStateMachine.GValue.mEngineTransform[mValueIndex];
                _index += mSerValue * 1000;
                return mStateMachine.GetTransform(_index) is not null;
            }
        }
        public override GValue Clone()
        {
#if UNITY_EDITOR
            GTransform n = new GTransform(mSerValue);
            n.mType = mType;
            n.mValueIndex = mValueIndex;
            return n;
#endif
            return this;
        }
    }
}
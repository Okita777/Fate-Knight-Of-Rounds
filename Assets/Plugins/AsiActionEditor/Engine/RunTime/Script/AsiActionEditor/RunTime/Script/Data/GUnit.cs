using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GUnit : GValue
    {
        [SerializeField] public byte mSerValue;
        // [SerializeField] public bool mIsLocalPlayer = false;
        public GUnit(byte _value = 0)
        {
            mSerValue = _value;
            mType = true;
        }
        public GUnit(bool _Type)
        {
            mType = _Type;
        }
        public Unit value
        {
            get
            {
                int _index = mStateMachine.GValue.mEngineUnit[mValueIndex];
                _index += mSerValue * 1000;
                return mStateMachine.GetUnit(_index);
            }
            set
            {
                int _index = mStateMachine.GValue.mEngineUnit[mValueIndex];
                _index += mSerValue * 1000;
                mStateMachine.SetUnit(_index, value);
            }
        }

        public bool IsValid
        {
            get
            {
                int _index = mStateMachine.GValue.mEngineUnit[mValueIndex];
                _index += mSerValue * 1000;
                return mStateMachine.GetUnit(_index) is not null;
            }
        }
        
        public override GValue Clone()
        {
#if UNITY_EDITOR
            GUnit n = new GUnit(mSerValue);
            n.mType = mType;
            n.mValueIndex = mValueIndex;
            return n;
#endif
            return this;
        }
    }
}
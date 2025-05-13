using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GPoint : GValue
    {
        [SerializeField] public byte mSerValue;
        public GPoint(byte _value = 0)
        {
            mSerValue = _value;
            mType = true;
        }
        public GPoint(bool _type)
        {
            mType = _type;
        }
        public PointData value
        {
            get
            {
                int _index = mStateMachine.GValue.mEnginePointData[mValueIndex];
                _index += mSerValue * 1000;
                return mStateMachine.GetPoint(_index);
            }
            set
            {
                int _index = mStateMachine.GValue.mEnginePointData[mValueIndex];
                _index += mSerValue * 1000;
                mStateMachine.SetPoint(_index, value);
            }
        }

        public void Remove()
        {
            int _index = mStateMachine.GValue.mEnginePointData[mValueIndex];
            _index += mSerValue * 1000;
            if (mStateMachine.PointDic.ContainsKey(_index))
            {
                mStateMachine.PointDic.Remove(_index);
            }
        }

        public bool IsValid
        {
            get
            {
                int _index = mStateMachine.GValue.mEnginePointData[mValueIndex];
                _index += mValueIndex * 1000;
                return mStateMachine.PointDic.ContainsKey(_index);
            }
        }
        
        

        public override GValue Clone()
        {
#if UNITY_EDITOR
            GPoint n = new GPoint(mSerValue);
            n.mType = mType;
            n.mValueIndex = mValueIndex;
            return n;
#endif
            return this;
        }
    }
}
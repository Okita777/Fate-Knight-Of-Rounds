using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public class GVector3 : GValue
    {
        public Vector3 mSerValue;

        public GVector3(Vector3 _value)
        {
            mSerValue = _value;
        }
        
        public Vector3 value
        {
            get
            {
                if (mType) return mStateMachine.GValue.mEngineVector3[mValueIndex];
                return mSerValue;
            }
            set
            {
                if (mType) mStateMachine.GValue.mEngineVector3[mValueIndex] = value;
                else mSerValue = value;
            }
        }
        public override GValue Clone()
        {
            GVector3 n = new GVector3(mSerValue);
            n.mType = mType;
            n.mValueIndex = mValueIndex;
            return n;
        }
    }
}
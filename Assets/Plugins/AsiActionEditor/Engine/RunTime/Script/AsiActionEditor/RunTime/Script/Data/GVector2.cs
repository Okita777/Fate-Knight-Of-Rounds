using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public class GVector2 : GValue
    {
        public Vector2 mSerValue;

        public GVector2(Vector2 _value)
        {
            mSerValue = _value;
        }
        
        public Vector2 value
        {
            get
            {
                if (mType) return mStateMachine.GValue.mEngineVector2[mValueIndex];
                return mSerValue;
            }
            set
            {
                if (mType) mStateMachine.GValue.mEngineVector2[mValueIndex] = value;
                else mSerValue = value;
            }
        }
        public override GValue Clone()
        {
            GVector2 n = new GVector2(mSerValue);
            n.mType = mType;
            n.mValueIndex = mValueIndex;
            return n;
        }
    }
}
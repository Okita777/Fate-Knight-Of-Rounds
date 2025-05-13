using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GColor : GValue
    {
        [SerializeField] public Color mSerValue;
        public GColor(Color _value)
        {
            mSerValue = _value;
        }
        public Color value
        {
            get
            {
                if (mType) return mStateMachine.GValue.mEngineColor[mValueIndex];
                return mSerValue;
            }
            set
            {
                if (mType) mStateMachine.GValue.mEngineColor[mValueIndex] = value;
                else mSerValue = value;
            }
        }
        public override GValue Clone()
        {
#if UNITY_EDITOR
            GColor n = new GColor(mSerValue);
            n.mType = mType;
            n.mValueIndex = mValueIndex;
            return n;
#endif
            return this;
        }
    }
}
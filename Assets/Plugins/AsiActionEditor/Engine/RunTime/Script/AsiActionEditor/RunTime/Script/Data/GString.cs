namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GString : GValue
    {
        [UnityEngine.SerializeField] public string mSerValue;

        public GString(string _value = "", bool _mType = false)
        {
            mSerValue = _value;
            mType = _mType;
        }
        public GString(bool _mType)
        {
            mType = _mType;
        }
        public string value
        {
            get
            {
                if (mType) return mStateMachine.GValue.mEngineString[mValueIndex];
                return mSerValue;
            }
            set
            {
                if (mType) mStateMachine.GValue.mEngineString[mValueIndex] = value;
                else mSerValue = value;
            }
        }
        public override GValue Clone()
        {
#if UNITY_EDITOR
            GString n = new GString(mSerValue);
            n.mType = mType;
            n.mValueIndex = mValueIndex;
            return n; 
#endif
            return this;
        }
    }
}
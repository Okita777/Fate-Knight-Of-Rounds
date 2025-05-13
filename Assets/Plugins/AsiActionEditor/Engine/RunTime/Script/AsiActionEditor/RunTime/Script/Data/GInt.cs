using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GInt : GValue
    {
        [UnityEngine.SerializeField] public int mSerValue;

        public GInt(int _value = 0, bool _mType = false)
        {
            mSerValue = _value;
            mType = _mType;
        }
        public GInt(bool _mType)
        {
            mType = _mType;
        }
        public int value
        {
            get
            {
                if (mType)
                {
#if UNITY_EDITOR
                    if (mStateMachine is null)
                    {
                        Debug.LogError("GInt 获取失败，未在使用前执行【Init】函数");
                        return 0;
                    }
                    if (mStateMachine.GValue.mEngineFloat.Length <= mValueIndex)
                    {
                        Debug.LogError("GInt 获取失败，尝试获取序号超出数组长度");
                        return 0;
                    }
#endif
                    return mStateMachine.GValue.mEngineInt[mValueIndex];
                }
                return mSerValue;
            }
            set
            {
                if (mType)
                {
#if UNITY_EDITOR
                    if (mStateMachine is null)
                    {
                        Debug.LogError("GInt 获取失败，未在使用前执行【Init】函数");
                        return;
                    }
                    if (mStateMachine.GValue.mEngineFloat.Length <= mValueIndex)
                    {
                        Debug.LogError("GInt 获取失败，尝试获取序号超出数组长度");
                        return;
                    }
#endif
                    mStateMachine.GValue.mEngineInt[mValueIndex] = value;
                }
                else mSerValue = value;
            }
        }
        public override GValue Clone()
        {
#if UNITY_EDITOR
            GInt n = new GInt(mSerValue);
            n.mType = mType;
            n.mValueIndex = mValueIndex;
            return n;
#endif
            return this;
        }
    }
}
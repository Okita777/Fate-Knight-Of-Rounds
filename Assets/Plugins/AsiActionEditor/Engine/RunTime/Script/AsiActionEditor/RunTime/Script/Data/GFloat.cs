using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GFloat : GValue
    {
        [UnityEngine.SerializeField] public float mSerValue;

        public GFloat(float _value = 0.0f, bool _mType = false)
        {
            mSerValue = _value;
            mType = _mType;
        }
        public GFloat(bool _mType)
        {
            mType = _mType;
        }
        public float value
        {
            get
            {
                if (mType)
                {
#if UNITY_EDITOR
                    if (mStateMachine is null)
                    {
                        Debug.LogError("GFloat 获取失败，未在使用前执行【Init】函数");
                        return 0.0f;
                    }
                    if (mStateMachine.GValue.mEngineFloat.Length <= mValueIndex)
                    {
                        Debug.LogError("GFloat 获取失败，尝试获取序号超出数组长度");
                        return 0.0f;
                    }
#endif
                    return mStateMachine.GValue.mEngineFloat[mValueIndex];
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
                        Debug.LogError("GFloat 获取失败，未在使用前执行【Init】函数");
                        return;
                    }
                    if (mStateMachine.GValue.mEngineFloat.Length <= mValueIndex)
                    {
                        Debug.LogError("GFloat 获取失败，尝试获取序号超出数组长度");
                        return;
                    }
#endif
                    mStateMachine.GValue.mEngineFloat[mValueIndex] = value;
                }
                else mSerValue = value;
            }
        }
        public override GValue Clone()
        {
#if UNITY_EDITOR
            GFloat n = new GFloat(mSerValue);
            n.mType = mType;
            n.mValueIndex = mValueIndex;
            return n;
#endif
            return this;
        }
    }
}
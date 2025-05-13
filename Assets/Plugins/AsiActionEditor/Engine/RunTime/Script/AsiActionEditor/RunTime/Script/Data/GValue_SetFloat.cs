using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GValue_SetFloat
    {
        // public GValue_SetFloat(){}
        //
        // public GValue_SetFloat(bool _isSet)
        // {
        //     m_IsSet = _isSet;
        // }

        [SerializeField] public GFloat m_Value = new GFloat(true);
        [SerializeField] public bool m_IsSet = true;
        public delegate float GetFloatDelegate();
        public void Init(ActionStateMachine stateMachine)
        {
            m_Value.Init(stateMachine);
        }
        public void Set(float pointData)
        {
            if (m_IsSet)
            {
                m_Value.value = pointData;
            }
        }
        
        public void Set(GetFloatDelegate action)
        {
            if (m_IsSet)
            {
#if UNITY_EDITOR
                if (action == null)
                {
                    EngineDebug.LogError("GValue_SetFloat: action is null");
                    return;
                }
#endif
                m_Value.value = action();
            }
        }
        public GValue_SetFloat Clone()
        {
#if UNITY_EDITOR
            GValue_SetFloat clone = new GValue_SetFloat();
            clone.m_Value = (GFloat)m_Value.Clone();
            clone.m_IsSet = m_IsSet;
            return clone;
#endif
            return this;
        }
    }
}
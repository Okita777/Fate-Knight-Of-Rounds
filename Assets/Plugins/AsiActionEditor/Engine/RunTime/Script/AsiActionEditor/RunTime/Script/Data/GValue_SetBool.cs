using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GValue_SetBool
    {
        [SerializeField] public GBool m_Value = new GBool(true);
        [SerializeField] public bool m_IsSet = true;
        public delegate bool GetBoolDelegate();
        public void Init(ActionStateMachine stateMachine)
        {
            m_Value.Init(stateMachine);
        }
        public void Set(bool pointData)
        {
            if (m_IsSet)
            {
                m_Value.value = pointData;
            }
        }
        
        public void Set(GetBoolDelegate action)
        {
            if (m_IsSet)
            {
#if UNITY_EDITOR
                if (action == null)
                {
                    EngineDebug.LogError("GValue_SetBool: action is null");
                    return;
                }
#endif
                m_Value.value = action();
            }
        }
        public GValue_SetBool Clone()
        {
#if UNITY_EDITOR
            GValue_SetBool clone = new GValue_SetBool();
            clone.m_Value = (GBool)m_Value.Clone();
            clone.m_IsSet = m_IsSet;
            return clone;
#endif
            return this;
        }
    }
}
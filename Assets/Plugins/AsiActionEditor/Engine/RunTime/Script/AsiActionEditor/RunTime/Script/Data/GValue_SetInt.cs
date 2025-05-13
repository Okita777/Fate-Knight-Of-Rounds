using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GValue_SetInt
    {
        [SerializeField] public GInt m_Value = new GInt(true);
        [SerializeField] public bool m_IsSet = true;
        public delegate int GetIntDelegate();
        public void Init(ActionStateMachine stateMachine)
        {
            m_Value.Init(stateMachine);
        }
        public void Set(int pointData)
        {
            if (m_IsSet)
            {
                m_Value.value = pointData;
            }
        }
        
        public void Set(GetIntDelegate action)
        {
            if (m_IsSet)
            {
#if UNITY_EDITOR
                if (action == null)
                {
                    EngineDebug.LogError("GValue_SetInt: action is null");
                    return;
                }
#endif
                m_Value.value = action();
            }
        }
        public GValue_SetInt Clone()
        {
#if UNITY_EDITOR
            GValue_SetInt clone = new GValue_SetInt();
            clone.m_Value = (GInt)m_Value.Clone();
            clone.m_IsSet = m_IsSet;
            return clone;
#endif
            return this;
        }
    }
}
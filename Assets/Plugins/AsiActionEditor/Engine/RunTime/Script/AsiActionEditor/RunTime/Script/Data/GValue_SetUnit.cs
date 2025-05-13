using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GValue_SetUnit
    {
        [SerializeField] public GUnit m_Value = new GUnit(true);
        [SerializeField] public bool m_IsSet = true;
        public delegate Unit GetUnitDelegate();
        public void Init(ActionStateMachine stateMachine)
        {
            m_Value.Init(stateMachine);
        }
        public void Set(Unit pointData)
        {
            if (m_IsSet)
            {
                m_Value.value = pointData;
            }
        }

        public void SetNull()
        {
            if (m_IsSet) m_Value = null;
        }

        
        public void Set(GetUnitDelegate action)
        {
            if (m_IsSet)
            {
#if UNITY_EDITOR
                if (action == null)
                {
                    EngineDebug.LogError("GValue_SetUnit: action is null"); 
                    return;
                }
#endif
                m_Value.value = action();
            }
        }
        public GValue_SetUnit Clone()
        {
#if UNITY_EDITOR
            GValue_SetUnit clone = new GValue_SetUnit();
            clone.m_Value = (GUnit)m_Value.Clone();
            clone.m_IsSet = m_IsSet;
            return clone;
#endif
            return this;
        }
    }
}
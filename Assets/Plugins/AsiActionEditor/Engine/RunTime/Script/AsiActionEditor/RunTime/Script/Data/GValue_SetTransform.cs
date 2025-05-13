using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GValue_SetTransform
    {
        [SerializeField] public GTransform m_Value = new GTransform(true);
        [SerializeField] public bool m_IsSet = true;
        public delegate Transform GetTransformDelegate();
        public void Init(ActionStateMachine stateMachine)
        {
            m_Value.Init(stateMachine);
        }
        public void Set(Transform pointData)
        {
            if (m_IsSet)
            {
                m_Value.value = pointData;
            }
        }

        public void SetNull()
        {
            if (m_IsSet) m_Value.value = null;
        }

        public void Set(GetTransformDelegate action)
        {
            if (m_IsSet)
            {
#if UNITY_EDITOR
                if (action == null)
                {
                    EngineDebug.LogError("GValue_SetTransform: action is null");
                    return;
                }
#endif
                m_Value.value = action();
            }
        }

        public GValue_SetTransform Clone()
        {
#if UNITY_EDITOR
            GValue_SetTransform clone = new GValue_SetTransform();
            clone.m_Value = (GTransform)m_Value.Clone();
            clone.m_IsSet = m_IsSet;
            return clone;
#endif
            return this;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GValue_SetPoint
    {
        [SerializeField] public GPoint m_Value = new GPoint(true);
        [SerializeField] public bool m_IsSet = true;
        public delegate PointData GetPointDataDelegate();
        public void Init(ActionStateMachine stateMachine)
        {
            m_Value.Init(stateMachine);
        }
        public void Set(PointData pointData)
        {
            if (m_IsSet)
            {
                m_Value.value = pointData;
            }
        }
        
        public void Set(Transform _transform)
        {
            if (m_IsSet)
            {
                m_Value.value = new PointData(_transform.position, _transform.rotation);
            }
        }
        public void Set(GetPointDataDelegate action)
        {
            if (m_IsSet)
            {
#if UNITY_EDITOR
                if (action == null)
                {
                    EngineDebug.LogError("GValue_SetPoint: action is null");
                    return;
                }
#endif
                m_Value.value = action();
            }
        }

        public void SetNull()
        {
            
        }
        
        public GValue_SetPoint Clone()
        {
#if UNITY_EDITOR
            GValue_SetPoint clone = new GValue_SetPoint();
            clone.m_Value = (GPoint)m_Value.Clone();
            clone.m_IsSet = m_IsSet;
            return clone;
#endif
            return this;
        }
    }
}
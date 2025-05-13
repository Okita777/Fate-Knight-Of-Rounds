using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEngine;
using UnityEngine.Events;

namespace AsiTimeLine.RunTime
{
    public class ActionEditor_InteractObj : MonoBehaviour, IInteractObject
    {
        public string InteractName;
        public bool AutoTrigger;
        public string m_TriggerActionName;
        public string m_TriggerActionName_Self;
        public int m_mixTime = 200;
        public int m_offsetTime = 0;
        public List<GValue_SetTransform> m_GValueSets = new List<GValue_SetTransform>();
        public List<Transform> m_Transforms = new List<Transform>();
        public UnityEvent OnInteract = new UnityEvent();
        
        public string m_InteractName() => InteractName;
        public bool m_AutoTrigger() => AutoTrigger;
        
        private Unit m_Unit;
        private bool m_SelfUnit = false;
        public void Start()
        {
            m_SelfUnit = TryGetComponent(out Unit m_Unit);
        }

        public void OnTrigger(ActionStateMachine _actionState)
        {
            for (int i = 0; i < m_GValueSets.Count; i++)
            {
                m_GValueSets[i].Init(_actionState);
                m_GValueSets[i].Set(m_Transforms[i]);
            }

            if (!string.IsNullOrEmpty(m_TriggerActionName))
            {
                _actionState.ChangeAction(m_TriggerActionName, m_mixTime, m_offsetTime);
            }

            if (m_SelfUnit && !string.IsNullOrEmpty(m_TriggerActionName_Self))
            {
                m_Unit.ActionStateMachine.ChangeAction(m_TriggerActionName, m_mixTime, m_offsetTime);
            }
            
            OnInteract?.Invoke();
        }
    }
}
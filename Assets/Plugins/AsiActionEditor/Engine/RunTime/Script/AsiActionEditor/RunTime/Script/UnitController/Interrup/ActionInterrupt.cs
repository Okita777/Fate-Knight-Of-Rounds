using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class ActionInterrupt : IProperty
    {
        [SerializeField] protected int mTriggerTime;
        [SerializeField] protected int mDuration;
        [SerializeField] protected int m_ExecuteTime;
        [SerializeField] protected string mActionName;
        [SerializeField] protected int m_SortID;
        [SerializeField] protected bool m_CheckAllCondition = true;
        [SerializeField] protected int m_CrossFadeTime;
        [SerializeField] protected int m_OffsetTime;
        [SerializeReference] protected List<IInterruptCondition> m_InterruptConditionList = new List<IInterruptCondition>();

        #region Properties
        
        public int TriggerTime
        {
            get { return mTriggerTime; }
            set { mTriggerTime = value; }
        }

        public int Duration
        {
            get { return mDuration; }
            set { mDuration = value; }
        }
        
        public int ExecuteTime
        {
            get { return m_ExecuteTime; }
            set { m_ExecuteTime = value; }
        }
        
        public string ActionName
        {
            get { return mActionName; }
            set { mActionName = value; }
        }
        
        public int SortID
        {
            get { return m_SortID; }
            set { m_SortID = value; }
        }
        
        public bool CheckAllCondition
        {
            get { return m_CheckAllCondition; }
            set { m_CheckAllCondition = value; }
        }

        public int CrossFadeTime
        {
            get { return m_CrossFadeTime; }
            set { m_CrossFadeTime = value; }
        }

        public int OffsetTime
        {
            get { return m_OffsetTime; }
            set { m_OffsetTime = value; }
        }

        public List<IInterruptCondition> InterruptConditionList
        {
            get { return m_InterruptConditionList; }
            set { m_InterruptConditionList = value; }
        }
        
        #endregion

        public ActionInterrupt Clone()
        {
            ActionInterrupt _actionInterrupt = new ActionInterrupt();
// EngineDebug.Log("复制");
            _actionInterrupt.mTriggerTime = mTriggerTime;
            _actionInterrupt.mDuration = mDuration;
            _actionInterrupt.m_ExecuteTime = m_ExecuteTime;
            _actionInterrupt.mActionName = mActionName;
            _actionInterrupt.m_SortID = m_SortID;
            _actionInterrupt.m_CheckAllCondition = m_CheckAllCondition;
            _actionInterrupt.m_CrossFadeTime = m_CrossFadeTime;
            _actionInterrupt.m_OffsetTime = m_OffsetTime;
// #if UNITY_EDITOR
//             foreach (IInterruptCondition i in m_InterruptConditionList)
//             {
//                 _actionInterrupt.m_InterruptConditionList.Add(i.Clone());
//             }
// #else
            _actionInterrupt.m_InterruptConditionList = m_InterruptConditionList;
// #endif

            return _actionInterrupt;
        }
    }
}
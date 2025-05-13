using System.Collections.Generic;
using AsiActionEngine.RunTime;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public class EditorActionInterrupt : ActionInterrupt
    {
        // public EInterruptType InterruptType = EInterruptType.EIT_CheckInput;
        public int InterruptType = 0;
        public int SelectID = 0;
        public List<bool> ConditionUnFold;

        public EditorActionInterrupt()
        {
            ConditionUnFold = new List<bool>();
            SelectID = -1;
        }

        public void CopyCondition(EditorActionInterrupt _editorActionInterrupt)
        {
            _editorActionInterrupt.m_InterruptConditionList = new List<IInterruptCondition>();
            _editorActionInterrupt.ConditionUnFold = new List<bool>();
            foreach (var VARIABLE in m_InterruptConditionList)
            {
                _editorActionInterrupt.m_InterruptConditionList.Add(VARIABLE.Clone());
            }
            foreach (var VARIABLE in ConditionUnFold)
            {
                _editorActionInterrupt.ConditionUnFold.Add(VARIABLE == true);
            }
        }

        public EditorActionInterrupt CloneEditor()
        {
            EditorActionInterrupt _actionInterrupt = new EditorActionInterrupt();
            
            // _actionInterrupt = (EditorActionInterrupt)this.Clone();
            
            _actionInterrupt.InterruptType = InterruptType;
            _actionInterrupt.SelectID = SelectID;
            _actionInterrupt.ConditionUnFold = new List<bool>();
            _actionInterrupt.ConditionUnFold.AddRange(ConditionUnFold);
            
            _actionInterrupt.mTriggerTime = mTriggerTime;
            _actionInterrupt.mDuration = mDuration;
            _actionInterrupt.m_ExecuteTime = m_ExecuteTime;
            _actionInterrupt.mActionName = mActionName;
            _actionInterrupt.m_SortID = m_SortID;
            _actionInterrupt.m_CheckAllCondition = m_CheckAllCondition;
            _actionInterrupt.m_CrossFadeTime = m_CrossFadeTime;
            _actionInterrupt.m_OffsetTime = m_OffsetTime;
            _actionInterrupt.m_InterruptConditionList = m_InterruptConditionList;
                
            return _actionInterrupt;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class ActionInterruptGroup
    {
        [SerializeField] private int m_ActionID;
        [SerializeField] private int m_Offset;
        [SerializeField] private List<bool> m_ActionHide;
        [SerializeField] private List<int> m_ActionOffset;

        public ActionInterruptGroup(int _mActionID, int _mOffset, List<bool> _mActionHide, List<int> _mActionOffset)
        {
            m_ActionID = _mActionID;
            m_Offset = _mOffset;
            m_ActionHide = _mActionHide;
            m_ActionOffset = _mActionOffset;
        }
        
        public int ActionID
        {
            get { return m_ActionID; }
            set { m_ActionID = value; }
        }
        public int Offset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }
        public List<bool>  ActionHide
        {
            get { return m_ActionHide; }
            set { m_ActionHide = value; }
        }
        public List<int> ActionOffset
        {
            get { return m_ActionOffset; }
            set { m_ActionOffset = value; }
        }
    }
}
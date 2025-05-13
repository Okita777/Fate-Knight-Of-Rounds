using System.Collections.Generic;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.GValueEquation;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public class EditorGValueEquation
    {
        public GValueEquation_FloatPart gValueEquation_Value;
        public string m_Name;//显示用的名称
        public int m_SorID;//排序用的ID
        public int m_RealID;

        public GValueEquation_FloatPart GetRunTimeData()
        {
            return gValueEquation_Value;
        }
        
        public EditorGValueEquation(string equationName, int sorID, int realID)
        {
            m_Name = equationName;
            m_SorID = sorID;
            m_RealID = realID;
        }

        public EditorGValueEquation Clone()
        {
            EditorGValueEquation _value = new EditorGValueEquation(m_Name, m_SorID, m_RealID);
            _value.gValueEquation_Value = gValueEquation_Value.Clone();
            return _value;
        }

    }
}
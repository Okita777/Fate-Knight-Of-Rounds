using AsiActionEngine.RunTime.Graph;
using UnityEngine;

namespace  AsiActionEngine.RunTime.GraphVal
{
    [System.Serializable]
    ///不带参的Vector3
    public class GraphEvent_NoValue_Int
    {
        [SerializeReference] protected BluePrint_Int m_BluePrint_Val = new GraphEvent_Value_Int();

        #region Property
        [EditorGraphProperty("Int", true, EditorGraphPropertyType.EEPT_Int)]
        public BluePrint_Int BluePrint_Val
        {
            get { return m_BluePrint_Val; }
            set { m_BluePrint_Val = value; }
        }
        #endregion
        [System.NonSerialized] private int m_ReturnVal;
        private void Init(ActionStatePart part, ActionMachineTime _time)
        {
            m_BluePrint_Val.Init(part, _time);
            m_ReturnVal = m_BluePrint_Val.value;
        }

        public int value(ActionStatePart part, ActionMachineTime _time)
        {
            Init(part, _time);
            return m_ReturnVal;
        }


        public GraphEvent_NoValue_Int Clone()
        {
#if UNITY_EDITOR
            GraphEvent_NoValue_Int bluePrintNoValueVector3 = new GraphEvent_NoValue_Int();
            bluePrintNoValueVector3.m_BluePrint_Val = (BluePrint_Int)m_BluePrint_Val.Clone();
            return bluePrintNoValueVector3;
#endif
            return this;
        }
    }
}
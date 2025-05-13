using AsiActionEngine.RunTime.Graph;
using UnityEngine;

namespace  AsiActionEngine.RunTime.GraphVal
{
    [System.Serializable]
    ///不带参的Vector3
    public class GraphEvent_NoValue_Point
    {
        [SerializeReference] protected BluePrint_PointData m_BluePrint_Val = new GraphEvent_BValue_Point();

        #region Property
        [EditorGraphProperty("Float", true, EditorGraphPropertyType.EEPT_PointData)]
        public BluePrint_PointData BluePrint_Val
        {
            get { return m_BluePrint_Val; }
            set { m_BluePrint_Val = value; }
        }
        #endregion

        [System.NonSerialized] private PointData m_ReturnVal;
        private void Init(ActionStatePart part, ActionMachineTime _time)
        {
            m_BluePrint_Val.Init(part, _time);
            m_ReturnVal = m_BluePrint_Val.value;
        }
        public PointData value(ActionStatePart part, ActionMachineTime _time)
        {
            Init(part, _time);
            return m_ReturnVal;
        }


        public GraphEvent_NoValue_Point Clone()
        {
#if UNITY_EDITOR
            GraphEvent_NoValue_Point bluePrintNoValueVector3 = new GraphEvent_NoValue_Point();
            bluePrintNoValueVector3.BluePrint_Val = (BluePrint_PointData)m_BluePrint_Val.Clone();
            return bluePrintNoValueVector3;
#endif
            return this;
        }
    }
}
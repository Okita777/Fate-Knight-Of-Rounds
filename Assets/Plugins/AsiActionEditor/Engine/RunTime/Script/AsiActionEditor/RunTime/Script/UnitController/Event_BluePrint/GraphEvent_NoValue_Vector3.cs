using AsiActionEngine.RunTime.Graph;
using UnityEngine;

namespace  AsiActionEngine.RunTime.GraphVal
{
    [System.Serializable]
    ///不带参的Vector3
    public class GraphEvent_NoValue_Vector3
    {
        [SerializeReference] protected BluePrint_Vector3 m_BluePrint_Val = new GraphEvent_Value_Vector3();

        #region Property

        [EditorGraphProperty("Vector3", true, EditorGraphPropertyType.EEPT_Vector3)]
        public BluePrint_Vector3 BluePrint_Val
        {
            get { return m_BluePrint_Val; }
            set { m_BluePrint_Val = value; }
        }

        #endregion
        
        [System.NonSerialized] private Vector3 m_ReturnVal;

        private void Init(ActionStatePart part, ActionMachineTime _time)
        {
            m_BluePrint_Val.Init(part, _time);
            m_ReturnVal = m_BluePrint_Val.value;
        }

        public Vector3 value(ActionStatePart part, ActionMachineTime _time)
        {
            Init(part, _time);
            return m_ReturnVal;
        }


        public GraphEvent_NoValue_Vector3 Clone()
        {
#if UNITY_EDITOR
            GraphEvent_NoValue_Vector3 bluePrintNoValueVector3 = new GraphEvent_NoValue_Vector3();
            bluePrintNoValueVector3.BluePrint_Val = (BluePrint_Vector3)m_BluePrint_Val.Clone();
            return bluePrintNoValueVector3;
#endif
            return this;
        }
    }
}
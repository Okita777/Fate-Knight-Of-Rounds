using AsiActionEngine.RunTime.Graph;
using UnityEngine;

namespace  AsiActionEngine.RunTime.GraphVal
{
    [System.Serializable]
    ///不带参的Vector3
    public class GraphEvent_NoValue_Bool
    {
        [SerializeReference] protected BluePrint_Bool m_BluePrint_Val = new GraphEvent_GValue_Bool();

        #region Property
        [EditorGraphProperty("Bool", true, EditorGraphPropertyType.EEPT_Bool)]
        public BluePrint_Bool BluePrint_Val
        {
            get { return m_BluePrint_Val; }
            set { m_BluePrint_Val = value; }
        }
        #endregion

        [System.NonSerialized] private bool m_ReturnVal;
        private void Init(ActionStatePart part, ActionMachineTime _time)
        {
            m_BluePrint_Val.Init(part, _time);
            m_ReturnVal = m_BluePrint_Val.value;
        }
        public bool value(ActionStatePart part, ActionMachineTime _time)
        {
            Init(part, _time);
            return m_ReturnVal;
        }


        public GraphEvent_NoValue_Bool Clone()
        {
#if UNITY_EDITOR
            GraphEvent_NoValue_Bool bluePrintNoValueVector3 = new GraphEvent_NoValue_Bool();
            bluePrintNoValueVector3.m_BluePrint_Val = (BluePrint_Bool)m_BluePrint_Val.Clone();
            return bluePrintNoValueVector3;
#endif
            return this;
        }
    }
}
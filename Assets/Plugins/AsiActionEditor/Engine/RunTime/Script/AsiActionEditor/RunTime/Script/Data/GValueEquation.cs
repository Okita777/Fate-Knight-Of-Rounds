using AsiActionEngine.RunTime.GraphVal;
using UnityEngine;

namespace AsiActionEngine.RunTime.GValueEquation
{
    [System.Serializable]
    public class GValueEquation
    {
        [SerializeReference] public GValueEquation_FloatPart[] gValueEquation_Value = null;
        public GValueEquation Clone()
        {
            return this;
        }
    }

    [System.Serializable]
    public class GValueEquation_FloatPart
    {
        public GraphEvent_NoValue_Float m_GraphEvent = new GraphEvent_NoValue_Float();
        public GValue_SetFloat m_Value = new GValue_SetFloat();

        #region Property
        [EditorProperty("公式蓝图", EditorPropertyType.EEPT_GraphValue)]
        public GraphEvent_NoValue_Float GraphEvent
        {
            get { return m_GraphEvent; }
            set { m_GraphEvent = value; }
        }
        [EditorProperty("目标GValue", EditorPropertyType.EEPT_SetGFloat)]
        public GValue_SetFloat Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        #endregion
        
        public void OnSetValue(ActionStatePart part, ActionMachineTime _actionMachineTime)
        {
            float _value = m_GraphEvent.value(part, _actionMachineTime);
            m_Value.Init(part.ActionStateMachine);
            m_Value.Set(_value);
        }
        
        public GValueEquation_FloatPart Clone()
        {
#if UNITY_EDITOR
            ActionSaveFlishEvent.Run();//复制蓝图前清空前蓝图引用
            GValueEquation_FloatPart clone = new GValueEquation_FloatPart();
            clone.GraphEvent = m_GraphEvent.Clone();
            clone.Value = m_Value.Clone();
            return clone;
#endif
            return this;
        }
    }
    
//     [System.Serializable]
//     public class GValueEquation_IntPart : GValueEquation_Value
//     {
//         public GraphEvent_NoValue_Int m_GraphEvent = new GraphEvent_NoValue_Int();
//         public GValue_SetFloat m_Value = new GValue_SetFloat();
//
//         #region Property
//         [EditorProperty("公式蓝图", EditorPropertyType.EEPT_GraphValue)]
//         public GraphEvent_NoValue_Int GraphEvent
//         {
//             get { return m_GraphEvent; }
//             set { m_GraphEvent = value; }
//         }
//         [EditorProperty("目标GValue", EditorPropertyType.EEPT_SetGInt)]
//         public GValue_SetFloat Value
//         {
//             get { return m_Value; }
//             set { m_Value = value; }
//         }
//         #endregion
//         
//         public override void OnSetValue(ActionStatePart part, ActionMachineTime _actionMachineTime)
//         {
//             float _value = m_GraphEvent.value(part, _actionMachineTime);
//             m_Value.Init(part.ActionStateMachine);
//             m_Value.Set(_value);
//         }
//
//         public override GValueEquation_Value Clone()
//         {
//             // GValueEquation_IntPart clone = new GValueEquation_IntPart();
//             // clone.m_GraphEvent = m_GraphEvent;
//             // return clone;
//             // throw new System.NotImplementedException();
// #if UNITY_EDITOR
//             ActionSaveFlishEvent.Run();//复制蓝图前清空前蓝图引用
//             GValueEquation_IntPart clone = new GValueEquation_IntPart();
//             clone.GraphEvent = m_GraphEvent.Clone();
//             clone.Value = m_Value.Clone();
//             return clone;
// #endif
//             return this;
//         }
//     }
//
//     [System.Serializable]
//     public abstract class GValueEquation_Value
//     {
//         public abstract GValueEquation_Value Clone();
//         public abstract void OnSetValue(ActionStatePart part, ActionMachineTime _actionMachineTime);
//     }
}
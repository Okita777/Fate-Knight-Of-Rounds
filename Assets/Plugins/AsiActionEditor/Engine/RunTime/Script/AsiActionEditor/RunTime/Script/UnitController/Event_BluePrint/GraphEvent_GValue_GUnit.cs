using UnityEngine;

namespace AsiActionEngine.RunTime.Graph
{
    [System.Serializable]
    //带参的Vector3
    public class GraphEvent_GValue_GUnit : BluePrint_Unit
    {
        [SerializeField] protected GUnit m_GUnitVal = new GUnit();
        #region Property

        [EditorGraphProperty("GUnit", false, EditorGraphPropertyType.EEPT_GUnit)]
        public GUnit GUnitVal
        {
            get { return m_GUnitVal; }
            set { m_GUnitVal = value; }
        }
        #endregion

        [System.NonSerialized] private Unit m_ReturnVal;

        public override void Init(ActionStatePart part, ActionMachineTime _time)
        {
            m_GUnitVal.Init(part.ActionStateMachine);
            m_ReturnVal = m_GUnitVal.value;
        }
        public override Unit value => m_ReturnVal;


#if UNITY_EDITOR
        //保存时避免重复实例化，导致引用地址变更
        [System.NonSerialized] protected GraphEvent_GValue_GUnit GraphEventG = null;
#endif

        public override BluePrint_Value Clone()
        {
#if UNITY_EDITOR
            if (GraphEventG is null)
            {
                GraphEventG = new GraphEvent_GValue_GUnit();
                GraphEventG.GUnitVal = (GUnit)m_GUnitVal.Clone();
                GraphEventG.IsNode = IsNode;
                //在保存好文件后重置状态
                ActionSaveFlishEvent.ActionEvent.AddListener(() =>
                {
                    GraphEventG = null;
                });
            }
            return GraphEventG;
#endif
            return this;
        }
    }
}
using UnityEngine;

namespace AsiActionEngine.RunTime.Graph
{
    [System.Serializable]
    //带参的Vector3
    public class GraphEvent_GValue_Bool : BluePrint_Bool
    {
        [SerializeField] protected bool m_BoolVal = false;
        #region Property

        [EditorGraphProperty("Bool", false, EditorGraphPropertyType.EEPT_Bool)]
        public bool BoolVal
        {
            get { return m_BoolVal; }
            set { m_BoolVal = value; }
        }

        #endregion

        public override void Init(ActionStatePart part, ActionMachineTime _time)
        {
            // base.Init(part, _time);
        }
        public override bool value => m_BoolVal;


#if UNITY_EDITOR
        //保存时避免重复实例化，导致引用地址变更
        [System.NonSerialized] protected GraphEvent_GValue_Bool GraphEventG = null;
#endif
        public override BluePrint_Value Clone()
        {
#if UNITY_EDITOR
            if (GraphEventG is null)
            {
                GraphEventG = new GraphEvent_GValue_Bool();
                GraphEventG.BoolVal = m_BoolVal;
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
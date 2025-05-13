using UnityEngine;

namespace AsiActionEngine.RunTime.Graph
{
    [System.Serializable]
    //带参的Vector3
    public class GraphEvent_TrackData_Vector_InputDir : BluePrint_Vector3
    {
        [SerializeField] protected bool m_Val = true;

        #region Property
        [EditorGraphProperty("按相机方向转换", false, EditorGraphPropertyType.EEPT_Bool, LabelWidth = 90)]
        public bool Val
        {
            get { return m_Val; }
            set { m_Val = value; }
        }
        #endregion

        public override Vector3 value => m_ReturnVal;
        
        [System.NonSerialized] private Vector3 m_ReturnVal;
        public override void Init(ActionStatePart part, ActionMachineTime _time)
        {
            if (m_Val)
            {
                Quaternion rot = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                m_ReturnVal = rot * part.ActionStateMachine.PlayerInputMoveDir;
            }
            else
            {
                m_ReturnVal = part.ActionStateMachine.PlayerInputMoveDir;
            }
        }
        
#if UNITY_EDITOR
        //保存时避免重复实例化，导致引用地址变更
        [System.NonSerialized] protected GraphEvent_TrackData_Vector_InputDir _graphEvent = null;
#endif
        public override BluePrint_Value Clone()
        {
#if UNITY_EDITOR
            if (_graphEvent is null)
            {
                _graphEvent = new GraphEvent_TrackData_Vector_InputDir();
                _graphEvent.Val = m_Val;
                _graphEvent.IsNode = IsNode;
                //在保存好文件后重置状态
                ActionSaveFlishEvent.ActionEvent.AddListener(() =>
                {
                    _graphEvent = null;
                });
            }
            return _graphEvent;
#endif
            return this;
        }
        

    }
}
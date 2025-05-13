using UnityEngine;

namespace AsiActionEngine.RunTime.Graph
{
    [System.Serializable]
    //带参的Vector3
    public class GraphEvent_Value_Transform : BluePrint_Transform
    {
        [SerializeReference] protected BluePrint_Unit m_UnitVal = new GraphEvent_GValue_GUnit();
        [SerializeField] protected int m_IntVal = 0;
        #region Property
        [EditorGraphProperty("Unit", true, EditorGraphPropertyType.EEPT_GUnit)]
        public BluePrint_Unit UnitVal
        {
            get { return m_UnitVal; }
            set { m_UnitVal = value; }
        }
        [EditorGraphProperty("挂点", false, EditorGraphPropertyType.EEPT_CharacteLimbType)]
        public int IntVal
        {
            get { return m_IntVal; }
            set { m_IntVal = value; }
        }

        #endregion

        public override Transform value => m_ReturnVal;
        
        [System.NonSerialized] private Transform m_ReturnVal;
        public override void Init(ActionStatePart part, ActionMachineTime _time)
        {
            if (m_UnitVal.IsNode)
            {
                m_UnitVal.Init(part, _time);
                if (m_UnitVal.value.ActionStateMachine.TryGetComponent(out CharacterConfig _config))
                {
                    if (!_config.HelpPointDic.TryGetValue((ECharacteLimbType)m_IntVal, out m_ReturnVal))
                    {
                    
                    }
                }
            }
            else
            {
                if (part.ActionStateMachine.TryGetComponent(out CharacterConfig _config))
                {
                    if (!_config.HelpPointDic.TryGetValue((ECharacteLimbType)m_IntVal, out m_ReturnVal))
                    {
                    
                    }
                }
            }
        }
        
#if UNITY_EDITOR
        //保存时避免重复实例化，导致引用地址变更
        [System.NonSerialized] protected GraphEvent_Value_Transform _graphEvent = null;
#endif
        public override BluePrint_Value Clone()
        {
#if UNITY_EDITOR
            if (_graphEvent is null)
            {
                _graphEvent = new GraphEvent_Value_Transform();
                _graphEvent.UnitVal = (BluePrint_Unit)m_UnitVal.Clone();
                _graphEvent.IntVal = m_IntVal;
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
using AsiActionEngine.RunTime;
using UnityEngine;
using UnityEngine.Serialization;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class CheckRadiusObj : IInterruptCondition
    {
        [SerializeField] protected int m_LayerMask = 0;
        [SerializeField] protected int m_pointType = 0;
        [SerializeField] protected float m_radius = 0;
        [SerializeField] protected bool m_debug = false;
        [SerializeField] protected GValue_SetTransform m_gValueSetTransform = new GValue_SetTransform();
        [SerializeField] protected GValue_SetPoint m_gValueSetPoint = new GValue_SetPoint();
        
        #region property
        [EditorProperty("LayerMask", EditorPropertyType.EEPT_LayerMask)]
        public int LayerMask
        {
            get { return m_LayerMask; }
            set { m_LayerMask = value; }
        }
        [EditorProperty("中心点", EditorPropertyType.EEPT_CharacteLimbType)]
        public int PointType
        {
            get { return m_pointType; }
            set { m_pointType = value; }
        }
        [EditorProperty("半径", EditorPropertyType.EEPT_Float)]
        public float Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }
        [EditorProperty("设置单位", EditorPropertyType.EEPT_SetGTransform)]
        public GValue_SetTransform GValueSetTransform
        {
            get { return m_gValueSetTransform; }
            set { m_gValueSetTransform = value; }
        }
        [EditorProperty("绘制半径", EditorPropertyType.EEPT_Bool)]
        public bool isDebug
        {
            get { return m_debug; }
            set { m_debug = value; }
        }
        #endregion
        public int InterruptType => (int)EConditionType.EIT_CheckRadiusObj;
        public bool CheckInterrupt(Unit unit, ActionStatePart actionStatePart)
        {
            ActionStateMachine actionStateMachine = actionStatePart.ActionStateMachine;
            m_gValueSetTransform.Init(actionStateMachine);
            
            if (actionStateMachine.TryGetComponent(out CharacterConfig _characterConfig))
            {
                if (_characterConfig.HelpPointDic.TryGetValue((ECharacteLimbType)m_pointType, out Transform _transform))
                {
                    Collider[] _objs = Physics.OverlapSphere(_transform.position, m_radius, m_LayerMask);
                    if (_objs.Length > 0)
                    {
                        if (_objs[0].TryGetComponent(out ActionEditor_InteractObj _interactObj))
                        {
                            _interactObj.OnTrigger(actionStateMachine);
                        }
                        m_gValueSetTransform.Set(_objs[0].transform);
                        m_gValueSetPoint.Set(new PointData(_objs[0].transform.position, _objs[0].transform.rotation));
#if UNITY_EDITOR
                        if (isDebug)
                        {
                            EngineDebug.DrawSphere(_transform.position, m_radius, Color.red,2);
                        }     
#endif
                        return true;
                    }
#if UNITY_EDITOR
                    if (isDebug)
                    {
                        EngineDebug.DrawSphere(_transform.position, m_radius, Color.green,2);
                    }     
#endif
                }
            }

            return false;
        }

        public IInterruptCondition Clone()
        {
            CheckRadiusObj _check = new CheckRadiusObj();

            _check.LayerMask = m_LayerMask;
            _check.PointType = m_pointType;
            _check.Radius = m_radius;
            _check.GValueSetTransform = m_gValueSetTransform.Clone();
            
            return _check;
        }
    }
}
using System;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_TargetingMove : IActionEventData
    {
        [SerializeField] protected int m_RefertBone;
        [SerializeField] protected bool m_FreeToX = true;
        [SerializeField] protected bool m_FreeToY = true;
        [SerializeField] protected GEnum m_TargetSpace = new GEnum();
        [SerializeField] protected GFloat m_WorldAngle = new GFloat();
        [SerializeField] protected int m_TargetBone;
        [SerializeField] protected int m_TargetBoneLinks;
        [SerializeField] protected float m_EnterTime;
        [SerializeField] protected float m_ExitTime;

        #region Serproperty

        [EditorProperty("参考朝向: ", EditorPropertyType.EEPT_CharacteLimbType)]
        public int RefertBone
        {
            get { return m_RefertBone; }
            set { m_RefertBone = value; }
        }
        [EditorProperty("目标朝向: ", EditorPropertyType.EEPT_Enum, EnumNames = new []{"相机前朝向", "面向锁定对象", "由程序注册的朝向", "世界朝向"})]
        public GEnum TargetSpace
        {
            get { return m_TargetSpace; }
            set { m_TargetSpace = value; }
        }
        [EditorProperty("Y轴角度: ", EditorPropertyType.EEPT_Float)]
        public GFloat WorldAngle
        {
            get { return m_WorldAngle; }
            set { m_WorldAngle = value; }
        }
        [EditorProperty("目标骨骼: ", EditorPropertyType.EEPT_CharacteLimbType)]
        public int TargetBone
        {
            get { return m_TargetBone; }
            set { m_TargetBone = value; }
        }
        [EditorProperty("骨骼链数: ", EditorPropertyType.EEPT_Int)]
        public int TargetBoneLinks
        {
            get { return m_TargetBoneLinks; }
            set { m_TargetBoneLinks = value; }
        }
        
        [EditorProperty("允许左右旋转: ", EditorPropertyType.EEPT_Bool)]
        public bool FreeToX
        {
            get { return m_FreeToX; }
            set { m_FreeToX = value; }
        }
        [EditorProperty("允许上下旋转: ", EditorPropertyType.EEPT_Bool)]
        public bool FreeToY
        {
            get { return m_FreeToY; }
            set { m_FreeToY = value; }
        }
        [EditorProperty("混入时间: ", EditorPropertyType.EEPT_Float)]
        public float EnterTime
        {
            get { return m_EnterTime; }
            set { m_EnterTime = value; }
        }
        [EditorProperty("混出时间: ", EditorPropertyType.EEPT_Float)]
        public float ExitTime
        {
            get { return m_ExitTime; }
            set { m_ExitTime = value; }
        }
        
        // [EditorProperty("骨骼链权重曲线: ", EditorPropertyType.EEPT_AnimationCurve)]
        // public SerAnimationCurve LinkWeights
        // {
        //     get { return m_LinkWeights; }
        //     set { m_LinkWeights = value; }
        // }
        // [NonSerialized] private Transform m_SerReferTransform;
        // [NonSerialized] private Transform m_SerTransform;
        [NonSerialized] private bool m_IsValid;
        #endregion

        public int GetEvenType() => (int)EEvenType.EET_TargetingMove;
        public IActionEventData Creact() => new Event_TargetingMove();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            m_IsValid = false;
            ActionStateMachine _actionStateMachine = _actionState.ActionStateMachine;
            m_TargetSpace.Init(_actionStateMachine);
            m_WorldAngle.Init(_actionStateMachine);
            if (_actionStateMachine.TryGetComponent(out CharacterConfig _config))
            {
                if (_config.HelpPointDic.TryGetValue((ECharacteLimbType)TargetBone, out Transform _transform))
                {
                    if (_config.HelpPointDic.TryGetValue((ECharacteLimbType)RefertBone, out Transform _referTransform))
                    {
                        m_IsValid = true;
                        if (_actionStateMachine.TryGetLogic(out Ex_Update_TargetingMove _exTargetingMove))
                        {
                            _exTargetingMove.OnSetRot(this, _transform, _referTransform);
                        }
                    }
                }
            }
        }

        public void Exit(ActionStatePart _actionState, bool _interruot)
        {
            ActionStateMachine _actionStateMachine = _actionState.ActionStateMachine;
            if (m_IsValid)
            {
                if (_actionStateMachine.TryGetLogic(out Ex_Update_TargetingMove _exTargetingMove))
                {
                    _exTargetingMove.OnExit(m_ExitTime);
                }
            }
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_TargetingMove _eventDataClone = _eventData as Event_TargetingMove;

            _eventDataClone.RefertBone = m_RefertBone;
            _eventDataClone.TargetBone = m_TargetBone;
            _eventDataClone.TargetBoneLinks = m_TargetBoneLinks;
            _eventDataClone.TargetSpace = (GEnum)m_TargetSpace.Clone();
            _eventDataClone.WorldAngle = (GFloat)m_WorldAngle.Clone();
            _eventDataClone.FreeToX = m_FreeToX;
            _eventDataClone.FreeToY = m_FreeToY;
            _eventDataClone.EnterTime = m_EnterTime;
            _eventDataClone.ExitTime = m_ExitTime;

            return _eventDataClone;
        }
    }
}
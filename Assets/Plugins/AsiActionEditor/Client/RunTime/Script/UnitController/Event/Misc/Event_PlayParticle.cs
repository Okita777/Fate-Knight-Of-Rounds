using System;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_PlayParticle : IActionEventData
    {
        [SerializeField] protected string m_PartoclePath;
        [SerializeField] protected int m_PartPointType;
        [SerializeField] protected float m_Life = 2;
        [SerializeField] protected EVector3 m_OffsetPos = new EVector3();
        [SerializeField] protected EVector3 m_OffsetRot = new EVector3();
        [SerializeField] protected EVector3 m_LocalScale = new EVector3(1, 1, 1);
        [SerializeField] protected bool m_AlwaysFollow = false;
        #region Property

        [EditorProperty("粒子特效", EditorPropertyType.EEPT_GameObject)]
        public string PartoclePath
        {
            get { return m_PartoclePath; }
            set { m_PartoclePath = value; }
        }

        [EditorProperty("目标挂点", EditorPropertyType.EEPT_CharacteLimbType)]
        public int PartPointType
        {
            get { return m_PartPointType; }
            set { m_PartPointType = value; }
        }
        [EditorProperty("始终跟随", EditorPropertyType.EEPT_Bool)]
        public bool AlwaysFollow
        {
            get { return m_AlwaysFollow; }
            set { m_AlwaysFollow = value; }
        }
        // [EditorProperty("粒子寿命", EditorPropertyType.EEPT_Float)]
        // public float Life
        // {
        //     get { return m_Life; }
        //     set { m_Life = value; }
        // }
        
        [EditorProperty("位置偏移", EditorPropertyType.EEPT_Vector3)]
        public EVector3 OffsetPos
        {
            get { return m_OffsetPos; }
            set { m_OffsetPos = value; }
        }
        
        [EditorProperty("角度偏移", EditorPropertyType.EEPT_Vector3)]
        public EVector3 OffsetRot
        {
            get { return m_OffsetRot; }
            set { m_OffsetRot = value; }
        }
        
        [EditorProperty("缩放", EditorPropertyType.EEPT_Vector3)]
        public EVector3 LocalScale
        {
            get { return m_LocalScale; }
            set { m_LocalScale = value; }
        }
        #endregion
        public int GetEvenType() => (int)EEvenType.EET_Partocle;
        public IActionEventData Creact() => new Event_PlayParticle();


        [NonSerialized] bool isValid = false;
        [NonSerialized] ActionEditor_Effects _effects;
        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            isValid = false;
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            ActionEngineResources.Instance.LoadToObjectPool<ActionEditor_Effects>(m_PartoclePath, (Component _obj) =>
            {
                if (_obj is ActionEditor_Effects _particle)
                {
                    isValid = true;
                    _effects = _particle;
                    _particle.Play();
                    bool _isFindPoint = false;
                    if (_stateMachine.TryGetComponent(out CharacterConfig _config))
                    {
                        if (_config.HelpPointDic.TryGetValue((ECharacteLimbType)m_PartPointType, out Transform _point))
                        {
                            _isFindPoint = true;
                            Vector3 _pos = _point.TransformPoint(m_OffsetPos.GetValue());
                            Quaternion _rot = _point.rotation * Quaternion.Euler(OffsetRot.GetValue());
                            _particle.transform.SetPositionAndRotation(_pos, _rot);
                            _effects.transform.localScale = m_LocalScale.GetValue();
                        }
                    }

                    if (!_isFindPoint)
                    {
                        Vector3 _pos = _stateMachine.CurUnit.transform.TransformPoint(m_OffsetPos.GetValue());
                        Quaternion _rot = _stateMachine.CurUnit.transform.rotation * Quaternion.Euler(OffsetRot.GetValue());
                        _particle.transform.SetPositionAndRotation(_pos, _rot);
                        _effects.transform.localScale = m_LocalScale.GetValue();
                    }
                }
            }, m_Life, 5);
        }

        public void Update(ActionStatePart _actionState, ActionMachineTime _actionTime)
        {
            if (isValid)
            {
                ActionEngineResources.Instance.ResetLife(_effects, _effects.Life);
                if (m_AlwaysFollow)
                {
                    ActionStateMachine _stateMachine = _actionState.ActionStateMachine;

                    bool _isFindPoint = false;
                    if (_stateMachine.TryGetComponent(out CharacterConfig _config))
                    {
                        if (_config.HelpPointDic.TryGetValue((ECharacteLimbType)m_PartPointType, out Transform _point))
                        {
                            _isFindPoint = true;
                            Vector3 _pos = _point.TransformPoint(m_OffsetPos.GetValue());
                            Quaternion _rot = _point.rotation * Quaternion.Euler(OffsetRot.GetValue());
                            _effects.transform.SetPositionAndRotation(_pos, _rot);
                            _effects.transform.localScale = m_LocalScale.GetValue();
                        }
                    }

                    if (!_isFindPoint)
                    {
                        Vector3 _pos = _stateMachine.CurUnit.transform.TransformPoint(m_OffsetPos.GetValue());
                        Quaternion _rot = _stateMachine.CurUnit.transform.rotation *
                                          Quaternion.Euler(OffsetRot.GetValue());
                        _effects.transform.SetPositionAndRotation(_pos, _rot);
                        _effects.transform.localScale = m_LocalScale.GetValue();
                    }
                }
            }
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_PlayParticle _event = _eventData as Event_PlayParticle;

            _event.PartoclePath = m_PartoclePath;
            _event.PartPointType = m_PartPointType;
            _event.OffsetPos = m_OffsetPos;
            _event.OffsetRot = m_OffsetRot;
            _event.LocalScale = m_LocalScale;
            _event.AlwaysFollow = m_AlwaysFollow;
            // _event.Life = m_Life;

            return _event;
        }
    }
}
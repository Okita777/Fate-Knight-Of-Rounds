using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.GraphVal;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_UnitRot : IActionEventData
    {
        [SerializeField] protected bool mIsMove = false;
        [SerializeField] protected bool mIsMovePre = false;

        [SerializeField] protected GraphEvent_NoValue_Vector3 mUnitRot = new GraphEvent_NoValue_Vector3();
        [SerializeField] protected bool m_IsEuler = true;
        // [SerializeField] protected ERotType mRotType;
        // [SerializeField] protected GEnum mRotaType = new GEnum();
        [SerializeField] protected int mRotLerp = 0;
        // [SerializeField] protected int mOffsetRotY = 0;
        // [SerializeField] protected int mRotPriority = 0;
        [SerializeField] protected GInt mRotPriority = new GInt(1);
        [SerializeField] protected float mTotalTime = 1f;

        #region Property
        [EditorProperty("仅移动输入时转向: ", EditorPropertyType.EEPT_Bool)]
        public bool IsMove
        {
            get { return mIsMove; }
            set { mIsMove = value; }
        }
        [EditorProperty("    预输入: ", EditorPropertyType.EEPT_Bool)]
        public bool IsMovePre
        {
            get { return mIsMovePre; }
            set { mIsMovePre = value; }
        }
        [EditorProperty("最终朝向 ", EditorPropertyType.EEPT_GraphValue)]
        public GraphEvent_NoValue_Vector3 UnitRot
        {
            get { return mUnitRot; }
            set { mUnitRot = value; }
        }
        [EditorProperty("输入是否为Euler: ", EditorPropertyType.EEPT_Bool)]
        public bool IsEuler
        {
            get { return m_IsEuler; }
            set { m_IsEuler = value; }
        }
        // [EditorProperty("朝向类型: ", EditorPropertyType.EEPT_Enum, EnumNames = new []{"面向相机前方向","面向移动输入方向","面向锁定目标","面向攻击者", "面向玩家"})]
        // public GEnum RotaType
        // {
        //     get { return mRotaType; }
        //     set { mRotaType = value; }
        // }
        [EditorProperty("转向速度(负为线性：s): ", EditorPropertyType.EEPT_Int)]
        public int RotLerp
        {
            get { return mRotLerp; }
            set { mRotLerp = value; }
        }
        // [EditorProperty("Y轴角度偏移(度): ", EditorPropertyType.EEPT_Int)]
        // public int OffsetRotY
        // {
        //     get { return mOffsetRotY; }
        //     set { mOffsetRotY = value; }
        // }
        [EditorProperty("旋转优先度", EditorPropertyType.EEPT_Int)]
        public GInt RotPriority
        {
            get { return mRotPriority; }
            set { mRotPriority = value; }
        }
        [EditorProperty("旋转持续时长", EditorPropertyType.EEPT_Float)]
        public float TotalTime
        {
            get { return mTotalTime; }
            set { mTotalTime = value; }
        }
        #endregion
        
        public int GetEvenType() => (int)EEvenType.EET_UnitRot;
        public IActionEventData Creact() => new Event_UnitRot();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            if (_isSingle)
            {
                ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
                // mRotaType.Init(_stateMachine);
                mRotPriority.Init(_stateMachine);
                if(!_stateMachine.IsLocalClient)return;//非本地客户端  不执行

                if (mIsMove && !(mIsMovePre ? _stateMachine.IsMoveInputPre : _stateMachine.IsMoveInput)) return;//不执行
                _stateMachine.TryGetLogic(out EX_Update_UnitRot _exUnitRot);
                Quaternion _quaternion = Quaternion.identity;
                if (m_IsEuler)
                {
                    _quaternion = Quaternion.Euler(mUnitRot.value(_actionState, new ActionMachineTime(0, 0, 0, 0)));
                }
                else
                {
                    _quaternion = Quaternion.LookRotation(mUnitRot.value(_actionState, new ActionMachineTime(0,0,0,0)));
                }
                if (TotalTime <= 0)
                {
                    // Transform _transform = _stateMachine.CurUnit.RootTarget;
                    // Quaternion _quaternion = _exUnitRot.GetTargetRot(_stateMachine, _transform, (ERotType)mRotaType.value);

                    // _transform.rotation = _quaternion;
                    if (_stateMachine.TryGetLogic(out Ex_Update_CharacterControl _characterControl))
                    {
                        _exUnitRot.ResetLifeTime();
                        _characterControl.SetRot(_quaternion, mRotPriority.value);
                    }
                }
                else
                {
                    _exUnitRot.SetRot(_quaternion, mRotLerp, mRotPriority.value, mTotalTime);
                }

            }
        }

        public void Update(ActionStatePart _actionState, ActionMachineTime _actionTime)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            if(!_stateMachine.IsLocalClient)return;//非本地客户端  不执行
            if(mIsMove && !(mIsMovePre ? _stateMachine.IsMoveInputPre : _stateMachine.IsMoveInput))return;//不执行

            _stateMachine.TryGetLogic(out EX_Update_UnitRot _exUnitRot);
            Transform _transform = _stateMachine.CurUnit.RootTarget;
            // Quaternion _targetRot = Quaternion.LookRotation(mUnitRot.value(_actionState, _actionTime));
            Quaternion _targetRot = Quaternion.identity;
            if (m_IsEuler)
            {
                _targetRot = Quaternion.Euler(mUnitRot.value(_actionState, _actionTime));
            }
            else
            {
                _targetRot = Quaternion.LookRotation(mUnitRot.value(_actionState, _actionTime));
            }
            // Quaternion _targetRot = _exUnitRot.GetTargetRot(_stateMachine, _transform, (ERotType)mRotaType.value) * Quaternion.Euler(0, mOffsetRotY, 0);
            if (_stateMachine.TryGetLogic(out Ex_Update_CharacterControl _characterControl))
            {
                if (mRotLerp == 0)
                {
                    _characterControl.SetRot(_targetRot, mRotPriority.value);
                }
                else if (mRotLerp > 0)
                {
                    _characterControl.SetRot(
                        Quaternion.Lerp(_transform.rotation, _targetRot, mRotLerp * _actionTime.Deltatime),
                        mRotPriority.value);

                }
                else
                {
                    mRotLerp *= -1;
                    _characterControl.SetRot(Quaternion.RotateTowards(_transform.rotation, _targetRot,
                        mRotLerp * _actionTime.Deltatime), mRotPriority.value);
                }
            }
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_UnitRot _event = _eventData as Event_UnitRot;

            _event.IsMove = mIsMove;
            _event.IsMovePre = mIsMovePre;
            _event.RotLerp = mRotLerp;
            _event.UnitRot = mUnitRot.Clone();
            _event.IsEuler = m_IsEuler;
            // _event.OffsetRotY = mOffsetRotY;
            _event.TotalTime = mTotalTime;
            // _event.RotaType = (GEnum)mRotaType.Clone();
            _event.RotPriority = (GInt)mRotPriority.Clone();
            
            return _event;
        }
    }
}
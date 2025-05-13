using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.Event_Extend;
using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.RunTime
{

    [System.Serializable]
    public class Event_AttackBox : IActionEventData
    {
        [SerializeField] private AttackBoxInfo mAttackBoxInfo = new AttackBoxInfo();
        [SerializeField] private GValue_Setting mGValue_Setting  = new GValue_Setting();
        [SerializeField] private GValue_Ratio mGValueRatio  = new GValue_Ratio();
        [SerializeField] private int mHitLayer;
        [SerializeReference] private IAttackInfo mAttackInfo;

        [NonSerialized] private List<GameObject> beHitTarget = new List<GameObject>();//已经击中的对象
        [NonSerialized] private CharacterConfig config;
        [NonSerialized] private ActionStatePart _actionStatePart;
        [NonSerialized] private float curTime = 0;
        [NonSerialized] private LayerMask layerMask;
        [NonSerialized] private float mHitInterval = 0.0f;
        [NonSerialized] private Vector3 mStartEndPos;

        #region property

        public AttackBoxInfo AttackBoxInfo
        {
            get { return mAttackBoxInfo; }
            set { mAttackBoxInfo = value; }
        }
        
        public IAttackInfo AttackInfo
        {
            get { return mAttackInfo; }
            set { mAttackInfo = value; }
        }

        [EditorProperty("检测层级", EditorPropertyType.EEPT_LayerMask)]
        public int HitLayer
        {
            get { return mHitLayer; }
            set { mHitLayer = value; } 
            
        }
        [EditorProperty("有效命中判定", EditorPropertyType.EEPT_GValueSRatio, LabelWidth = 0)]
        public GValue_Ratio GValueRatio
        {
            get { return mGValueRatio; }
            set { mGValueRatio = value; } 
        }
        [EditorProperty("修改被命中者GValue", EditorPropertyType.EEPT_GValueSetting)]
        public GValue_Setting GValue_Setting
        {
            get { return mGValue_Setting; } set { mGValue_Setting = value; } 
            
        }
        #endregion


        public int GetEvenType() => -(int)EEvenTypeInternal.EET_AttackBox;
        public IActionEventData Creact() => new Event_AttackBox();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            mHitInterval = 0.0f;
            // ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            // _stateMachine.OnHitObject = null;
            // _stateMachine.OnHitUnit = null;

            //初始化参数
            _lastHitPartIndex = 0;
            
            layerMask = _actionState.ActionStateMachine.GetLayer(mHitLayer);
            mAttackInfo.OnHitStart(_actionState);
            curTime = 0;
            _actionStatePart = _actionState;
            if (_actionState.ActionStateMachine.TryGetComponent(out CharacterConfig _config))
            {
                config = _config;
                // layerMask = _config.AttackLayer;
                if (config.HelpPointDic.TryGetValue(mAttackBoxInfo.ReferPoint, out Transform _transform))
                {
                    Vector3 _startPos = _transform.TransformPoint(mAttackBoxInfo.OffsetPos.GetValue());
                    Quaternion _rot = _transform.rotation * Quaternion.Euler(mAttackBoxInfo.OffsetRot.GetValue());
                    mStartEndPos = (_startPos + _rot * Vector3.forward * mAttackBoxInfo.Scale.x);
                }
            }
            else
            {
                // layerMask = int.MaxValue;
                config = null;
            }
            beHitTarget.Clear();
        }

        public void Update(ActionStatePart _actionState, ActionMachineTime _actionTime)
        {
            if (mAttackBoxInfo.HitInterval > 0)
            {

                mHitInterval += _actionTime.Deltatime;
                if (mHitInterval >= mAttackBoxInfo.HitInterval)
                {
                    mHitInterval = 0;
                    beHitTarget.Clear();
                }
            }
            
            int boxNumber = mAttackBoxInfo.Box.Length;
            if (boxNumber > 0)
            {
                _eventStartTime = _actionTime.TriggerTime;
                CheckTime(curTime, _actionState.ElapsedTime, _actionState);
                // curTime = _actionState.ElapsedTime;

            }//有烘焙数据，去烘焙数据检验碰撞结果
            else
            {
                if (config is not null)
                {
                    if (config.HelpPointDic.TryGetValue(mAttackBoxInfo.ReferPoint, out Transform _transform))
                    {
                        Vector3 _startPos = _transform.TransformPoint(mAttackBoxInfo.OffsetPos.GetValue());
                        Quaternion _rot = _transform.rotation * Quaternion.Euler(mAttackBoxInfo.OffsetRot.GetValue());
                        Vector3 _endPos = _startPos + _rot * Vector3.forward * mAttackBoxInfo.Scale.x;
                        CheckBox(_startPos, _endPos, _actionState);
                    }
                    else
                    {
                        Transform _unitTrans = _actionState.ActionStateMachine.CurUnit.transform;

                        Vector3 _startPos = _unitTrans.TransformPoint(mAttackBoxInfo.OffsetPos.GetValue());
                        Quaternion _rot = _unitTrans.rotation * Quaternion.Euler(mAttackBoxInfo.OffsetRot.GetValue());
                        Vector3 _endPos = _startPos + _rot * Vector3.forward * mAttackBoxInfo.Scale.x;
                        CheckBox(_startPos, _endPos, _actionState);
                    }
                }
            }//无烘焙数据，绑定挂点判定
            curTime = _actionState.ElapsedTime;
            // throw new System.NotImplementedException();
        }

        public void Exit(ActionStatePart _actionState, bool _interruot)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;

            if (!_interruot && mAttackBoxInfo.Box.Length > 0)
            {
                //把剩下的攻击盒一口气全部绘制
                CheckTime(curTime, float.MaxValue, _actionState);
            }
            _stateMachine.OnHitObject = null;
            _stateMachine.HitUnit = null;
            mAttackInfo.OnHitEnd(_actionState);

        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_AttackBox _event = _eventData as Event_AttackBox;

#if UNITY_EDITOR
            _event.AttackBoxInfo = mAttackBoxInfo.Clone();
            _event.AttackInfo = mAttackInfo.Clone();
#else
            _event.AttackBoxInfo = mAttackBoxInfo;
            _event.AttackInfo = mAttackInfo;
#endif
            _event.GValue_Setting = GValue_Setting.Clone();
            _event.GValueRatio = mGValueRatio.Clone();
            _event.HitLayer = mHitLayer;
            // _event.HitSpeed = mHitSpeed;
            // _event.Hitduration = mHitduration;

            return _event;
        }

        #region Funtion
        // [NonSerialized]AttackBoxPart _lastHitPart;//上一次的最末端攻击盒信息
        [NonSerialized]private int _lastHitPartIndex = 0;//上一次循环的结束位置
        [NonSerialized]private int _eventStartTime = 0;
        private void CheckTime(float _startTime, float _endTime, ActionStatePart _actionState)
        {
            int boxNumber = mAttackBoxInfo.Box.Length;

            bool _isFindBox = false;
            for (int i = _lastHitPartIndex; i < boxNumber; i++)
            {
                AttackBoxPart _boxPart = mAttackBoxInfo.Box[i];
                int _boxTriggerTime = _boxPart.TriggerTime + _eventStartTime;
                if (_boxTriggerTime >= _startTime)
                {
                    if (_boxTriggerTime > _endTime)
                    {
                        break;
                    }
                    Transform _transform = _actionState.ActionStateMachine.CurUnit.transform;
                    Vector3 _startPos = _transform.TransformPoint(_boxPart.StartPos.GetValue());
                    Vector3 _endPos = 
                        _startPos + _transform.TransformDirection(_boxPart.Dir.GetValue()) * mAttackBoxInfo.Scale.x;
                    CheckBox(_startPos, _endPos, _actionState);
                    // _lastHitPart = _boxPart;
                    _lastHitPartIndex = i;
                    _isFindBox = true;
                }
            }

            if (!_isFindBox)
            {
                //重复取上一个判定
                AttackBoxPart _boxPart = mAttackBoxInfo.Box[_lastHitPartIndex];
                Transform _transform = _actionState.ActionStateMachine.CurUnit.transform;
                Vector3 _startPos = _transform.TransformPoint(_boxPart.StartPos.GetValue());
                Vector3 _endPos = 
                    _startPos + _transform.TransformDirection(_boxPart.Dir.GetValue()) * mAttackBoxInfo.Scale.x;
                CheckBox(_startPos, _endPos, _actionState,false);
            }
        }
        
        private void CheckBox(Vector3 _startPos,Vector3 _endPos,ActionStatePart _actionState, bool _isDraw = true)
        {
#if UNITY_EDITOR
            // Debug.DrawLine(_startPos, _endPos, Color.red, 0.5f);
            // EngineDebug.LogWarning("");
#endif

            bool _isHit = false;
            if (mAttackBoxInfo.AttackBoxType == 0)
            {
                // bool _isHit2 = false;
                Collider[] _colliders =
                    Physics.OverlapCapsule(_startPos, _endPos, mAttackBoxInfo.Scale.y, layerMask);
                foreach (var VARIABLE in _colliders)
                {
                    if (BeHitListAdd(VARIABLE.gameObject, _actionState)) _isHit = true;
                }

                if (_isDraw)
                    EngineDebug.DrawCapsule(_startPos, _endPos, mAttackBoxInfo.Scale.y,
                        _isHit ? Color.green : Color.red, 1);

            } //胶囊
            else if (mAttackBoxInfo.AttackBoxType == 1)
            {
                Vector3 _dir = _endPos - _startPos;
                RaycastHit[] _colliders = Physics.RaycastAll(_startPos, _dir, _dir.magnitude, layerMask);
                foreach (var VARIABLE in _colliders)
                {
                    // if (!_isHit)
                    // {
                    //     Quaternion _rot = Quaternion.LookRotation(_startPos - _endPos, _endPos - mStartEndPos);
                    // mAttackInfo.OnHitUpdate(_actionState, VARIABLE.point, _rot);
                    //     _isHit = true;
                    // }
                    if(BeHitListAdd(VARIABLE.collider.gameObject, _actionState)) _isHit = true;
                }

                if (_isDraw)
                    EngineDebug.DrawLine(_startPos, _endPos, _isHit ? Color.green : Color.red, 1);

            } //射线
            else if (mAttackBoxInfo.AttackBoxType == 2)
            {
            } //方块
            else if (mAttackBoxInfo.AttackBoxType == 3)
            {
            } //球
        }

        //最终伤害输出
        private bool BeHitListAdd(GameObject _obj, ActionStatePart _selfActionState)
        {
            if (!beHitTarget.Contains(_obj))
            {
                ActionStateMachine _stateMachine = _actionStatePart.ActionStateMachine;
                Vector3 _hitPoint = Vector3.zero;//命中位置
                
                if (_obj.TryGetComponent(out Unit _unit))
                {
                    //如果命中对象携带Unit组件的话判定为单位
                    if (mGValueRatio.CheckValue(_unit.ActionStateMachine, _selfActionState.ActionStateMachine))
                    {
                        _stateMachine.UnitOnHit(mAttackInfo, _obj, _unit, _hitPoint);
                        _unit.ActionStateMachine.UnitBehit(mAttackInfo, _stateMachine.CurUnit, _hitPoint, GValue_Setting);
                        if (_stateMachine.TryGetStaticLogic(out Ex_AttackBox _attackBox))
                        {
                            _attackBox.ExtrudEvent(mAttackInfo);
                        }
                    }
                    // EngineDebug.LogWarning("_unitName: " + _unit.transform.name);
                }
                else
                {
                    _stateMachine.UnitOnHit(mAttackInfo, _obj, null, _hitPoint);
                    if (_stateMachine.TryGetStaticLogic(out Ex_AttackBox _attackBox))
                    {
                        _attackBox.ExtrudEvent(mAttackInfo);
                    }
                }
                // _stateMachine.SetSpeed(_attackInfo.HitSpeed,_attackInfo.mHitduration);

                beHitTarget.Add(_obj);
                return true;
            }

            return false;
        }
        #endregion

        #region DrawFuntion

        #if UNITY_EDITOR
        public void EditorDraw(CharacterConfig characterConfig, ActionStatePart _actionState, ActionMachineTime _actionTime)
        {
            Update_EventAttackBox(_actionTime, characterConfig);
        }

        private void Update_EventAttackBox(ActionMachineTime _actionTime, CharacterConfig characterConfig)
        {
            int _BoxDrawType = BoxDrawType;
            if (_BoxDrawType == 2 || _BoxDrawType == 0)
            {
                DrawDefaultBox(AttackBoxInfo, characterConfig, _actionTime, Color.green);
            }

            if (_BoxDrawType == 2 || _BoxDrawType == 1)
            {
                DrawBekaBox(AttackBoxInfo, _actionTime, characterConfig.transform, Color.blue);
            }
        }

        private void DrawDefaultBox(AttackBoxInfo attackBoxInfo, CharacterConfig _config, ActionMachineTime _actionTime,
            Color color)
        {
            if (_config.HelpPointDic.TryGetValue(attackBoxInfo.ReferPoint, out Transform _target))
            {
                // Debug.Log("绘制攻击盒33");
                Vector3 _startPos = _target.TransformPoint(attackBoxInfo.OffsetPos.GetValue());
                Quaternion _dir = (_target.rotation * Quaternion.Euler(attackBoxInfo.OffsetRot.GetValue()));
                Vector3 _endPos = _startPos + (_dir * Vector3.forward * attackBoxInfo.Scale.x);

                //常规绘制
                bool _isDraw = _actionTime.CurrentTime >= _actionTime.TriggerTime;
                if (_isDraw) _isDraw = _actionTime.CurrentTime <= _actionTime.TriggerTime + _actionTime.Duration;
                if (_isDraw)
                {
                    DrawBox(_startPos, _endPos, _dir * Vector3.up, attackBoxInfo, color);
                }
            }
        }

        private void DrawBekaBox(AttackBoxInfo attackBoxInfo, ActionMachineTime _actionTime, Transform _target, Color color)
        {
            int _curTrigerTime = (int)(_actionTime.CurrentTime - _actionTime.TriggerTime);

            if (_curTrigerTime >= 0)
            {
                for (int i = 0; i < attackBoxInfo.Box.Length; i++)
                {
                    AttackBoxPart _boxPart = attackBoxInfo.Box[i];
                    bool _isDraw = _curTrigerTime >= _boxPart.TriggerTime;
                    if (_isDraw)
                    {
                        int _life = BakerBoxLife;
                        if (i == attackBoxInfo.Box.Length - 1)
                        {
                            _isDraw = _curTrigerTime <= _boxPart.TriggerTime + _life;
                        }//末端
                        else
                        {
                            _isDraw = _curTrigerTime < attackBoxInfo.Box[i + 1].TriggerTime + _life;
                        }//常规

                        if (_isDraw)
                        {
                            Vector3 _startPos = _target.TransformPoint(_boxPart.StartPos.GetValue());
                            Vector3 _endPos = _startPos + _target.TransformDirection(_boxPart.Dir.GetValue()) 
                                * attackBoxInfo.Scale.x;
                            DrawBox(_startPos, _endPos, Vector3.up, attackBoxInfo, color);
                        }
                    }
                }
            }
        }

        private void DrawBox(Vector3 _startPos, Vector3 _endPos, Vector3 _AxisY, AttackBoxInfo attackBoxInfo, Color color)
        {
            // EngineDebug.Log("绘制攻击盒");
            if (attackBoxInfo.AttackBoxType == 0)
            {
                EngineScenceDraw.Capsule(_startPos, _endPos, attackBoxInfo.Scale.y, color);
            } //胶囊
            else if (attackBoxInfo.AttackBoxType == 1)
            {
                EngineScenceDraw.Line(_startPos,_endPos, color);
            } //射线
            else if (attackBoxInfo.AttackBoxType == 2)
            {
                EngineScenceDraw.Line(_startPos,_endPos, color);

            } //方块
            else if (attackBoxInfo.AttackBoxType == 3)
            {
                EngineScenceDraw.Line(_startPos,_endPos, color);
            } //球
        }
        

        private int BoxDrawType => PlayerPrefs.GetInt("BoxDrawType", 0);

        private int BakerBoxLife => PlayerPrefs.GetInt("BakerBoxLife", 0);

        #endif
        #endregion
    }
}
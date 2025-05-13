using System.Collections.Generic;
// using System.Linq;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public partial class ActionStateMachine
    {
        #region 事件和委托
        public event OnChangedData OnChange; //切换Action时调用
        public delegate void OnChangedData(int _actionID, int _mixTime, int _offsetTime);
        #endregion
        public float TimeScale
        {
            get { return mTimeScale; }
            set { mTimeScale = value; }
        }

        // public List<float> TimeScaleList = new List<float>();
        
        public readonly float HoldKeyIntervalTime = 0.2f;//长按判定时间
        public string ActionGroupName => ActionStateInfo.ActionGroupName;
        public bool IsLocalClient { get; set; } = true;//是否为本地客户端
        public bool IsMoveInput { get; private set; }//是否持续输入位移
        public bool IsMoveInputPre = false;//位移输入的预输入
        public bool IsLock;//是否锁定
        public float DeltaTime { get; private set; }
        public Vector3 PlayerInputMoveDir { get; private set; }//玩家输入的方向
        public Vector3 RootWeight { get; set; } = Vector3.one; 
        public Animator CurAnimator { get; private set; }//当前更新的动画
        public Unit CurUnit { get; private set; }//当前绑定单位
        public Unit HitUnit { get; set; }//命中的单位
        public Unit AttackerUnit => onAttacker;

        public Transform LockTransform;
        public GameObject OnHitObject { get; set; }//命中的对象
        public ActionStateInfo ActionStateInfo => mActionStateInfo;
        public List<ActionStatePart> AllActionStatePart => mAllActionStatePart;//所有层级
        public Dictionary<int, ActionState> ActionStates => mActionStates;
        public Dictionary<string, int> ActionStateID => mActionStateID;
        public void UnitBehit(IAttackInfo _attackInfo, Unit _attacker, Vector3 _hitPoint, GValue_Setting _gValueRatio) 
            => OnBeHit(_attackInfo,_attacker, _hitPoint, _gValueRatio);

        public void UnitOnHit(IAttackInfo _attackInfo, GameObject _BeHit, Unit _BeHitUnit, Vector3 _hitPoint)
            => OnOnHit(_attackInfo, _BeHit, _BeHitUnit, _hitPoint);
        
        public void OnUpdate(float _deltaTime) => OnUpdateState(_deltaTime);
        public void OnLateUpdate(float _deltatime) => OnLateUpdateState(_deltatime);
        public void SetKeyDown(string _keyName) => OnSetKeyDown(_keyName);
        public void SetKeyUp(string _keyName) => OnSetKeyUp(_keyName);
        public void SendKeyDown(string _keyName, int _inputType = 0) => OnSendKeyDown(_keyName, _inputType);
        
        //Animator
        public float GetAnimatorFloat(string _name) => OnGetAnimatorFloat(_name);
        public int GetAnimatorInt(string _name) => OnGetAnimatorInt(_name);
        public void SetAnimatorFloat(string _name, float _value) => OnSetAnimatorFloat(_name, _value);
        public void SetAnimatorInt(string _name, int _value) => OnSetAnimatorInt(_name, _value);

        public void ChangeAction(string _name, int _mixTime, int _setTime) => OnChangeAction(_name, _mixTime, _setTime);
        public void ChangeAction(int _id, int _mixTime, int _setTime) => OnChangeAction(_id, _mixTime, _setTime);

        /// <summary>
        /// 设置状态机运行速度(例如放慢或者加快角色速度等)
        /// </summary>
        /// <param name="_speed"></param>
        /// <param name="_duration"></param>
        public void SetSpeed(float _speed, float _duration = -1f) => OnSetSpeed(_speed, _duration);
        
        /// <summary>
        /// 注册方向输入
        /// </summary>
        /// <param name="_dir">输入的向量</param>
        public void SetMoveInput(Vector3 _dir) => OnSetMoveInput(_dir);
        
        /// <summary>
        /// 注册方向停止输入
        /// </summary>
        public void SetMoveInputStop() => OnSetMoveInputStop();
        
        /// <summary>
        /// 注册朝向（一般情况请传入相机的 Quaternion ）
        /// </summary>
        /// <param name="_rot">朝向</param>
        public void SetMouseXY(Quaternion _rot) => OnSetHeadRot(_rot);
        public void SetCamRot(Quaternion _rot){mCamRot = _rot;}
        public Quaternion GetCamPointRot() => mMouseXY;
        public Quaternion GetCamRot() => mCamRot;

        public Quaternion GetCharacterFor;
        public GValue_Setting InitGValue_Setting;

        #region Valid
        public bool OnHitValid
        {
            get
            {
                if (hasHiter)
                {
                    if (HitUnit == null)
                    {
#if UNITY_EDITOR
                        EngineDebug.LogWarning("尝试获取命中单位，但是命中单位已被销毁");
#endif

                        hasHiter = false;
                        return false;
                    }
                    return true;
                }
#if UNITY_EDITOR
                EngineDebug.LogWarning("尝试获取命中单位，但是命中单位为空");
#endif

                return false;
            }
        }

        public bool OnAttackerValid
        {
            get
            {
                if (hasAttacker)
                {
                    if (onAttacker == null)
                    {
#if UNITY_EDITOR
                        EngineDebug.LogWarning("尝试获取攻击单位，但是攻击单位已被销毁");
#endif
                        hasAttacker = false;
                        return false;
                    }
                    return true;
                }
#if UNITY_EDITOR
                EngineDebug.LogWarning("尝试获取攻击单位，但是攻击单位为空");
#endif

                return false;
            }
        }

        public bool OnHitObjectValid
        {
            get
            {
                if (hasHitobj)
                {
                    if (OnHitObject == null)
                    {
#if UNITY_EDITOR
                        EngineDebug.LogWarning("尝试获取命中对象，但是命中对象已被销毁");
#endif
                        hasHitobj = false;
                        return false;
                    }
                    return true;
                }
#if UNITY_EDITOR
                EngineDebug.LogWarning("尝试获取命中对象，但是命中对象为空");
#endif

                return false;
            }
        }
        #endregion
    }
}
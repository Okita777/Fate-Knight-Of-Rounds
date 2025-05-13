using System;
using System.Collections.Generic;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public partial class ActionStateMachine
    {
        private float mSetSpeedDuration = -1;
        private float mTimeScale = 1;//行为状态机时间膨胀
        private ActionStateInfo mActionStateInfo;
        private bool IsMoveInputValue = false;
        
        private Dictionary<int, ActionState> mActionStates = new Dictionary<int, ActionState>();//当前角色的所有行为
        private Dictionary<string, int> mActionStateID = new Dictionary<string, int>();//角色行为的ID
        private List<ActionStatePart> mAllActionStatePart = new List<ActionStatePart>();//所有并行执行的Action
        private List<Action<string>> mDwonAction = new List<Action<string>>();

        // public string NowInputClickKey { get; private set; } //当前点击的按键
        // public string NowInputDownKey { get; set; } //当前按下的按键
        // public string NowInputUpKey { get; private set; } //当前松开的按键
        // public string NowInputHoldKey { get; private set; } //当前长按的按键
        // public List<string> NowInputKey { get; private set; } = new List<string>(); //当前按键状态
        
        public ActionStateMachine(
            Unit unit, 
            Animator _animtor, 
            ActionStateInfo _actionStateInfo,
            EngineGValue _engineGValue,
            GValue_Setting _setting
            // int _StateMachineID
        )
        {
            GValueInit(_engineGValue);
            // EngineDebug.LogError("长度: " + GValue.mEngineInt.Length);
            InitGValue_Setting = _setting;
            _setting.OnSet(this);
            CurUnit = unit;
            CurAnimator = _animtor;
            mActionStateInfo = _actionStateInfo;
            // StateMachineID = _StateMachineID;
            foreach (var VARIABLE in _actionStateInfo.mActionState)
            {
                if (!ActionStates.TryAdd(VARIABLE.ID, VARIABLE))
                {
                    EngineDebug.LogError($"Action字典初始化失败,出现同ID:{VARIABLE.ID}");
                }

                if (!ActionStateID.TryAdd(VARIABLE.Name, VARIABLE.ID))
                {
                    EngineDebug.LogError($"Action字典初始化失败,出现同名:{VARIABLE.Name}");
                }
            }

            for (int i = 0; i < 5; i++)
            {
                mAllActionStatePart.Add(new ActionStatePart(this, i));
            }

            Init_EventPool();
            Init(mActionStateInfo.mDefaultAction);
            if(!string.IsNullOrEmpty(_actionStateInfo.mDefaultAction_Limb))ChangeAction(_actionStateInfo.mDefaultAction_Limb,0,0);
            if(!string.IsNullOrEmpty(_actionStateInfo.mDefaultAction_Upper))ChangeAction(_actionStateInfo.mDefaultAction_Upper,0,0);
            if(!string.IsNullOrEmpty(_actionStateInfo.mDefaultAction_Script))ChangeAction(_actionStateInfo.mDefaultAction_Script,0,0);

            // OnChangeAction();
            // EngineDebug.Log($"默认Action: {mActionStateInfo.mDefaultAction}\n受击Action：{mActionStateInfo.mHitAction}");
        }

        /// <summary>
        /// 注册按下行为的回调
        /// </summary>
        /// <param name="_action"></param>
        public void AddDwonAction(Action<string> _action)
        {
            mDwonAction.Add(_action);
        }
        
        private void Init(string _actionName)
        {
            if (string.IsNullOrEmpty(_actionName))
            {
                EngineDebug.LogWarning($"Action初始化失败，当前UnitWarp未设置默认Action, 已默认加载列表内第一个Action");
            }
            else
            {
                if (ActionStateID.ContainsKey(_actionName))
                {
                    OnEnter(ActionStates[ActionStateID[_actionName]]);
                    return;
                }
                else
                {
                    EngineDebug.LogWarning($"Action初始化失败，当前数据不存在:<color=#FF3333>{_actionName}</color>, 已默认加载列表内第一个Action");
                }
            }

            foreach (var VARIABLE in ActionStates)
            {
                if (VARIABLE.Value.AnimaLayer == 0)
                {
                    OnEnter(VARIABLE.Value);
                    break;
                }
            }
        }
        
        private void OnEnter(ActionState _action)
        {
            mAllActionStatePart[0].OnEnter(_action);
        }

        private void OnUpdateState(float _deltatime)
        {
            SetSpeedUpdate(_deltatime);
            _deltatime *= mTimeScale;
            DeltaTime = _deltatime;
            foreach (ActionStatePart VARIABLE in mAllActionStatePart)
            {
                VARIABLE.OnUpdate(_deltatime);
            }

            Update_Extend(_deltatime);
        }

        private void OnLateUpdateState(float _deltatime)
        {
            foreach (ActionStatePart VARIABLE in mAllActionStatePart)
            {
                VARIABLE.OnLateUpdate(_deltatime);
            }
            // for (int i = mAllActionStatePart.Count - 1; i >= 0; i--)
            // {
            //     mAllActionStatePart[i].OnLateUpdate(_deltatime);
            // }
            LateUpdate_Extend(_deltatime);
        }
        
        //设置输入
        private void OnSetMoveInput(Vector3 _move)
        {
            PlayerInputMoveDir = _move;
            IsMoveInput = true;
            IsMoveInputPre = true;
        }
        private void OnSetHeadRot(Quaternion _rot)
        {
            mMouseXY = _rot;
        }
        private void OnSetMoveInputStop()
        {
            IsMoveInput = false;
        }

        private void OnSetSpeed(float _speed, float _duration)
        {
            mSetSpeedDuration = _duration;
            mTimeScale = _speed;
            if (CurAnimator is not null)
            {
                CurAnimator.speed = _speed;
            }
        }

        private void SetSpeedUpdate(float _deltatime)
        {
            if (mSetSpeedDuration > 0)
            {
                mSetSpeedDuration -= _deltatime;
                if (mSetSpeedDuration <= 0)
                {
                    mTimeScale = 1;
                    if (CurAnimator is not null)
                    {
                        CurAnimator.speed = 1;
                    }
                }
            }
        }
        
        // 注册按键按下
        private void OnSetKeyDown(string _keyName)
        {
            // EngineDebug.Log($"按下按键 {_keyName}");
            foreach (var VARIABLE in mAllActionStatePart)
            {
                if (VARIABLE.ActionEnble)
                {
                    VARIABLE.SetKeyDown(_keyName);
                }
            }
        }
        // 注册按键抬起
        private void OnSetKeyUp(string _keyName)
        {
            foreach (var _action in mDwonAction)
            {
                _action(_keyName);
            }
            foreach (var VARIABLE in mAllActionStatePart)
            {
                if (VARIABLE.ActionEnble)
                {
                    VARIABLE.SetKeyUp(_keyName);
                }
            }
        }

        private void OnSendKeyDown(string _keyName, int _inputType)
        {
            //按下
            if (_inputType == 0)
            {
                foreach (var VARIABLE in mAllActionStatePart)
                {
                    if (VARIABLE.ActionEnble)
                    {
                        VARIABLE.NowInputDownKey = _keyName;
                    }
                }
            }
            //抬起
            else if(_inputType == 1)
            {
                foreach (var VARIABLE in mAllActionStatePart)
                {
                    if (VARIABLE.ActionEnble)
                    {
                        VARIABLE.NowInputUpKey = _keyName;
                        // VARIABLE.SetKeyUp(_keyName);
                        //重置长按按钮
                        // if (_keyName == NowInputHoldKey)
                        // {
                        //     NowInputHoldKey = MotionEngineConst.NondKeyName;
                        // }
                    }
                }
            }
            //点击
            else if(_inputType == 2)
            {
                foreach (var VARIABLE in mAllActionStatePart)
                {
                    if (VARIABLE.ActionEnble)
                    {
                        VARIABLE.NowInputClickKey = _keyName;
                    }
                }
            }
            //长按
            else if(_inputType == 3)
            {
                foreach (var VARIABLE in mAllActionStatePart)
                {
                    if (VARIABLE.ActionEnble)
                    {
                        VARIABLE.NowInputHoldKey = _keyName;
                    }
                }
            }
        }

        // public void Touch_UnLoadHoldAction(string _actionName)
        // {
        //     foreach (var VARIABLE in mAllActionStatePart)
        //     {
        //         if (VARIABLE.NowInputHoldKey == _actionName)
        //         {
        //             VARIABLE.NowInputHoldKey = "";
        //         }
        //     }
        // }
        private ActionState OnGetActionState(string _name)
        {
            if (ActionStateID.TryGetValue(_name, out int _id))
            {
                return OnGetActionState(_id);
            }
            else
            {
                EngineDebug.LogError($"不存在这个Action: {_name}");
            }
            return null;
        }

        private ActionState OnGetActionState(int _id)
        {
            if (ActionStates.TryGetValue(_id, out ActionState _action))
            {
                return _action;
            }
            return null;
        }

        private void OnChangeAction(string _name, int _mixTime, int _offsetTime)
        {
            ActionState _actionState = OnGetActionState(_name);
            if (_actionState is null) return;
            mAllActionStatePart[_actionState.AnimaLayer].ChangeState(_actionState, _mixTime, _offsetTime);
        }
        private void OnChangeAction(int _id, int _mixTime, int _offsetTime)
        {
            ActionState _actionState = OnGetActionState(_id);
            if (_actionState is null) return;
            mAllActionStatePart[_actionState.AnimaLayer].ChangeState(_actionState, _mixTime, _offsetTime);
        }

        // private void OnSetToClient(bool _isLocal)
        // {//设置为客户端  屏蔽部分逻辑的执行
        //     foreach (var VARIABLE in mAllActionStatePart)
        //     {
        //         VARIABLE.IsLocalClient = _isLocal;
        //     }
        // }
        public void MachineExecuteChangeEven(int _id, int _mixTime, int _offsetTime)
        {
            OnChange?.Invoke(_id, _mixTime, _offsetTime);
        }
    }
}
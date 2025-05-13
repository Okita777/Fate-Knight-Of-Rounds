using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AsiActionEngine.RunTime
{

    /// <summary>
    /// 单位行为逻辑核心处理
    /// </summary>
    public partial class ActionStatePart
    {
        // private float ElapsedTime;//已经经过的时间
        private float mElapsedTime_last;//切换Action时，上一个Action的时间
        
        private ActionStateMachine mActionStateMachine;
        private List<ActionState> mCurrentActionStates = new List<ActionState>();//当前执行的次要事件
        private List<ActionEvent> mCurrentActionEvents = new List<ActionEvent>();//当前正在运行的行为事件
        private List<ActionInterrupt> mCurActionInterrupt = new List<ActionInterrupt>();//当前正在执行判断的打断轨

        private Unit mCurUnit => mActionStateMachine.CurUnit;//当前绑定单位

        public int AnimaLayer { get; private set; }
        public int MixTime { get; private set; }
        public int OffsetTime { get; private set; }

        public ActionStatePart(ActionStateMachine _actionStateMachine, int curActionLayer)
        {
            mActionStateMachine = _actionStateMachine;
            mCurActionLayer = curActionLayer;
            ActionEnble = false;
        }

        private bool OnChangeState(string _actionName, int _mixTime = 0, int _offsetTime = 0)
        {
            if (ActionStateID.ContainsKey(_actionName))
            {
                return OnChangeState(ActionStates[ActionStateID[_actionName]], _mixTime, _offsetTime);
                // EngineDebug.Log("切换状态至: " + _actionName);
            }
            else
            {
                EngineDebug.LogWarning($"Action切换失败，当前数据不存在:{_actionName}");
            }
            return false;
        }
        
        private void OnEnterState(ActionState _action)
        {
            ActionEnble = true;
            CurrentActionState = _action;
            mCurActionInterrupt.AddRange(_action.InterruptList);
            mCurActionInterrupt.AddRange(GetInterruptGroup(_action.InterruptGroupList));
            
            //避免参数修改  统一实例化
            foreach (var _actionEvent in _action.EventList)
            {
                if (_actionEvent.EditorInheritable)
                {
                    mCurrentActionEvents.Add(mActionStateMachine.CreactAction(_actionEvent));
                    // mCurrentActionEvents.Add(_actionEvent.Clone());
                }
                else
                {
                    mCurrentActionEvents.Add(_actionEvent);
                }
            }
            
            //单独执行动画事件轨道
            if (_action.AnimEvent != null && _action.AnimEvent.EventData != null)
            {
                _action.AnimEvent.EventData.Enter(this, false);
            }
            
            OnEventsEnter(mCurrentActionEvents);
        }
        private void OnUpdateState(float _deltatime)
        {
            if (!ActionEnble || CurrentActionState == null)
            {
#if UNITY_EDITOR
                if(ActionEnble && CurrentActionState == null) EngineDebug.LogWarning("ActionStateMachine未工作,Action加载失败");
#endif
                return;
            }

            //执行事件
            // PercentTime = mElapsedTime / CurrentActionState.TotalTime;
            OnUpdateEvent(mCurrentActionEvents, _deltatime);
            
            if (mActionStateMachine.IsLocalClient)
            {//仅本地客户端执行
                //检查并更新按钮交互状态
                UpdateInputKey(_deltatime);

                //检查行为是否结束  如果结束后并跳转则无视后续行为
                if (ActionStateCheckEnd())
                {
                    return;
                }

                //检查行为是否跳转  如果跳转则无视后续行为
                if (OnInterrupUpdate())
                {
                    return;
                }
            }



            ElapsedTime += _deltatime *  MotionEngineConst.TimeDoubling;
        }

        private void OnLateUpdateState(float _deltatime)
        {
            OnLateUpdateEvent(mCurrentActionEvents, _deltatime);
        }

        private void OnUpdateAnim(float _deltaTime)
        {
            
        }

        public bool OnChangeState(ActionState _action, int _mixTime = 0, int _offsetTime = 0)
        {

            //开启状态机运行状态
            ActionEnble = true;
            
            //切换到自身Action层级时才能往下执行  
            if (_action.AnimaLayer != mCurActionLayer)
            {
                mActionStateMachine.AllActionStatePart[_action.AnimaLayer].
                    ChangeState(_action.Name, _mixTime, _offsetTime);
                //跳转其它层级
                return false;
            }
            //执行回调事件
            mActionStateMachine.MachineExecuteChangeEven(_action.ID, _mixTime, _offsetTime);
            
            //动画事件层级
            AnimaLayer = _action.AnimaLayer;
            
            //动画相关参数
            MixTime = _mixTime;
            OffsetTime = _offsetTime;
            
            //重置按键 
            ReSetKeys(true);
            
            //切换主Action
            CurrentActionState_Last = CurrentActionState;
            CurrentActionState = _action;
            
            //清空并重新装载新Action的事件
            mCurActionInterrupt.Clear();
            mCurActionInterrupt.AddRange(_action.InterruptList);
            mCurActionInterrupt.AddRange(GetInterruptGroup(_action.InterruptGroupList));
            OnChangeEvent(_action.EventList, ElapsedTime, _offsetTime);

            //当前Action状态重新计时
            mElapsedTime_last = ElapsedTime;
            ElapsedTime = OffsetTime;

            //结束时不跳过当前结尾动画事件
            IsJumpEnd = false;
            
            //单独执行动画事件轨道
            if (_action.AnimEvent != null && _action.AnimEvent.EventData != null)
            {
                _action.AnimEvent.EventData.Enter(this,false);
            }

            //成功跳转自身Action
            return true;
        }


    }
}

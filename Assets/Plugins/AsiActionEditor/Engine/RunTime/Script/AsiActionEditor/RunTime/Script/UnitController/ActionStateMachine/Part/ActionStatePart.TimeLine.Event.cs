using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public partial class ActionStatePart
    {
        private List<ActionEvent> mActionEvenInit = new List<ActionEvent>();//已经初始化过的事件
        private void OnEventsEnter(List<ActionEvent> _ActionEvent, int _EnterTime = 0)
        {
            mActionEvenInit.Clear();
            for (int i = 0; i < _ActionEvent.Count; i++)
            {
                ActionEvent _actionEvent = _ActionEvent[i];
                if (_actionEvent.ForceInit)
                {
                    int _endPos = _actionEvent.TriggerTime + _actionEvent.Duration;

                    if (_actionEvent.Duration > 0 && _endPos < _EnterTime)
                    {//剔除掉非无限执行的事件
                        _ActionEvent.Remove(_actionEvent);
                        i--;
                    }
                    else if (_actionEvent.Duration == 0)
                    {
                        if (_actionEvent.TriggerTime == _EnterTime)
                        {
                            _actionEvent.EventData.Enter(this, true);
                            _ActionEvent.Remove(_actionEvent);
                        }
                    }//单帧只执行进入不入列表
                    else
                    {
                        if (_actionEvent.TriggerTime < _EnterTime)
                        {
                            _actionEvent.EventData.Enter(this, false);
                            mActionEvenInit.Add(_actionEvent);
                        }
                    }
                }
                else
                {
                    if (_actionEvent.TriggerTime < _EnterTime)
                    {
                        _actionEvent.EventData.Enter(this, _actionEvent.Duration == 0);
                        if (_actionEvent.Duration != 0)
                        {
                            mActionEvenInit.Add(_actionEvent);
                        }//单帧不入列表
                    }
                }
            }
        }
        // private void OnEventsExit(List<ActionEvent> _ActionEvent, bool _interruot)
        // {
        //     foreach (var VARIABLE in _ActionEvent)
        //     {
        //         if (VARIABLE.TriggerTime < ElapsedTime)
        //             VARIABLE.EventData.Exit(this, _interruot);
        //     }
        //     mActionEvenInit.Clear();
        // }

        private void OnUpdateEvent(List<ActionEvent> _ActionEvent, float _deltatime)
        {
            for (int i = 0; i < _ActionEvent.Count; i++)
            {
                ActionEvent _actionEvent = _ActionEvent[i];
                if (ElapsedTime >= _actionEvent.TriggerTime)
                {
#if  UNITY_EDITOR
                    if (_actionEvent.EventData == null)
                    {
                        EngineDebug.LogError(string.Format(
                            "Action出现空事件!! 出现问题的单位: {0}\nActionName: {1}  触发时间: {2} ",
                            mCurUnit.name,
                            CurrentActionState.Name,
                            _actionEvent.TriggerTime
                        ));
                        return;
                    }
#endif
                    if (_actionEvent.Duration == 0)
                    {
                        _actionEvent.EventData.Enter(this, true);
                        _ActionEvent.Remove(_actionEvent);
                        i--;
                    }//单帧直接执行后踢出列表
                    else if (_actionEvent.Duration > 0)
                    {
                        if (!mActionEvenInit.Contains(_actionEvent))
                        {
                            mActionEvenInit.Add(_actionEvent);
                            _actionEvent.EventData.Enter(this, false);
                        }//如果没有初始化过  那就先初始化
                        else
                        {
                            if (ElapsedTime < _actionEvent.TriggerTime + _actionEvent.Duration)
                            {
                                _actionEvent.EventData.Update(this,new ActionMachineTime
                                    (_deltatime,ElapsedTime,_actionEvent.TriggerTime,_actionEvent.Duration));
                            }
                            else
                            {
                                if (i < _ActionEvent.Count - 1 &&
                                    _ActionEvent[i + 1].EventData.GetEvenType() == _actionEvent.EventData.GetEvenType())
                                {
                                    ActionEvent _next = _ActionEvent[i + 1];
                                    if (_next.Inheritable && _next.TriggerTime ==
                                        _actionEvent.TriggerTime + _actionEvent.Duration)
                                    {
                                        _next.EventData = _next.EventData.Clone(_actionEvent.EventData);
                                        mActionEvenInit.Add(_next);
                                    } //同一帧并且同一时间类型往下继承非序列化函数
                                    else
                                    {
                                        _actionEvent.EventData.Exit(this, false);
                                        // EngineDebug.Log("没有继承对象?");

                                    }
                                } //不是最后一个  并且下一个事件是同一个事件时
                                else
                                {
                                    _actionEvent.EventData.Exit(this, false);
                                }
                                mActionEvenInit.Remove(_actionEvent);
                                _ActionEvent.Remove(_actionEvent);
                                i--;
                            }
                        }
                    }//有限帧
                    else
                    {
                        if (!mActionEvenInit.Contains(_actionEvent))
                        {
                            mActionEvenInit.Add(_actionEvent);
                            _actionEvent.EventData.Enter(this, false);
                        }//如果没有初始化过  那就先初始化
                        else
                        {
                            _actionEvent.EventData.Update(this, new ActionMachineTime
                                (_deltatime, ElapsedTime, _actionEvent.TriggerTime, _actionEvent.Duration));
                        }
                    }//无限帧
                }//当前时间处于事件触发时间之后
            }
        }

        private void OnLateUpdateEvent(List<ActionEvent> _ActionEvent, float _deltatime)
        {
            foreach (ActionEvent VARIABLE in _ActionEvent)
            {
                VARIABLE.EventData.LateUpdate(this,new ActionMachineTime
                    (_deltatime, ElapsedTime, VARIABLE.TriggerTime, VARIABLE.Duration));
            }
            // _ActionEvent.EventData.Update(this, _deltatime);
        }

        private List<ActionEvent> OnChangeEvent(List<ActionEvent> _targetActionEvent, float _currentTime, float _offsetTime)
        {
            List<ActionEvent> _returnEvents = new List<ActionEvent>();
            List<ActionEvent> _EnterEvents = new List<ActionEvent>();

            //仅经历过初始化后的事件可以往下继承， 清理初始化事件列表
            mCurrentActionEvents.Clear();
            mCurrentActionEvents.AddRange(mActionEvenInit);
            mActionEvenInit.Clear();
            
            //遍历新事件列表
            for (int i = 0; i < _targetActionEvent.Count; i++)
            {
                ActionEvent _nextEvent = _targetActionEvent[i];
                bool _isRun = _offsetTime >= _nextEvent.TriggerTime;
                if (_isRun)
                {
                    if (_nextEvent.Duration > 0)
                    {
                        int _EvenEndTime = _nextEvent.TriggerTime + _nextEvent.Duration;
                        _isRun = _EvenEndTime > _offsetTime;
                    }//有限范围帧将可能跳过
                    else if (_nextEvent.Duration == 0)
                    {
                        if (_nextEvent.TriggerTime == _offsetTime)
                        {
                            _EnterEvents.Add(_nextEvent);
                        }//直接执行事件 不加入列表
                        _isRun = false;
                    }//单帧

                    if (_isRun)
                    {
                        bool _findEvent = false;
                        foreach (ActionEvent _currentEnve in mCurrentActionEvents)
                        {
                            if (_nextEvent.EventData.GetEvenType() == _currentEnve.EventData.GetEvenType())
                            {
                                //复制上一个Action的事件到当前列表
                                ActionEvent _event = _nextEvent;
                                if (_nextEvent.EditorInheritable)
                                {
                                    _event = mActionStateMachine.CreactAction(_nextEvent);
                                }
                                if (_nextEvent.Inheritable)
                                {
                                    _event.EventData = _nextEvent.EventData.Clone(_currentEnve.EventData);
                                    mActionEvenInit.Add(_event);
                                    mCurrentActionEvents.Remove(_currentEnve);
                                    // EngineDebug.Log(
                                    //     $"切换Action继承: {_nextEvent.EventData.GetType()}" + "\n" +
                                    //     "当前的对象参数： " + _nextEvent.Duration + "\n" +
                                    //     "上一个对象参数： " + _currentEnve.Duration + "\n" +
                                    //     ""
                                    // );
                                }
                                else
                                {
                                    _currentEnve.EventData.Exit(this,true);
                                    // EngineDebug.Log($"拒接继承: {_nextEvent.EventData.GetType()}");
                                }
                                
                                _returnEvents.Add(_event);
                                _findEvent = true;
                                break;
                            }//找到同类型的轨道后继承变量
                        }

                        if (!_findEvent)
                        {
                            ActionEvent _event = _nextEvent;

                            if (_nextEvent.EditorInheritable)
                            {
                                _event = mActionStateMachine.CreactAction(_nextEvent);
                            }
                            _returnEvents.Add(_event);
                            _EnterEvents.Add(_event);
                            mActionEvenInit.Add(_event);
                        }//未找到 直接创建新的  执行进入函数
                    }
                }//只判断进入触发时间范围的事件
                else
                {
                    if (_nextEvent.EditorInheritable)
                    {
                        _returnEvents.Add(mActionStateMachine.CreactAction(_nextEvent));
                    }
                    else
                    {
                        _returnEvents.Add(_nextEvent);
                    }
                }//未进入触发范围  仅加入列表
            }

            //将剩下的旧事件列表都执行一下退出函数
            foreach (var _actionEvent in mCurrentActionEvents)
            {
                _actionEvent.EventData.Exit(this, true);
            }
            //执行完退出函数后再执行初始函数
            foreach (var _actionEvent in _EnterEvents)
            {
                _actionEvent.EventData.Enter(this, _actionEvent.Duration == 0);
            }
            
            mCurrentActionEvents = _returnEvents;
            return mCurrentActionEvents;
        }
        
        
    }
}
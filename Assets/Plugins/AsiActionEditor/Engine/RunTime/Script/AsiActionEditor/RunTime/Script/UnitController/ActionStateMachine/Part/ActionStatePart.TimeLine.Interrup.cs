using System.Collections.Generic;

namespace AsiActionEngine.RunTime
{
    public partial class ActionStatePart
    {
        private bool OnInterrupUpdate()
        {
            bool _isKeyDown = false;
            bool _isKeyUp = false;
            bool _isKeyClick = false;
            foreach (var VARIABLE in mCurActionInterrupt)
            {
                int _endTime = VARIABLE.TriggerTime + VARIABLE.Duration;
                if (_endTime < ElapsedTime && VARIABLE.Duration >= 0)
                {
                    //当前时间已经大于当前打断轨最末端的时间，因此不再会执行而销毁
                    // curActionInterrupt.Remove(VARIABLE);
                }
                else
                {
                    if (VARIABLE.TriggerTime <= ElapsedTime)
                    {
                        int _interrupTime = VARIABLE.TriggerTime + VARIABLE.ExecuteTime;
                        if (_interrupTime <= ElapsedTime)
                        {
                            //直接跳转
                            if (CheckInterrupCondition(VARIABLE.InterruptConditionList, VARIABLE.CheckAllCondition))
                            {
                                if (OnChangeState(VARIABLE.ActionName, VARIABLE.CrossFadeTime,
                                        VARIABLE.OffsetTime))
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            //在预输入轨  在这里决定是否重置按键
                            foreach (var _interruptCondition in VARIABLE.InterruptConditionList)
                            {
                                if (_interruptCondition.InterruptType == -(int)EInterruptTypeInternal.EIT_CheckInput)
                                {
                                    CheckCostomKey _checkCostomKey = _interruptCondition as CheckCostomKey;
                                    if (_checkCostomKey.InputType == EInputKeyType.OnDown)
                                    {
                                        if (!_isKeyDown)
                                        {
                                            if (NowInputDownKey == _checkCostomKey.CheckKeyName)
                                                _isKeyDown = true;
                                        }
                                    }
                                    else if (_checkCostomKey.InputType == EInputKeyType.OnUp)
                                    {
                                        if (!_isKeyUp)
                                        {
                                            if (NowInputUpKey == _checkCostomKey.CheckKeyName)
                                                _isKeyUp = true;
                                        }
                                    }
                                    else
                                    {
                                        if (!_isKeyClick)
                                        {
                                            if (NowInputClickKey == _checkCostomKey.CheckKeyName)
                                                _isKeyClick = true;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //预输入轨没有满足按键的条件  所以回收按键
            if (!_isKeyDown) NowInputDownKey = MotionEngineConst.NondKeyName;
            if (!_isKeyUp) NowInputUpKey = MotionEngineConst.NondKeyName;
            if (!_isKeyClick) NowInputClickKey = MotionEngineConst.NondKeyName;

            return false;
        }

        private bool CheckInterrupCondition(List<IInterruptCondition> _conditions, bool _checkAllCondition)
        {
            if (_checkAllCondition)
            {
                foreach (var VARIABLE in _conditions)
                {
                    if (!VARIABLE.CheckInterrupt(mCurUnit,this))
                        return false;
                }
                return true;
            }
            else
            {
                foreach (var VARIABLE in _conditions)
                {
                    if (VARIABLE.CheckInterrupt(mCurUnit,this))
                        return true;
                }
                return false;
            }
        }

        private List<ActionInterrupt> GetInterruptGroup(List<ActionInterruptGroup> _interruptGroups)
        {
            List<ActionInterrupt> _actionInterrupts = new List<ActionInterrupt>();

            foreach (var _interruptGroup in _interruptGroups)
            {
                if (ActionStates.ContainsKey(_interruptGroup.ActionID))
                {
                    List<ActionInterrupt> _targets = ActionStates[_interruptGroup.ActionID].InterruptList;
                    if (_targets.Count != _interruptGroup.ActionHide.Count)
                    {
                        foreach (var _target in _targets)
                        {
                            ActionInterrupt _newInterrupt = _target.Clone();
                            _newInterrupt.TriggerTime += (_interruptGroup.Offset);
                            _actionInterrupts.Add(_newInterrupt);
                        }
#if UNITY_EDITOR
                        EngineDebug.LogWarning("序列化可能存在错误，打断组配置丢失");
#endif
                        continue;
                    }
                    for (int i = 0; i < _targets.Count; i++)
                    {
                        if (!_interruptGroup.ActionHide[i])
                        {
                            //todo: 这里有runtime时的实例化，后期优化
                            ActionInterrupt _newInterrupt = _targets[i].Clone();
                            _newInterrupt.TriggerTime += (_interruptGroup.Offset + _interruptGroup.ActionOffset[i]);
                            _actionInterrupts.Add(_newInterrupt);
                        }
                    }
                }
                else
                {
                    EngineDebug.LogError($"组添加失败，Action丢失： {_interruptGroup.ActionID}");
                }
            }
            
            return _actionInterrupts;
        }

        // private List<ActionInterrupt> CheckInterrupInit(List<ActionInterruptGroup> _interruptGroups)
        // {
        //     
        // }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public partial class ActionStateMachine
    {
        private string hitActionName => mActionStateInfo.mHitAction;
        private List<ActionInterrupt> hitActionInterrup = new List<ActionInterrupt>();
        private Unit onAttacker = null;
        
        private bool hasAttacker = false;
        private bool hasHiter = false;
        private bool hasHitobj = false;

        /// <summary>
        /// 被击中时调用的函数
        /// </summary>
        /// <param name="_attackInfo">攻击信息</param>
        /// <param name="_attacker">攻击者</param>
        /// <param name="_hitPoint">攻击点位</param>
        private void OnBeHit(IAttackInfo _attackInfo, Unit _attacker, Vector3 _hitPoint, GValue_Setting _gValueRatio)
        {
            onAttacker = _attacker;
            hasAttacker = true;
            // EngineDebug.Log("命中");
            _gValueRatio.OnSet(this);
            bool _isInterrup = false;
            //受击轨道调用
            foreach (var _actionStatePart in mAllActionStatePart)
            {
                if (_actionStatePart.ActionEnble)
                {
                    // EngineDebug.LogWarning($"尝试寻找打断轨道 {_actionStatePart.CurrentActionState.InterruptList_BeHit.Count}");
                    foreach (var _actionInterrupt in _actionStatePart.CurrentActionState.InterruptList_BeHit)
                    {
                        if (CheckValueTrack(_actionInterrupt, (int)_actionStatePart.ElapsedTime))
                        {
                            if (CheckInterrupCondition(_actionInterrupt.InterruptConditionList,
                                    _actionInterrupt.CheckAllCondition, _actionStatePart))
                            {
                                // EngineDebug.Log($"成功从打断轨跳受击 {(int)_actionStatePart.ElapsedTime}");
                                _actionStatePart.ChangeState(_actionInterrupt.ActionName,
                                    _actionInterrupt.CrossFadeTime,
                                    _actionInterrupt.OffsetTime);
                                _isInterrup = true;
                                break;
                            } //检查条件是否满足
                        }
                    }//所有受击轨道
                }
            }//所有层级

            if (!_isInterrup)
            {
                if (mActionStateID.TryGetValue(hitActionName, out int _id))
                {
                    hitActionInterrup.Clear();
                    ActionState _hitActionState = mActionStates[_id];
                    hitActionInterrup.AddRange(_hitActionState.InterruptList);
                    hitActionInterrup.AddRange(_hitActionState.InterruptList_BeHit);
                    ActionStatePart _curActionStatePart = mAllActionStatePart[_hitActionState.AnimaLayer];

                    if (hitActionInterrup.Count < 1)
                    {
                        _curActionStatePart.ChangeState(hitActionName);
                    }
                    else
                    {
                        foreach (var _actionInterrupt in hitActionInterrup)
                        {
                            if (CheckInterrupCondition(_actionInterrupt.InterruptConditionList,
                                          _actionInterrupt.CheckAllCondition, _curActionStatePart))
                            {
                                _curActionStatePart.ChangeState(_actionInterrupt.ActionName,
                                    _actionInterrupt.CrossFadeTime,
                                    _actionInterrupt.OffsetTime);

                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(hitActionName))
                    {
                        EngineDebug.LogWarning($"没有配置受击Action  mode:{CurUnit.name}");
                    }
                    else
                    {
                        EngineDebug.LogWarning($"受击Action配置出错 [{hitActionName}]  mode:{CurUnit.name}");
                    }
                }
            }//没有检测到受击打断轨时才执行
            _attackInfo.BeHit(_attackInfo, this, _attacker, _hitPoint);
        }

        /// <summary>
        /// 命中对象时调用的函数
        /// </summary>
        /// <param name="_BeHit">命中的对象</param>
        /// <param name="_BeHitUnit">命中的单位（当命中对象不为单位时，此值为空）</param>
        /// <param name="_hitPoint">命中位置</param>
        private void OnOnHit(IAttackInfo _attackInfo, GameObject _BeHit, Unit _BeHitUnit, Vector3 _hitPoint)
        {
            OnHitObject = _BeHit;
            hasHitobj = true;
            if (_BeHitUnit is not null)
            {
                hasHiter = true;
                HitUnit = _BeHitUnit;
            }
            _attackInfo.OnHit(_attackInfo, this, _BeHitUnit, _hitPoint);

            //命中轨道调用 
            foreach (var _actionStatePart in mAllActionStatePart)
            {
                if (_actionStatePart.ActionEnble)
                {
                    foreach (var _actionInterrupt in _actionStatePart.CurrentActionState.InterruptList_OnHit)
                    {
                        if (CheckValueTrack(_actionInterrupt, (int)_actionStatePart.ElapsedTime))
                        {
                            if (CheckInterrupCondition(_actionInterrupt.InterruptConditionList,
                                    _actionInterrupt.CheckAllCondition, _actionStatePart))
                            {
                                _actionStatePart.ChangeState(_actionInterrupt.ActionName,
                                    _actionInterrupt.CrossFadeTime,
                                    _actionInterrupt.OffsetTime);
                                break;
                            } //检查条件是否满足
                        }
                    }//所有命中轨道
                }//是否启用
            }//所有层级
        }
        
        //检查条件
        private bool CheckInterrupCondition(List<IInterruptCondition> _conditions,
            bool _checkAllCondition, ActionStatePart _actionStatePart)
        {
            if (_checkAllCondition)
            {
                foreach (var VARIABLE in _conditions)
                {
                    if (!VARIABLE.CheckInterrupt(CurUnit,_actionStatePart))
                        return false;
                }
                return true;
            }
            else
            {
                foreach (var VARIABLE in _conditions)
                {
                    if (VARIABLE.CheckInterrupt(CurUnit,_actionStatePart))
                        return true;
                }

                if (_conditions.Count < 1)
                {
                    return true;
                }
                return false;
            }
        }

        private bool CheckValueTrack(ActionInterrupt _actionInterrupt, int _curTime)
        {
            if (_actionInterrupt.Duration < 0)
                return _curTime >= _actionInterrupt.TriggerTime;
            
            if (_curTime >= _actionInterrupt.TriggerTime)
            {
                if (_curTime <= _actionInterrupt.TriggerTime + _actionInterrupt.Duration)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class ActionEventDescripted
    {
        public bool mIsInit = false;//初始化?
        public void Draw(EditorActionEvent _actionEvent)
        {
            if (mIsInit)
            {
                DrawEditorAttribute.NeedInit = true;
            }
            IActionEventData _eventData = _actionEvent.EventData;
            if (_eventData == null)
            {
                GUILayout.Label("无法绘制，资源序列化失败");
                return;
            }

            if (_eventData.GetEvenType() < 0)
            {

                EEvenTypeInternal _GetEvenType = (EEvenTypeInternal)(-_eventData.GetEvenType());

                using (new GUIColorScope(Color.gray))
                {
                    GUILayout.Label(_GetEvenType.ToString());
                }
                
                GUILayout.Space(10);
                switch (_GetEvenType)
                {
                    case EEvenTypeInternal.EET_DTD_PlayAnim:
                        DrawPlayAnim(_actionEvent);
                        break;
                    case EEvenTypeInternal.EET_AttackBox:
                        DrawAttackBox((Event_AttackBox)_eventData, _actionEvent);
                        break;
                    case EEvenTypeInternal.EET_RayCast:
                        DrawRayCast(_actionEvent);
                        break;
                    default:
                        DrawEditorAttribute.Draw(_eventData);
                        break;
                }
            }
            else
            {
                ActionWindowMain.ActionEditorFuntion.DrawEventDate(_actionEvent, mIsInit);
            }

            mIsInit = false;
        }
        #region EventDrawFunction

        private void DrawRayCast(EditorActionEvent _actionEvent)
        {
            Event_Cast_Ray _ecr = _actionEvent.EventData as Event_Cast_Ray;
            DrawEditorAttribute.Draw(_ecr);
            DrawEditorAttribute.DrawRayHitData(_ecr.RayCastData);
        }

        private AnimationClip mPlayAnimClip = null;
        private void DrawPlayAnim(EditorActionEvent _actionEvent)
        {
            Event_PlayAnim _epa = _actionEvent.EventData as Event_PlayAnim;
            float _disSpeed = 1f;
            EditorActionState _editorActionState = ResourcesWindow.Instance.GetEditorActionStateToSelect();
            int _selectLayer = _editorActionState.AnimaLayer;
            AnimatorState _motionInfo = ResourcesWindow.Instance.GetMotionToAnimName(_epa.AnimName, _selectLayer);
            float _clipLenth = -2;
            
            if (mIsInit)
            {
                mPlayAnimClip = null;
                if (_motionInfo is not null && _motionInfo.motion is not null)
                {
                    if (_motionInfo.motion is AnimationClip _clip)
                    {
                        mPlayAnimClip = _clip;
                    }
                    else if (_motionInfo.motion is BlendTree _blend)
                    {
                        mPlayAnimClip = GetClipToBlend(_blend);
                    }
                }
            }
            
            if (mPlayAnimClip is not null)
            {
                //当前动画时长
                float _currentLength = (float)_actionEvent.Duration / RunTime.MotionEngineConst.TimeDoubling;
                _clipLenth = mPlayAnimClip.length;
                _disSpeed = _clipLenth / _currentLength;
                _motionInfo.speed = _disSpeed;
                // _epa.Speed = (int)(_disSpeed * Runtime.MotionEngineConst.TimeDoubling);
            }
            else
            {
                GUILayout.Label("当前State不含动画资源");
            }
            
            GUILayout.Label("AnimaName : " + _epa.AnimName);
            GUILayout.Label("AnimaSpeed: " + _disSpeed.ToString("0.000"));
            
            GUILayout.Space(20);

            if (GUILayout.Button("重置动画轨"))
            {
                _actionEvent.TriggerTime = 0;
                _actionEvent.Duration = Mathf.Abs((int)(_clipLenth * RunTime.MotionEngineConst.TimeDoubling));
            }

            if (GUILayout.Button("将Action长度对齐至动画轨"))
            {
                _editorActionState.TotalTime = _actionEvent.TriggerTime + _actionEvent.Duration;
                TimeLineWindow.Instance.Repaint();
            }
            GUI.changed = true;
        }

        #endregion
        // public void DrawProperty(object obj)
        private EditorActionInterrupt ActionInterrupt_copy = null;
        private IInterruptCondition IInterruptCondition_copy = null;
        public void DrawInterruptCondition(EditorActionInterrupt _actionEvent)
        {
            if (mIsInit)
            {
                DrawEditorAttribute.NeedInit = true;
            }
            _actionEvent.CheckAllCondition = EditorGUILayout.Toggle($"需要满足所有条件: ", _actionEvent.CheckAllCondition);
            using (new GUILayout.HorizontalScope())
            {
                int _select = _actionEvent.InterruptType;
                if (_actionEvent.InterruptType < 0)
                {
                    _select *= -1;
                }
                else
                {
                    _select += ActionWindowMain.ConditionType_m.Length;
                }
                
                _select = EditorGUILayout.Popup(_select, ActionWindowMain.ConditionTypesDes.ToArray());
                if (_select < ActionWindowMain.ConditionType_m.Length)
                {
                    _actionEvent.InterruptType = _select * -1;
                }
                else
                {
                    _actionEvent.InterruptType = _select - ActionWindowMain.ConditionType_m.Length;
                }
                
                using (new GUIColorScope(Color.green))
                {
                    if (GUILayout.Button("新建", GUILayout.Width(60)))
                    {
                        if (_actionEvent.InterruptType < 0)
                        {
                            EInterruptTypeInternal _interruptType = (EInterruptTypeInternal)(-_actionEvent.InterruptType);
                            if (
                                // _interruptType == EInterruptTypeInternal.EIT_CheckInput ||
                                _interruptType == EInterruptTypeInternal.EIT_CheckMove
                                || _interruptType == EInterruptTypeInternal.EIT_CheckBeHit
                                || _interruptType == EInterruptTypeInternal.EIT_CheckOnHit
                                || _interruptType == EInterruptTypeInternal.EIT_CheckWeightRange
                               )
                            {
                                foreach (var _interruptCondition in _actionEvent.InterruptConditionList)
                                {
                                    if (_interruptCondition.InterruptType == (int)_actionEvent.InterruptType)
                                    {
                                        EditorUtility.DisplayDialog("警告",
                                            $"{_interruptType} 已存在，请勿重复添加", "我知道了");
                                        return;
                                    }
                                }
                            } //这些判断条件不能存在多个  浪费性能
                        }

                        IInterruptCondition _condition = ActionEventCreact.CreactCondition(_actionEvent.InterruptType);
                        _actionEvent.InterruptConditionList.Add(_condition);
                        _actionEvent.ConditionUnFold.Add(true);
                        mIsInit = true;
                    }
                    GUI.color = Color.red;
                    if (GUILayout.Button("删除选择", GUILayout.Width(60)))
                    {
                        if (_actionEvent.SelectID < _actionEvent.ConditionUnFold.Count)
                        {
                            _actionEvent.InterruptConditionList.RemoveAt(_actionEvent.SelectID);
                            _actionEvent.ConditionUnFold.RemoveAt(_actionEvent.SelectID);
                        }
                    }
                    if (GUILayout.Button("删除全部", GUILayout.Width(60)))
                    {
                        _actionEvent.InterruptConditionList.Clear();
                        _actionEvent.ConditionUnFold.Clear();
                    }
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("复制当前条件列表", GUILayout.Width(120)))
                {
                    ActionInterrupt_copy = _actionEvent;
                }

                bool _isCopy = ActionInterrupt_copy is not null;
                using (new GUIColorScope(Color.green, _isCopy && ActionInterrupt_copy != _actionEvent))
                {
                    if (GUILayout.Button("粘贴" + (_isCopy ? $" [{ActionInterrupt_copy.ActionName}]" : "")))
                    {
                        // string _actionName = _actionEvent.ActionName;
                        if (_isCopy && ActionInterrupt_copy != _actionEvent)
                            ActionInterrupt_copy.CopyCondition(_actionEvent);
                        // _actionEvent.ActionName = _actionName;
                        // _actionEvent.CopyCondition(ActionInterrupt_copy);
                    }
                }
            }
            GUILayout.Space(10);
            for (int i = 0; i < _actionEvent.InterruptConditionList.Count; i++)
            {
                IInterruptCondition _condition = _actionEvent.InterruptConditionList[i];
                int _select = 0;
                if (_condition.InterruptType > 0)
                {
                    _select = _condition.InterruptType + ActionWindowMain.ConditionType_m.Length;
                }
                else
                {
                    _select = -_condition.InterruptType;
                }
                string _name = ActionWindowMain.ConditionTypesDes[_select];

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(_actionEvent.ConditionUnFold[i] ? "-" : "+", GUILayout.Width(20)))
                    {
                        _actionEvent.ConditionUnFold[i] = !_actionEvent.ConditionUnFold[i];
                    }

                    using (new GUIColorScope(_actionEvent.SelectID == i ? Color.gray : GUI.color))
                    {
                        if (GUILayout.Button(_name))
                        {
                            if (_actionEvent.SelectID == i)
                            {
                                _actionEvent.ConditionUnFold[i] = !_actionEvent.ConditionUnFold[i];
                            }

                            _actionEvent.SelectID = i;
                        }
                    }

                    if (GUILayout.Button("Copy", GUILayout.Width(60)))
                    {
                        IInterruptCondition_copy = _condition.Clone();
                    }

                    if (IInterruptCondition_copy is not null && 
                        IInterruptCondition_copy.InterruptType == _condition.InterruptType) 
                    {
                        using (new GUIColorScope(Color.green))
                        {
                            if (GUILayout.Button("Past", GUILayout.Width(60)))
                            {
                                // if (IInterruptCondition_copy != _condition )
                                _actionEvent.InterruptConditionList[i] = IInterruptCondition_copy.Clone();
                            }
                        }
                    }
                }

                if (_actionEvent.ConditionUnFold[i])
                {
                    DrawCondition(_actionEvent.InterruptConditionList[i]);
                }
            }
            mIsInit = false;
        }

        //条件的属性面板绘制
        private void DrawCondition(IInterruptCondition _interruptCondition)
        {
            if (_interruptCondition == null)
            {
                GUILayout.Label("无法绘制，事件序列化失败");
                return;
            }

            if (_interruptCondition.InterruptType < 0)
            {
                EInterruptTypeInternal _InterruptType = (EInterruptTypeInternal)(-_interruptCondition.InterruptType);
                switch (_InterruptType)
                {
                    case EInterruptTypeInternal.EIT_CheckInput:
                        DrawCheckCostomKey((CheckCostomKey)_interruptCondition);
                        break;
                    case EInterruptTypeInternal.EIT_CheckBeHit:
                        DrawCheckBeHit();
                        break;
                    case EInterruptTypeInternal.EIT_CheckOnHit:
                        DrawCheckOnHit();
                        break;
                    case EInterruptTypeInternal.EIT_CheckWeightRange:
                        DrawWeightRange((CheckWeightRange)_interruptCondition);
                        break;
                    default:
                        DrawEditorAttribute.Draw(_interruptCondition);
                        break;
                }
            }//绘制内部条件
            else
            {
                ActionWindowMain.ActionEditorFuntion.DrawCondition(_interruptCondition, mIsInit);
            }//绘制客户端条件
        }
        
        #region InterrupDrawFuntion

        private int inputActionID = -1;
        private void DrawCheckCostomKey(CheckCostomKey _condition)
        {
            if (mIsInit)
            {
                inputActionID = -1;
                if (!string.IsNullOrEmpty(_condition.CheckKeyName))
                {
                    for (int i = 0; i < InputActionList.Instance.ActionList.Count; i++)
                    {
                        if (_condition.CheckKeyName == InputActionList.Instance.ActionList[i])
                        {
                            inputActionID = i;
                            break;
                        }
                    }
                }
                else
                {
                    _condition.CheckKeyName = InputActionList.Instance.ActionList[0];
                    inputActionID = 0;
                }
                // Debug.Log("初始化");
            }
            
            using (new GUILayout.HorizontalScope())
            {
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    if (inputActionID == -1)
                    {
                        _condition.CheckKeyName = EditorGUILayout.TextField(_condition.CheckKeyName);
                        if (_check.changed)
                        {
                            mIsInit = true;
                        }
                    }
                    else
                    {
                        inputActionID =
                            EditorGUILayout.Popup(inputActionID, InputActionList.Instance.ActionList.ToArray());
                        if (_check.changed)
                        {
                            _condition.CheckKeyName = InputActionList.Instance.ActionList[inputActionID];
                        }
                    }
                }
                // _condition.CheckKeyName = EditorGUILayout.TextField(_condition.CheckKeyName);
                _condition.InputType = (EInputKeyType)EditorGUILayout.EnumPopup(_condition.InputType,GUILayout.Width(80));
            }
        }

        private void DrawWeightRange(CheckWeightRange _condition)
        {
            _condition.checkWeight = EditorGUILayout.Slider("触发概率", _condition.checkWeight, 0, 1);
        }
        private void DrawCheckBeHit()
        {
            GUILayout.Label("描述：单位受击时通过");
        }

        private void DrawCheckOnHit()
        {
            GUILayout.Label("描述：单位攻击命中对象时通过");
        }
        #endregion

        #region LocalFuntion

        private AnimationClip GetClipToBlend(BlendTree _blend)
        {
            if (_blend is not null)
            {
                foreach (var _childMotion in _blend.children)
                {
                    if (_childMotion.motion is not null)
                    {
                        if (_childMotion.motion is AnimationClip _clip)
                        {
                            return _clip;
                        }
                        else if (_childMotion.motion is BlendTree _blendTree)
                        {
                            AnimationClip _outClip = GetClipToBlend(_blendTree);
                            if (_outClip is not null)
                            {
                                return _outClip;
                            }
                        }
                    }
                }
            }
            return null;
        }
        

        #endregion
    }
}
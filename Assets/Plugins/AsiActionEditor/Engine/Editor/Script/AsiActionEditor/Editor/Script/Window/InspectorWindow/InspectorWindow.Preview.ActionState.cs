using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class InspectorWindow
    {
        private string[] m_AllActionName = new string[0];
        private int m_SelectDefaultActionState = 0;
        private int m_SelectDefaultAnimName = 0;
        private int m_SelectDefaultAnimLayer = 0;
        private int m_SelectActionType;

        // private int m_newID;
        // private string m_newName;
        private void DrawActionState(EditorActionState _actionState)
        {
            if (!InitActionState(_actionState))
            {
                return;
            }

            //绘制头部
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("序号：", GUILayout.Width(60));
                // m_newID = EditorGUILayout.IntField(m_newID);
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    int _newID = EditorGUILayout.DelayedIntField(_actionState.ID);
                    if (_check.changed)
                    {
                        ActionEngineReName.ReID(_actionState.ID, _newID);
                        ResourcesWindow.Instance.ActionListSort();
                        // ResourcesWindow.Instance.GetSelectIDToAnimName()
                        ResourcesWindow.Instance.OnSelectActionState(_actionState);
                        ResourcesWindow.Instance.Repaint();
                    }
                }
                
                // bool _ischange = m_newID != _actionState.ID;
                // using (new GUIColorScope(Color.red, _ischange))
                // {
                //     if (_ischange)
                //     {
                //         if (Event.current.type == EventType.KeyDown)
                //         {
                //             if (Event.current.keyCode == KeyCode.Return)
                //             {
                //                 ActionEngineReName.ReID(_actionState.ID, m_newID);
                //                 ResourcesWindow.Instance.Repaint();
                //             }
                //         }
                //     }
                //     if (GUILayout.Button("应用", GUILayout.Width(39)))
                //     {
                //         ActionEngineReName.ReID(_actionState.ID, m_newID);
                //         ResourcesWindow.Instance.Repaint();
                //     }
                // }
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("名称：", GUILayout.Width(60));
                // m_newName = EditorGUILayout.TextField(m_newName);
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    string _newName = EditorGUILayout.DelayedTextField(_actionState.Name);
                    if (_check.changed)
                    {
                        ActionEngineReName.ReName(_actionState.Name, _newName);
                        ResourcesWindow.Instance.Repaint();
                    }
                }
                // EditorGUILayout.DelayedTextField
                // bool _ischange = m_newName != _actionState.Name;
                // using (new GUIColorScope(Color.red, _ischange))
                // {
                //     if (_ischange)
                //     {
                //         if (Event.current.type == EventType.KeyDown)
                //         {
                //             if (Event.current.keyCode == KeyCode.Return)
                //             {
                //                 ActionEngineReName.ReName(_actionState.Name, m_newName);
                //                 ResourcesWindow.Instance.Repaint();
                //             }
                //         }
                //     }
                //     if (GUILayout.Button("应用", GUILayout.Width(39)))
                //     {
                //         ActionEngineReName.ReName(_actionState.Name, m_newName);
                //         ResourcesWindow.Instance.Repaint();
                //     }
                // }
            }
            
            //绘制身体
            GUILayout.Space(10);
            using (new GUIColorScope(Color.cyan))
            {
                GUILayout.Label("--基础参数");
            }
            _actionState.TotalTime = EditorGUILayout.IntField("总时长：", _actionState.TotalTime);
            // _actionState.ActionType = (EActionType)EditorGUILayout.EnumPopup("行为类型",_actionState.ActionType);
            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                m_SelectActionType =
                    EditorGUILayout.Popup(m_SelectActionType, ResourcesWindow.Instance.ActionType.ToArray());
                if (_check.changed)
                {
                    _actionState.EditorActionType = ResourcesWindow.Instance.ActionType[m_SelectActionType];
                }

            }
            

            GUILayout.Space(10);
            using (new GUIColorScope(Color.cyan))
            {
                GUILayout.Label("--动画相关");
            }

            //动画层下拉菜单绘制
            using (var _chack = new EditorGUI.ChangeCheckScope())
            {
                EAnimLayerType _layerType = (EAnimLayerType)m_SelectDefaultAnimLayer;
                _layerType = (EAnimLayerType)EditorGUILayout.EnumPopup("动画层级(类型): ", _layerType);
                if (_chack.changed)
                {
                    m_SelectDefaultAnimLayer = (int)_layerType;
                    if (m_SelectDefaultAnimLayer > 3)
                    {
                        _actionState.EditorAnimaLayer = _actionState.AnimaLayer;
                        _actionState.EditorAnimaID = m_SelectDefaultAnimName;
                        
                        _actionState.AnimaLayer = m_SelectDefaultAnimLayer;
                        m_SelectDefaultAnimName = 0;
                    }
                    else
                    {
                        if (_actionState.AnimaLayer > 3)
                        {
                            _actionState.AnimaLayer = _actionState.EditorAnimaLayer;
                            m_SelectDefaultAnimLayer = _actionState.EditorAnimaLayer;
                            m_SelectDefaultAnimName = _actionState.EditorAnimaID;
                        }
                        else
                        {
                            m_SelectDefaultAnimName = 0;
                        }
                    }
                }
            }

            //选择动画  当层级为Script层时不允许选择动画
            if (m_SelectDefaultAnimLayer < 4)
            {
                SelectAnim(_actionState);
            }

            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                m_SelectDefaultActionState =
                    EditorGUILayout.Popup("结束后衔接行为", m_SelectDefaultActionState, m_AllActionName);
                if (_check.changed)
                {
                    _actionState.DefaultAction = m_AllActionName[m_SelectDefaultActionState].Split('/')[^1];
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("融合时间:");
                _actionState.MixTime = EditorGUILayout.IntField(_actionState.MixTime);
                GUILayout.Label("剪切时间:");
                _actionState.OffsetTime = EditorGUILayout.IntField(_actionState.OffsetTime);
            }

            GUILayout.Space(10);
            using (new GUIColorScope(Color.cyan))
            {
                GUILayout.Label("--跳转模板");
            }
            ActionEventDescripted.Instance.DrawInterruptCondition(_actionState.EditorActionInterrupt);
        }

        private void SelectAnim(EditorActionState _actionState)
        {
            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                string[] _animFullName = ResourcesWindow.Instance
                    .GetAnimFullName(m_SelectDefaultAnimLayer).ToArray();
                if (m_SelectDefaultAnimName >= _animFullName.Length)
                {
                    m_SelectDefaultAnimName = 0;
                }
                using (new GUILayout.HorizontalScope())
                {
                    m_SelectDefaultAnimName = 
                        EditorGUILayout.Popup(m_SelectDefaultAnimName, _animFullName,GUILayout.Width(20));
                    string _animName = String.Empty;
                    if (_actionState.EditorAnimEvent != null)
                    {
                        _animName = ((Event_PlayAnim)_actionState.EditorAnimEvent.EventData).AnimName;
                    }

                    GUIStyle _style = new GUIStyle();
                    _style.normal.textColor = Color.white;
                    _style.alignment = TextAnchor.LowerLeft;
                    GUILayout.Label($"当前动画  <color=#FFF100>{_animName}</color>", _style);
                }

                if (_check.changed)
                {
                    Event_PlayAnim _eventPlayAnim = new Event_PlayAnim();
                    _actionState.AnimaLayer = m_SelectDefaultAnimLayer;
                    if (m_SelectDefaultAnimName != 0)
                    {
                        _eventPlayAnim.AnimName = _animFullName[m_SelectDefaultAnimName].Split('/')[^1];
                    }
                    else
                    {
                        _eventPlayAnim.AnimName = String.Empty;
                    }

                    AnimatorState _animaState =
                        ResourcesWindow.Instance.GetMotionToAnimName(_eventPlayAnim.AnimName, _actionState.AnimaLayer);

                    // _actionState.EditorAnimEvent = null;
                    _actionState.EditorAnimEvent = new EditorActionEvent(_eventPlayAnim);
                    _actionState.EditorAnimEvent.TriggerTime = 0;
                    _actionState.EditorAnimEvent.Duration = 1000;
                    _actionState.TotalTime = 1000;
                    if (_animaState is not null && _animaState.motion is Motion _anim)
                    {
                        if (_anim is AnimationClip _clip)
                        {
                            if (_clip is not null)
                            {

                                int _DurationTime = Mathf.RoundToInt
                                    (_clip.length * RunTime.MotionEngineConst.TimeDoubling / _animaState.speed);
                                _actionState.EditorAnimEvent.Duration = _DurationTime;
                                _actionState.TotalTime = _DurationTime;
                                TimeLineWindow.Instance.InitTimeLine();
                            }
                        }
                        else if (_anim is BlendTree _blend)
                        {
                            if (!GetClipToBlend(_blend,_actionState,_animaState,_eventPlayAnim))
                            {
                                EngineDebug.LogError(
                                    $"没有在 <color=#FFCC00>[{_animaState.name}]</color> 中找到Clip" +
                                    $"，请检查该 BlendTree "
                                );
                                return;
                            }
                        }
                        needInit = true;
                    }

                    if (_actionState.EditorAnimEvent is null)
                    {
                        if (m_SelectDefaultAnimName != 0)
                        {
                            EngineDebug.LogWarning("动画轨道空值");
                        }
                        TimeLineWindow.Instance.InitTimeLine();
                        // _actionState.EditorAnimEvent = new EditorActionEvent(_eventPlayAnim);
                    }
                    else
                    {
                        _actionState.EditorAnimEvent.EventData = _eventPlayAnim;
                    }
                }
            }
        }
        
        #region 初始化
        public bool InitActionState(EditorActionState _actionState)
        {
            if (!needInit)
            {
                return true;
            }
            
            ActionEventDescripted.Instance.mIsInit = true;
            
            //读取当前所选层级
            m_SelectDefaultAnimLayer = _actionState.AnimaLayer;
            
            // //初始化参数
            // m_newID = _actionState.ID;
            // m_newName = _actionState.Name;
            
            //找到层级序号
            m_SelectActionType = ResourcesWindow.Instance.GetIDToActionType(_actionState.EditorActionType);
            
            //收集所有的Action，排除掉自己，并寻找【默认衔接】Action的序号 
            List<EditorActionState> _AllAction = ResourcesWindow.Instance.AllActionState;
            if (_AllAction == null)
            {
                return false;
            }
            m_AllActionName = new String[_AllAction.Count];
            m_AllActionName[0] = MotionEngineConst.NullActionName;
            bool _isFindDefault = false;
            bool _isFindCurrtAction = false;
            for (int i = 0; i < _AllAction.Count; i++)
            {
                int _id = i + (_isFindCurrtAction ? 0 : 1);
                if (_id < _AllAction.Count)
                {
                    // string _enumDescript = _AllAction[i].ActionType.ToString();
                    string _enumDescript = _AllAction[i].EditorActionType;
                    m_AllActionName[_id] = $"{_enumDescript}/{_AllAction[i].Name}";
                    if (!_isFindCurrtAction && _actionState == _AllAction[i])
                    {
                        _isFindCurrtAction = true;
                    }
                }

                if (!_isFindDefault)
                {
                    if (_AllAction[i].Name == _actionState.DefaultAction)
                    {
                        _isFindDefault = true;
                        m_SelectDefaultActionState = _id;
                    }
                }
            }
            
            //没有找到【默认衔接】Action的序号的话就给“Null”
            if (!_isFindDefault)
            {
                _actionState.DefaultAction = string.Empty;
                m_SelectDefaultActionState = 0;
                // Debug.Log("没有找到默认衔接或为空");
            }

            //寻找 ActionState 的动画事件
            if (_actionState.EditorAnimEvent == null || _actionState.AnimaLayer > 3)
            {
                m_SelectDefaultAnimName = 0;
            }
            else
            {

                Event_PlayAnim _eventPlayAnim = (Event_PlayAnim)_actionState.EditorAnimEvent.EventData;
                if(_eventPlayAnim is not null)
                m_SelectDefaultAnimName =
                    ResourcesWindow.Instance.GetSelectIDToAnimName(_eventPlayAnim.AnimName, _actionState.AnimaLayer);
            }

            needInit = false;
            return true;
        }
        #endregion

        #region LocalFuntion

        //把BlendTree中所有的Clip全部翻出来
        private bool GetClipToBlend(BlendTree _blendTree, EditorActionState _actionState,
            AnimatorState _animaState, Event_PlayAnim _eventPlayAnim)
        {
            if (_blendTree is not null)
            {
                foreach (var _childMotion in _blendTree.children)
                {
                    if (_childMotion.motion is not null)
                    {
                        if (_childMotion.motion is AnimationClip _animClip)
                        {
                            if (_animClip is not null)
                            {
                                _actionState.EditorAnimEvent = new EditorActionEvent(_eventPlayAnim);
                                _actionState.EditorAnimEvent.TriggerTime = 0;
                                int _DurationTime = Mathf.RoundToInt(_animClip.length *
                                    RunTime.MotionEngineConst.TimeDoubling / _animaState.speed);
                                _actionState.EditorAnimEvent.Duration = _DurationTime;
                                _actionState.TotalTime = _DurationTime;
                                TimeLineWindow.Instance.InitTimeLine();
                                return true;
                            }
                        }
                        else if (_childMotion.motion is BlendTree _blendC)
                        {
                            if (GetClipToBlend(_blendC, _actionState, _animaState, _eventPlayAnim))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            
            return false;
        }

        #endregion
    }
}
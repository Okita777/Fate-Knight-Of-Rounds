using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class InspectorWindow
    {
        private void DrawActionInterruptGUI(EditorActionInterrupt _actionEvent)
        {
            if (!InitActionInterruptGUI(_actionEvent))
            {
                return;
            }
            
            //背景绘制
            float _heighr = EditorGUIUtility.singleLineHeight;

            Rect rect = new Rect(0, _heighr * 9 + 20, position.width, _heighr * 3 + 12);
            EditorGUI.DrawRect(rect, Color.black * 0.5f);

            rect.y+=rect.height + 2;
            rect.x += 2;
            rect.width -= 4;
            rect.height = position.height - rect.y;
            EditorGUI.DrawRect(rect, Color.black * 0.2f);

            using (var _change = new EditorGUI.ChangeCheckScope())
            {
                _actionEvent.TriggerTime = EditorGUILayout.IntField($"触发时间: ", _actionEvent.TriggerTime);
                _actionEvent.Duration = EditorGUILayout.IntField($"持续时间: ", _actionEvent.Duration);
                _actionEvent.ExecuteTime = EditorGUILayout.IntField($"跳转时间: ", _actionEvent.ExecuteTime);
                
                if (_change.changed)
                {
                    if (_actionEvent.ExecuteTime < 0 ||
                        (_actionEvent.ExecuteTime >= _actionEvent.Duration && _actionEvent.Duration>0))
                    {
                        _actionEvent.ExecuteTime = 0;
                    }
                    TimeLineWindow.Instance.Repaint();
                }
            }

            GUILayout.Space(10);
            using (new GUIColorScope(Color.cyan))
            {
                GUILayout.Label("--详细参数:");
            }

            using (new GUILayout.HorizontalScope())
            {
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    m_SelectDefaultActionState =
                        EditorGUILayout.Popup("跳转Action", m_SelectDefaultActionState, m_AllActionName);
                    if (_check.changed)
                    {
                        _actionEvent.ActionName = m_AllActionName[m_SelectDefaultActionState].Split('/')[^1];
                        // if (m_SelectDefaultActionState > 0)
                        // {
                        //     ResourcesWindow.Instance.AllActionState[m_SelectDefaultActionState - 1].EditorActionInterrupt
                        //         .CopyCondition(_actionEvent);
                        // }
                    }
                }

                if (GUILayout.Button("应用模板", GUILayout.Width(70)))
                {
                    // _actionEvent.ActionName = m_AllActionName[m_SelectDefaultActionState].Split('/')[^1];
                    if (m_SelectDefaultActionState > 0)
                    {
                        ResourcesWindow.Instance.AllActionState[m_SelectDefaultActionState - 1].EditorActionInterrupt
                            .CopyCondition(_actionEvent);
                    }
                }
            }

            
            _actionEvent.CrossFadeTime = EditorGUILayout.IntField($"融合时间: ", _actionEvent.CrossFadeTime);
            _actionEvent.OffsetTime = EditorGUILayout.IntField($"剪切时间: ", _actionEvent.OffsetTime);
            
            GUILayout.Space(10);
            using (new GUIColorScope(Color.cyan))
            {
                GUILayout.Label("--跳转条件:");
            }
            ActionEventDescripted.Instance.DrawInterruptCondition(_actionEvent);
        }
        
        private bool InitActionInterruptGUI(EditorActionInterrupt _actionEvent)
        {
            if (!needInit)
            {
                return true;
            }
            ActionEventDescripted.Instance.mIsInit = true;

            //收集所有的Action，排除掉自己，并寻找【默认衔接】Action的序号 
            EditorActionState _actionState = ResourcesWindow.Instance.GetEditorActionStateToSelect();
            List<EditorActionState> _AllAction = ResourcesWindow.Instance.AllActionState;
            if(_AllAction == null)
            {
                return false;
            }
            m_AllActionName = new String[_AllAction.Count+1];
            m_AllActionName[0] = MotionEngineConst.NullActionName;
            bool _isFindDefault = false;
            for (int i = 1; i < _AllAction.Count + 1; i++)
            {
                int _id = i;
                if (_id < _AllAction.Count + 1)
                {
                    string _enumDescript = _AllAction[i - 1].EditorActionType;
                    m_AllActionName[_id] = $"{_enumDescript}/{_AllAction[i - 1].Name}";

                }

                if (!_isFindDefault)
                {
                    if (_AllAction[i - 1].Name == _actionEvent.ActionName)
                    {
                        _isFindDefault = true;
                        m_SelectDefaultActionState = _id;
                    }
                }
            }

            //没有找到【默认衔接】Action的序号的话就给“第一个动画”
            if (!_isFindDefault)
            {
                if (_AllAction.Count > 0)
                {
                    _actionEvent.ActionName = _AllAction[0].Name;
                }
                else
                {
                    _actionEvent.ActionName = string.Empty;
                }
                // Debug.Log("(interrup)没有找到默认衔接或为空");
                m_SelectDefaultActionState = 0;
            }

            needInit = false;
            return true;
        }
    }
}
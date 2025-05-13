using System;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class InspectorWindow
    {
        private bool InitActionEventGUI()
        {
            if (!needInit)
            {
                return true;
            }

            ActionEventDescripted.Instance.mIsInit = true;
            
            needInit = false;
            return true;
        }
        
        private void DrawActionEventGUI(EditorActionEvent _actionEvent)
        {
            if (!InitActionEventGUI())
            {
                return;
            }

            using (var _change = new EditorGUI.ChangeCheckScope())
            {
                _actionEvent.TriggerTime = EditorGUILayout.IntField($"触发时间: ", _actionEvent.TriggerTime);
                _actionEvent.Duration = EditorGUILayout.IntField($"持续时间: ", _actionEvent.Duration);
                if (_change.changed)
                {
                    if (_actionEvent.Duration == 0)
                    {
                        _actionEvent.TriggerTime = Mathf.Max(0, _actionEvent.TriggerTime);
                    }
                    TimeLineWindow.Instance.Repaint();
                }
            }
            GUILayout.Space(10);
            using (new GUILayout.HorizontalScope())
            {
                using (new GUIColorScope(Color.cyan))
                {
                    GUILayout.Label("Event:");
                }

                GUILayout.FlexibleSpace();
                bool _isDrawHelpWindows = ActionEventDescripted.HelpWindow(_actionEvent, out Action _drawCallback);
                using (new GUIColorScope(Color.green, _isDrawHelpWindows))
                {
                    if (GUILayout.Button("事件说明"))
                    {
                        // if (_isDrawHelpWindows)
                        ActionEventHelpWindow.Show(_drawCallback, _actionEvent);
                    }
                }
            }

            if (_actionEvent.EditorInheritable)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("继承事件参数: ",GUILayout.Width(80));
                    _actionEvent.Inheritable = EditorGUILayout.Toggle(_actionEvent.Inheritable);
                }
            }
            else
            {
                _actionEvent.Inheritable = false;
            }

            ActionEventDescripted.Instance.Draw(_actionEvent);
        }
    }
}
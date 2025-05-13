using System;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class ActionEventDescripted
    {
        public static bool HelpWindow(EditorActionEvent _actionEvent, out Action _drawCallback)
        {
            _drawCallback = null;
            
            IActionEventData _eventData = _actionEvent.EventData;
            if (_eventData == null)
            {
                GUILayout.Label("无法绘制，资源序列化失败");
                _drawCallback = null;
                return false;
            }
            
            if (_eventData.GetEvenType() < 0)
            {

                EEvenTypeInternal _GetEvenType = (EEvenTypeInternal)(-_eventData.GetEvenType());
                
                GUILayout.Space(10);
                switch (_GetEvenType)
                {
                    case EEvenTypeInternal.EET_DTD_PlayAnim:
                        _drawCallback = () => { };
                        break;
                    case EEvenTypeInternal.EET_AttackBox:
                        _drawCallback = () => { };
                        break;
                    default:
                        return false;
                }
            }
            else
            {
                return ActionWindowMain.ActionEditorFuntion.DrawEventHelpWindows(_actionEvent, out _drawCallback);
            }
            
            return true;
        }
    }
}
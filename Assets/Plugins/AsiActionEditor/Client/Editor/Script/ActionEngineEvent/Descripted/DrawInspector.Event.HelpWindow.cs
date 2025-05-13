using AsiActionEditor_Ex.RunTime;
using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using AsiTimeLine.RunTime;
using System;
using UnityEngine;

namespace AsiTimeLine.Editor
{
    public class DrawInspectorHelpWindow
    {
        public static bool DrawEventHelpWindows(EditorActionEvent _actionEvent, out Action _drawCallBack)
        {
            _drawCallBack = null;
            IActionEventData _eventData = _actionEvent.EventData;

            EEvenType _evenType = (EEvenType)_eventData.GetEvenType();
            
            //属性面板绘制
            switch (_evenType)
            {
                case EEvenType.EET_CameraChange:
                    DrawCameraChange((Event_CameraChange)_eventData);
                    break;
                
                case EEvenType.EET_Partocle:
                    break;
                
                case EEvenType.EET_SetAnimFloat:
                    break;
                case EEvenType.EET_UnitRot:
                    break;
                case EEvenType.EET_SetGValue:
                    break;
                case EEvenType.EET_RaycastHit:
                    break;
                default:
                    return false;

            }

            return true;
        }

        #region DrawFuntion

        private static void DrawCameraChange(Event_CameraChange _cameraChange)
        {
            
        }
        

        #endregion
    }
    
}
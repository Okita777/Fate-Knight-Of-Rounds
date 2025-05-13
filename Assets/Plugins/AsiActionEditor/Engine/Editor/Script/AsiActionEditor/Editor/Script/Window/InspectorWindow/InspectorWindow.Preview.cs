using AsiActionEngine.RunTime;
using UnityEditor;

namespace AsiActionEngine.Editor
{
    public partial class InspectorWindow
    {
        public IProperty CurSelectProperty => mSelectProperty;
        private IProperty mSelectProperty = null;
        private void DrawPerviewGUI()
        {
            if(mSelectProperty == null)return;
            if (mSelectProperty is EditorUnitWarp _unitWarp)
            {
                DrawUnitWarp(_unitWarp);
            }
            else if (mSelectProperty is ItemWarp _weaponWarp)
            {
                
            }            
            else if (mSelectProperty is EditorActionState _editorActionState)
            {
                using (var _chack = new EditorGUI.ChangeCheckScope())
                {
                    DrawActionState(_editorActionState);
                    if (_chack.changed)
                    {
                        ResourcesWindow.Instance.ActionOnChange();
                    }
                }
            }
            else if (mSelectProperty is EditorActionEvent _editorActionEvent)
            {
                using (var _chack = new EditorGUI.ChangeCheckScope())
                {
                    DrawActionEventGUI(_editorActionEvent);
                    if (_chack.changed)
                    {
                        ResourcesWindow.Instance.ActionOnChange();
                        ScenceDraw.Instance.OnUpdateDraw();
                    }
                }
            }
            else if (mSelectProperty is EditorActionInterrupt _editorActionInterrupt)
            {
                using (var _chack = new EditorGUI.ChangeCheckScope())
                {
                    DrawActionInterruptGUI(_editorActionInterrupt);
                    if (_chack.changed)
                    {
                        ResourcesWindow.Instance.ActionOnChange();
                        ScenceDraw.Instance.OnUpdateDraw();
                    }
                }
            }
            else if (mSelectProperty is EditorCameraWarp _cameraWarp)
            {
                DrawCameraWarpGUI(_cameraWarp);
            }
            else if (mSelectProperty is EditorEngineGValuePart _gValue)
            {
                using (var _chack = new EditorGUI.ChangeCheckScope())
                {
                    DrawGValueGUI(_gValue);
                    if (_chack.changed)
                    {
                        ResourcesWindow.Instance.GValueOnChange();
                    }
                }
            }
        }

        public void UpdateWindow()
        {
            DrawPerviewGUI();
            Repaint();
        }

    }
}
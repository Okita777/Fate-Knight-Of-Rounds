using System;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class BluePrintWindows_ReturnValue : EditorWindow
    {
        public static void OnShow(object _property, string _propertyName, string _nodeName)
        {
            if (_property is null)
            {
                EngineDebug.Log("初始化失败");
            }

            // if (CurrentObject is not null) UpdateEditorEvent();
            EdiKey = _propertyName;
            NodeName = _nodeName;
            
            CurrentObject = _property;
            DrawGraphEditorAttribute.Instance.Init(_propertyName);
            _nodeDisplay = DrawGraphEditorAttribute.Instance.CreactNode(_property);
            
            GetWindow<BluePrintWindows_ReturnValue>("蓝图编辑器");
        }
        
        public static string EdiKey = String.Empty;
        public static string NodeName = String.Empty;

        private static object CurrentObject = null;
        private static NodeDisplay _nodeDisplay;

        private GUIStyle centerGUI = new GUIStyle();
        private bool isUpdate = false;

        private Rect BodyRect;
        private void OnEnable()
        {
            centerGUI.alignment = TextAnchor.MiddleCenter;
            centerGUI.normal.textColor = Color.white;
        }

        private void OnDisable()
        {
            // UpdateEditorEvent();
            // EngineDebug.Log("数量: " + NodeDisPlay.EditorActionEvent.BluePrintPos.Count);
        }

        private void OnGUI()
        {
            //绘制大块界面
            float bodyWidth = valueFlid ? valueWidth : 0;
            BodyRect = new Rect(bodyWidth, headHeight, position.width - bodyWidth, position.height);
            DrawBluePrint(BodyRect);
            DrawValue(position);
            DrawHeadTool(position);
            
            //鼠标松开的事件
            if (Event.current.type == EventType.MouseUp)
            {
                callback_up?.Invoke();
                callback_up = null;
                callback_update = null;
                InteractType = EInteractType.Default;
            }
            
            isUpdate = callback_update is not null;
            
            //每帧刷新
            if (isUpdate)
            {
                callback_update();
                GUI.changed = true;
            }
        }
    }
}
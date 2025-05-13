using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.Graph;
using AsiActionEngine.RunTime.GraphVal;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class DrawGraphEditorAttribute
    {
        private static DrawGraphEditorAttribute _instance = null;

        public static DrawGraphEditorAttribute Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new DrawGraphEditorAttribute();
                    _instance.OnLoad();
                }
                return _instance;
            }
        }

        public object m_CloneBluePrint = null;
        public NodeEdiData _NodeEdiData = null;
        public string _eventTypeName = string.Empty;
        
        public void Init(string EdiKey)
        {
            NodesDataInit(EdiKey);
        }
        public void DrawGraphButton(string EdiKey, object obj, EditorPropertyAttribute epa)
        {
            int height = 45;
            float posY = EditorGUILayout.GetControlRect(GUILayout.Height(height)).y;
            Rect rect = new Rect(0, posY, InspectorWindow.Instance.position.width, height);
            DrawGraphButton(rect, EdiKey, obj, epa.PropertyName, epa.Tooltip);
            rect.y += 20;
            if (InspectorWindow.Instance.CurSelectProperty is EditorActionEvent _editorActionEvent)
            {
                if (_editorActionEvent.NodeEdiDataDic.TryGetValue(EdiKey, out NodeEdiData data))
                {
                    rect.height = 20;
                    rect.y += 2;
                    GUI.Label(rect, new GUIContent("  " + data.nodeTitle, data.nodeToolTip));
                }
                else
                {
                    _editorActionEvent.NodeEdiDataDic.Add(EdiKey, new NodeEdiData());
                }
            }

            // GUI.Label();
            // if (InspectorWindow.Instance.CurSelectProperty is EditorActionEvent _editorActionEvent)
            // {
            //     //获取当前所处的Rect
            //     float posY = EditorGUILayout.GetControlRect().y;
            //     int height = 20;
            //     Rect rect = new Rect(0, posY, InspectorWindow.Instance.position.width, height);
            //     rect.x += 2;
            //     rect.width -= 4;
            //     
            //     //背景绘制
            //     Rect breackRect = new Rect(rect);
            //     EditorGUI.DrawRect(breackRect, Color.black * 0.3f);
            //
            //     //头部绘制
            //     Rect headerRect = new Rect(rect);
            //     headerRect.height = 20;
            //     EditorGUI.DrawRect(headerRect, Color.black * 0.3f);
            //     GUI.Label(headerRect, new GUIContent(" " + epa.PropertyName,epa.Tooltip));
            //     headerRect.width = 120;
            //     headerRect.x += rect.width - headerRect.width;
            //     
            //     using (new GUIColorScope(Color.black * 0.3f))
            //     {
            //         if (GUI.Button(headerRect, "", EditorStyles.toolbarButton))
            //         {
            //             BluePrintWindows_ReturnValue.OnShow(obj, EdiKey);
            //         }
            //     }
            //     GUI.Label(headerRect, "   [ Open BluePrint ]");
            // }
            // else
            // {
            //     using (new GUILayout.HorizontalScope())
            //     {
            //         using (new GUIColorScope(Color.red))
            //         {
            //             GUILayout.Label(new GUIContent(" " + epa.PropertyName,epa.Tooltip),GUILayout.Width(epa.LabelWidth));
            //             GUILayout.Label(new GUIContent("出了一个不可能的错误","当前选中的对象不是事件轨道！！"));
            //         }
            //     }
            // }
        }

        public void DrawGraphButton(Rect _rect, string EdiKey, object obj, string disName, string tooltip)
        {
            if (InspectorWindow.Instance.CurSelectProperty is EditorActionEvent _editorActionEvent)
            {
                //获取当前所处的Rect
                Rect rect = new Rect(_rect);
                rect.x += 2;
                rect.width -= 4;

                //背景绘制
                Rect breackRect = new Rect(rect);
                EditorGUI.DrawRect(breackRect, Color.black * 0.3f);

                //头部绘制
                Rect headerRect = new Rect(rect);
                headerRect.height = 20;
                EditorGUI.DrawRect(headerRect, Color.black * 0.3f);
                GUI.Label(headerRect, new GUIContent(" " + disName, tooltip));
                headerRect.width = 120;
                headerRect.x += rect.width - headerRect.width;

                if (GUI.Button(headerRect, "", EditorStyles.toolbarButton))
                {
                    if (Event.current.button == 1)
                    {
                        NodeCloneMenu(obj, EdiKey);
                    }
                    else
                    {
                        BluePrintWindows_ReturnValue.OnShow(obj, EdiKey, disName);
                    }
                }

                EditorGUI.DrawRect(headerRect, Color.black * 0.7f);
                GUI.Label(headerRect, "   [ Open BluePrint ]");
            }
            else
            {
                using (new GUILayout.HorizontalScope())
                {
                    using (new GUIColorScope(Color.red))
                    {
                        GUILayout.Label(new GUIContent(" " + disName, tooltip));
                        GUILayout.Label(new GUIContent("出了一个不可能的错误", "当前选中的对象不是事件轨道！！"));
                    }
                }
            }
        }

        private object GetProperty(object obj, string propertyName)
        {
            return obj.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, obj, null);
        }

        private void SetProperty(object obj, string propertyName, object newValue)
        {
            if(obj is not null)
            obj.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, obj, new object[] { newValue });
        }

        private void NodeCloneMenu(object copyBluePrint, string ediKey)
        {
            GenericMenu _menu = new GenericMenu();
            
            // _menu.AddDisabledItem(new GUIContent("粘贴参数(未复制过)"));
            // _menu.AddItem(new GUIContent("删除节点"), false, () =>
            // {
            //     if (nodeDisplay.isReturn)
            //     {
            //         EditorUtility.DisplayDialog("警告", "此节点不可删除", "我知道了");
            //     }
            //     else
            //     {
            //         DrawGraphEditorAttribute.Instance.RemoveNode(nodeDisplay);
            //     }
            // });
            _menu.AddItem(new GUIContent("复制蓝图"), false, () =>
            {
                ActionSaveFlishEvent.Run();
                if (copyBluePrint is GraphEvent_NoValue_Bool _valueBool)
                {
                    m_CloneBluePrint = _valueBool.Clone();
                }else if (copyBluePrint is GraphEvent_NoValue_Float _valueFloat)
                {
                    m_CloneBluePrint = _valueFloat.Clone();

                }else if (copyBluePrint is GraphEvent_NoValue_Int _valueInt)
                {
                    m_CloneBluePrint = _valueInt.Clone();

                }else if (copyBluePrint is GraphEvent_NoValue_Vector3 _valueVector3)
                {
                    m_CloneBluePrint = _valueVector3.Clone();
                }else if (copyBluePrint is GraphEvent_NoValue_Point _valuePoint)
                {
                    m_CloneBluePrint = _valuePoint.Clone();
                }

                if (InspectorWindow.Instance.CurSelectProperty is EditorActionEvent _editorActionEvent)
                {
                    _NodeEdiData = _editorActionEvent.NodeEdiDataDic[ediKey];
                    _eventTypeName = _editorActionEvent.EventData.GetType().Name;
                }
            });
            
            if (m_CloneBluePrint is null)
            {
                _menu.AddDisabledItem(new GUIContent("粘贴蓝图(未复制过)"));
                _menu.AddDisabledItem(new GUIContent("粘贴到当前角色所有Action下的同事件轨(未复制过)"));
                _menu.AddDisabledItem(new GUIContent("粘贴到当前角色所有Action下的同事件轨且类型名的蓝图(未复制过)"));
            }
            else if(m_CloneBluePrint.GetType() != copyBluePrint.GetType())
            {
                _menu.AddDisabledItem(new GUIContent(m_CloneBluePrint.GetType().Name));
                _menu.AddDisabledItem(new GUIContent("粘贴蓝图(不同类型)"));
                _menu.AddDisabledItem(new GUIContent("粘贴到当前角色所有Action下的同事件轨(不同类型)"));
                _menu.AddDisabledItem(new GUIContent("粘贴到当前角色所有Action下的同事件轨且类型名的蓝图(不同类型)"));
            }
            else
            {
                _menu.AddItem(new GUIContent("粘贴蓝图"), false, () =>
                {
                    if (InspectorWindow.Instance.CurSelectProperty is EditorActionEvent _editorActionEvent)
                    {
                        _editorActionEvent.NodeEdiDataDic[ediKey].SetNewPos(_NodeEdiData);
                        SetProperty(_editorActionEvent.EventData, ediKey, GetNewBluePrint());
                    }
                });
                _menu.AddItem(new GUIContent("粘贴到当前角色所有Action下的同事件轨"), false, () =>
                {
                    PestToAllTrack(true, ediKey);

                });
                _menu.AddItem(new GUIContent("粘贴到当前角色所有Action下的同事件轨且同类型名的蓝图"), false, () =>
                {
                    PestToAllTrack(false, ediKey);

                });
            }
            
            _menu.ShowAsContext();
        }

        private object GetNewBluePrint()
        {
            ActionSaveFlishEvent.Run();
            // _actionEvent.NodeEdiDataDic[EdiKey].nodeTitle 
            if (m_CloneBluePrint is GraphEvent_NoValue_Bool _valueBool)
            {
                return _valueBool.Clone();
            }else if (m_CloneBluePrint is GraphEvent_NoValue_Float _valueFloat)
            {
                return _valueFloat.Clone();
            }else if (m_CloneBluePrint is GraphEvent_NoValue_Int _valueInt)
            {
                return _valueInt.Clone();
            }else if (m_CloneBluePrint is GraphEvent_NoValue_Vector3 _valueVector3)
            {
                return _valueVector3.Clone();
            }else if (m_CloneBluePrint is GraphEvent_NoValue_Point _valuePoint)
            {
                return _valuePoint.Clone();
            }
            EngineDebug.LogError("蓝图粘贴类型错误！！！！");
            return null;
        }

        private void PestToAllTrack(bool actionType, string ediKey)
        {
            //找到所有Action单位
            foreach (EditorActionState _actionState in ResourcesWindow.Instance.mActionState)
            {
                //找到当前Action下所有轨道组
                foreach (ActionTrackGroup _actionTrackGroup in _actionState.AllEventTrackGroup)
                {
                    //找到当前轨道组下所有轨道
                    foreach (IActionTrack _actionTrack in _actionTrackGroup.CurActiontTrack)
                    {
                        if (_actionTrack is EventTrack _eventTrack)
                        {
                            //找到所有事件单位
                            foreach (EventDisplay _eventDisplay in _eventTrack.CurEventDisplay)
                            {
                                EditorActionEvent _actionEvent = _eventDisplay.MainEvent;
                                //同类型事件
                                NodeEdiDataDic _dataDic = _actionEvent.NodeEdiDataDic;
                                if (_actionEvent.EventData.GetType().Name == _eventTypeName)
                                {
                                    if (actionType)
                                    {
                                        if (_dataDic.TryGetValue(ediKey, out NodeEdiData _data))
                                        {
                                            if (_data.nodeTitle == _NodeEdiData.nodeTitle)
                                            {
                                                _data.nodeToolTip = _NodeEdiData.nodeToolTip;
                                                _data.SetNewPos(_NodeEdiData);
                                            }
                                        }
                                        else
                                        {
                                            _dataDic.Add(ediKey, _NodeEdiData.Clone());
                                        }
                                        SetProperty(_actionEvent.EventData, ediKey, GetNewBluePrint());
                                    }
                                    else
                                    {
                                        if (_dataDic.TryGetValue(ediKey, out NodeEdiData _nodeEdiData))
                                        {
                                            if (_nodeEdiData.nodeTitle == _NodeEdiData.nodeTitle)
                                            {
                                                _nodeEdiData.SetNewPos(_NodeEdiData);
                                                SetProperty(_actionEvent.EventData, ediKey, GetNewBluePrint());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
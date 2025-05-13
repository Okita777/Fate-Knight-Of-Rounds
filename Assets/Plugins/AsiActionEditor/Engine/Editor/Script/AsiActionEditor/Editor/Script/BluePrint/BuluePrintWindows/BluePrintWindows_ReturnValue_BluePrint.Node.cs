using System;
using System.Collections.Generic;
using System.Reflection;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.Graph;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class BluePrintWindows_ReturnValue
    {

        
        private object selectObject = null;
        private NodeDisplay SelecteNodeDisplay = null;
        private object SetObject = null;
        private NodeDisplayValue _SetObjectValue = null;
        private string SetObjectVal = String.Empty;
        private int objectValIndex = 1;

        private void DrawBluePrintNode(NodeDisplay nodeDisplay)
        {
            int nodeKey = nodeDisplay.Main.GetHashCode();
            
            //获取宽高
            float height = 20;
            float width = 180;
            foreach (NodeDisplayValue VARIABLE in nodeDisplay.NodeValues)
            {
                float mWidth = VARIABLE.drawValData.LabelWidth + 160;
                if (mWidth > width)
                {
                    width = mWidth;
                }
                height += VARIABLE.height;
            }

            height = Mathf.Max(height, 40);

            //todo: 靠editor储存的节点位置获取
            Vector2 nodePos = Vector2.zero;
            if (!DrawGraphEditorAttribute.Instance.node_Vector2_pos.TryGetValue(nodeKey, out nodePos))
            {
                EngineDebug.LogError("不可能没有啊");
            }
            
            //节点位置
            Vector2 nodeCenterPos = nodePos + viewcenterPos - new Vector2(width / 2, height / 2);
            
            //节点背景
            int tooltipHeight = (nodeDisplay.isReturn ? 150 : 0);
            Rect nodeRect = new Rect(nodeCenterPos.x, nodeCenterPos.y, width, height + tooltipHeight);
            Color color = Color.white * 0.2f;
            color.a = 1;
            EditorGUI.DrawRect(nodeRect, color);
            
            //节点头部
            Rect nodeHeadRect = new Rect(nodeRect);
            nodeHeadRect.height = 20;
            EditorGUI.DrawRect(nodeHeadRect, new Color(0,0.4f,0.6f,1));
            
            //输出的引脚
            if (nodeDisplay.isReturn)
            {
                GUI.Label(nodeHeadRect, BluePrintEvent.DisName(nodeDisplay.Main.GetType().Name)+ $"   [ {NodeName} ]", centerGUI);

                Rect tooltil = new Rect(nodeRect);
                tooltil.height = 20;
                tooltil.y-= tooltil.height;
                tooltil.x += 5;
                tooltil.width = 80;
                EditorGUI.DrawRect(tooltil, Color.black * 0.2f);
                GUI.Label(tooltil,"   [最终输出]");

                tooltil.y = nodeRect.y + nodeRect.height - 140;
                tooltil.height = 20;
                tooltil.width = 70;
                GUI.Label(tooltil, "蓝图类型名: ");
                tooltil.x += tooltil.width;
                tooltil.width = nodeRect.width - 80;
                EditorActionEvent _actionEvent = DrawGraphEditorAttribute.Instance.nowEditorActionEvent;
                _actionEvent.NodeEdiDataDic[EdiKey].nodeTitle = GUI.TextField(tooltil, _actionEvent.NodeEdiDataDic[EdiKey].nodeTitle);
                Rect tooltip = new Rect(nodeRect.x + 5,tooltil.y + 25,nodeRect.width - 10,110);
                _actionEvent.NodeEdiDataDic[EdiKey].nodeToolTip =
                    EditorGUI.TextArea(tooltip, _actionEvent.NodeEdiDataDic[EdiKey].nodeToolTip);
                // _actionEvent.NodeEdiDataDic[EdiKey].nodeToolTip = GUI.TextField(tooltip, _actionEvent.NodeEdiDataDic[EdiKey].nodeToolTip);

            }
            else
            {
                GUI.Label(nodeHeadRect, BluePrintEvent.DisName(nodeDisplay.Main.GetType().Name), centerGUI);

                Rect tooltil = new Rect(nodeRect);
                tooltil.x += tooltil.width;
                tooltil.y += 24;
                tooltil.width = 8;
                tooltil.height = 12;
                // color = (tooltil.Contains(Event.current.mousePosition) ? Color.blue : Color.white) * 0.6f;
                color = Color.white * 0.6f;
                EditorGUI.DrawRect(tooltil, color);
                if (InteractType == EInteractType.Default)
                {
                    Handl_Head_CallBack(tooltil, nodeDisplay);
                    EditorGUIUtility.AddCursorRect(tooltil, MouseCursor.Link);
                }

                if (InteractType == EInteractType.Handle_Foot)
                {
                    if (GetValIsType(nodeDisplay.Main))
                    {
                        EditorGUIUtility.AddCursorRect(tooltil, MouseCursor.Link);
                        if (tooltil.Contains(Event.current.mousePosition))
                        {
                            selectObject = nodeDisplay.Main;
                            SelecteNodeDisplay = nodeDisplay;
                        }
                    }
                    else
                    {
                        EditorGUI.DrawRect(nodeRect, Color.black * 0.3f);
                    }
                }

                //添加引线起点
                Vector2 _startPos = tooltil.position;
                _startPos.x += tooltil.width;
                _startPos.y += tooltil.height / 2;
                nodeDisplay._handl_Head_Pos = _startPos;
            }
            
            //输入的成员和引脚
            DrawNodeValueGUI(nodeHeadRect, nodeDisplay);

            if (!nodeDisplay.isReturn)
            {
                HeadCallBack(nodeRect, nodeKey, nodeDisplay);
                if (InteractType == EInteractType.Default)
                    EditorGUIUtility.AddCursorRect(nodeRect, MouseCursor.MoveArrow);
            }
        }
        
        private object GetProperty(object obj, string propertyName)
        {
            return obj.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, obj, null);
        }

        private void SetProperty(object obj, string propertyName, object newValue)
        {
            obj.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, obj, new object[] { newValue });
        }

        private void HeadCallBack(Rect _rect, int _nodeKey, NodeDisplay nodeDisplay)
        {
            InteractRect(_rect, () =>
            {
                ResourcesWindow.Instance.ActionOnChange(true,"蓝图变动");
                //节点拖拽
                callback_update = () =>
                {
                    // if (Event.current.type == EventType.DragUpdated)
                        DrawGraphEditorAttribute.Instance.node_Vector2_pos[_nodeKey] += Event.current.delta * 0.5f;
                };
                //右键菜单
                callback_up = () =>
                {
                    if (Event.current.button == 1 && (viewStartPos - viewPos).sqrMagnitude < 2)
                    {
                        CreactDefaultNodeMenu(nodeDisplay, DrawGraphEditorAttribute.Instance.node_Vector2_pos[_nodeKey]);
                        // EngineDebug.Log("右键菜单"); 
                    }
                };
            });
        }
        
        //引脚头部交互标记
        private void Handl_Head_CallBack(Rect _rect, NodeDisplay _node)
        {
            InteractRect(_rect, () =>
            {
                SelecteNodeDisplay = _node;
                selectObject = _node.Main;
                SetValType(selectObject);
                InteractType = EInteractType.Handle_Head;
                // ResourcesWindow.Instance.ActionOnChange(true,"蓝图变动");
                callback_update = () =>
                {
                    bluePrintLines.Add(new BluePrintLine(Vector2.zero, GetMousePos(), selectObject.GetHashCode(),objectValIndex));
                    //重置被设置的蓝图参数
                    SetObject = null;
                };
                
                //设置参数
                callback_up = () =>
                {
                    if (SetObject is not null)
                    {
                        SetProperty(SetObject, SetObjectVal, selectObject);
                        ResourcesWindow.Instance.ActionOnChange(true,"蓝图变动");
                        _SetObjectValue._inputTarget = SelecteNodeDisplay;
                    }
                    else
                    {
                        CreateNodeMenu_Out(_node, GetNodePosToPos(Event.current.mousePosition));
                    }
                };
            });
        }
        
        //引脚交互标记
        private void Handl_Foot_CallBack(Rect _rect, NodeDisplayValue _nodeValue, object _nodeMain)
        {
            InteractRect(_rect, () =>
            {
                object _obj = GetProperty(_nodeMain, _nodeValue.valName);
                if (_nodeValue._inputTarget is not null && _obj is not null)
                {
                    int _nodeKey = _obj.GetHashCode();
                    callback_update = () =>
                    {
                        bluePrintLines.Add(new BluePrintLine(Vector2.zero, GetMousePos(), _nodeKey,objectValIndex));
                        //重置被设置的蓝图参数
                        SetObject = null;
                    };
                    
                    selectObject = _obj;
                    // EngineDebug.Log($"  Name: {selectObject.GetType().Name}");
                    SetValType(selectObject);
                    InteractType = EInteractType.Handle_Head;

                    _nodeValue._inputTarget = null;
                    SetProperty(_nodeMain, _nodeValue.valName, null);
                    ResourcesWindow.Instance.ActionOnChange(true,"蓝图变动");
                    
                    //设置参数
                    callback_up = () =>
                    {
                        if (SetObject is not null)
                        {
                            SetProperty(SetObject, SetObjectVal, selectObject);
                            ResourcesWindow.Instance.ActionOnChange(true,"蓝图变动");
                            _SetObjectValue._inputTarget = SelecteNodeDisplay;
                        }
                        // else
                        // {
                        //     CreateNodeMenu_Out(SelecteNodeDisplay, GetNodePosToPos(Event.current.mousePosition));
                        // }

                    };
                }
                else
                {
                    selectObject = null;
                    SetObject = _nodeMain;
                    SetObjectVal = _nodeValue.valName;
                    SetValType(GetProperty(SetObject, _nodeValue.valName));
                    InteractType = EInteractType.Handle_Foot;

                    Vector2 startPos = _rect.position;
                    startPos.y += _rect.height / 2;
                    callback_update = () =>
                    {
                        bluePrintLines.Add(new BluePrintLine(GetMousePos(), startPos, 0,objectValIndex));
                        //重置被设置的蓝图参数
                        selectObject = null;
                    };
                    
                    //设置参数
                    callback_up = () =>
                    {
                        if (selectObject is not null)
                        {
                            SetProperty(SetObject, SetObjectVal, selectObject);
                            _SetObjectValue._inputTarget = SelecteNodeDisplay;
                            ResourcesWindow.Instance.ActionOnChange(true,"蓝图变动");
                        }
                        else
                        {
                            CreateNodeMenu_Input(_nodeValue, GetNodePosToPos(Event.current.mousePosition));
                        }
                    };
                }
            });
        }

        private Vector2 GetMousePos()
        {
            Vector2 newPos = Event.current.mousePosition;
            newPos.x -= valueFlid ? valueWidth : 0;
            newPos.y -= headHeight;
            return newPos;
        }

        private Vector2 GetNodePosToPos(Vector2 newPos)
        {
            newPos.x -= (valueFlid ? valueWidth : 0) + BodyRect.width/2;
            newPos.y -= headHeight + BodyRect.height/2;
            newPos -= viewPos;
            return newPos;
        }

        #region 引脚创建节点
        List<string> _disNodes = new List<string>();//DrawGraphEditorAttribute.Instance

        private void CreateNodeMenu_Input(NodeDisplayValue _nodeDisplay, Vector2 _pos)
        {
            GenericMenu _menu = new GenericMenu();
            
            object obj = GetProperty(_nodeDisplay.Main, _nodeDisplay.valName);
            if (obj is BluePrint_Bool) _disNodes = DrawGraphEditorAttribute.Instance.m_OutType_Bool;
            else if (obj is BluePrint_Int) _disNodes = DrawGraphEditorAttribute.Instance.m_OutType_Int;
            else if (obj is BluePrint_Float) _disNodes = DrawGraphEditorAttribute.Instance.m_OutType_Float;
            else if (obj is BluePrint_Unit) _disNodes = DrawGraphEditorAttribute.Instance.m_OutType_Unit;
            else if (obj is BluePrint_Transform) _disNodes = DrawGraphEditorAttribute.Instance.m_OutType_Transform;
            else if (obj is BluePrint_PointData) _disNodes = DrawGraphEditorAttribute.Instance.m_OutType_Point;
            else if (obj is BluePrint_Vector3) _disNodes = DrawGraphEditorAttribute.Instance.m_OutType_Vector3;
            foreach (string _nodeName in _disNodes)
            {
                string[] names = _nodeName.Split('&');
                _menu.AddItem(new GUIContent(names[1]), false, () =>
                {
                    IProperty _node = BluePrintEvent.Creact(names[0]);
                    if (_node is BluePrint_Value _printValue) _printValue.IsNode = true;
                    int hashCode = _node.GetHashCode();
                    SetProperty(_nodeDisplay.Main, _nodeDisplay.valName, _node);
                    _nodeDisplay._inputTarget = DrawGraphEditorAttribute.Instance.CreactNode(_node);
                    DrawGraphEditorAttribute.Instance.node_Vector2_pos[hashCode] = _pos;
                });
            }
            
            _menu.ShowAsContext();
        }
        
        private void CreateNodeMenu_Out(NodeDisplay _nodeDisplay, Vector2 _pos)
        {
            GenericMenu _menu = new GenericMenu();
            
            object obj = _nodeDisplay.Main;
            if (obj is BluePrint_Bool) _disNodes = DrawGraphEditorAttribute.Instance.m_InputType_Bool;
            else if (obj is BluePrint_Int)
            {
                _disNodes = DrawGraphEditorAttribute.Instance.m_InputType_Int;
                EngineDebug.Log($"尝试创建Int({_disNodes.Count}):  {obj.GetType().Name}");
            }
            else if (obj is BluePrint_Float) _disNodes = DrawGraphEditorAttribute.Instance.m_InputType_Float;
            else if (obj is BluePrint_Unit) _disNodes = DrawGraphEditorAttribute.Instance.m_InputType_Unit;
            else if (obj is BluePrint_Transform) _disNodes = DrawGraphEditorAttribute.Instance.m_InputType_Transform;
            else if (obj is BluePrint_PointData) _disNodes = DrawGraphEditorAttribute.Instance.m_InputType_Point;
            else if (obj is BluePrint_Vector3) _disNodes = DrawGraphEditorAttribute.Instance.m_InputType_Vector3;
            foreach (string _nodeName in _disNodes)
            {
                string[] names = _nodeName.Split('&');
                _menu.AddItem(new GUIContent(names[1]), false, () =>
                {
                    IProperty _node = BluePrintEvent.Creact(names[0]);
                    if (_node is BluePrint_Value _printValue) _printValue.IsNode = true;
                    int hashCode = _node.GetHashCode();
                    // SetProperty(_nodeDisplay.Main, _nodeDisplay.valName, _node);
                    NodeDisplay _mNodeDisplay = DrawGraphEditorAttribute.Instance.CreactNode(_node);
                    DrawGraphEditorAttribute.Instance.node_Vector2_pos[hashCode] = _pos;
                    
                    //设置参数
                    // EngineDebug.LogWarning("设置过参数了哦");
                    foreach (NodeDisplayValue VARIABLE in _mNodeDisplay.NodeValues)
                    {
                        if (VARIABLE.drawValData.IsInput)
                        {
                            VARIABLE._inputTarget = _mNodeDisplay;
                            break;
                        }
                    }
                    SetProperty(_mNodeDisplay.Main, names[2], _nodeDisplay.Main);
                });
            }
            _menu.ShowAsContext();
        }
        #endregion
    }
}
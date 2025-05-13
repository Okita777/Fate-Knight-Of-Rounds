using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.Graph;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class BluePrintWindows_ReturnValue
    {
        private Vector2 viewPos = Vector2.zero;
        private Vector2 viewcenterPos;
        private Action callback_up;
        private Action callback_update;
        
        private Vector2 viewStartPos = Vector2.zero;
        private void DrawBluePrint(Rect rect)
        {
            //绘制背景
            // float width = valueFlid ? valueWidth : 0;
            Rect mainRect = new Rect(rect);
            // EditorGUIUtility.AddCursorRect(mainRect, MouseCursor.Pan);

            //当前位置
            viewcenterPos = viewPos + new Vector2(mainRect.width / 2, mainRect.height / 2);

            using (new GUI.GroupScope(mainRect))
            {
                mainRect.position = Vector2.one * 2;
                BluePrintView.Instance.Background(mainRect, viewcenterPos, 1);
                
                // //绘制节点位置
                // string nodeName = string.Empty;
                // foreach (var VARIABLE in DrawGraphEditorAttribute.Instance.All_NodeDisPlay)
                // {
                //     int hashcode = VARIABLE.Main.GetHashCode();
                //     nodeName += $"  \n Name:{VARIABLE.Main.GetType().Name}   Pos:{DrawGraphEditorAttribute.Instance.node_Vector2_pos[hashcode]}";
                // }
                // GUI.Label(mainRect, nodeName);

                if (CurrentObject is null)
                {
                    Rect errorRect = BluePrintView.Instance.DrawNode(new Vector2(100, 50), Vector2.zero, viewcenterPos);
                    EditorGUI.DrawRect(errorRect,Color.red * 0.8f);
                    GUI.Label(errorRect, "节点丢失了！！！");
                }
                else
                {
                    //让所有的线找到端点
                    for (int i = 0; i < bluePrintLines.Count; i++)
                    {
                        BluePrintLine VARIABLE = bluePrintLines[i];

                        foreach (NodeDisplay _node in DrawGraphEditorAttribute.Instance.All_NodeDisPlay)
                        {
                            if (VARIABLE.inputTarget == _node.Main.GetHashCode())
                            {
                                bluePrintLines[i] = VARIABLE.SetStartPos(_node._handl_Head_Pos);
                                break;
                            }
                        }
                    }
                    
                    //绘制所有的线
                    foreach (BluePrintLine VARIABLE in bluePrintLines)
                    {
                        VARIABLE.Draw(Color.white * 1);
                    }
                    bluePrintLines.Clear();
                    List<NodeDisplay> nodeDisPlayList = DrawGraphEditorAttribute.Instance.All_NodeDisPlay;
                    foreach (NodeDisplay VARIABLE in nodeDisPlayList)
                    {
                        DrawBluePrintNode(VARIABLE);
                    }
                    
                    //按ID顺序储存好位置
                    EditorActionEvent _actionEvent = DrawGraphEditorAttribute.Instance.nowEditorActionEvent;
                    _actionEvent.NodeEdiDataDic[EdiKey].nodePositions.Clear();
                    
                    hashCode.Clear();
                    AddPos(_nodeDisplay);
                    
                    // //绘制正式节点位置
                    // string nodeName = string.Empty;
                    // foreach (var VARIABLE in _actionEvent.NodeEdiDataDic[EdiKey].nodePositions)
                    // {
                    //     nodeName += $"  \n Pos:{VARIABLE}";
                    // }
                    // GUI.Label(mainRect, nodeName);
                }
                
                //背景拖拽
                InteractRect(mainRect, () =>
                {
                    //拖拽视图
                    callback_update = () =>
                    {
                        // if (Event.current.type == EventType.DragUpdated)
                            viewPos += Event.current.delta * 0.5f;
                        EditorGUIUtility.AddCursorRect(mainRect, MouseCursor.Pan);
                    };
                    callback_up = () =>
                    {
                        if (Event.current.button == 1 && (viewStartPos - viewPos).sqrMagnitude < 2)
                        {
                            // EngineDebug.Log("右键菜单"); 
                        }
                    };
                });
            }
        }

        private void InteractRect(Rect rect, Action callback_down)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    viewStartPos = viewPos;

                    callback_up = null;
                    callback_update = null;
                    callback_down?.Invoke();
                    Event.current.Use();
                }
            }
        }

        private List<int> hashCode = new List<int>();
        private void AddPos(NodeDisplay nodeDisplay)
        {
            if (nodeDisplay is not null)
            {
                hashCode.Add(nodeDisplay.GetHashCode());

                if (DrawGraphEditorAttribute.Instance.node_Vector2_pos.TryGetValue(nodeDisplay.Main.GetHashCode(),
                        out Vector2 nodePos))
                {
                    EditorActionEvent _actionEvent = DrawGraphEditorAttribute.Instance.nowEditorActionEvent;
                    if (!_actionEvent.NodeEdiDataDic[EdiKey].nodePositions.Contains(nodePos))
                        _actionEvent.NodeEdiDataDic[EdiKey].nodePositions.Add(nodePos);
                }

                foreach (NodeDisplayValue VARIABLE in nodeDisplay.NodeValues)
                {
                    if (VARIABLE.drawValData.IsInput)
                    {
                        // bool isValid = GetProperty(nodeDisplay.Main, VARIABLE.valName) is not null;
                        bool isValid = VARIABLE._inputTarget is not null;
                        
                        if (isValid)
                        {
                            //避免重复添加
                            if (!hashCode.Contains(VARIABLE._inputTarget.GetHashCode()))
                                AddPos(VARIABLE._inputTarget);
                        }
                    }
                }
            }
        }

        private void CreactDefaultNodeMenu(NodeDisplay nodeDisplay, Vector2 pos)
        {
            GenericMenu _menu = new GenericMenu();
            // _menu.AddItem(new GUIContent("复制事件"), false, () => { });
            // _menu.AddDisabledItem(new GUIContent("粘贴参数(未复制过)"));
            _menu.AddItem(new GUIContent("删除节点"), false, () =>
            {
                if (nodeDisplay.isReturn)
                {
                    EditorUtility.DisplayDialog("警告", "此节点不可删除", "我知道了");
                }
                else
                {
                    DrawGraphEditorAttribute.Instance.RemoveNode(nodeDisplay);
                }
            });

            string[] _graphType = nodeDisplay.graph_names;
            Action<string> valType = null;
            if (_graphType[2] == "Add")
            {
                valType = (string name) =>
                {
                    IProperty _node = BluePrintEvent.Creact($"GraphEvent_Math_Add_{name}");
                    int hashCode = _node.GetHashCode();
                    DrawGraphEditorAttribute.Instance.CreactNode(_node);
                    DrawGraphEditorAttribute.Instance.node_Vector2_pos[hashCode] = pos;
                };
            }else if (_graphType[2] == "Sub")
            {
                valType = (string name) =>
                {
                    IProperty _node = BluePrintEvent.Creact($"GraphEvent_Math_Sub_{name}");
                    int hashCode = _node.GetHashCode();
                    DrawGraphEditorAttribute.Instance.CreactNode(_node);
                    DrawGraphEditorAttribute.Instance.node_Vector2_pos[hashCode] = pos;
                };
            }else if (_graphType[2] == "Mul")
            {
                valType = (string name) =>
                {
                    IProperty _node = BluePrintEvent.Creact($"GraphEvent_Math_Mul_{name}");
                    int hashCode = _node.GetHashCode();
                    DrawGraphEditorAttribute.Instance.CreactNode(_node);
                    DrawGraphEditorAttribute.Instance.node_Vector2_pos[hashCode] = pos;
                };
            }else if (_graphType[2] == "Div")
            {
                valType = (string name) =>
                {
                    IProperty _node = BluePrintEvent.Creact($"GraphEvent_Math_Div_{name}");
                    int hashCode = _node.GetHashCode();
                    DrawGraphEditorAttribute.Instance.CreactNode(_node);
                    DrawGraphEditorAttribute.Instance.node_Vector2_pos[hashCode] = pos;
                };
            }
            if (valType is not null)
            {
                _menu.AddItem(new GUIContent("切换节点至/Float"), false, () =>
                {
                    DrawGraphEditorAttribute.Instance.RemoveNode(nodeDisplay);
                    valType("Float");
                });
                _menu.AddItem(new GUIContent("切换节点至/Int"), false, () =>
                {
                    DrawGraphEditorAttribute.Instance.RemoveNode(nodeDisplay);
                    valType("Int");
                });
                _menu.AddItem(new GUIContent("切换节点至/Vector3"), false, () =>
                {
                    DrawGraphEditorAttribute.Instance.RemoveNode(nodeDisplay);
                    valType("Vector3");
                });
            }
            
            _menu.AddItem(new GUIContent("编辑节点脚本"), false, () =>
            {
                string _path = "";
                string[] _guid = AssetDatabase.FindAssets(nodeDisplay.Main.GetType().Name);
                string[] _nowPath = AssetDatabase.GUIDToAssetPath(_guid[0]).Split('/');
                for (int i = 0; i < _nowPath.Length-1; i++)
                    _path += (_nowPath[i] + "/");
                _path += _nowPath[^1];
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(_path, 10);
            });
            _menu.ShowAsContext();
        }
    }
}
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.Graph;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class BluePrintWindows_ReturnValue
    {
        private Vector2 valueViewScrollPosition = Vector2.zero;
        private Rect _char;
        private Color _char_color = Color.white * 0.5f;

        //节点创建相关
        private Rect _CreateNode;
        private string _CreateNodeDisplayName = "";
        private string _CreateNodeName = "";

        private void DrawValue(Rect _rect)
        {
            Rect rectMain = new Rect(0, headHeight, valueWidth, _rect.height-headHeight);
            EditorGUI.DrawRect(rectMain, Color.black * 0.1f);

            Rect body = new Rect(rectMain);
            //蓝图资产高度
            DrawGraphEditorAttribute _graph = DrawGraphEditorAttribute.Instance;
            List<string> _node_Math = _graph.m_BlueprintEvents_Math;
            float height = (_node_Math.Count > 0 && _graph.m_Fold_Math ? (_node_Math.Count * 20) : 0) + 30;
            List<string> _node_Value = _graph.m_BlueprintEvents_Value;
            height += (_node_Value.Count > 0 && _graph.m_Fold_Value ? (_node_Value.Count * 20) : 0) + 30;
            List<string> _node_GValue = _graph.m_BlueprintEvents_GValue; 
            height += (_node_GValue.Count > 0 && _graph.m_Fold_GValue ? (_node_GValue.Count * 20) : 0) + 30;
            List<string> _node_TrackData = _graph.m_BlueprintEvents_TrackData;
            height += (_node_TrackData.Count > 0 && _graph.m_Fold_TrackData ? (_node_TrackData.Count * 20) : 0) + 30;
            // List<string> _node_NoValue = _graph.m_BlueprintEvents_NoValue;
            // height += (_node_NoValue.Count > 0 && _graph.m_Fold_NoValue ? (_node_NoValue.Count * 20) : 0) + 30;
            List<string> _node_Other = _graph.m_BlueprintEvents_Other;
            height += (_node_Other.Count > 0 && _graph.m_Fold_Other ? (_node_Other.Count * 20 + 30) : 0);
            
            List<string> _node_NotValid = _graph.m_BlueprintEvents_NotValid;
            height += (_node_NotValid.Count > 0 ? (_node_NotValid.Count * 20  + 30) : 0);
            body.height = height;
            // body.x = 2000;
            // body.width -= 4;
            body.width -= body.height > rectMain.height ? 15 : 0;
            using (var _scroll = new GUI.ScrollViewScope(rectMain, valueViewScrollPosition, body, false, false))
            {
                valueViewScrollPosition = _scroll.scrollPosition;
                
                body.x += 3;
                body.width -= 6;
                
                _char = new Rect(body);
                _char.x += 3;
                _char.width -= 6;
                _char.height = 18;
                
                //绘制运算用的蓝图节点
                body.y += 10;
                body.height = 20;
                if (GUI.Button(body, "")) _graph.m_Fold_Math = !_graph.m_Fold_Math;
                EditorGUI.DrawRect(body, Color.black * (_graph.m_Fold_Math ? 0.7f : 0.5f));
                GUI.Label(body, "运算类", centerGUI);
                if (_graph.m_Fold_Math && _node_Math.Count > 0)
                {
                    body.y += body.height;
                    body.height = _node_Math.Count * 20 + 2;
                    EditorGUI.DrawRect(body, Color.black * 0.2f);

                    DrawChars(_node_Math, body.y + 2);
                }

                //绘制常量蓝图节点
                body.y += body.height + 10;
                body.height = 20;
                if (GUI.Button(body, "")) _graph.m_Fold_Value = !_graph.m_Fold_Value;
                EditorGUI.DrawRect(body, Color.black * (_graph.m_Fold_Value ? 0.7f : 0.5f));
                GUI.Label(body, "常量", centerGUI);
                if (_graph.m_Fold_Value && _node_Value.Count > 0)
                {
                    body.y += body.height;
                    body.height = _node_Value.Count * 20 + 2;
                    EditorGUI.DrawRect(body, Color.black * 0.2f);
                    
                    DrawChars(_node_Value, body.y + 2);
                }
                
                //绘制常量蓝图节点
                body.y += body.height + 10;
                body.height = 20;
                if (GUI.Button(body, "")) _graph.m_Fold_GValue = !_graph.m_Fold_GValue;
                EditorGUI.DrawRect(body, Color.black * (_graph.m_Fold_GValue ? 0.7f : 0.5f));
                GUI.Label(body, "常量 (GValue)", centerGUI);
                if (_graph.m_Fold_GValue && _node_GValue.Count > 0)
                {
                    body.y += body.height;
                    body.height = _node_GValue.Count * 20 + 2;
                    EditorGUI.DrawRect(body, Color.black * 0.2f);
                    
                    DrawChars(_node_GValue, body.y + 2);
                }
                
                //绘制轨道参数蓝图节点
                body.y += body.height + 10;
                body.height = 20;
                if (GUI.Button(body, "")) _graph.m_Fold_TrackData = !_graph.m_Fold_TrackData;
                EditorGUI.DrawRect(body, Color.black * (_graph.m_Fold_TrackData ? 0.7f : 0.5f));
                GUI.Label(body, "事件轨数据", centerGUI);
                if (_graph.m_Fold_TrackData && _node_TrackData.Count > 0)
                {
                    body.y += body.height;
                    body.height = _node_TrackData.Count * 20 + 2;
                    EditorGUI.DrawRect(body, Color.black * 0.2f);
                    
                    DrawChars(_node_TrackData, body.y + 2);
                }
                
                //绘制其它蓝图
                if (_node_Other.Count > 0)
                {
                    body.y += body.height + 10;
                    body.height = 20;
                    if (GUI.Button(body, "")) _graph.m_Fold_Other = !_graph.m_Fold_Other;
                    EditorGUI.DrawRect(body, Color.black * (_graph.m_Fold_Other ? 0.7f : 0.5f));
                    GUI.Label(body, "其它", centerGUI);
                    if (_graph.m_Fold_Other)
                    {
                        body.y += body.height;
                        body.height = _node_Other.Count * 20 + 2;
                        EditorGUI.DrawRect(body, Color.black * 0.2f);

                        DrawChars(_node_Other, body.y + 2);
                    }
                }
                
                
                //Error蓝图
                if (_node_NotValid.Count > 0)
                {
                    body.y += body.height + 10;
                    body.height = 20;
                    EditorGUI.DrawRect(body, Color.red * 0.7f);
                    GUI.Label(body, "未实现实例化的蓝图", centerGUI);
                    body.y += body.height;
                    body.height = _node_NotValid.Count * 20 + 2;
                    EditorGUI.DrawRect(body, Color.red * 0.2f);
                    Rect chil = new Rect(body);
                    chil.height = 18;
                    chil.y += 2;
                    chil.x += 4;
                    chil.width -= 8;
                    for (int i = 0; i < _node_NotValid.Count; i++)
                    {
                        string VARIABLE = _node_NotValid[i];

                        if (GUI.Button(chil, ""))
                        {
                            Object select = 
                                AssetDatabase.LoadAssetAtPath<TextAsset>(_graph.m_BlueprintEvents_Path_NotValid[i]);
                            EditorGUIUtility.PingObject(select);
                            Selection.activeObject = select;
                        }
                        EditorGUI.DrawRect(chil,Color.red * 0.2f);
                        GUI.Label(chil, VARIABLE, centerGUI);
                        chil.y += 20;
                    }
                }
            }
        }

        private void DrawChars(List<string> _nodeNameGroup, float _startPosY)
        {
            _char.y = _startPosY;
            foreach (string _nodeName in _nodeNameGroup)
            {
                EditorGUI.DrawRect(_char, _char_color);
                GUI.Label(_char, BluePrintEvent.DisName(_nodeName), centerGUI); 
                EditorGUIUtility.AddCursorRect(_char, MouseCursor.MoveArrow);
                
                InteractRect(_char, () =>
                {
                    _CreateNode = new Rect(_char);
                    _CreateNodeDisplayName = BluePrintEvent.DisName(_nodeName);
                    _CreateNodeName = _nodeName;
                    InteractType = EInteractType.Handle_Head;
                    
                    //每帧绘制要创建的节点
                    callback_update = () =>
                    {
                        EditorGUI.DrawRect(_CreateNode, _char_color);
                        GUI.Label(_CreateNode, _CreateNodeDisplayName, centerGUI);

                        _CreateNode.position += Event.current.delta * 0.5f;
                    };

                    //松开时创建节点
                    callback_up = () =>
                    {
                        Vector2 mousePos = Event.current.mousePosition;
                        if (BodyRect.Contains(mousePos))
                        {
                            if (CurrentObject is not null)
                            {
                                IProperty _node = BluePrintEvent.Creact(_CreateNodeName);
                                if (_node is BluePrint_Value _printValue) _printValue.IsNode = true;
                                int hashCode = _node.GetHashCode();
                                DrawGraphEditorAttribute.Instance.CreactNode(_node);
                                DrawGraphEditorAttribute.Instance.node_Vector2_pos[hashCode] = GetNodePosToPos(mousePos);
                                // EngineDebug.Log($"创建名为 [{_CreateNodeDisplayName}] 的节点, 节点数量: {DrawGraphEditorAttribute.Instance.node_Vector2_pos.Count}");
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("警告", "你不能在空事件下创建蓝图", "我知道了");
                            }
                        }
                    };
                });
                
                _char.y += 20;
            }
        }
        
        
    }
}
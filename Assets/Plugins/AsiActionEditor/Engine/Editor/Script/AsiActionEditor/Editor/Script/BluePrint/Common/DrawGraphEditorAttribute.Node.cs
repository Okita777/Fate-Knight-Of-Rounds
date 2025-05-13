using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.Graph;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class DrawGraphEditorAttribute
    {
        private Action<Rect, object, NodeDisplayValue> drawGraphAction;
        private int DrawHeight = 20;
        private string EdiKey = String.Empty;
        private int nodeID = -1;
        public Dictionary<int,Vector2> node_Vector2_pos = new Dictionary<int,Vector2>();
        public List<NodeDisplay> All_NodeDisPlay = new List<NodeDisplay>();
        // public List<int> All_NodeDisPlayHash = new List<int>();
        private Dictionary<int, NodeDisplay> All_NodeDisPlayHash = new Dictionary<int, NodeDisplay>();
        public EditorActionEvent nowEditorActionEvent;

        private void NodesDataInit(string EdiKey)
        {
            nodeID = -1;
            node_Vector2_pos.Clear();
            All_NodeDisPlay.Clear();
            All_NodeDisPlayHash.Clear();
            this.EdiKey = EdiKey;
            if (InspectorWindow.Instance.CurSelectProperty is EditorActionEvent _editorActionEvent)
            {
                nowEditorActionEvent = _editorActionEvent;
            }
            else
            {
                nowEditorActionEvent = null;
            }
        }

        public NodeDisplay CreactNode(object obj)
        {
            nodeID++;
            
            //输入锚点参数类型
            if (nowEditorActionEvent is not null)
            {
                int _hashCode = obj.GetHashCode();
                NodeDisplay nodeDisplay = new NodeDisplay(obj);
                All_NodeDisPlayHash.Add(_hashCode,nodeDisplay);
                
                bool isReturn = nodeID == 0;

                NodeEdiData BluePrintPos;
                if (!nowEditorActionEvent.NodeEdiDataDic.TryGetValue(EdiKey, out BluePrintPos))
                {
                    BluePrintPos = new NodeEdiData();
                    nowEditorActionEvent.NodeEdiDataDic.Add(EdiKey, BluePrintPos);
                }
                if (nodeID < BluePrintPos.nodePositions.Count)
                {
                    node_Vector2_pos.Add(_hashCode, BluePrintPos.nodePositions[nodeID]);
                }
                else
                {
                    node_Vector2_pos.Add(_hashCode, Vector2.left * (nodeID * 300));
                }
                // EngineDebug.Log($"读取ID： [{nodeID}   长度: [{BluePrintPos.nodePositions.Count}]]");

                PropertyInfo[] pis = obj.GetType().GetProperties().OrderBy(p => p.MetadataToken).ToArray();
                
                //当前节点所有可见参数
                List<NodeDisplayValue> NodeValues = new List<NodeDisplayValue>();
                
                //这里找的是所有的接口
                for (int i = 0; i < pis.Length; ++i)
                {
                    bool isDraw = true;

                    object[] attrs = pis[i].GetCustomAttributes(typeof(EditorGraphPropertyAttribute), false);
                    if (attrs.Length == 1 && isDraw)
                    {
                        //当前变量
                        object val = GetProperty(obj, pis[i].Name);

                        EditorGraphPropertyAttribute epa = (EditorGraphPropertyAttribute)attrs[0];

                        bool isDis = true;
                        drawGraphAction = null;
                        DrawHeight = 20;

                        //输入接口的所有类型
                        isDis = DrawVal(obj, val, epa, pis[i].Name);

                        //添加到可显节点
                        if (isDis)
                        {
                            NodeDisplayValue nv = new NodeDisplayValue();
                            nv.drawValData = epa;
                            nv.valName = pis[i].Name;
                            nv.drawFuntion = drawGraphAction;
                            nv.height = DrawHeight;
                            nv.Main = obj;
                            
                            //检查引脚是否有蓝图
                            if (epa.IsInput)
                            {
                                bool isFind = true;
                                nv._inputTarget = null;

                                if (val is not null)
                                {
                                    if(val is BluePrint_Value _printValue)
                                    {
                                        string[] valName = _printValue.GetType().Name.Split('_');
                                        if (valName[1] == "Value" || valName[1] == "GValue" || valName[1] == "BValue")
                                        {
                                            if (!_printValue.IsNode)
                                            {
                                                isFind = false;
                                            }
                                        }
                                    }
                                    
                                    if (isFind)
                                    {
                                        int hash = val.GetHashCode();
                                        if (!All_NodeDisPlayHash.ContainsKey(hash))
                                        {
                                            //寻找上一个节点
                                            NodeDisplay findNodeDisplay = CreactNode(val);
                                            nv._inputTarget = findNodeDisplay;
                                        }
                                        else
                                        {
                                            nv._inputTarget = All_NodeDisPlayHash[hash];
                                        }
                                    }
                                }
                            }
                            NodeValues.Add(nv);
                        }
                    }
                }

                //生成可显节点

                nodeDisplay.NodeValues = NodeValues;
                nodeDisplay.isReturn = isReturn;
                nodeDisplay.graph_names = obj.GetType().Name.Split('_');
                
                All_NodeDisPlay.Add(nodeDisplay);
                return nodeDisplay;
            }
            return null;
        }

        public bool RemoveNode(NodeDisplay nodeDisplay)
        {
            if (All_NodeDisPlay.Contains(nodeDisplay))
            {
                All_NodeDisPlay.Remove(nodeDisplay);
                return node_Vector2_pos.Remove(nodeDisplay.Main.GetHashCode());
            }
            return false;
        }
    }
    
    
    
    //蓝图节点的绘制
    public class NodeDisplay
    {
        public object Main;//当前节点
        public List<NodeDisplayValue> NodeValues;//所有可显示的参数
        public Vector2 _handl_Head_Pos;
        public bool isReturn = false;
        public string[] graph_names;
        public NodeDisplay(object Main)
        {
            this.Main = Main;
        }
    }
        
    //节点内的可配置变量
    public class NodeDisplayValue
    {
        //所属节点
        public object Main;
        //绘制时会用到的信息
        public EditorGraphPropertyAttribute drawValData;
        //变量名
        public string valName;
        //此变量的绘制高度
        public int height = 20;
        //此变量的绘制方案
        public Action<Rect, object, NodeDisplayValue> drawFuntion = null;
        //输入的对象
        public NodeDisplay _inputTarget = null;
    }
}
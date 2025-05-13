using System;
using System.Collections.Generic;
using System.IO;
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
        public bool m_Fold_Math = false;
        public bool m_Fold_Value = false;
        public bool m_Fold_GValue = false;

        public bool m_Fold_TrackData = false;
        public bool m_Fold_NoValue = false;
        public bool m_Fold_Other = false;
        
        public List<string> m_BlueprintEvents_Math = new List<string>();//运算相关的蓝图
        public List<string> m_BlueprintEvents_Value = new List<string>();//常量标拾
        public List<string> m_BlueprintEvents_GValue = new List<string>();//G常量标拾
        public List<string> m_BlueprintEvents_NoValue = new List<string>();//只负责返回参数的蓝图
        public List<string> m_BlueprintEvents_TrackData = new List<string>();//只负责返回参数的蓝图
        public List<string> m_BlueprintEvents_Other = new List<string>();//不知道啥玩意，命名不规范的蓝图
        public List<string> m_BlueprintEvents_NotValid = new List<string>();//未实现实例化的蓝图
        public List<string> m_BlueprintEvents_Path_NotValid = new List<string>();//未实现实例化的蓝图

        public List<string> m_OutType_Bool = new List<string>();
        public List<string> m_OutType_Int = new List<string>();
        public List<string> m_OutType_Float = new List<string>();
        public List<string> m_OutType_Vector3 = new List<string>();
        public List<string> m_OutType_Transform = new List<string>();
        public List<string> m_OutType_Unit = new List<string>();
        public List<string> m_OutType_Point = new List<string>();

        public List<string> m_InputType_Bool = new List<string>();
        public List<string> m_InputType_Int = new List<string>();
        public List<string> m_InputType_Float = new List<string>();
        public List<string> m_InputType_Vector3 = new List<string>();
        public List<string> m_InputType_Transform = new List<string>();
        public List<string> m_InputType_Unit = new List<string>();
        public List<string> m_InputType_Point = new List<string>();
        //初始化时加载路径下所有蓝图类型
        public void OnLoad()
        {
            //重置列表
            m_BlueprintEvents_Math.Clear();
            m_BlueprintEvents_Value.Clear();
            m_BlueprintEvents_GValue.Clear();
            m_BlueprintEvents_NoValue.Clear();
            m_BlueprintEvents_TrackData.Clear();
            m_BlueprintEvents_Other.Clear();
            m_BlueprintEvents_NotValid.Clear();

            m_OutType_Bool.Clear();
            m_OutType_Int.Clear();
            m_OutType_Float.Clear();
            m_OutType_Vector3.Clear();
            m_OutType_Transform.Clear();
            m_OutType_Unit.Clear();
            m_OutType_Point.Clear();

            m_InputType_Bool.Clear();
            m_InputType_Int.Clear();
            m_InputType_Float.Clear();
            m_InputType_Vector3.Clear();
            m_InputType_Transform.Clear();
            m_InputType_Unit.Clear();
            m_InputType_Point.Clear();

            //获取所有名为“BluePrintEvent”的文件夹，并获取文件夹下的所有脚本
            string _graphPath = string.Empty;
            string[] _guid = AssetDatabase.FindAssets("Event_BluePrint");
            // EngineDebug.Log($"返回的路径： {AssetDatabase.GUIDToAssetPath(_guid[0])}\n数量: {_guid.Length}"); 
            for (int i = 0; i < _guid.Length; i++)
            {
                string nowPath = AssetDatabase.GUIDToAssetPath(_guid[i]);
                // EngineDebug.Log($"返回的路径： {nowPath}");

                DirectoryInfo dir = new DirectoryInfo(nowPath);
                FileInfo[] fileInfo = dir.GetFiles("*");
                foreach (FileInfo _fileInfo in fileInfo)
                {
                    string[] names = _fileInfo.Name.Split('.');
                    string _name = names[0];
                    if (names.Length < 3)
                    {
                        string[] _graphType = _name.Split('_');
                        if (_graphType.Length > 1)
                        {
                            if (_graphType[1] != "NoValue" && _graphType[1] != "BValue")
                            {
                                // EngineDebug.Log("找到的文件: " + _name);
                                IProperty _property = BluePrintEvent.Creact(_name);
                                if (_property is not null)
                                {
                                    if (_graphType[1] == "Math")
                                    {
                                        if (_graphType[2] == "Add" || _graphType[2] == "Sub" ||
                                            _graphType[2] == "Mul" || _graphType[2] == "Div"
                                           )
                                        {
                                            if (_graphType[3] == "Float")
                                                m_BlueprintEvents_Math.Add(_name);
                                        }
                                        else
                                        {

                                        }

                                    }
                                    else if (_graphType[1] == "Value")
                                    {
                                        m_BlueprintEvents_Value.Add(_name);
                                    }
                                    else if (_graphType[1] == "TrackData")
                                    {
                                        m_BlueprintEvents_TrackData.Add(_name);
                                    }
                                    else if (_graphType[1] == "GValue")
                                    {
                                        m_BlueprintEvents_GValue.Add(_name);
                                    }
                                    // else if (_graphType[1] == "NoValue")
                                    // {
                                    //     m_BlueprintEvents_NoValue.Add(_name);
                                    // }
                                    else
                                    {
                                        m_BlueprintEvents_Other.Add(_name);
                                    }
                                    
                                    SetOutBluePrint(_property, _name);
                                }
                                else
                                {
                                    m_BlueprintEvents_NotValid.Add(_name);
                                    string[] _pathChil = _fileInfo.FullName.Split('\\');
                                    string _path = _pathChil[0];
                                    for (int j = 1; j < _pathChil.Length; j++)
                                    {
                                        string localNowPath = _pathChil[j];
                                        if (localNowPath == "Assets") _path = "Assets";
                                        else _path += $"\\{_pathChil[j]}";
                                    }

                                    m_BlueprintEvents_Path_NotValid.Add(_path);
                                    // EngineDebug.Log(_path); 
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetOutBluePrint(IProperty _property, string _name)
        {
            string outName = $"{_name}&{BluePrintEvent.DisName(_name)}";

            if (_property is BluePrint_Bool)
            {
                m_OutType_Bool.Add(outName);
            }
            else if (_property is BluePrint_Int)
            {
                m_OutType_Int.Add(outName);
            }
            else if (_property is BluePrint_Float)
            {
                m_OutType_Float.Add(outName);
            }
            else if (_property is BluePrint_Vector3)
            {
                m_OutType_Vector3.Add(outName);
            }
            else if (_property is BluePrint_Transform)
            {
                m_OutType_Transform.Add(outName);
            }
            else if (_property is BluePrint_Unit)
            {
                m_OutType_Unit.Add(outName);
            }
            else if (_property is BluePrint_PointData)
            {
                m_OutType_Point.Add(outName);
            }
            else
            {
                return;
            }
            
            // EngineDebug.LogError($"添加节点菜单(Out): {_property.GetType().Name}");
            //找第一个接入口
            PropertyInfo[] pis = _property.GetType().GetProperties().OrderBy(p => p.MetadataToken).ToArray();
            foreach (PropertyInfo _propertyInfo in pis)
            {
                object[] attrs = _propertyInfo.GetCustomAttributes(typeof(EditorGraphPropertyAttribute), false);
                if (attrs.Length > 0)
                {
                    EditorGraphPropertyAttribute epa = (EditorGraphPropertyAttribute)attrs[0];
                    if (epa.IsInput)
                    {
                        object _val = GetProperty(_property, _propertyInfo.Name);
                        if (_val is not null)
                        {
                            SetInputBluePrint(_val, _propertyInfo.Name, _name);
                            break;
                        }
                        // else
                        // {
                        //     EngineDebug.LogError($"空参数节点(Input)： {_property.GetType().Name} ");
                        // }
                    }
                }
            }
        }

        private void SetInputBluePrint(object _property, string _valName, string _nodeName)
        {
            // EngineDebug.LogError($"添加节点菜单(Input)： {_property.GetType().Name} ");
            string outName = $"{_nodeName}&{BluePrintEvent.DisName(_nodeName)}&{_valName}";
            if (_property is BluePrint_Bool)
            {
                m_InputType_Bool.Add(outName);
            }
            else if (_property is BluePrint_Int)
            {
                m_InputType_Int.Add(outName);
            }
            else if (_property is BluePrint_Float)
            {
                m_InputType_Float.Add(outName);
            }
            else if (_property is BluePrint_Vector3)
            {
                m_InputType_Vector3.Add(outName);
            }
            else if (_property is BluePrint_Transform)
            {
                m_InputType_Transform.Add(outName);
            }
            else if (_property is BluePrint_Unit)
            {
                m_InputType_Unit.Add(outName);
            }
            else if (_property is BluePrint_PointData)
            {
                m_InputType_Point.Add(outName);
            }
            else
            {
                return;
            }
        }
    }
}
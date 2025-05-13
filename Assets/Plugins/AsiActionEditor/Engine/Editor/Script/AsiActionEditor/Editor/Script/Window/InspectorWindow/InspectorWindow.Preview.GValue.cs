using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class InspectorWindow
    {
        private void DrawGValueGUI(EditorEngineGValuePart _gValue)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("序号：", GUILayout.Width(60));
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    _gValue.IndexID = EditorGUILayout.DelayedIntField(_gValue.IndexID);
                    if (_check.changed)
                    {
                        ResourcesWindow.Instance.GValueSort();
                        ResourcesWindow.Instance.Repaint();
                    }
                }
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("名称：", GUILayout.Width(60));
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    _gValue.Name = EditorGUILayout.DelayedTextField(_gValue.Name);
                    if (_check.changed)
                    {
                        ResourcesWindow.Instance.UpdateGvalueGroupNames();
                        ResourcesWindow.Instance.Repaint();
                    }
                }
            }

            using (new GUIColorScope(Color.gray))
            {
                GUILayout.Label("真实序号：" + _gValue.Index);
            }
            
            GUILayout.Space(20);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("参数类型：", GUILayout.Width(60));
                GUILayout.Label(_gValue.ValueType.ToString());
            }
            DrawEditorAttribute.DrawGValueInspector(_gValue);
            _gValue.IsDefaultValue = EditorGUILayout.Toggle("是否默认添加进单位", _gValue.IsDefaultValue);

            
            if (_gValue.ValueType == EGValueType.GEnum || _gValue.ValueType == EGValueType.GPoint || 
                _gValue.ValueType == EGValueType.GTransform || _gValue.ValueType == EGValueType.GUnit)
            {
                GUILayout.Space(20);
                using (new GUIColorScope(Color.cyan))
                {
                    GUILayout.Label("枚举成员编辑: ");
                }

                // List<SEnumName> enumNames = null;
                // if(_gValue.ValueType == EGValueType.GEnum)enumNames = ResourcesWindow.Instance.EnumNames[_gValue.ValueID].names
                // using (var _scroll = new EditorGUILayout.ScrollViewScope())
                {
                    using (new GUIColorScope(Color.green))
                    {
                        if (GUILayout.Button("添加枚举成员"))
                        {
                            if (_gValue.ValueType == EGValueType.GEnum)
                                ResourcesWindow.Instance.EnumNames[_gValue.ValueID].names.Add("New Members");
                            else if(_gValue.ValueType == EGValueType.GTransform)
                                ResourcesWindow.Instance.TransNames[_gValue.ValueID].names.Add("New Members");
                            else if(_gValue.ValueType == EGValueType.GPoint)
                                ResourcesWindow.Instance.PointNames[_gValue.ValueID].names.Add("New Members");
                            else if(_gValue.ValueType == EGValueType.GUnit)
                                ResourcesWindow.Instance.UnitNames[_gValue.ValueID].names.Add("New Members");
                        }
                    }

                    int lenght = 0;
                    if (_gValue.ValueType == EGValueType.GEnum)
                        lenght = ResourcesWindow.Instance.EnumNames[_gValue.ValueID].names.Count;
                    else if(_gValue.ValueType == EGValueType.GTransform)
                        lenght = ResourcesWindow.Instance.TransNames[_gValue.ValueID].names.Count;
                    else if(_gValue.ValueType == EGValueType.GPoint)
                        lenght = ResourcesWindow.Instance.PointNames[_gValue.ValueID].names.Count;
                    else if(_gValue.ValueType == EGValueType.GUnit)
                        lenght = ResourcesWindow.Instance.UnitNames[_gValue.ValueID].names.Count;
                    
                    for (int i = 0; i < lenght; i++)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            if (_gValue.ValueType == EGValueType.GEnum)
                                ResourcesWindow.Instance.EnumNames[_gValue.ValueID].names[i] = EditorGUILayout.
                                    TextField(ResourcesWindow.Instance.EnumNames[_gValue.ValueID].names[i]);
                            else if(_gValue.ValueType == EGValueType.GTransform)
                                ResourcesWindow.Instance.TransNames[_gValue.ValueID].names[i] = EditorGUILayout.
                                    TextField(ResourcesWindow.Instance.TransNames[_gValue.ValueID].names[i]);
                            else if(_gValue.ValueType == EGValueType.GPoint)
                                ResourcesWindow.Instance.PointNames[_gValue.ValueID].names[i] = EditorGUILayout.
                                    TextField(ResourcesWindow.Instance.PointNames[_gValue.ValueID].names[i]);
                            else if(_gValue.ValueType == EGValueType.GUnit)
                                ResourcesWindow.Instance.UnitNames[_gValue.ValueID].names[i] = EditorGUILayout.
                                    TextField(ResourcesWindow.Instance.UnitNames[_gValue.ValueID].names[i]);
                            
                            using (new GUIColorScope(Color.red))
                            {
                                if (GUILayout.Button("-", GUILayout.Width(20)))
                                {
                                    if (_gValue.ValueType == EGValueType.GEnum)
                                        ResourcesWindow.Instance.EnumNames[_gValue.ValueID].names.RemoveAt(i);
                                    else if (_gValue.ValueType == EGValueType.GTransform)
                                        ResourcesWindow.Instance.TransNames[_gValue.ValueID].names.RemoveAt(i);
                                    else if (_gValue.ValueType == EGValueType.GPoint)
                                        ResourcesWindow.Instance.PointNames[_gValue.ValueID].names.RemoveAt(i);
                                    else if (_gValue.ValueType == EGValueType.GUnit)
                                        ResourcesWindow.Instance.UnitNames[_gValue.ValueID].names.RemoveAt(i);
                                    i--;
                                }
                            }
                        }
                    }
                }
            }
            // switch (_gValue.ValueType)
            // {
            //     case EGValueType.GEnum:
            //         break;
            // }
        }
        
        
    }
}
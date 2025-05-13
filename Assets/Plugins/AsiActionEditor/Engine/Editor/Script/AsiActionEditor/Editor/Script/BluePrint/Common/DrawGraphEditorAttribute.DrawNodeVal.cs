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
        public bool _changed
        {
            set
            {
                if (value)
                {
                    ScenceDraw.Instance.OnUpdateDraw();
                    // EngineDebug.Log("更新图形");
                }
            }
        }

        private bool DrawVal(object obj, object val, EditorGraphPropertyAttribute epa, string valueName)
        {
            // _changed = false;
            
            bool isDis = true;
            switch (epa.PropertyType)
            {
                case EditorGraphPropertyType.EEPT_Vector3:
                    DrawVector3(obj, val, epa, valueName);
                    break;
                case EditorGraphPropertyType.EEPT_Int:
                    DrawInt(obj, val, epa, valueName);
                    break;
                case EditorGraphPropertyType.EEPT_Float:
                    DrawFloat(obj, val, epa, valueName);
                    break;
                case EditorGraphPropertyType.EEPT_PointData:
                    DrawPoint(obj, val, epa, valueName);
                    break;
                case EditorGraphPropertyType.EEPT_Transform:
                    DrawTransform(obj, val, epa, valueName);
                    break;
                case EditorGraphPropertyType.EEPT_Bool:
                    DrawBool(obj, val, epa, valueName);
                    break;
                case EditorGraphPropertyType.EEPT_Enum:
                    DrawEnum(obj, val, epa, valueName);
                    break;
                case EditorGraphPropertyType.EEPT_EnumCustom:
                    DrawEnumCustom(obj, val, epa, valueName);
                    break;
                case EditorGraphPropertyType.EEPT_CharacteLimbType:
                    DrawCharacteLimbType(obj, val, epa, valueName);
                    break;
                case EditorGraphPropertyType.EEPT_GInt:
                    DrawGValue(obj, val, epa, valueName, EGValueType.GInt);
                    break;
                case EditorGraphPropertyType.EEPT_GFloat:
                    DrawGValue(obj, val, epa, valueName, EGValueType.GFloat);
                    break;

                case EditorGraphPropertyType.EEPT_GTransform:
                    DrawGObject(obj, val, epa, valueName, EGValueType.GTransform);
                    break;
                case EditorGraphPropertyType.EEPT_GPoint:
                    DrawGObject(obj, val, epa, valueName, EGValueType.GPoint);
                    break;
                case EditorGraphPropertyType.EEPT_GUnit:
                    DrawUnit(obj, val, epa, valueName);
                    break;
                default:
                    isDis = false;
                    break;
            }

            return isDis;
        }

        #region DrawFuntion
        private void DrawVector3(object obj, object val, EditorGraphPropertyAttribute epa, string valueName)
        {
            //收集所有Vector3引脚
            // if (epa.IsInput)
            // {
            //     if (val is not null)
            //     {
            //         if (val is BluePrint_Vector3 _bluePrintVector3)
            //             node_Vector3_input.Add(_bluePrintVector3);
            //         else
            //             EngineDebug.Log(
            //                 $"类型错误!!  参数名:[{valueName}]  参数类型:[{val.GetType().FullName}]");
            //     }
            // }
            // else
            {
                if (val is EVector3 _bluePrintVector3) 
                {
                    drawGraphAction = (rect, o, arg3) =>
                    {
                        float width = arg3.drawValData.LabelWidth;
                        Rect _head = new Rect(rect);
                        _head.width = width;
                        GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));

                        _head.x += _head.width;
                        _head.width = rect.width - _head.width;
                        using (var _check = new EditorGUI.ChangeCheckScope())
                        {
                            _bluePrintVector3.SetValue(EditorGUI.Vector3Field(_head, "", _bluePrintVector3.GetValue()));
                            if (_check.changed)
                            {
                                ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                                SetProperty(obj, valueName, _bluePrintVector3);
                            }
                        }
                    };
                }
            }

            //将蓝图绘制简化为参数变量
            if (drawGraphAction is null)
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    float width = arg3.drawValData.LabelWidth;
                    Rect _head = new Rect(rect);
                    _head.width = width;
                    GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));
                    
                    if (o is null)
                    {
                        CreactBluePrintEvent(arg3);
                    }
                    else if(o is GraphEvent_Value_Vector3 _value)
                    {
                        if (arg3._inputTarget is null)
                        {
                            _head.x += _head.width;
                            _head.width = rect.width - _head.width;
                            using (var _check = new EditorGUI.ChangeCheckScope())
                            {
                                EVector3 aa = new EVector3(EditorGUI.Vector3Field(_head, "", _value.mEVector3.GetValue()));
                                if (_check.changed)
                                {
                                    ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                                    SetProperty(_value, "mEVector3", aa.Clone());
                                    _changed = true;
                                }
                            }
                        }
                    }
                };
            }
        }
        private void DrawInt(object obj, object val, EditorGraphPropertyAttribute epa, string valueName)
        {
            //收集所有Vector3引脚
            if (val is int _val) 
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    float width = arg3.drawValData.LabelWidth;
                    Rect _head = new Rect(rect);
                    _head.width = width;
                    GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));

                    _head.x += _head.width;
                    _head.width = rect.width - _head.width;
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        _val = (EditorGUI.IntField(_head, "", _val));
                        if (_check.changed)
                        {
                            ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                            SetProperty(obj, valueName, _val);
                        }
                    }
                };
            }

            //将蓝图绘制简化为参数变量
            if (drawGraphAction is null)
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    float width = arg3.drawValData.LabelWidth;
                    Rect _head = new Rect(rect);
                    _head.width = width;
                    GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));
                    
                    if (o is null)
                    {
                        CreactBluePrintEvent(arg3);
                    }
                    else if(o is GraphEvent_Value_Int _value)
                    {
                        if (arg3._inputTarget is null)
                        {
                            _head.x += _head.width;
                            _head.width = rect.width - _head.width;
                            using (var _check = new EditorGUI.ChangeCheckScope())
                            {
                                int aa = (EditorGUI.IntField(_head, "", _value.IntVal));
                                if (_check.changed)
                                {
                                    ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                                    SetProperty(_value, "IntVal", aa);
                                    _changed = true;

                                }
                            }
                        }
                    }
                };
            }
        }
        private void DrawFloat(object obj, object val, EditorGraphPropertyAttribute epa, string valueName)
        {
            //收集所有Vector3引脚
            if (val is float _val) 
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        _val = (EditorGUI.FloatField(rect, new GUIContent(arg3.drawValData.PropertyName,arg3.drawValData.Tooltip), _val));
                        if (_check.changed)
                        {
                            ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                            SetProperty(obj, valueName, _val);
                        }
                    }
                };
            }

            //将蓝图绘制简化为参数变量
            if (drawGraphAction is null)
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    float width = arg3.drawValData.LabelWidth;
                    Rect _head = new Rect(rect);
                    _head.width = width;
                    GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));
                    
                    if (o is null)
                    {
                        CreactBluePrintEvent(arg3);
                    }
                    else if(o is GraphEvent_Value_Float _value)
                    {
                        if (arg3._inputTarget is null)
                        {
                            _head.x += _head.width;
                            _head.width = rect.width - _head.width;
                            using (var _check = new EditorGUI.ChangeCheckScope())
                            {
                                float aa = (EditorGUI.FloatField(_head, "", _value.FloatVal));
                                if (_check.changed)
                                {
                                    ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                                    SetProperty(_value, "FloatVal", aa);
                                    _changed = true;

                                }
                            }
                        }
                    }
                };
            }
        }
        private void DrawUnit(object obj, object val, EditorGraphPropertyAttribute epa, string valueName)
        {
            //收集所有Vector3引脚
            if (val is GUnit _val)
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    EditorGraphPropertyAttribute drawAttr = epa;
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        // GValue _Gval = DrawEditorAttribute.DrawGValue(rect, drawAttr.PropertyName, _val, EGValueType.GUnit,
                        //     (int)drawAttr.LabelWidth, drawAttr.Tooltip);
                        // GValue _value = (GValue)obj;
                        GValue _Gval = DrawEditorAttribute.DrawGObject(rect, _val, EGValueType.GUnit,drawAttr.PropertyName,drawAttr.Tooltip,drawAttr.LabelWidth);
                        if (_check.changed)
                        {
                            ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                            SetProperty(obj, valueName, _Gval);
                        }
                    }
                };
            }

            //将蓝图绘制简化为参数变量
            if (drawGraphAction is null)
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    if (o is null)
                    {
                        CreactBluePrintEvent(arg3);
                    }
                    else if (o is GraphEvent_GValue_GUnit _value)
                    {
                        if (_value.IsNode)
                        {
                            if (arg3._inputTarget is null)
                            {
                                EditorGraphPropertyAttribute drawAttr = epa;
                                using (var _check = new EditorGUI.ChangeCheckScope())
                                {
                                    GValue _Gval = DrawEditorAttribute.DrawGObject(rect, _value.GUnitVal, EGValueType.GUnit,drawAttr.PropertyName,drawAttr.Tooltip,drawAttr.LabelWidth);
                                    if (_check.changed)
                                    {
                                        ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                                        SetProperty(_value, "GUnitVal", _Gval);
                                        _changed = true;

                                    }
                                }
                            }
                        }
                        else
                        {
                            GUI.Box(rect,"自身Unit");
                        }
                    }
                };
            }
        }
        private void DrawPoint(object obj, object val, EditorGraphPropertyAttribute epa, string valueName)
        {
            //将蓝图绘制简化为参数变量
            if (drawGraphAction is null)
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    if (o is null)
                    {
                        CreactBluePrintEvent(arg3);
                    }
                    else if (o is GraphEvent_BValue_Point _value)
                    {
                        if (_value.IsNode)
                        {
                            Rect _head = new Rect(rect);
                            GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));
                        }
                        else
                        {
                            GUI.Box(rect,"空PiontData");
                        }
                    }
                };
            }
        }
        private void DrawTransform(object obj, object val, EditorGraphPropertyAttribute epa, string valueName)
        {
            //将蓝图绘制简化为参数变量
            if (drawGraphAction is null)
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    if (o is null)
                    {
                        CreactBluePrintEvent(arg3);
                    }
                    else if (o is GraphEvent_BValue_Transform _value)
                    {
                        if (_value.IsNode)
                        {
                            Rect _head = new Rect(rect);
                            GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));
                        }
                        else
                        {
                            GUI.Box(rect,"空Transform");
                        }
                    }
                };
            }
        }

        private void DrawBool(object obj, object val, EditorGraphPropertyAttribute epa, string valueName)
        {
            //收集所有Vector3引脚
            if (val is bool _val) 
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    float width = arg3.drawValData.LabelWidth;
                    Rect _head = new Rect(rect);
                    _head.width = width;
                    GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));

                    _head.x += _head.width;
                    _head.width = rect.width - _head.width;
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        _val = (EditorGUI.Toggle(_head, "", _val));
                        if (_check.changed)
                        {
                            ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                            SetProperty(obj, valueName, _val);
                        }
                    }
                };
            }

            //将蓝图绘制简化为参数变量
            if (drawGraphAction is null)
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    float width = arg3.drawValData.LabelWidth;
                    Rect _head = new Rect(rect);
                    _head.width = width;
                    GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));
                    
                    if (o is null)
                    {
                        CreactBluePrintEvent(arg3);
                    }
                    else if(o is GraphEvent_Value_Bool _value)
                    {
                        if (arg3._inputTarget is null)
                        {
                            _head.x += _head.width;
                            _head.width = rect.width - _head.width;
                            using (var _check = new EditorGUI.ChangeCheckScope())
                            {
                                bool aa = (EditorGUI.Toggle(_head, "", _value.IntVal));
                                if (_check.changed)
                                {
                                    ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                                    SetProperty(_value, "IntVal", aa);
                                    _changed = true;

                                }
                            }
                        }
                    }
                };
            }
        }
        private void DrawEnum(object obj, object val, EditorGraphPropertyAttribute epa, string valueName)
        {
            //收集所有Vector3引脚
            if (val is Enum _val) 
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    float width = arg3.drawValData.LabelWidth;
                    Rect _head = new Rect(rect);
                    _head.width = width;
                    GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));

                    _head.x += _head.width;
                    _head.width = rect.width - _head.width;
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        _val = (EditorGUI.EnumPopup(_head, "", _val));
                        if (_check.changed)
                        {
                            ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                            SetProperty(obj, valueName, _val);
                        }
                    }
                };
            }
        }
        private void DrawEnumCustom(object obj, object val, EditorGraphPropertyAttribute epa, string valueName)
        {
            //收集所有Vector3引脚
            if (val is byte _val) 
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    float width = arg3.drawValData.LabelWidth;
                    Rect _head = new Rect(rect);
                    _head.width = width;
                    GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));

                    _head.x += _head.width;
                    _head.width = rect.width - _head.width;
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        _val = (byte)EditorGUI.Popup(_head, _val, epa.EnumNames);//EditorGUI.EnumPopup(_head, "", _val)
                        if (_check.changed)
                        {
                            ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                            SetProperty(obj, valueName, _val);
                        }
                    }
                };
            }
        }
        private void DrawCharacteLimbType(object obj, object val, EditorGraphPropertyAttribute epa, string valueName)
        {
            //收集所有Vector3引脚
            if (val is int _val)
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    float width = arg3.drawValData.LabelWidth;
                    Rect _head = new Rect(rect);
                    _head.width = width;
                    GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));

                    _head.x += _head.width;
                    _head.width = rect.width - _head.width;
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        // _val = (EditorGUI.EnumPopup(_head, "", _val));
                        DrawEditorAttribute.LimbPointType(_head, arg3.Main, arg3.valName, o);
                        if (_check.changed)
                        {
                            //     ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                            //     SetProperty(obj, valueName, _val);
                            _changed = true;
                        }
                    }
                };
            }
        }
        private void DrawGValue(object obj, object val, EditorGraphPropertyAttribute epa, string valueName, EGValueType valueType)
        {
            if (val is GValue _val)
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    EditorGraphPropertyAttribute drawAttr = epa;
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        GValue _Gval = DrawEditorAttribute.DrawGValue(rect, drawAttr.PropertyName, _val, valueType,
                            (int)drawAttr.LabelWidth, drawAttr.Tooltip);
                        if (_check.changed)
                        {
                            ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                            SetProperty(obj, valueName, _Gval);
                            _changed = true;

                        }
                    }
                };
            }
        }
        private void DrawGObject(object obj, object val, EditorGraphPropertyAttribute epa, string valueName, EGValueType valueType)
        {
            if (val is GValue _val)
            {
                drawGraphAction = (rect, o, arg3) =>
                {
                    // float width = arg3.drawValData.LabelWidth;
                    // Rect _head = new Rect(rect);
                    // _head.width = width;
                    // GUI.Label(_head, new GUIContent(arg3.drawValData.PropertyName, arg3.drawValData.Tooltip));
                    //
                    // _head.x += _head.width;
                    // _head.width = rect.width - _head.width;
                    EditorGraphPropertyAttribute drawAttr = epa;
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        GValue _Gval = DrawEditorAttribute.DrawGObject(rect, _val, valueType,drawAttr.PropertyName,drawAttr.Tooltip,drawAttr.LabelWidth);
                        if (_check.changed)
                        {
                            ResourcesWindow.Instance.ActionOnChange(true, "蓝图参数变更");
                            SetProperty(obj, valueName, _Gval);
                            _changed = true;

                        }
                    }
                };
            }
        }
        // DrawGObject

        #endregion

        private void CreactBluePrintEvent(NodeDisplayValue _value)
        {
            object vv = null;
            if (_value.drawValData.PropertyType == EditorGraphPropertyType.EEPT_Vector3)
            {
                vv = BluePrintEvent.Creact(nameof(GraphEvent_Value_Vector3));
            }
            else if (_value.drawValData.PropertyType == EditorGraphPropertyType.EEPT_Int)
            {
                vv = BluePrintEvent.Creact(nameof(GraphEvent_Value_Int));
            }
            else if (_value.drawValData.PropertyType == EditorGraphPropertyType.EEPT_Float)
            {
                vv = BluePrintEvent.Creact(nameof(GraphEvent_Value_Float));
            }else if (_value.drawValData.PropertyType == EditorGraphPropertyType.EEPT_GUnit)
            {
                vv = BluePrintEvent.Creact(nameof(GraphEvent_GValue_GUnit));
            }else if (_value.drawValData.PropertyType == EditorGraphPropertyType.EEPT_PointData)
            {
                vv = BluePrintEvent.Creact(nameof(GraphEvent_BValue_Point));
            }else if (_value.drawValData.PropertyType == EditorGraphPropertyType.EEPT_Transform)
            {
                vv = BluePrintEvent.Creact(nameof(GraphEvent_BValue_Transform));
            }else if (_value.drawValData.PropertyType == EditorGraphPropertyType.EEPT_Bool)
            {
                vv = BluePrintEvent.Creact(nameof(GraphEvent_Value_Bool));
            }
            else
            {
                EngineDebug.LogError($"错误类型: {_value.drawValData.PropertyType}");
                return;
            }
            SetProperty(_value.Main, _value.valName, vv);
        }
    }
}
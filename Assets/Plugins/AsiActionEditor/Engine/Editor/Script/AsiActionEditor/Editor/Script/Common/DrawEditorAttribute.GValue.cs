using System;
using System.Collections.Generic;
using System.Linq;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class DrawEditorAttribute
    {
        #region DrawValue
        private const int GValueHeadHeight = 20;
        private const int GValueElementHeight = 22;
        private static readonly string[] GValue_fh1 = new[] { "==", "!=" };
        private static readonly string[] GValue_fh2 = new[] {">=","<=",">","<"};
        private static readonly string[] GValue_fh3 = new[] { "==", "!=", ">=","<=",">","<"};
        private static readonly string[] GValue_fh4 = new[] { "true", "false" };

        
        private static void DrawGbool(object obj, string propertyName, GBool val)
        {
            DrawGValueInfo(obj, propertyName, val, EGValueType.GBool, () =>
            {
                val.value = EditorGUILayout.Toggle(val.value);
                SetProperty(obj, propertyName, val);
            });
        }
        private static void DrawGfloat(object obj, string propertyName, GFloat val, string _disName)
        {
            DrawGValueInfo(obj, propertyName, val, EGValueType.GFloat, () =>
            {
                val.value = EditorGUILayout.FloatField(val.value);
                SetProperty(obj, propertyName, val);
            });
        }
        
        private static void DrawGint(object obj, string propertyName, GInt val)
        {
            DrawGValueInfo(obj, propertyName, val, EGValueType.GInt, () =>
            {
                val.value = EditorGUILayout.IntField(val.value);
                SetProperty(obj, propertyName, val);
            });
        }
        
        private static void DrawGstring(object obj, string propertyName, GString val)
        {
            DrawGValueInfo(obj, propertyName, val, EGValueType.GString, () =>
            {
                val.value = EditorGUILayout.TextField(val.value);
                SetProperty(obj, propertyName, val);
            });
        }

        public static GValue DrawGValue(Rect rect, string _disName, GValue val, EGValueType _type, int weight = 120,string _tooltip = "")
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                float width = rect.width;
                rect.width = weight;
                GUI.Label(rect, new GUIContent(_disName, _tooltip));
                val.mType = true;

                if (val.mType)
                {
                    string[] names = ResourcesWindow.Instance.GetGValueName(_type).ToArray();
                    if (names is null || names.Length == 0)
                    {
                        using (new GUIColorScope(Color.red))
                        {
                            GUI.Label(rect,"编辑器未配置任何此类型 GValue"); 
                        }
                    }
                    else
                    {
                        rect.x += rect.width;
                        rect.width = width - rect.width;
                        val.mValueIndex = (ushort)EditorGUI.Popup(rect, val.mValueIndex,names);
                    }
                }
            }

            return val;
        }

        public static GValue DrawGObject(Rect rect, GValue val, EGValueType _type, string _disName, string _tooltip, float lenth)
        {
            //将GValue设定为XX值
            if (ResourcesWindow.Instance.GetGValueName(EGValueType.GPoint) is null)
            {
                using (new GUIColorScope(Color.red))
                {
                    GUI.Label(rect,$"Gvalue缺失 {_type} 参数！！");
                }
                return null;
            }

            Rect gRect = new Rect(rect);
            gRect.width = lenth;
            GUI.Label(gRect,new GUIContent(_disName,_tooltip));
            
            gRect.x += gRect.width;
            gRect.width = (rect.width - gRect.width) * 0.5f;
            string[] names = ResourcesWindow.Instance.GetGValueName(_type).ToArray();
            val.mValueIndex = (ushort)EditorGUI.Popup(gRect, val.mValueIndex, names);
            gRect.x += gRect.width;
            if (_type == EGValueType.GPoint)
            {
                GPoint gval = val as GPoint;
                gval.mSerValue = (byte)EditorGUI.Popup(gRect, gval.mSerValue,
                    ResourcesWindow.Instance.PointNames[val.mValueIndex].names.ToArray());
                return gval;
            }
            if (_type == EGValueType.GTransform)
            {
                GTransform gval = val as GTransform;
                gval.mSerValue = (byte)EditorGUI.Popup(gRect, gval.mSerValue,
                    ResourcesWindow.Instance.PointNames[val.mValueIndex].names.ToArray());
                return gval;
            }
            if (_type == EGValueType.GUnit)
            {
                GUnit gval = val as GUnit;
                gval.mSerValue = (byte)EditorGUI.Popup(gRect, gval.mSerValue,
                    ResourcesWindow.Instance.PointNames[val.mValueIndex].names.ToArray());
                return gval;
            }
            return null;
        }

        private static void DrawGenum(object obj, string propertyName, GEnum val, string[] enumNames)
        {
            DrawGValueInfo(obj, propertyName, val, EGValueType.GEnum, () =>
            {
                val.value = (byte)EditorGUILayout.Popup(val.value, enumNames);
                SetProperty(obj, propertyName, val);
            });
        }
        private static void DrawGpoint(object obj, string propertyName, GPoint val)
        {
            using (new GUILayout.HorizontalScope())
            {
                DrawGValueInfo(obj, propertyName, val, EGValueType.GPoint);
                val.mSerValue = (byte)EditorGUILayout.Popup(val.mSerValue,
                    ResourcesWindow.Instance.PointNames[val.mValueIndex].names.ToArray());
            }
        }
        // private static void DrawGtransform(object obj, string propertyName, GTransform val)
        // {
        //     DrawGValueInfo(obj, propertyName, val, EGValueType.GTransform);
        // }
        // private static void DrawGunit(object obj, string propertyName, GUnit val)
        // {
        //     DrawGValueInfo(obj, propertyName, val, EGValueType.GUnit);
        // }
        // private static void DrawGColor(object obj, string propertyName, GColor val)
        // {
        //     DrawGValueInfo(obj, propertyName, val, EGValueType.GColor, () =>
        //     {
        //         val.value = EditorGUILayout.ColorField(val.value);
        //         SetProperty(obj, propertyName, val);
        //     });
        // }
        // private static void DrawGVector2(object obj, string propertyName, GVector2 val)
        // {
        //     DrawGValueInfo(obj, propertyName, val, EGValueType.GVector2, () =>
        //     {
        //         val.value = EditorGUILayout.Vector2Field("",val.value);
        //         SetProperty(obj, propertyName, val);
        //     });
        // }
        // private static void DrawGVector3(object obj, string propertyName, GVector3 val)
        // {
        //     DrawGValueInfo(obj, propertyName, val, EGValueType.GVector3, () =>
        //     {
        //         val.value = EditorGUILayout.Vector3Field("", val.value);
        //         SetProperty(obj, propertyName, val);
        //     });
        // }
        #region 基础绘制
        private static void DrawGValueInfo(object obj, string propertyName, GValue val, EGValueType _type, Action _DrawValueCallback)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(val.mType ? "G" : "L",GUILayout.Width(20)))
                {
                    val.mType = !val.mType;
                }

                if (val.mType)
                {
                    string[] names = ResourcesWindow.Instance.GetGValueName(_type).ToArray();
                    if (names is null || names.Length == 0)
                    {
                        using (new GUIColorScope(Color.red))
                        {
                            GUILayout.Label("编辑器未配置任何此类型 GValue");
                        }
                    }
                    else
                    {
                        val.mValueIndex = (ushort)EditorGUILayout.Popup(val.mValueIndex,names);
                        SetProperty(obj, propertyName, val);
                    }
                }
                else
                {
                    _DrawValueCallback();
                }
            }
        }
        private static void DrawGValueInfo(object obj, string propertyName, GValue val, EGValueType _type)
        {
            val.mType = true;
            string[] names = ResourcesWindow.Instance.GetGValueName(_type).ToArray();
            if (names.Length < 1)
            {
                using (new GUIColorScope(Color.red))
                {
                    GUILayout.Label("编辑器未配置任何此类型 GValue");
                }
            }
            else
            {
                val.mValueIndex = (ushort)EditorGUILayout.Popup(val.mValueIndex, names);
                SetProperty(obj, propertyName, val);
            }

        }
        //GValue设置列表
        private static void DrawGValueSetting(object obj, string propertyName, GValue_Setting val, string name)
        {
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            //预留空间
            float _bodyheight = GValueHeadHeight + (val.isOpen ? GValueElementHeight * val.parameter.Count : 0);
            Rect _rect = EditorGUILayout.GetControlRect(GUILayout.Height(_bodyheight));
            Rect _elementRect = new Rect(_rect.x, _rect.y, _rect.width, GValueElementHeight);
            using (new GUILayout.AreaScope(_rect)){}

            //头部绘制
            Rect _headRect = new Rect(_elementRect.x, _elementRect.y, _elementRect.width, GValueHeadHeight);
            EditorGUI.DrawRect(_headRect, Color.black * 0.5f);
            GUI.Label(_headRect,(val.isOpen ? "- " : "+") + "[写入] " + name);// + "   [ GValue设置列表 ]"
            _headRect.width = 110;
            _headRect.x = _rect.x + _rect.width - _headRect.width;
            Color _colorbase = _headRect.Contains(Event.current.mousePosition) ? Color.white * 0.3f : Color.black *0.3f;
            using (new GUIColorScope(_colorbase))
            {
                if (GUI.Button(_headRect, "", EditorStyles.toolbarButton))
                {
                    val.isOpen = true;
                    val.parameter.Add(new GValue_SettingPar(EGValueType.GInt, new GVS_Int(0)));
                }
            }

            GUI.Label(_headRect," [添加Gvalue设置]");
            _headRect.x = _rect.x;
            _headRect.width =  _rect.width - _headRect.width;
            if (GUI.Button(_headRect, "", EditorStyles.label))
                val.isOpen = !val.isOpen;
            
            //绘制元素
            if (val.isOpen)
            {
                Rect _elementBodyRect = new Rect(_rect);
                _elementBodyRect.y += _headRect.height;
                _elementBodyRect.height -=  _headRect.height - 5;
                EditorGUI.DrawRect(_elementBodyRect, Color.black * 0.3f);
                _elementRect.y += _headRect.height + 5;
                for (int i = 0; i < val.parameter.Count; i++)
                {
                    GValue_SettingPar VARIABLE = val.parameter[i];
                    Rect _enumRect = new Rect(_elementRect);
                    _enumRect.width = 90;
                    _enumRect.x += 5;
                    _enumRect.y += 1;
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        VARIABLE.gValueType = (EGValueType)EditorGUI.EnumPopup(_enumRect, VARIABLE.gValueType);
                        if (_check.changed)
                        {
                            VARIABLE.GValueValue = GVSInit(VARIABLE.gValueType);
                        }
                    }
                    
                    Rect _remove = new Rect(_elementRect);
                    _remove.x += _elementRect.width - 30;
                    _remove.width = 25;
                    using (new GUIColorScope(Color.red))
                    {
                        if (GUI.Button(_remove, "-"))
                        {
                            val.parameter.Remove(VARIABLE);
                        }
                    }
                    
                    Rect _indexRect = new Rect(_elementRect);
                    _indexRect.x = _enumRect.x + _enumRect.width;
                    _indexRect.width = (_remove.x - _indexRect.x) * 0.5f;
                    _indexRect.y += 1;
                    List<string> _list = ResourcesWindow.Instance.GetGValueName(VARIABLE.gValueType);
                    if (_list is null)
                    {
                        GUI.Label(_indexRect,"GValue列表未配置此参数");
                    }
                    else
                    {
                        VARIABLE.GValueIndexID = (byte)EditorGUI.Popup(_indexRect, VARIABLE.GValueIndexID, _list.ToArray());
                    
                        Rect bodyRect = new Rect(_elementRect);
                        bodyRect.x = _indexRect.x + _indexRect.width;
                        bodyRect.width = _remove.x - bodyRect.x;
                        // VARIABLE.GValueValue = DrawGvalueElement(bodyRect, VARIABLE.gValueType, VARIABLE.GValueValue);
                        DrawGvalueElement(bodyRect, VARIABLE);
                    }

                    
                    Vector2 _startPos = new Vector2(_elementRect.x, _elementRect.y + _elementRect.height);
                    Vector2 _endtPos = new Vector2(_elementRect.x+_elementRect.width, _elementRect.y + _elementRect.height);
                    Handles.color = Color.black*0.3f;
                    Handles.DrawLine(_startPos, _endtPos);
                    _elementRect.y += GValueElementHeight; 
                }
            }
            GUILayout.Space(10);

            SetProperty(obj, propertyName, val);
        }
        private static void DrawGvalueRatio(object obj, string propertyName, GValue_Ratio val, string name, bool _attackBox)
        {
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            //预留空间
            float bHeight = val.isOpen ? GValueElementHeight * val.GValue_RatioPart.Count + (_attackBox ? 20 : 0) : 0;
            float _bodyheight = GValueHeadHeight + bHeight;
            Rect _rect = EditorGUILayout.GetControlRect(GUILayout.Height(_bodyheight));
            Rect _elementRect = new Rect(_rect.x, _rect.y, _rect.width, GValueElementHeight);
            using (new GUILayout.AreaScope(_rect)){}

            //头部绘制
            Rect _headRect = new Rect(_elementRect.x, _elementRect.y, _elementRect.width, GValueHeadHeight);
            EditorGUI.DrawRect(_headRect, Color.black * 0.5f);
            GUI.Label(_headRect,(val.isOpen ? "- " : "+") + "[读取] " + name);// + "   [ GValue设置列表 ]"
            _headRect.width = 110;
            _headRect.x = _rect.x + _rect.width - _headRect.width;
            Color _colorbase = _headRect.Contains(Event.current.mousePosition) ? Color.white * 0.3f : Color.black *0.3f;
            using (new GUIColorScope(_colorbase))
            {
                if (GUI.Button(_headRect, "", EditorStyles.toolbarButton))
                {
                    val.isOpen = true;
                    val.GValue_RatioPart.Add(new GValue_RatioPart(EGValueType.GInt, new GInt(0,true),0,new GInt(0)));
                }
            }

            GUI.Label(_headRect," [添加Gvalue判断]");
            
            //是否全部判定的按钮
            _headRect.x -= 150;
            _headRect.width = 150;
            if (GUI.Button(_headRect, "", EditorStyles.label))
            {
                val.checkAll = !val.checkAll;
            }
            string checkAllCon = val.checkAll ? " true" : "false";
            GUI.Label(_headRect,$"[需要满足所有成立： {checkAllCon}]");

            //展开按钮绘制
            _headRect.x = _rect.x;
            _headRect.width =  _rect.width - _headRect.width;
            if (GUI.Button(_headRect, "", EditorStyles.label))
                val.isOpen = !val.isOpen;
            
            //描述绘制
            if (val.isOpen && _attackBox)
            {
                Rect _AttackBoxRect = new Rect(_rect);
                _AttackBoxRect.y = _headRect.y + _headRect.height;
                _AttackBoxRect.height = 20;
                EditorGUI.DrawRect(_AttackBoxRect, Color.black * 0.1f);
                _AttackBoxRect.width -= 120;
                _AttackBoxRect.width /= 2;
                _AttackBoxRect.x += 95;
                EditorGUI.DrawRect(_AttackBoxRect, Color.cyan * 0.5f);
                GUI.Box(_AttackBoxRect,"命中单位");
                _AttackBoxRect.x += _AttackBoxRect.width + 10;
                EditorGUI.DrawRect(_AttackBoxRect, Color.cyan * 0.5f);
                GUI.Box(_AttackBoxRect,"当前单位");
            }
            
            //绘制元素
            if (val.isOpen)
            {
                Rect _elementBodyRect = new Rect(_rect);
                _elementBodyRect.y = _headRect.y + _headRect.height;
                _elementBodyRect.height -= _headRect.height - 5;
                EditorGUI.DrawRect(_elementBodyRect, Color.black * 0.3f);
                _elementRect.y += _headRect.height + 5 + (_attackBox ? 20 : 0);
                for (int i = 0; i < val.GValue_RatioPart.Count; i++)
                {
                    GValue_RatioPart VARIABLE = val.GValue_RatioPart[i];
                    Rect _enumRect = new Rect(_elementRect);
                    _enumRect.width = 90;
                    _enumRect.x += 5;
                    _enumRect.y += 1;
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        VARIABLE.GValueType = (EGValueType)EditorGUI.EnumPopup(_enumRect, VARIABLE.GValueType);
                        if (_check.changed)
                        {
                            VARIABLE.GValue_L = GvalueInit(VARIABLE.GValueType, true);
                            VARIABLE.GValue_R = GvalueInit(VARIABLE.GValueType, false);
                        }
                    }

                    Rect _remove = new Rect(_elementRect);
                    _remove.x += _elementRect.width - 30;
                    _remove.width = 25;
                    using (new GUIColorScope(Color.red))
                    {
                        if (GUI.Button(_remove, "-"))
                        {
                            val.GValue_RatioPart.Remove(VARIABLE);
                        }
                    }

                    Rect _indexRect = new Rect(_elementRect);
                    _indexRect.x = _enumRect.x + _enumRect.width;
                    _indexRect.width = (_remove.x - _indexRect.x);
                    DrawGvalueElement(_indexRect, VARIABLE);

                    Vector2 _startPos = new Vector2(_elementRect.x, _elementRect.y + _elementRect.height);
                    Vector2 _endtPos = new Vector2(_elementRect.x + _elementRect.width,
                        _elementRect.y + _elementRect.height);
                    Handles.color = Color.black * 0.3f;
                    Handles.DrawLine(_startPos, _endtPos);
                    _elementRect.y += GValueElementHeight;
                }
            }
            GUILayout.Space(10);

            SetProperty(obj, propertyName, val);
        }

        private static void DrawGvalueElement(Rect _rect, GValue_SettingPar _obj)
        {
            switch (_obj.gValueType)
            {
                case EGValueType.GBool:
                    ((GVS_Bool)_obj.GValueValue).value = EditorGUI.Toggle(_rect, ((GVS_Bool)_obj.GValueValue).value);
                    break;
                case EGValueType.GInt:
                    ((GVS_Int)_obj.GValueValue).value = EditorGUI.IntField(_rect, ((GVS_Int)_obj.GValueValue).value);
                    break;
                case EGValueType.GFloat:
                    ((GVS_Float)_obj.GValueValue).value = EditorGUI.FloatField(_rect, ((GVS_Float)_obj.GValueValue).value);
                    break;
                case EGValueType.GString:
                    ((GVS_String)_obj.GValueValue).value = EditorGUI.TextField(_rect, ((GVS_String)_obj.GValueValue).value);
                    break;
                case EGValueType.GEnum:
                    ((GVS_Enum)_obj.GValueValue).value = 
                        (byte)EditorGUI.Popup(_rect,((GVS_Enum)_obj.GValueValue).value, ResourcesWindow.Instance.EnumNames[_obj.GValueIndexID].names.ToArray());
                    break;
                case EGValueType.GPoint:
                    ((GVS_Enum)_obj.GValueValue).value = 
                        (byte)EditorGUI.Popup(_rect,((GVS_Enum)_obj.GValueValue).value, ResourcesWindow.Instance.PointNames[_obj.GValueIndexID].names.ToArray());
                    break;
                case EGValueType.GTransform:
                    ((GVS_Enum)_obj.GValueValue).value = 
                        (byte)EditorGUI.Popup(_rect,((GVS_Enum)_obj.GValueValue).value, ResourcesWindow.Instance.TransNames[_obj.GValueIndexID].names.ToArray());
                    break;
                case EGValueType.GUnit:
                    ((GVS_Enum)_obj.GValueValue).value = 
                    (byte)EditorGUI.Popup(_rect,((GVS_Enum)_obj.GValueValue).value, ResourcesWindow.Instance.UnitNames[_obj.GValueIndexID].names.ToArray());
                    break;
                // case EGValueType.GColor:
                //     return EditorGUI.ColorField(_rect, (Color)_obj);
                // case EGValueType.GVector2:
                //     return EditorGUI.Vector2Field(_rect, "", (Vector2)_obj);
                // case EGValueType.GVector3:
                //     return EditorGUI.Vector3Field(_rect, "", (Vector3)_obj);
                // case EGValueType.GQuaternion:
                //     return
                //         Quaternion.Euler(EditorGUI.Vector3Field(_rect, "",
                //             ((Quaternion)_obj).eulerAngles));
                // case EGValueType.GUnit:
                //     // _gValue.DefaultValue = EditorGUILayout.ColorField((Color)_gValue.DefaultValue);
                //     break;
                // case EGValueType.GTransform:
                //     // _gValue.DefaultValue = EditorGUILayout.ColorField((Color)_gValue.DefaultValue);
                //     break;
            }
        }
        private static void DrawGvalueElement(Rect _rect, GValue_RatioPart _obj)
        {
            List<string> _list = ResourcesWindow.Instance.GetGValueName(_obj.GValueType);
            
            int _fhRectWindth = 30;//对比符号的宽度
            int _swRectWindth = 20;//切换按钮的宽度
            float _enumRectWidth = (_rect.width - _fhRectWindth) * 0.5f - _swRectWindth;
            
            Rect _swRangeRect_l = new Rect(_rect);//切换按钮1
            _swRangeRect_l.width = _swRectWindth;
            
            Rect _enumRangeRect_l = new Rect(_rect);
            _enumRangeRect_l.width = _enumRectWidth;
            _enumRangeRect_l.x = _swRangeRect_l.x + _swRangeRect_l.width;
            
            Rect _fhRangeRect = new Rect(_rect);
            _fhRangeRect.width = _fhRectWindth;
            _fhRangeRect.x =_enumRangeRect_l.x + _enumRangeRect_l.width;
            
            Rect _swRangeRect_r = new Rect(_rect);//切换按钮2
            _swRangeRect_r.width = _swRectWindth;
            _swRangeRect_r.x = _fhRangeRect.x + _fhRangeRect.width;
            
            Rect _enumRangeRect_r = new Rect(_rect);
            _enumRangeRect_r.width = _enumRectWidth;
            _enumRangeRect_r.x = _swRangeRect_r.x + _swRangeRect_r.width;

            Action<Rect,GValue> _drawGValue = (_nowRect,_nowGValue) =>
            {
                if (!_nowGValue.mType)
                {
                    switch (_obj.GValueType)
                    {
                        case EGValueType.GBool:
                            ((GBool)_nowGValue).mSerValue = EditorGUI.Toggle(_nowRect, ((GBool)_nowGValue).mSerValue);
                            break;
                        case EGValueType.GInt:
                            ((GInt)_nowGValue).mSerValue = EditorGUI.IntField(_nowRect, ((GInt)_nowGValue).mSerValue);
                            break;
                        case EGValueType.GFloat:
                            ((GFloat)_nowGValue).mSerValue = EditorGUI.FloatField(_nowRect,((GFloat)_nowGValue).mSerValue);
                            break;
                        case EGValueType.GString:
                            ((GString)_nowGValue).mSerValue = EditorGUI.TextField(_nowRect,((GString)_nowGValue).mSerValue);
                            break;
                    }
                }
                else
                {
                    if (_list is null)
                    {
                        GUI.Label(_nowRect, "GValue列表未配置此参数");
                    }
                    else
                    {
                        _nowRect.y += 1;
                        _nowGValue.mValueIndex =
                            (ushort)EditorGUI.Popup(_nowRect, _nowGValue.mValueIndex, _list.ToArray());
                        
                    }
                }
            };
            
            Action<Rect,GValue> _drawEnum = (_nowRect, _nowGValue) =>
            {
                if (GUI.Button(_nowRect, _nowGValue.mType ? "G" : "L"))
                {
                    _nowGValue.mType = !_nowGValue.mType;
                }
            };
            
            string[] GvaluesFH = GValue_fh1;
            if (_obj.GValueType == EGValueType.GInt) GvaluesFH = GValue_fh3;
            if (_obj.GValueType == EGValueType.GFloat) GvaluesFH = GValue_fh2;
            if (_obj.GValueType == EGValueType.GTransform || _obj.GValueType == EGValueType.GUnit ||
                _obj.GValueType == EGValueType.GPoint) GvaluesFH = GValue_fh4;
            if (_obj.GValueType == EGValueType.GEnum)
            {
                _swRangeRect_l.width += _enumRangeRect_l.width;
                _swRangeRect_r.width += _enumRangeRect_r.width;

                _obj.GValue_L.mType = true;
                if (_list is null)
                {
                    GUI.Label(_swRangeRect_l, "GValue列表未配置此参数");
                    return;
                }
                else
                {
                    _swRangeRect_l.y += 1;
                    _obj.GValue_L.mValueIndex =
                        (ushort)EditorGUI.Popup(_swRangeRect_l, _obj.GValue_L.mValueIndex, _list.ToArray());
                        
                }
                _obj.Ratio = (byte)EditorGUI.Popup(_fhRangeRect,_obj.Ratio,GvaluesFH,EditorStyles.label);
                _obj.GValue_R.mType = false;
                ((GEnum)_obj.GValue_R).mSerValue = (byte)EditorGUI.Popup(_swRangeRect_r,
                    ((GEnum)_obj.GValue_R).mSerValue,
                    ResourcesWindow.Instance.EnumNames[_obj.GValue_L.mValueIndex].names.ToArray());
            }else if (_obj.GValueType == EGValueType.GTransform || _obj.GValueType == EGValueType.GPoint || _obj.GValueType == EGValueType.GUnit)
            {
                _swRangeRect_l.width += _enumRangeRect_l.width;
                _swRangeRect_r.x = _swRangeRect_l.x + _swRangeRect_l.width;
                _swRangeRect_r.width += _enumRangeRect_r.width;
                _fhRangeRect.x = _swRangeRect_r.x + _swRangeRect_r.width;
                if (_list is null)
                {
                    GUI.Label(_swRangeRect_l, "GValue列表未配置此参数");
                    return;
                }
                _swRangeRect_l.y += 1;
                _obj.GValue_L.mValueIndex =
                    (ushort)EditorGUI.Popup(_swRangeRect_l, _obj.GValue_L.mValueIndex, _list.ToArray());

                if (_obj.GValueType == EGValueType.GTransform)
                {
                    ((GTransform)_obj.GValue_L).mSerValue = (byte)EditorGUI.Popup(_swRangeRect_r,
                        ((GTransform)_obj.GValue_L).mSerValue,
                        ResourcesWindow.Instance.TransNames[_obj.GValue_L.mValueIndex].names.ToArray());
                }else if (_obj.GValueType == EGValueType.GPoint)
                {
                    ((GPoint)_obj.GValue_L).mSerValue = (byte)EditorGUI.Popup(_swRangeRect_r,
                        ((GPoint)_obj.GValue_L).mSerValue,
                        ResourcesWindow.Instance.PointNames[_obj.GValue_L.mValueIndex].names.ToArray());
                }else if (_obj.GValueType == EGValueType.GUnit)
                {
                    ((GUnit)_obj.GValue_L).mSerValue = (byte)EditorGUI.Popup(_swRangeRect_r,
                        ((GUnit)_obj.GValue_L).mSerValue,
                        ResourcesWindow.Instance.UnitNames[_obj.GValue_L.mValueIndex].names.ToArray());
                }

                
                _obj.Ratio = (byte)EditorGUI.Popup(_fhRangeRect,_obj.Ratio,GvaluesFH,EditorStyles.label);
            }
            else
            {
                _drawEnum(_swRangeRect_l, _obj.GValue_L);
                _drawGValue(_enumRangeRect_l, _obj.GValue_L);
                _obj.Ratio = (byte)EditorGUI.Popup(_fhRangeRect, _obj.Ratio, GvaluesFH, EditorStyles.label);
                _drawEnum(_swRangeRect_r, _obj.GValue_R);
                _drawGValue(_enumRangeRect_r, _obj.GValue_R);
            }
        }
        private static GValue GvalueInit(EGValueType _gValueType, bool _isGV = false)
        {
            switch (_gValueType)
            {
                case EGValueType.GBool:
                    return new GBool(false,_isGV);
                case EGValueType.GInt:
                    return new GInt(0,_isGV);
                case EGValueType.GFloat:
                    return new GFloat(0.0f,_isGV);
                case EGValueType.GString:
                    return new GString("",_isGV);
                case EGValueType.GEnum:
                    return new GEnum(0,_isGV);
                case EGValueType.GPoint:
                    return new GPoint(0);
                case EGValueType.GTransform:
                    return new GTransform(0);
                case EGValueType.GUnit:
                    return new GUnit();
            }
            return null;
        }
        
        private static GVS GVSInit(EGValueType _gValueType, bool _isGV = false)
        {
            switch (_gValueType)
            {
                case EGValueType.GBool:
                    return new GVS_Bool(false);
                case EGValueType.GInt:
                    return new GVS_Int(0);
                case EGValueType.GFloat:
                    return new GVS_Float(0.0f);
                case EGValueType.GString:
                    return new GVS_String("");
                case EGValueType.GEnum:
                case EGValueType.GPoint:
                case EGValueType.GTransform:
                case EGValueType.GUnit:
                    return new GVS_Enum(0);

            }
            return null;
        }
        #endregion
        #endregion

        #region DrawSetGValue

        private static void DrawTrans(object obj, string propertyName, GTransform val)
        {
            string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GTransform).ToArray();
            val.mValueIndex = (ushort)EditorGUILayout.Popup(val.mValueIndex, names);
            val.mSerValue = (byte)EditorGUILayout.Popup(val.mSerValue, ResourcesWindow.Instance.TransNames[val.mValueIndex].names.ToArray());
            SetProperty(obj, propertyName, val);
        }

        private static void DrawSetGBool(object obj, string propertyName, GValue_SetBool val)
        {//将GValue设定为XX值
            using (new GUIColorScope(val.m_IsSet ? Color.white : Color.gray))
            {
                val.m_IsSet = EditorGUILayout.Toggle(val.m_IsSet, GUILayout.Width(20));
                if (ResourcesWindow.Instance.GetGValueName(EGValueType.GBool) is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        GUILayout.Label("Gvalue缺失 GBool 参数！！");
                    }
                    return;
                }
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GBool).ToArray();

                val.m_Value.mValueIndex = (ushort)EditorGUILayout.Popup(val.m_Value.mValueIndex, names);
                SetProperty(obj, propertyName, val);
            }
        }
        private static void DrawSetGInt(object obj, string propertyName, GValue_SetInt val)
        {//将GValue设定为XX值
            using (new GUIColorScope(val.m_IsSet ? Color.white : Color.gray))
            {
                val.m_IsSet = EditorGUILayout.Toggle(val.m_IsSet, GUILayout.Width(20));
                if (ResourcesWindow.Instance.GetGValueName(EGValueType.GPoint) is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        GUILayout.Label("Gvalue缺失 GBool 参数！！");
                    }
                    return;
                }
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GPoint).ToArray();

                val.m_Value.mValueIndex = (ushort)EditorGUILayout.Popup(val.m_Value.mValueIndex, names);
                SetProperty(obj, propertyName, val);
            }
        }
        private static void DrawSetGFloat(object obj, string propertyName, GValue_SetFloat val)
        {//将GValue设定为XX值
            using (new GUIColorScope(val.m_IsSet ? Color.white : Color.gray))
            {
                val.m_IsSet = EditorGUILayout.Toggle(val.m_IsSet, GUILayout.Width(20));
                if (ResourcesWindow.Instance.GetGValueName(EGValueType.GFloat) is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        GUILayout.Label("Gvalue缺失 GBool 参数！！");
                    }
                    return;
                }
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GFloat).ToArray();

                val.m_Value.mValueIndex = (ushort)EditorGUILayout.Popup(val.m_Value.mValueIndex, names);
                SetProperty(obj, propertyName, val);
            }
        }
        private static void DrawSetGString(object obj, string propertyName, GValue_SetString val)
        {//将GValue设定为XX值
            using (new GUIColorScope(val.m_IsSet ? Color.white : Color.gray))
            {
                val.m_IsSet = EditorGUILayout.Toggle(val.m_IsSet, GUILayout.Width(20));
                if (ResourcesWindow.Instance.GetGValueName(EGValueType.GString) is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        GUILayout.Label("Gvalue缺失 GBool 参数！！");
                    }
                    return;
                }
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GString).ToArray();

                val.m_Value.mValueIndex = (ushort)EditorGUILayout.Popup(val.m_Value.mValueIndex, names);
                SetProperty(obj, propertyName, val);
            }
        }
        private static void DrawSetGPoint(object obj, string propertyName, GValue_SetPoint val)
        {//将GValue设定为XX值
            using (new GUIColorScope(val.m_IsSet ? Color.white : Color.gray))
            {
                val.m_IsSet = EditorGUILayout.Toggle(val.m_IsSet, GUILayout.Width(20));
                if (ResourcesWindow.Instance.GetGValueName(EGValueType.GPoint) is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        GUILayout.Label("Gvalue缺失 GPoint 参数！！");
                    }
                    return;
                }
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GPoint).ToArray();

                val.m_Value.mValueIndex = (ushort)EditorGUILayout.Popup(val.m_Value.mValueIndex, names);
                val.m_Value.mSerValue = (byte)EditorGUILayout.Popup(val.m_Value.mSerValue, ResourcesWindow.Instance.PointNames[val.m_Value.mValueIndex].names.ToArray());

                SetProperty(obj, propertyName, val);
            }
        }
        private static void DrawSetGUnit(object obj, string propertyName, GValue_SetUnit val)
        {//将GValue设定为XX值
            using (new GUIColorScope(val.m_IsSet ? Color.white : Color.gray))
            {
                val.m_IsSet = EditorGUILayout.Toggle(val.m_IsSet, GUILayout.Width(20));
                List<string> nameList = ResourcesWindow.Instance.GetGValueName(EGValueType.GUnit);
                if (nameList is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        GUILayout.Label("Gvalue缺失 GUnit 参数！！");
                    }
                    return;
                }
                string[] names = nameList.ToArray();

                val.m_Value.mValueIndex = (ushort)EditorGUILayout.Popup(val.m_Value.mValueIndex, names);
                val.m_Value.mSerValue = (byte)EditorGUILayout.Popup(val.m_Value.mSerValue, ResourcesWindow.Instance.UnitNames[val.m_Value.mValueIndex].names.ToArray());

                SetProperty(obj, propertyName, val);
            }
        }
        private static void DrawSetGTransform(object obj, string propertyName, GValue_SetTransform val)
        {//将GValue设定为XX值
            using (new GUIColorScope(val.m_IsSet ? Color.white : Color.gray))
            {
                val.m_IsSet = EditorGUILayout.Toggle(val.m_IsSet, GUILayout.Width(20));
                List<string> nameList = ResourcesWindow.Instance.GetGValueName(EGValueType.GTransform);
                if (nameList is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        GUILayout.Label("Gvalue缺失 GTransform 参数！！");
                    }
                    return;
                }
                string[] names = nameList.ToArray();

                val.m_Value.mValueIndex = (ushort)EditorGUILayout.Popup(val.m_Value.mValueIndex, names);
                val.m_Value.mSerValue = (byte)EditorGUILayout.Popup(val.m_Value.mSerValue, ResourcesWindow.Instance.TransNames[val.m_Value.mValueIndex].names.ToArray());

                SetProperty(obj, propertyName, val);
            }
        }
        private static void DrawSetGBool(Rect _rect, string propertyName, GValue_SetBool val)
        {//将GValue设定为XX值
            float width = _rect.width - 90;
            _rect.width = 70;
            GUI.Label(_rect, new GUIContent("SetGBool: ", propertyName));

            using (new GUIColorScope(val.m_IsSet ? Color.white : Color.gray))
            {
                _rect.x += _rect.width;
                _rect.width = 20;
                val.m_IsSet = EditorGUI.Toggle(_rect, val.m_IsSet);
                if (ResourcesWindow.Instance.GetGValueName(EGValueType.GBool) is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        _rect.x += _rect.width;
                        _rect.width = width;
                        GUI.Label(_rect, "Gvalue缺失 GBool 参数！！");
                    }
                    return;
                }
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GBool).ToArray();
                _rect.x += _rect.width;
                _rect.width = width;
                val.m_Value.mValueIndex = (ushort)EditorGUI.Popup(_rect, val.m_Value.mValueIndex, names);
            }
        }
        private static void DrawSetGFloat(Rect _rect, string propertyName, GValue_SetFloat val)
        {//将GValue设定为XX值
            float width = _rect.width - 90;
            _rect.width = 70;
            GUI.Label(_rect, new GUIContent("SetGFloat: ", propertyName));

            using (new GUIColorScope(val.m_IsSet ? Color.white : Color.gray))
            {
                _rect.x += _rect.width;
                _rect.width = 20;
                val.m_IsSet = EditorGUI.Toggle(_rect, val.m_IsSet);
                if (ResourcesWindow.Instance.GetGValueName(EGValueType.GFloat) is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        _rect.x += _rect.width;
                        _rect.width = width;
                        GUI.Label(_rect, "Gvalue缺失 GFloat 参数！！");
                    }
                    return;
                }
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GFloat).ToArray();
                _rect.x += _rect.width;
                _rect.width = width;
                val.m_Value.mValueIndex = (ushort)EditorGUI.Popup(_rect, val.m_Value.mValueIndex, names);
            }
        }
        private static void DrawSetGString(Rect _rect, string propertyName, GValue_SetString val)
        {//将GValue设定为XX值
            float width = _rect.width - 90;
            _rect.width = 70;
            GUI.Label(_rect, new GUIContent("SetGString: ", propertyName));

            using (new GUIColorScope(val.m_IsSet ? Color.white : Color.gray))
            {
                _rect.x += _rect.width;
                _rect.width = 20;
                val.m_IsSet = EditorGUI.Toggle(_rect, val.m_IsSet);
                if (ResourcesWindow.Instance.GetGValueName(EGValueType.GString) is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        _rect.x += _rect.width;
                        _rect.width = width;
                        GUI.Label(_rect, "Gvalue缺失 GString 参数！！");
                    }
                    return;
                }
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GString).ToArray();
                _rect.x += _rect.width;
                _rect.width = width;
                val.m_Value.mValueIndex = (ushort)EditorGUI.Popup(_rect, val.m_Value.mValueIndex, names);
            }
        }
        private static void DrawSetGPoint(Rect rect, string propertyName, GValue_SetPoint val)
        {//将GValue设定为XX值
            float width = rect.width;
            rect.width = 70;
            GUI.Label(rect, new GUIContent("SetGPoint: ", propertyName));

            using (new GUIColorScope(val.m_IsSet ? Color.white : Color.gray))
            {
                rect.x += rect.width;
                rect.width = 20;
                val.m_IsSet = EditorGUI.Toggle(rect, val.m_IsSet);
                if (ResourcesWindow.Instance.GetGValueName(EGValueType.GPoint) is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        rect.x += rect.width;
                        rect.width = width - rect.x;
                        GUI.Label(rect, "Gvalue缺失 GPoint 参数！！");
                    }
                    return;
                }
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GPoint).ToArray();

                rect.x += rect.width;
                rect.width = (width - rect.x) * 0.5f;
                val.m_Value.mValueIndex = (ushort)EditorGUI.Popup(rect, val.m_Value.mValueIndex, names);
                rect.x += rect.width;
                val.m_Value.mSerValue = (byte)EditorGUI.Popup(rect, val.m_Value.mSerValue, ResourcesWindow.Instance.PointNames[val.m_Value.mValueIndex].names.ToArray());
            }
        }
        private static void DrawSetGTransform(Rect rect, string propertyName, GValue_SetTransform val)
        {
            float Width = rect.width;
            rect.width = 70;
            GUI.Label(rect,new GUIContent("SetGTrans: ",propertyName));
            using (new GUIColorScope(val.m_IsSet ? Color.white : Color.gray))
            {

                
                rect.x += rect.width;
                rect.width = 20;
                val.m_IsSet = EditorGUI.Toggle(rect, val.m_IsSet);
                List<string> nameList = ResourcesWindow.Instance.GetGValueName(EGValueType.GTransform);
                if (nameList is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        rect.x += rect.width;
                        rect.width = Width - rect.x;
                        GUI.Label(rect, "Gvalue缺失 GTransform 参数！！");
                    }
                    return;
                }
                string[] names = nameList.ToArray();

                rect.x += rect.width;
                rect.width = (Width - rect.x) * 0.5f;
                val.m_Value.mValueIndex = (ushort)EditorGUI.Popup(rect, val.m_Value.mValueIndex, names);
                rect.x += rect.width;
                val.m_Value.mSerValue = (byte)EditorGUI.Popup(rect, val.m_Value.mSerValue, ResourcesWindow.Instance.TransNames[val.m_Value.mValueIndex].names.ToArray());
            }
        }

        private static void DrawEVector3(Rect rect, string propertyName, EVector3 val)
        {
            float halfWidth = (rect.width - 90);
            rect.width = 90;
            GUI.Label(rect,propertyName);
            rect.x += rect.width;
            rect.width = halfWidth;
            val.SetValue(EditorGUI.Vector3Field(rect,"",val.GetValue()));
        }
        private static float DrawFloat(Rect rect, string propertyName, float val)
        {
            float halfWidth = (rect.width - 90);
            rect.width = 90;
            GUI.Label(rect,propertyName);
            rect.x += rect.width;
            rect.width = halfWidth;
            return EditorGUI.FloatField(rect, val);
            // val.SetValue(EditorGUI.Vector3Field(rect,"",val.GetValue()));
        }
        private static int DrawLayerField(Rect _rect, string propertyName, int val, int width = 90)
        {
            int layerInt = val;
            
            Rect _layerRect = new Rect(_rect);
            
            _layerRect.width = width;
            GUI.Label(_layerRect,propertyName);
            
            _layerRect.x += _layerRect.width;
            _layerRect.width = 150;
            //层级遮罩切换
            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                int _selectID = layerInt < 0 ? 0 : 1;
                int _selectIDB = GUI.Toolbar(_layerRect, _selectID, 
                    new[] { "自层级组件", "自场景层级" });
                if (_check.changed)
                {
                    if (_selectID != _selectIDB)
                    {
                        layerInt = _selectIDB - 1;
                        return layerInt;
                        // SetProperty(obj, propertyName, layerInt);
                    }
                }
            }

            _layerRect.x += _layerRect.width;
            _layerRect.width = _rect.width - _layerRect.x;
            
            //层级下拉菜单绘制
            if (layerInt < 0)
            {//选择组件中的层级
                if (ResourcesWindow.Instance.GetRole() != null)
                {
                    ActionEditor_LayerMask _layerMask =
                        ResourcesWindow.Instance.GetRole().GetComponent<ActionEditor_LayerMask>();

                    if (_layerMask != null)
                    {
                        if (layerInt >= _layerMask.mLayerName.Length)
                        {
                            layerInt = _layerMask.mLayerName.Length - 1;
                        }

                        using (var _check = new EditorGUI.ChangeCheckScope())
                        {
                            int layerInt2 = EditorGUI.Popup(_layerRect, -layerInt - 1, _layerMask.mLayerName);
                            if (_check.changed)
                            {
                                // SetProperty(obj, propertyName, -layerInt2 - 1);
                                return -layerInt2 - 1;
                            }
                        }
                    }
                    else
                    {
                        EditorGUI.TextField(_layerRect,"预览的角色未添加【ActionEditor_LayerMask】组件");
                    }
                }
                else
                {
                    EditorGUI.TextField(_layerRect, "预览的角色未添加【ActionEditor_LayerMask】组件");
                }
            }
            else
            {//直接选择层级
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    LayerMask _mask = EditorGUI.MaskField(
                        _layerRect, InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerInt),
                        InternalEditorUtility.layers);
                    layerInt = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(_mask);
                    if (_check.changed)
                    {
                        // SetProperty(obj, propertyName, layerInt);
                        return layerInt;
                    }
                }
            }

            return val;
            // //尾部空间
            // GUILayout.Space(5);
        }

        public static void OnDrawSetGTransform(GValue_SetTransform val, bool _isSet)
        {//将GValue设定为XX值
            using (new GUIColorScope(_isSet ? Color.white : Color.gray))
            {
                val.m_IsSet = EditorGUILayout.Toggle(val.m_IsSet, GUILayout.Width(20));
                List<string> nameList = ResourcesWindow.Instance.GetGValueName(EGValueType.GTransform);
                if (nameList is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        GUILayout.Label("Gvalue缺失 GTransform 参数！！");
                    }
                    return;
                }
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GTransform).ToArray();

                val.m_Value.mValueIndex = (ushort)EditorGUILayout.Popup(val.m_Value.mValueIndex, names);
                val.m_Value.mSerValue = (byte)EditorGUILayout.Popup(val.m_Value.mSerValue, ResourcesWindow.Instance.TransNames[val.m_Value.mValueIndex].names.ToArray());
            }
        }
        public static void OnDrawSetGTransform(GValue_SetTransform val)
        {//将GValue设定为XX值
            // using (new GUIColorScope(_isSet ? Color.white : Color.gray))
            {
                val.m_IsSet = true;
                List<string> nameList = ResourcesWindow.Instance.GetGValueName(EGValueType.GTransform);
                if (nameList is null)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        GUILayout.Label("Gvalue缺失 GTransform 参数！！");
                    }
                    return;
                }
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GTransform).ToArray();

                val.m_Value.mValueIndex = (ushort)EditorGUILayout.Popup(val.m_Value.mValueIndex, names);
                val.m_Value.mSerValue = (byte)EditorGUILayout.Popup(val.m_Value.mSerValue, ResourcesWindow.Instance.TransNames[val.m_Value.mValueIndex].names.ToArray());
            }
        }
        #endregion
        public static void DrawGValueInspector(EditorEngineGValuePart _gValue)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("默认值：", GUILayout.Width(60));
                switch (_gValue.ValueType)
                {
                    case EGValueType.GBool:
                        ((GVS_Bool)_gValue.DefaultValue).value = EditorGUILayout.Toggle(((GVS_Bool)_gValue.DefaultValue).value);
                        break;
                    case EGValueType.GInt:
                        ((GVS_Int)_gValue.DefaultValue).value = EditorGUILayout.DelayedIntField(((GVS_Int)_gValue.DefaultValue).value);
                        break;
                    case EGValueType.GFloat:
                        ((GVS_Float)_gValue.DefaultValue).value = EditorGUILayout.DelayedFloatField(((GVS_Float)_gValue.DefaultValue).value);
                        break;
                    case EGValueType.GString:
                        ((GVS_String)_gValue.DefaultValue).value = EditorGUILayout.DelayedTextField(((GVS_String)_gValue.DefaultValue).value);
                        break;
                    case EGValueType.GEnum:
                        ((GVS_Enum)_gValue.DefaultValue).value = (byte)EditorGUILayout.Popup(
                            ((GVS_Enum)_gValue.DefaultValue).value,
                            ResourcesWindow.Instance.EnumNames[_gValue.ValueID].names.ToArray());
                        break;
                    case EGValueType.GPoint:
                        ((GVS_Enum)_gValue.DefaultValue).value = (byte)EditorGUILayout.Popup(
                            ((GVS_Enum)_gValue.DefaultValue).value,
                            ResourcesWindow.Instance.PointNames[_gValue.ValueID].names.ToArray());
                        break;
                    case EGValueType.GTransform:
                        ((GVS_Enum)_gValue.DefaultValue).value = (byte)EditorGUILayout.Popup(
                            ((GVS_Enum)_gValue.DefaultValue).value,
                            ResourcesWindow.Instance.TransNames[_gValue.ValueID].names.ToArray());
                        break;
                    case EGValueType.GUnit:
                        ((GVS_Enum)_gValue.DefaultValue).value = (byte)EditorGUILayout.Popup(
                            ((GVS_Enum)_gValue.DefaultValue).value,
                            ResourcesWindow.Instance.UnitNames[_gValue.ValueID].names.ToArray());
                        break;
                    // case EGValueType.GColor:
                    //     _gValue.DefaultValue = EditorGUILayout.ColorField((Color)_gValue.DefaultValue);
                    //     break;
                    // case EGValueType.GVector2:
                    //     _gValue.DefaultValue = EditorGUILayout.Vector2Field("", (Vector2)_gValue.DefaultValue);
                    //     break;
                    // case EGValueType.GVector3:
                    //     _gValue.DefaultValue = EditorGUILayout.Vector3Field("", (Vector3)_gValue.DefaultValue);
                    //     break;
                    // case EGValueType.GQuaternion:
                    //     _gValue.DefaultValue =
                    //         Quaternion.Euler(EditorGUILayout.Vector3Field("",
                    //             ((Quaternion)_gValue.DefaultValue).eulerAngles));
                    //     break;
                    // case EGValueType.GUnit:
                    //     // _gValue.DefaultValue = EditorGUILayout.ColorField((Color)_gValue.DefaultValue);
                    //     break;
                    // case EGValueType.GTransform:
                    //     // _gValue.DefaultValue = EditorGUILayout.ColorField((Color)_gValue.DefaultValue);
                    //     break;
                }
            }
        }

        private static void OnDrawEvector3(object obj, string propertyName, EVector3 val)
        {
            val.SetValue(EditorGUILayout.Vector3Field("", val.GetValue()));
            SetProperty(obj, propertyName, val);
        }
    }
}
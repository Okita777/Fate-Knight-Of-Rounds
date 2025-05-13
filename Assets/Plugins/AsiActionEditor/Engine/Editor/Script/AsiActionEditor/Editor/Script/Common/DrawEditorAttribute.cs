using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace AsiActionEngine.Editor
{
    public partial class DrawEditorAttribute
    {
        public static bool NeedInit = false;
        public static Dictionary<string, GameObject> LoadObjectDic = new Dictionary<string, GameObject>();
        public static List<string> _eventNames = new List<string>();

        public static void Draw(object obj, string[] filter = null, bool isContain = true)
        {
            _eventNames.Clear();
            if (filter is not null)
                _eventNames.AddRange(filter);
            Draw(obj, _eventNames, isContain);
        }
        public static void Draw(object obj, List<string> filter, bool isContain = true)
        {
            PropertyInfo[] pis = obj.GetType().GetProperties().OrderBy(p => p.MetadataToken).ToArray();
            for (int i = 0; i < pis.Length; ++i)
            {
                bool isDraw = true;
                if (filter.Count > 0)
                {
                    isDraw = filter.Contains(pis[i].Name) == isContain;
                }

                object[] attrs = pis[i].GetCustomAttributes(typeof(EditorPropertyAttribute), false);
                if (attrs.Length == 1 && isDraw)
                {
                    //主要变量
                    object val = GetProperty(obj, pis[i].Name);

                    EditorPropertyAttribute epa = (EditorPropertyAttribute)attrs[0];
                    // if (!string.IsNullOrEmpty(epa.Deprecated)) continue;

                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    if (epa.PropertyType != EditorPropertyType.EEPT_GValueSetting &&
                        epa.PropertyType != EditorPropertyType.EEPT_GValueSRatio &&
                        epa.PropertyType != EditorPropertyType.EEPT_GraphValue &&
                        epa.PropertyType != EditorPropertyType.EEPT_Float ||
                        (epa.PropertyType == EditorPropertyType.EEPT_Float && val is GFloat)
                       )
                    {
                        if (epa.LabelWidth > 0) 
                            GUILayout.Label(new GUIContent(epa.PropertyName, epa.Tooltip), GUILayout.Width(epa.LabelWidth));
                        else 
                            GUILayout.Label(new GUIContent(epa.PropertyName, epa.Tooltip)); //TooltipEvent("asd")

                        // Rect _tooltipRect = new Rect(0, 0, epa.LabelWidth, 12);
                        // using (new GUILayout.AreaScope(_tooltipRect))
                        // {
                        //     GUI.Label(_tooltipRect,epa.PropertyName);//TooltipEvent("asd")
                        //
                        //     if (!string.IsNullOrEmpty(epa.Tooltip))
                        //     {
                        //         if (_tooltipRect.Contains(Event.current.mousePosition))
                        //         {
                        //             GUI.tooltip = epa.Tooltip;
                        //         }
                        //     }
                        // }
                    }

                    // GUIStyle style = new GUIStyle();
                    
                    
                    bool onHorizontal = true;
                    // GUILayout.Label("pis[i].Name: " + pis[i].Name);

                    if (epa.Edit)
                    {
                        switch (epa.PropertyType)
                        {
                            case EditorPropertyType.EEPT_Bool:
                                if (val is GBool _gbool) DrawGbool(obj, pis[i].Name, _gbool);
                                else SetProperty(obj, pis[i].Name, GUILayout.Toggle((bool)val, ""));
                                break;
                            case EditorPropertyType.EEPT_Int:
                                if (val is GInt _gint) DrawGint(obj, pis[i].Name, _gint);
                                else SetProperty(obj, pis[i].Name, EditorGUILayout.DelayedIntField((int)val));
                                break;
                            case EditorPropertyType.EEPT_Float:
                                // GUIContent content = new GUIContent(epa.PropertyName);
                                if(val is GFloat _gfloat)DrawGfloat(obj, pis[i].Name, _gfloat,epa.PropertyName);
                                else SetProperty(obj, pis[i].Name, EditorGUILayout.FloatField(epa.PropertyName,(float)val));
                                break;
                            case EditorPropertyType.EEPT_String:
                                if(val is GString _gString)DrawGstring(obj, pis[i].Name, _gString);
                                else SetProperty(obj, pis[i].Name, EditorGUILayout.DelayedTextField((string)val));
                                break;
                            case EditorPropertyType.EEPT_Vector2:
                                SetProperty(obj, pis[i].Name, EditorGUILayout.Vector2Field("", (Vector2)val));
                                break;
                            case EditorPropertyType.EEPT_Vector3:
                                if(val is EVector3 _eVector3)OnDrawEvector3(obj, pis[i].Name, _eVector3);
                                else
                                {
                                    // GUILayout.Space(5);
                                    // SetProperty(obj, pis[i].Name, EditorGUILayout.Vector3Field("", (Vector3)val));
                                    GUILayout.Label("Vector3 无法直接序列化， 请使用 EVector3 类型");
                                    // GUILayout.Space(5);
                                }
                                break;
                            case EditorPropertyType.EEPT_Vector4:
                                SetProperty(obj, pis[i].Name, EditorGUILayout.Vector4Field("", (Vector4)val));
                                break;

                            case EditorPropertyType.EEPT_Color:
                                SetProperty(obj, pis[i].Name, EditorGUILayout.ColorField((Color)val));
                                break;
                            case EditorPropertyType.EEPT_Quaternion:
                                {
                                    Quaternion q = (Quaternion)val;
                                    q.eulerAngles = EditorGUILayout.Vector3Field("", q.eulerAngles);
                                    SetProperty(obj, pis[i].Name, q);
                                }
                                break;

                            case EditorPropertyType.EEPT_Enum:
                                // DrawGenum(obj, pis[i].Name, (GEnum)val);
                                if(val is GEnum _gEnum)DrawGenum(obj, pis[i].Name, _gEnum, epa.EnumNames);
                                else SetProperty(obj, pis[i].Name, EditorGUILayout.EnumPopup((Enum)val));
                                break;
                            case EditorPropertyType.EEPT_LayerMask:
                                LayerField(obj, pis[i].Name, val);
                                break;
                            case EditorPropertyType.EEPT_AnimationCurve:
                                SetProperty(obj, pis[i].Name, EditorGUILayout.CurveField((AnimationCurve)val));
                                // LayerField(obj, pis[i].Name, val);
                                break;
                            case EditorPropertyType.EEPT_GameObject:
                                GameObjectField(obj, pis[i].Name, val);
                                break;
                            case EditorPropertyType.EEPT_GPoint:
                                DrawGpoint(obj, pis[i].Name, (GPoint)val);
                                break;
                            case EditorPropertyType.EEPT_GTransform:
                                DrawTrans(obj, pis[i].Name, (GTransform)val);
                                break;
                            case EditorPropertyType.EEPT_SelectTransform:
                                SelectTransform(obj, pis[i].Name, (SelectTransform)val);
                                break;
                            case EditorPropertyType.EEPT_SetGBool:
                                DrawSetGBool(obj, pis[i].Name, (GValue_SetBool)val);
                                break;
                            case EditorPropertyType.EEPT_SetGInt:
                                DrawSetGInt(obj, pis[i].Name, (GValue_SetInt)val);
                                break;
                            case EditorPropertyType.EEPT_SetGFloat:
                                 DrawSetGFloat(obj, pis[i].Name, (GValue_SetFloat)val);
                                 break;
                            case EditorPropertyType.EEPT_SetGString:
                                 DrawSetGString(obj, pis[i].Name, (GValue_SetString)val);
                                 break;
                            
                            case EditorPropertyType.EEPT_SetGPoint:
                                 DrawSetGPoint(obj, pis[i].Name, (GValue_SetPoint)val);
                                 break;
                            case EditorPropertyType.EEPT_SetGTransform:
                                DrawSetGTransform(obj, pis[i].Name, (GValue_SetTransform)val);
                                break;
                            case EditorPropertyType.EEPT_SetGUnit:
                                DrawSetGUnit(obj, pis[i].Name, (GValue_SetUnit)val);
                                break;

                            //单位状态标签
                            case EditorPropertyType.EEPT_ActionLable:
                                ActionLableField(obj, pis[i].Name, val);
                                break;
                            case EditorPropertyType.EEPT_CharacteLimbType:
                                LimbPointType(obj, pis[i].Name, val);
                                break;
                            case EditorPropertyType.EEPT_Camera:
                                CameraList(obj, pis[i].Name, val);
                                break;
                            case EditorPropertyType.EEPT_GValueSetting:
                                onHorizontal = false;
                                DrawGValueSetting(obj, pis[i].Name, (GValue_Setting)val, epa.PropertyName);
                                break;//GValue_Ratio
                            case EditorPropertyType.EEPT_GValueSRatio:
                                onHorizontal = false;
                                DrawGvalueRatio(obj, pis[i].Name, (GValue_Ratio)val, epa.PropertyName, epa.LabelWidth == 0);
                                break;
                            case EditorPropertyType.EEPT_GraphValue:
                                onHorizontal = false;
                                GUILayout.EndHorizontal();
                                DrawGraphEditorAttribute.Instance.DrawGraphButton(pis[i].Name, val, epa);
                                break;
                        }
                    }
                    else
                    {
                        string sz = (val == null ? string.Empty : val.ToString());
                        GUILayout.Label(sz, GUILayout.Width(epa.LabelWidth));
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        TimeLineWindow.Instance.Repaint();
                    }
                    if(onHorizontal) GUILayout.EndHorizontal();
                }

            }

            NeedInit = false;
        }
        
        private static object GetProperty(object obj, string propertyName)
        {
            return obj.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, obj, null);
        }

        private static void SetProperty(object obj, string propertyName, object newValue)
        {
            obj.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, obj, new object[] { newValue });
        }

        private static void LayerField(object obj, string propertyName, object val)
        {
            int layerInt = (int)val;
            
            //层级遮罩切换
            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                int _selectID = layerInt < 0 ? 0 : 1;
                int _selectIDB = GUILayout.Toolbar(_selectID, 
                    new[] { "自层级组件", "自场景层级" }, GUILayout.Width(150));
                if (_check.changed)
                {
                    if (_selectID != _selectIDB)
                    {
                        layerInt = _selectIDB - 1;
                        SetProperty(obj, propertyName, layerInt);
                    }
                }
            }
            
            //层级下拉菜单绘制
            if (layerInt < 0)
            {//选择组件中的层级
                if (ResourcesWindow.Instance.GetRole() != null)
                {
                    // ActionEditor_LayerMask _layerMask =
                    //     ResourcesWindow.Instance.GetRole().GetComponent<ActionEditor_LayerMask>();

                    if (ResourcesWindow.Instance.GetRole().TryGetComponent(out ActionEditor_LayerMask _layerMask))
                    {
                        if (layerInt >= _layerMask.mLayerName.Length)
                        {
                            layerInt = _layerMask.mLayerName.Length - 1;
                        }

                        using (var _check = new EditorGUI.ChangeCheckScope())
                        {
                            int layerInt2 = EditorGUILayout.Popup(-layerInt - 1, _layerMask.mLayerName);
                            if (_check.changed)
                            {
                                SetProperty(obj, propertyName, -layerInt2 - 1);
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.TextField("预览的角色未添加【ActionEditor_LayerMask】组件");
                    }
                }
                else
                {
                    EditorGUILayout.TextField("预览的角色未添加【ActionEditor_LayerMask】组件");
                }
            }
            else
            {//直接选择层级
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    LayerMask _mask = EditorGUILayout.MaskField(
                        InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerInt),
                        InternalEditorUtility.layers);
                    layerInt = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(_mask);
                    if (_check.changed)
                    {
                        SetProperty(obj, propertyName, layerInt);
                    }
                }
            }
            
            // //尾部空间
            // GUILayout.Space(5);
        }

        private static void LayerComponetField(object obj, string propertyName, object val)
        {
            int layerInt = (int)val;
        }
        
        private static void GameObjectField(object obj, string propertyName, object val)
        {
            GameObject go = null;
            var szPath = (string)val;

            if (!string.IsNullOrEmpty(szPath))
            {
                if (LoadObjectDic.ContainsKey(szPath))
                {
                    go = LoadObjectDic[szPath];
                }
                else
                {
                    // EngineDebug.LogWarning("加载数据");
                    //todo: 临时的加载路径
                    go = AssetDatabase.LoadAssetAtPath<GameObject>(szPath);
                    // go = Resources.Load<GameObject>(szPath);
                    LoadObjectDic.Add(szPath, go);
                }
            }
            else
            {

            }

            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                go = EditorGUILayout.ObjectField(go, typeof(GameObject), false) as GameObject;
                if (_check.changed)
                {
                    string name = AssetDatabase.GetAssetPath(go);
                    // EngineDebug.Log($"预制体路径: {name}");
                    // string[] _nameChills = name.Split('/');
                    //
                    // //todo: 设置配置路径的限制
                    // // if (_nameChills.Length < 2 || _nameChills[1] != "Resources")
                    // // {
                    // //     EditorUtility.DisplayDialog("对象错误", "该资源必须位于 Resources 路径下", "我知道了");
                    // //     return;
                    // // }
                    //
                    // name = string.Empty;
                    // for (int i = 2; i < _nameChills.Length - 1; i++)
                    // {
                    //     name += _nameChills[i] + "/";
                    // }
                    // name += _nameChills[^1].Split('.')[0];
                    SetProperty(obj, propertyName, (go is null ? string.Empty : name));
                }
            }
        }
        private static void ActionLableField(object obj, string propertyName, object val)
        {
            int _selectId = (int)val;
            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                int _popup = EditorGUILayout.Popup(_selectId, ResourcesWindow.Instance.ActionLable.ToArray());
                if (_check.changed)
                {
                    SetProperty(obj, propertyName, _popup);
                }
            }
        }
        private static void CameraList(object obj, string propertyName, object val)
        {
            int _id = (int)val;

            GameObject _preCam = ResourcesWindow.Instance.PreCamera;
            if (_preCam)
            {
                Behaviour[] _behaviours = ResourcesWindow.Instance.PreCamControl.allCinemachine;
                string[] _cameraList = new string[_behaviours.Length];
                for (int i = 0; i < _behaviours.Length; i++)
                {
                    Transform _transform = _behaviours[i].transform;
                    _cameraList[i] = $"{_transform.parent.name}/{_transform.name}";
                }

                _id = EditorGUILayout.Popup(_id, _cameraList);
                
                SetProperty(obj, propertyName, _id);
            }
            else
            {
                using (new GUIColorScope(Color.red))
                {
                    GUILayout.Label("未创建相机实例，无法预览");
                }
            }
        }
        private static void LimbPointType(object obj, string propertyName, object val)
        {
            int _selectId = (int)val;

            //已配置的挂点
            List<string> _pointTypesStr = new List<string>();
            List<ECharacteLimbType> _poinTypes = new List<ECharacteLimbType>();
            int _selectID = 0;
            if (ResourcesWindow.Instance.TryGetCharacterConfig(out CharacterConfig _config))
            {
                ECharacteLimbType[] _ECLT = _config.HelpPointDic.Keys.ToArray();
                for (int i = 0; i < _ECLT.Length; i++)
                {
                    if ((int)_ECLT[i] == _selectId)
                    {
                        _selectID = i;
                    }
                    _pointTypesStr.Add(_ECLT[i].ToString());
                    _poinTypes.Add(_ECLT[i]);
                }
            }
            else
            {
                GUIStyle _style = new GUIStyle();
                _style.normal.textColor = Color.red;
                _style.alignment = TextAnchor.MiddleCenter;
                _style.fontSize = 24;
                GUILayout.Label("角色未挂载CharacterConfig组件");
                return;
            }

            if (_pointTypesStr.Count < 1)
            {
                GUILayout.Label("角色挂载了CharacterConfig组件，\n但是并未配置任何挂点");
                return;
            }
            
            //挂点选择
            _selectID = EditorGUILayout.Popup(_selectID, _pointTypesStr.ToArray());
            SetProperty(obj, propertyName, (int)_poinTypes[_selectID]);
        }

        private static void SelectTransform(object obj, string propertyName, SelectTransform val)
        {
            if (GUILayout.Button(val.m_IsCharacterLimb ? "L" : "G", GUILayout.Width(25)))
            {
                val.m_IsCharacterLimb = !val.m_IsCharacterLimb;
            }
            if (val.m_IsCharacterLimb)
            {
                //已配置的挂点
                List<string> _pointTypesStr = new List<string>();
                List<ECharacteLimbType> _poinTypes = new List<ECharacteLimbType>();
                int _selectID = 0;
                if (ResourcesWindow.Instance.TryGetCharacterConfig(out CharacterConfig _config))
                {
                    ECharacteLimbType[] _ECLT = _config.HelpPointDic.Keys.ToArray();
                    for (int i = 0; i < _ECLT.Length; i++)
                    {
                        if ((int)_ECLT[i] == val.m_Value)
                        {
                            _selectID = i;
                        }
                        _pointTypesStr.Add(_ECLT[i].ToString());
                        _poinTypes.Add(_ECLT[i]);
                    }
                }
                else
                {
                    GUIStyle _style = new GUIStyle();
                    _style.normal.textColor = Color.red;
                    _style.alignment = TextAnchor.MiddleCenter;
                    _style.fontSize = 24;
                    GUILayout.Label("角色未挂载CharacterConfig组件");
                    return;
                }

                if (_pointTypesStr.Count < 1)
                {
                    GUILayout.Label("角色挂载了CharacterConfig组件，\n但是并未配置任何挂点");
                    return;
                }
            
                //挂点选择
                _selectID = EditorGUILayout.Popup(_selectID, _pointTypesStr.ToArray());
                val.m_Value = (byte)_poinTypes[_selectID];
                // SetProperty(obj, propertyName, (int)_poinTypes[_selectID]);
            }
            else
            {
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GTransform).ToArray();
                val.m_Index = (ushort)EditorGUILayout.Popup(val.m_Index, names);
                val.m_Value = (byte)EditorGUILayout.Popup(val.m_Value, ResourcesWindow.Instance.TransNames[val.m_Index].names.ToArray());
            }
            SetProperty(obj, propertyName, val);

        }

        private static void DrawSelectTransform(Rect _rect, string propertyName, SelectTransform val)
        {
            float _referWidth = _rect.width - 90;
            
            _rect.width = 65;
            GUI.Label(_rect,new GUIContent("Refer:",propertyName));
            
            _rect.x += _rect.width;
            _rect.width = 25;
            if (GUI.Button(_rect,val.m_IsCharacterLimb ? "L" : "G"))
            {
                val.m_IsCharacterLimb = !val.m_IsCharacterLimb;
            }

            _rect.x += _rect.width;
            _rect.width = _referWidth;
            if (val.m_IsCharacterLimb)
            {
                //已配置的挂点
                List<string> _pointTypesStr = new List<string>();
                List<ECharacteLimbType> _poinTypes = new List<ECharacteLimbType>();
                int _selectID = 0;
                if (ResourcesWindow.Instance.TryGetCharacterConfig(out CharacterConfig _config))
                {
                    ECharacteLimbType[] _ECLT = _config.HelpPointDic.Keys.ToArray();
                    for (int i = 0; i < _ECLT.Length; i++)
                    {
                        if ((int)_ECLT[i] == val.m_Value)
                        {
                            _selectID = i;
                        }

                        _pointTypesStr.Add(_ECLT[i].ToString());
                        _poinTypes.Add(_ECLT[i]);
                    }
                }
                else
                {
                    GUIStyle _style = new GUIStyle();
                    _style.normal.textColor = Color.red;
                    _style.alignment = TextAnchor.MiddleCenter;
                    _style.fontSize = 24;
                    GUI.Label(_rect,"角色未挂载CharacterConfig组件");
                    return;
                }

                if (_pointTypesStr.Count < 1)
                {
                    GUI.Label(_rect, "角色挂载了CharacterConfig组件，\n但是并未配置任何挂点");
                    return;
                }

                //挂点选择
                _selectID = EditorGUI.Popup(_rect, _selectID, _pointTypesStr.ToArray());
                val.m_Value = (byte)_poinTypes[_selectID];
                // SetProperty(obj, propertyName, (int)_poinTypes[_selectID]);
            }
            else
            {
                string[] names = ResourcesWindow.Instance.GetGValueName(EGValueType.GTransform).ToArray();
                _rect.width = _referWidth * 0.5f;
                val.m_Index = (ushort)EditorGUI.Popup(_rect, val.m_Index, names);
                _rect.x += _rect.width;
                val.m_Value = (byte)EditorGUI.Popup(_rect, val.m_Value,
                    ResourcesWindow.Instance.TransNames[val.m_Index].names.ToArray());
            }
            // SetProperty(obj, propertyName, val);
        }

        public static void LimbPointType(Rect _rect, object obj, string propertyName, object val)
        {
            int _selectId = (int)val;

            //已配置的挂点
            List<string> _pointTypesStr = new List<string>();
            List<ECharacteLimbType> _poinTypes = new List<ECharacteLimbType>();
            int _selectID = 0;
            if (ResourcesWindow.Instance.TryGetCharacterConfig(out CharacterConfig _config))
            {
                ECharacteLimbType[] _ECLT = _config.HelpPointDic.Keys.ToArray();
                for (int i = 0; i < _ECLT.Length; i++)
                {
                    if ((int)_ECLT[i] == _selectId)
                    {
                        _selectID = i;
                    }

                    _pointTypesStr.Add(_ECLT[i].ToString());
                    _poinTypes.Add(_ECLT[i]);
                }
            }
            else
            {
                GUIStyle _style = new GUIStyle();
                _style.normal.textColor = Color.red;
                _style.alignment = TextAnchor.MiddleCenter;
                _style.fontSize = 24;
                GUI.Label(_rect, "角色未挂载CharacterConfig组件");
                return;
            }

            if (_pointTypesStr.Count < 1)
            {
                GUI.Label(_rect, "角色挂载了CharacterConfig组件，\n但是并未配置任何挂点");
                return;
            }

            //挂点选择
            _selectID = EditorGUI.Popup(_rect, _selectID, _pointTypesStr.ToArray());
            SetProperty(obj, propertyName, (int)_poinTypes[_selectID]);
        }
    }
}
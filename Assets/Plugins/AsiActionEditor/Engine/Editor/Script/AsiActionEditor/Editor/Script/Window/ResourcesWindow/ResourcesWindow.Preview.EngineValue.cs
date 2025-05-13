using System;
using System.Collections.Generic;
using System.Linq;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.GValueEquation;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class ResourcesWindow
    {
        public EditorEngineGValue EditorEngineGValue;
        private const string GValueSaveName = "GValue";
        public EditorEngineGValuePart CurSclect_GValue = null;
        public bool GValueIsChange = false;

        [NonSerialized] public List<SEnumName> EnumNames = new List<SEnumName>();
        [NonSerialized] public List<SEnumName> TransNames = new List<SEnumName>();
        [NonSerialized] public List<SEnumName> PointNames = new List<SEnumName>();
        [NonSerialized] public List<SEnumName> UnitNames = new List<SEnumName>();
        [NonSerialized] public List<int> RemoveIDs = new List<int>();
        private List<EditorEngineGValuePart> gvaluePart = new List<EditorEngineGValuePart>();
        private List<EditorEngineGValuePart> gvaluePart_Dis = new List<EditorEngineGValuePart>();
        private List<string> gvalueGroupNames = new List<string>();
        private string[] gvalueGroupNamesArray = new string[0];
        private Dictionary<int,EditorEngineGValuePart> gvaluePartDict = new Dictionary<int,EditorEngineGValuePart>();
        private Dictionary<EGValueType,List<string>> GvalueDictionary = new Dictionary<EGValueType, List<string>>();
        private int mGvalue_filter = -1;
        // private int mGValueDisType = 0;
        private Vector2 gValueScrollPos;
        private EGValueType gValueType;

        private GUIStyle mGvalueStyle = new GUIStyle();
        
        //GValue公式用的参数
        public int EquationSelctID = 0;
        public EditorGValueEquation CurSelectedGValueEquation = null;
        private bool isDisPlayConstGValue = true;
        [NonSerialized] public List<int> RemoveEquationIDs = new List<int>();
        private List<EditorGValueEquation> gvalueEquations = new List<EditorGValueEquation>();
        // private List<EditorGValueEquation> gvalueEquations_Dis = new List<EditorGValueEquation>();

        public void GValueOnChange(bool _change = true)
        {
            if (GValueIsChange != _change)
            {
                ResourcesWindow.Instance.Repaint();
                GValueIsChange = _change;
            }
        }

        public void GValueSort()
        {
            gvaluePart.Sort((x, y) => { return x.IndexID.CompareTo(y.IndexID);});
            gvaluePart_Dis.Sort((x, y) => { return x.IndexID.CompareTo(y.IndexID);});
        }
        
        private void InitGValueGUI()
        {
            mGvalueStyle.fontSize = 11;
            mGvalueStyle.fontStyle = FontStyle.Bold;
            mGvalueStyle.alignment = TextAnchor.MiddleCenter;
            mGvalueStyle.normal.textColor = Color.gray;
            
            LoadGValueInfo();
            gValueType = EGValueType.GInt;
        }
        private void DrwaGValueGUI()
        {
            Rect windowPos = position;
            windowPos.position = Vector2.zero;
            if (isDisPlayConstGValue)
            {
                DrawConstGValueGUI(windowPos);
            }
            else
            {
                DrawGValueEquation(windowPos);
            }
        }

        #region GValue公式绘制

        private void DrawGValueEquation(Rect _mainRect)
        {
            using (new GUI.GroupScope(_mainRect))
            {

                //头部绘制
                using (new GUILayout.HorizontalScope(GUILayout.Width(position.width - 60)))
                {
                    GUILayout.Label("", GUILayout.Width(53));
                    if (GUILayout.Button("新建", EditorStyles.toolbarButton))
                    {
                        int id = GetGValueEquationSelfID(
                            CurSelectedGValueEquation is null ? 0 : CurSelectedGValueEquation.m_SorID);

                        //新建
                        EditorGValueEquation v =
                            new EditorGValueEquation("公式", gvalueEquations.Count, gvalueEquations.Count);
                        gvalueEquations.Add(v);
                        GValueOnChange();
                    }

                    if (GUILayout.Button("删除", EditorStyles.toolbarButton))
                    {

                    }
                }

                GUILayout.Space(3);
                using (new GUILayout.HorizontalScope(GUILayout.Width(position.width - 60)))
                {
                    GUILayout.Label("", GUILayout.Width(53));

                    //筛选
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        mGvalue_filter = EditorGUILayout.Popup(mGvalue_filter,
                            gvalueGroupNamesArray, EditorStyles.toolbarPopup);
                        if (_check.changed)
                        {
                            gvaluePart_Dis = new List<EditorEngineGValuePart>();
                            foreach (var gValue in gvaluePart)
                            {
                                if (gValue.Name.Contains(gvalueGroupNamesArray[mGvalue_filter]))
                                {
                                    gvaluePart_Dis.Add(gValue);
                                }
                            }
                            //过滤
                        }
                    }

                    if (mGvalue_filter > -1)
                    {
                        if (GUILayout.Button("取消过滤", EditorStyles.toolbarButton))
                        {
                            mGvalue_filter = -10;
                            gvaluePart_Dis = gvaluePart;
                        }
                    }

                }

                Rect _saveRect = position;
                _saveRect.width = 60;
                _saveRect.x = position.width - _saveRect.width;
                _saveRect.y = 20; //40
                _saveRect.height = 46;

                using (new GUIColorScope(Color.red, GValueIsChange))
                {
                    if (GUI.Button(_saveRect, "保存"))
                    {
                        SaveGValueInfo();
                    }
                }

                _saveRect.x = 0;
                using (new GUIColorScope(Color.cyan))
                {
                    if (GUI.Button(_saveRect, ""))
                    {
                        isDisPlayConstGValue = true;
                    }
                }

                GUI.Label(_saveRect, "     公式\n");
                GUI.Label(_saveRect, "  \n \n至常量>>", mGvalueStyle);

                //滑动列表绘制
                Rect _rect = new Rect(position);
                float _HeadHeight = _saveRect.height + _saveRect.y;
                _rect.y = _HeadHeight;
                _rect.x = 0;
                _rect.height -= _HeadHeight;

                float _ElementHeight = 20;
                Rect _ElementRect = new Rect(_rect);
                _ElementRect.height = gvalueEquations.Count * _ElementHeight;
                _ElementRect.width -= 15;
                using (var _scroll = new GUI.ScrollViewScope(_rect, gValueScrollPos, _ElementRect))
                {
                    gValueScrollPos = _scroll.scrollPosition;
                    int _removeCount = 0;
                    for (int i = 0; i < gvalueEquations.Count; i++)
                    {
                        EditorGValueEquation _curPart = gvalueEquations[i];

                        //不绘制已经删除过的Gvalue
                        if (RemoveIDs.Contains(_curPart.m_RealID))
                        {
                            _removeCount++;
                            continue;
                        }

                        float posY = (i - _removeCount) * _ElementHeight + _HeadHeight;

                        Rect _buttonRect = new Rect(_ElementRect);
                        _buttonRect.y = posY;
                        _buttonRect.height = _ElementHeight;
                        using (new GUIColorScope(Color.gray, _curPart == CurSelectedGValueEquation))
                        {
                            if (GUI.Button(_buttonRect, _curPart.m_Name))
                            {
                                CurSelectedGValueEquation = _curPart;
                                EquationSelctID = i;
                            }
                        }
                    }
                }
            }
        }

        private int GetGValueEquationSelfID(int _nowID)
        {
            List<int> gvalueIDs = new List<int>();
            foreach (var VARIABLE in gvalueEquations)
            {
                gvalueIDs.Add(VARIABLE.m_SorID);
            }
            
            // if(gvalueIDs)
            for (int i = 0; i < gvalueIDs.Count; i++)
            {
                _nowID++;
                if (!gvalueIDs.Contains(_nowID))
                {
                    return _nowID;
                }
            }
            
            return 0;
        }
        #endregion

        #region GValue常量绘制
        private void DrawConstGValueGUI(Rect _mainRect)
        {
            using (new GUI.GroupScope(_mainRect))
            {
                //头部绘制
                using (new GUILayout.HorizontalScope(GUILayout.Width(position.width - 60)))
                {
                    GUILayout.Label("", GUILayout.Width(53));
                    gValueType = (EGValueType)EditorGUILayout.EnumPopup("", gValueType, EditorStyles.toolbarButton);
                    if (GUILayout.Button("新建", EditorStyles.toolbarButton))
                    {
                        // if (gValueType != EGValueType.Null)
                        if (!TryFindAndCreactGValue(gValueType))
                        {
                            gvaluePart.Add(CreactGValuePart(gValueType));
                            GValueOnChange();
                            UpdateGvaluePartDict(gvaluePart);
                            GValueSort();
                            UpdateGvalueGroupNames();
                        }
                    }

                    if (GUILayout.Button("删除", EditorStyles.toolbarButton))
                    {
                        if (!RemoveIDs.Contains(CurSclect_GValue.Index))
                        {
                            if (EditorUtility.DisplayDialog("警告", "确定要删除此GValue吗？", "确定", "取消"))
                            {
                                RemoveIDs.Add(CurSclect_GValue.Index);
                                UpdateGvalueGroupNames();
                            }
                        }
                    }
                }

                GUILayout.Space(3);
                using (new GUILayout.HorizontalScope(GUILayout.Width(position.width - 60)))
                {
                    GUILayout.Label("", GUILayout.Width(53));

                    //筛选
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        mGvalue_filter = EditorGUILayout.Popup(mGvalue_filter,
                            gvalueGroupNamesArray, EditorStyles.toolbarPopup);
                        if (_check.changed)
                        {
                            gvaluePart_Dis = new List<EditorEngineGValuePart>();
                            foreach (var gValue in gvaluePart)
                            {
                                if (gValue.Name.Contains(gvalueGroupNamesArray[mGvalue_filter]))
                                {
                                    gvaluePart_Dis.Add(gValue);
                                }
                            }
                            //过滤
                        }
                    }

                    if (mGvalue_filter > -1)
                    {
                        if (GUILayout.Button("取消过滤", EditorStyles.toolbarButton))
                        {
                            mGvalue_filter = -10;
                            gvaluePart_Dis = gvaluePart;
                        }
                    }

                }

                Rect _saveRect = position;
                _saveRect.width = 60;
                _saveRect.x = position.width - _saveRect.width;
                _saveRect.y = 20; //40
                _saveRect.height = 46;

                using (new GUIColorScope(Color.red, GValueIsChange))
                {
                    if (GUI.Button(_saveRect, "保存"))
                    {
                        SaveGValueInfo();
                    }
                }

                _saveRect.x = 0;
                using (new GUIColorScope(Color.cyan))
                {
                    if (GUI.Button(_saveRect, ""))
                    {
                        isDisPlayConstGValue = false;
                    }
                }
                
                GUI.Label(_saveRect, "     常量\n");
                GUI.Label(_saveRect, "  \n \n<<至公式", mGvalueStyle);

                Rect _rect = new Rect(position);
                float _HeadHeight = _saveRect.height + _saveRect.y;
                _rect.y = _HeadHeight;
                _rect.x = 0;
                _rect.height -= _HeadHeight;

                float _ElementHeight = 20;
                Rect _ElementRect = new Rect(_rect);
                _ElementRect.height = gvaluePart_Dis.Count * _ElementHeight;
                _ElementRect.width -= 15;
                using (var _scroll = new GUI.ScrollViewScope(_rect, gValueScrollPos, _ElementRect))
                {
                    gValueScrollPos = _scroll.scrollPosition;
                    int _removeCount = 0;
                    for (int i = 0; i < gvaluePart_Dis.Count; i++)
                    {
                        EditorEngineGValuePart _curPart = gvaluePart_Dis[i];

                        //不绘制已经删除过的Gvalue
                        if (RemoveIDs.Contains(_curPart.Index))
                        {
                            _removeCount++;
                            continue;
                        }

                        float posY = (i - _removeCount) * _ElementHeight + _HeadHeight;

                        Rect _buttonRect = new Rect(_ElementRect);
                        _buttonRect.y = posY;
                        _buttonRect.height = _ElementHeight;
                        using (new GUIColorScope(Color.gray, _curPart == CurSclect_GValue))
                        {
                            string _displayName = _curPart.Name;
                            if (mGvalue_filter > -1)
                            {
                                _displayName = _displayName.Split("_")[^1];
                            }

                            if (GUI.Button(_buttonRect, _displayName))
                            {
                                // CurSclect_GValueID = i;
                                OnSelectGValue(_curPart);
                            }
                        }

                        {
                            GUIStyle _style = new GUIStyle();
                            _style.alignment = TextAnchor.MiddleLeft;
                            Color _color = Color.green;
                            if (_curPart.ValueType == EGValueType.GBool) _color = Color.black;
                            else if (_curPart.ValueType == EGValueType.GEnum) _color = Color.cyan;
                            else if (_curPart.ValueType == EGValueType.GInt) _color = Color.blue;
                            else if (_curPart.ValueType == EGValueType.GFloat) _color = Color.magenta;
                            else if (_curPart.ValueType == EGValueType.GString) _color = Color.yellow;
                            else if (_curPart.ValueType == EGValueType.GPoint) _color = Color.white;
                            else if (_curPart.ValueType == EGValueType.GTransform) _color = Color.gray;

                            _style.normal.textColor = _color;
                            GUI.Label(_buttonRect, "  " + _curPart.ValueType, _style);

                            _style.alignment = TextAnchor.MiddleRight;
                            _style.normal.textColor = Color.red + Color.blue * 0.7f;
                            GUI.Label(_buttonRect, _curPart.IndexID + "  ", _style);
                        }
                    }
                }
            }
        }
        

        #endregion
        
        #region 加载和保存
        private void LoadGValueInfo()
        {
            if (ActionWindowMain.ActionEditorFuntion.LoadGValueData(out EditorEngineGValue _saveData, GValueSaveName))
            {
                EditorEngineGValue = _saveData;
                gvaluePart.Clear();
                EnumNames.Clear();
                TransNames.Clear();
                PointNames.Clear();
                UnitNames.Clear();
                RemoveIDs.Clear();
                gvalueEquations.Clear();
                gvaluePart.AddRange(_saveData.part);
                EnumNames.AddRange(_saveData.EnumNames);
                TransNames.AddRange(_saveData.TransNames);
                PointNames.AddRange(_saveData.PointNames);
                UnitNames.AddRange(_saveData.UnitNames);
                RemoveIDs.AddRange(_saveData.RemoveIDs);
                gvalueEquations.AddRange(_saveData.Equations);
                UpdateGvalueDictionary(_saveData);
                UpdateGvaluePartDict(gvaluePart);
                UpdateGvalueGroupNames();
                gvaluePart_Dis = gvaluePart;
            }
            else
            {
                string _debug =
                    "单位数据读取失败" + "\n" +
                    "客户端未实现GValue数据的读取方法" + "\n" +
                    $"ID: {GValueSaveName}" + "\n" +
                    "";
                EditorUtility.DisplayDialog("警告", _debug, "我知道了");
            }
        }

        public void SaveGValueInfo()
        {
            GValueIsChange = false;

            EditorEngineGValue _saveData = new EditorEngineGValue();
            _saveData.part = gvaluePart.ToArray();
            _saveData.EnumNames = EnumNames;
            _saveData.PointNames = PointNames;
            _saveData.TransNames = TransNames;
            _saveData.UnitNames = UnitNames;
            _saveData.RemoveIDs = RemoveIDs;
            _saveData.Equations = gvalueEquations;
            if (!ActionWindowMain.ActionEditorFuntion.SaveGValueData(_saveData, GValueSaveName))
            {
                EngineDebug.Log($"客户端未实现GValue列表保存: {GValueSaveName}");
                return;
            }
            UpdateGvalueDictionary(_saveData);
            AssetDatabase.Refresh();
        }
        #endregion

        #region 创建GValue
        List<int> gvalueIndex = new List<int>();
        private EditorEngineGValuePart CreactGValuePart(EGValueType gValueType)
        {
            EditorEngineGValuePart _engineGValuePart = new EditorEngineGValuePart();
            _engineGValuePart.Index = gvaluePart.Count;
            _engineGValuePart.ValueType = gValueType;
            switch (gValueType)
            {
                case EGValueType.GBool:
                    _engineGValuePart.DefaultValue = new GVS_Bool(false);
                    break;
                case EGValueType.GInt:
                    _engineGValuePart.DefaultValue = new GVS_Int(0);
                    break;
                case EGValueType.GFloat:
                    _engineGValuePart.DefaultValue = new GVS_Float(0);
                    break;
                case EGValueType.GString:
                    _engineGValuePart.DefaultValue = new GVS_String();
                    break;
                case EGValueType.GEnum:
                    GVS_Enum _gEnum = new GVS_Enum();
                    _engineGValuePart.DefaultValue = _gEnum;
                    _engineGValuePart.ValueID = EnumNames.Count;
                    SEnumName _enumName = new SEnumName();
                    _enumName.names = new List<string>();
                    _enumName.names .Add("Default");
                    EnumNames.Add(_enumName);
                    break;
                case EGValueType.GPoint:
                    GVS_Enum _gEnum1 = new GVS_Enum();
                    _engineGValuePart.DefaultValue = _gEnum1;
                    _engineGValuePart.ValueID = PointNames.Count;
                    SEnumName _enumName1 = new SEnumName();
                    _enumName1.names = new List<string>();
                    _enumName1.names .Add("Default");
                    PointNames.Add(_enumName1);
                    break;
                case EGValueType.GTransform:
                    GVS_Enum _gEnum2 = new GVS_Enum();
                    _engineGValuePart.DefaultValue = _gEnum2;
                    _engineGValuePart.ValueID = TransNames.Count;
                    SEnumName _enumName2 = new SEnumName();
                    _enumName2.names = new List<string>();
                    _enumName2.names .Add("Default");
                    TransNames.Add(_enumName2);
                    break;
                case EGValueType.GUnit:
                    GVS_Enum _gEnum3 = new GVS_Enum();
                    _engineGValuePart.DefaultValue = _gEnum3;
                    _engineGValuePart.ValueID = UnitNames.Count;
                    SEnumName _enumName3 = new SEnumName();
                    _enumName3.names = new List<string>();
                    _enumName3.names .Add("Default");
                    UnitNames.Add(_enumName3);
                    break;
                // case EGValueType.GColor:
                //     _engineGValuePart.DefaultValue = Color.white;
                //     break;
                // case EGValueType.GVector2:
                //     _engineGValuePart.DefaultValue = Vector2.zero;
                //     break;
                // case EGValueType.GVector3:
                //     _engineGValuePart.DefaultValue = Vector3.zero;
                //     break;
                // case EGValueType.GQuaternion:
                //     _engineGValuePart.DefaultValue = Quaternion.identity;
                //     break;
                // case EGValueType.GUnit:
                //     _engineGValuePart.DefaultValue = null;
                //     break;
                // case EGValueType.GTransform:
                //     _engineGValuePart.DefaultValue = null;
                //     break;
            }

            // if (CurSclect_GValue is not null)
            // {
            //     foreach (var VARIABLE in gvaluePart)
            //         gvalueIndex.Add(VARIABLE.IndexID);
            //     int curid = CurSclect_GValue.IndexID + 1;
            //     for (int i = 0; i < gvaluePart.Count; i++)
            //     {
            //         if (gvalueIndex.Contains(curid)) curid++;
            //         else break;
            //     }
            //     _engineGValuePart.IndexID = curid;
            // }
            // else
            // {
            //     int curid = 0;
            //     foreach (var VARIABLE in gvaluePart)
            //     {
            //         if (VARIABLE.IndexID >= curid) curid = VARIABLE.IndexID + 1;
            //     }
            //     _engineGValuePart.IndexID = curid;
            // }
            // _engineGValuePart.Name = GValueSaveName + "_" + _engineGValuePart.IndexID;
            _engineGValuePart = SetNewGValue(_engineGValuePart);
            return _engineGValuePart;
        }

        private bool TryFindAndCreactGValue(EGValueType gValueType)
        {
            // foreach (int _removeID in RemoveIDs)
            // {
            //     if (gvaluePartDict.TryGetValue(_removeID, out EditorEngineGValuePart _engineGValuePart))
            //     {
            //         
            //     }
            // }
            for (int i = 0; i < RemoveIDs.Count; i++)
            {
                if (gvaluePartDict.TryGetValue(RemoveIDs[i], out EditorEngineGValuePart _engineGValuePart))
                {
                    if (_engineGValuePart.ValueType == gValueType)
                    {
                        RemoveIDs.Remove(RemoveIDs[i]);
                        SetNewGValue(_engineGValuePart);
                        return true;
                    }
                }
                else
                {
                    EngineDebug.LogError($"Gvalue新建出现问题，删除列表出现了不存在的Gvalue: [{RemoveIDs[i]}]");
                    // return true;
                }
            }
            return false;
        }

        private EditorEngineGValuePart SetNewGValue(EditorEngineGValuePart _engineGValuePart)
        {
            gvalueIndex.Clear();
            if (CurSclect_GValue is not null)
            {//OnSelectGValue
                foreach (var VARIABLE in gvaluePart)
                {
                    if (!RemoveIDs.Contains(VARIABLE.Index))
                    {
                        gvalueIndex.Add(VARIABLE.IndexID);
                    }
                }
                int curid = CurSclect_GValue.IndexID + 1;
                for (int i = 0; i < gvaluePart.Count; i++)
                {
                    // if(RemoveIDs.Contains())
                    if (gvalueIndex.Contains(curid)) curid++;
                    else break;
                }
                _engineGValuePart.IndexID = curid;
            }
            else
            {
                int curid = 0;
                foreach (var VARIABLE in gvaluePart)
                {
                    if (VARIABLE.IndexID >= curid) curid = VARIABLE.IndexID + 1;
                }
                _engineGValuePart.IndexID = curid;
            }
            _engineGValuePart.Name = GValueSaveName + "_" + _engineGValuePart.IndexID;
            return _engineGValuePart;
        }
        #endregion

        #region GValueNameDictionary
        private void UpdateGvalueDictionary(EditorEngineGValue _gValue)
        {
            GvalueDictionary.Clear();
            List<EditorEngineGValuePart> parta = _gValue.part.ToList();
            parta.Sort((x, y) => { return x.IndexID.CompareTo(y.IndexID); });
            foreach (EditorEngineGValuePart part in parta)
            {
                if (!GvalueDictionary.ContainsKey(part.ValueType))
                    GvalueDictionary.Add(part.ValueType, new List<string>());
                GvalueDictionary[part.ValueType].Add(part.Name);
            }
        }

        private void UpdateGvaluePartDict(List<EditorEngineGValuePart> _gvaluePart)
        {
            gvaluePartDict.Clear();
            foreach (var VARIABLE in _gvaluePart)
            {
                if (!gvaluePartDict.TryAdd(VARIABLE.Index, VARIABLE))
                {
                    EngineDebug.LogError($"Gvalue字典错误，真实序号冲突： [{VARIABLE.Name}]");
                }
            }
        }

        public List<string> GetGValueName(EGValueType gValueType)
        {
            if (GvalueDictionary.TryGetValue(gValueType, out List<string> gValueName))
                return gValueName;
            return null;
        }

        public void UpdateGvalueGroupNames()
        {
            gvalueGroupNames.Clear();
            foreach (var _engineGValuePart in gvaluePart)
            {
                if (!RemoveIDs.Contains(_engineGValuePart.Index))
                {
                    string[] _names = _engineGValuePart.Name.Split('_');
                    if (_names.Length > 1)
                    {
                        string _newName = _names[0];
                        for (int i = 1; i < _names.Length - 1; i++)
                        {
                            _newName += "/" + _names[i];
                        }

                        if (!gvalueGroupNames.Contains(_newName))
                        {
                            if (_names.Length > 2)
                            {
                                string _newName2 = _names[0];

                                for (int i = 1; i < _names.Length - 2; i++)
                                {
                                    _newName2 += "/" + _names[i];
                                }
                                if (!gvalueGroupNames.Contains(_newName2))
                                    gvalueGroupNames.Add(_newName2);

                            }
                            gvalueGroupNames.Add(_newName);
                        }
                    }
                }
            }

            gvalueGroupNamesArray = gvalueGroupNames.ToArray();
        }
        #endregion

    }
}
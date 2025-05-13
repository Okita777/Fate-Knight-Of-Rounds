using System.Collections.Generic;
using System.IO;
using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using Cinemachine;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class ResourcesWindow
    {
        public List<EditorUnitWarp> UnitWarp => mUnitWarp;
        
        private string UnitWarpDataInfoName => RunTime.MotionEngineConst.UnitSaveName;

        private Vector2 mUnitScroll;
        private List<EditorUnitWarp> mUnitWarp = new List<EditorUnitWarp>();

        private void InitUnitGUI()
        {
            mUnitWarp.Clear();
            LoadAllUnitInfo();
        }
        private void DrawUnitGUI()
        {
            //头部菜单绘制
            using (new GUILayout.HorizontalScope())
            {
                if (!Application.isPlaying)
                {
                    if (GUILayout.Button("预览", EditorStyles.toolbarButton))
                    {
                        CreactPreviewUnit();
                    }
                }

                if (GUILayout.Button("新建", EditorStyles.toolbarButton))
                {
                    int _id = mUnitWarp.Count;
                    mUnitWarp.Add(new EditorUnitWarp(_id, $"单位 {_id}", GetInitGValueSetting()));
                }
                if (GUILayout.Button("复制", EditorStyles.toolbarButton))
                {
                    int _id = mUnitWarp.Count;
                    mUnitWarp.Add(new EditorUnitWarp(_id, $"单位 {_id}", GetInitGValueSetting()));
                }
                if (GUILayout.Button("删除", EditorStyles.toolbarButton))
                {
                    if (mOnSelectUnitID < mUnitWarp.Count)
                    {
                        DestroyUnit(mUnitWarp[mOnSelectUnitID]);
                        InitUnitGUI();
                    }
                }

                if (GUILayout.Button("保存", EditorStyles.toolbarButton))
                {
                    SaveUnitInfo();
                }
            }
            
            //单位列表绘制
            using (var _Scroll = new GUILayout.ScrollViewScope(mUnitScroll))
            {
                mUnitScroll = _Scroll.scrollPosition;
                for (int i = 0; i < mUnitWarp.Count; i++)
                {
                    string _name = mUnitWarp[i].Name;
                    bool isSelect = mOnSelectUnitID == i;
                    string _select = isSelect ? $"--> {_name}" : _name;
                    using (new GUIColorScope(Color.gray, isSelect))
                    {
                        if (GUILayout.Button(_select))
                        {
                            OnSelectUnit(i);
                        }
                    }
                }
            }
        }

        public void UnitReName(string _from, string _to)
        {
            int _findID = -1;
            for (int i = 0; i < mUnitWarp.Count; i++)
            {
                string _name = mUnitWarp[i].Name;
                if (_from == _name)
                {
                    _findID = i;
                }

                if (_to == _name)
                {
                    EngineDebug.LogError($"命名修改失败! 已存在 {_to}");
                    return;
                }
            }

            if (_findID > -1)
            {
                EditorUnitWarp _unitWarp = mUnitWarp[_findID];

                if (!ActionWindowMain.ActionEditorFuntion.SaveUnitData(_unitWarp, _to))
                {
                    EngineDebug.LogError($"命名修改失败: {_from} to {_to}");
                    return;
                }
                DestroyUnit(_unitWarp);
                _unitWarp.Name = _to;
                InitUnitGUI();
                AssetDatabase.Refresh();
            }
            else
            {
                EngineDebug.LogError($"命名修改失败!!!");
            }
        }

        public void UnitReID(int _from, int _to)
        {
            int _findID = -1;
            for (int i = 0; i < mUnitWarp.Count; i++)
            {
                int _name = mUnitWarp[i].ID;
                if (_from == _name)
                {
                    _findID = i;
                }

                if (_to == _name)
                {
                    EngineDebug.Log($"ID修改失败:! 已存在 {_to}");
                    return;
                }
            }

            if (_findID > -1)
            {
                EditorUnitWarp _unitWarp = mUnitWarp[_findID];
                _unitWarp.ID = _to;
            }
            else
            {
                EngineDebug.LogError($"ID修改失败!!!");
            }
        }
        private void CreactPreviewUnit()
        {
            if (mUnitWarp.Count > mOnSelectUnitID)
            {
                EditorUnitWarp _unitWarp = mUnitWarp[mOnSelectUnitID];
                SetActionStateName(_unitWarp.Action);
                LoadActionStateInfo(_unitWarp.Action);
                PreviewModel(_unitWarp);
                mSelectActionState = _unitWarp.Action;

                if (CameraWarps.Count < 1)
                {
                    InitCameraGUI();
                }
#if Cinemachine
                if(CheckCinemachine()) CreactPreviewCamera(_unitWarp.CameID);
#elif UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayDialog("环境缺失", "正在尝试使用 【Cinemachine】 的相关功能，请安装相关环境包", "我知道了");
#endif
                
                InspectorWindow.Instance.needInit = true;
                InspectorWindow.Instance.Repaint();
                ActionIsChange = false;
                // ActionOnChange(false);
                ScenceDraw.Instance.DrawBoxInit();
            }
        }
        private void DestroyUnit(EditorUnitWarp _targe)
        {
            if(_targe is null)
                return;
            
            string _path = ActionWindowMain.ActionEditorFuntion.GetUnitEditorDataPath(_targe.Name);
            AssetDatabase.DeleteAsset(_path);

            _path = string.Format(ActionWindowMain.ActionEditorFuntion.GetRunTimeDataPath(), "Unit", _targe.Name);
            if (!AssetDatabase.DeleteAsset(_path))
            {
                EngineDebug.Log($"删除失败: {_path}");
            }
            // _path = ActionWindowMain.ActionEditorFuntion.GetUnitDataPath(_targe.Name);
            // if (GetPorjectPathTo(out string _fullPath,_path))
            // {
            //     AssetDatabase.DeleteAsset(_fullPath);
            // }
            // else
            // {
            //     _path = ActionWindowMain.GetAssetPathToResources(_path);
            //     if (!AssetDatabase.DeleteAsset(_path))
            //     {
            //         EngineDebug.Log($"删除失败: {_path}");
            //     }
            // }
            
        }

        private GValue_Setting GetInitGValueSetting()
        {
            GValue_Setting _gValueSetting = new GValue_Setting();
            foreach (var VARIABLE in gvaluePart)
            {
                if (VARIABLE.IsDefaultValue)
                {
                    GValue_SettingPar _settingPar = new GValue_SettingPar(VARIABLE.ValueType, VARIABLE.DefaultValue.Clone());
                    _gValueSetting.parameter.Add(_settingPar);
                }
            }
            _gValueSetting.isOpen = true;
            return _gValueSetting;
        }
        private bool CheckCinemachine()
        {
            if (!MotionEngineConst.FindObjects(out CinemachineBrain _cinemachineBrain))
            {
                if (Camera.main != null)
                {
                    Camera.main.gameObject.AddComponent<CinemachineBrain>();
                    return true;
                }
                if (MotionEngineConst.FindObjects(out Camera _camera))
                {
                    _camera.gameObject.AddComponent<CinemachineBrain>();
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", "未在场景中发现相机，请保证场景至少存在一个相机", "我知道了");
                    return false;
                }
            }

            return true;
        }
        #region Unit列表数据存取

        public void LoadAllUnitInfo()
        {
            DirectoryInfo dir = new DirectoryInfo(ActionWindowMain.ActionEditorFuntion.GetUnitEditorDataPath());
            FileInfo[] fileInfo = dir.GetFiles("*");
            foreach (var VARIABLE in fileInfo)
            {
                string[] _name = VARIABLE.Name.Split('.');
                if (_name[^1] == "json")
                {
                    EditorUnitWarp _unitWrap = LoadUnitInfo(_name[0]);
                    _unitWrap.Name = _name[0];
                    mUnitWarp.Add(_unitWrap);
                }
            }
            mUnitWarp.Sort((x, y) => { return x.ID.CompareTo(y.ID);});
            // foreach (var _cameraWarp in mUnitWarp)
            // {
            //     // _cameraWarp.Name = 
            //     // mCameraWarps.Add(_cameraWarp.Name, _cameraWarp);
            // }
        }
        
        public EditorUnitWarp LoadUnitInfo(string _name)
        {
            //尝试加载客户端序列化的数据
            if (!ActionWindowMain.ActionEditorFuntion.LoadUnitData(out EditorUnitWarp _saveData, _name))
            {
                string _debug =
                    "单位数据读取失败" + "\n" +
                    "客户端未实现单位数据的读取方法" + "\n" +
                    $"ID: {_name}" + "\n" +
                    "";
                EditorUtility.DisplayDialog("警告", _debug, "我知道了");
                return null;
            }

            return _saveData;
        }
        public void SaveUnitInfo()
        {
            //检查重名
            CheckDuplicateName _checkDuplicateName = new CheckDuplicateName();
            foreach (var VARIABLE in mUnitWarp)
            {
                _checkDuplicateName.OnCheck(VARIABLE.Name);
            }

            if (_checkDuplicateName.IsDuplicate())
            {
                EditorUtility.DisplayDialog("保存出错！！！",_checkDuplicateName.GetDuplicateName(),"我知道了");
            }
            else
            {
                foreach (var _editorUnitWarp in mUnitWarp)
                {
                    if (!ActionWindowMain.ActionEditorFuntion.SaveUnitData(_editorUnitWarp, _editorUnitWarp.Name))
                    {
                        EngineDebug.Log($"保存Unit列表失败: {_editorUnitWarp.Name}");
                    }
                }
                EngineDebug.Log($"已完成对Unit列表的保存");

                AssetDatabase.Refresh();

                // EditorUnitWarp _saveData = new EditorUnitWarp(0,"null");
                //
                // //客户端保存序列化数据
                // if(ActionWindowMain.ActionEditorFuntion.SaveUnitData(_saveData, UnitWarpDataInfoName)) return;
                //
                // // string _str = JsonUtility.ToJson(_saveData);
                // // File.WriteAllText(MotionEngineConst.EditorUnitSavePath(UnitWarpDataInfoName),_str);
                // // RunTimeDataManager.SaveUnit(_saveData, UnitWarpDataInfoName);
                // // EngineDebug.Log("保存Unit列表成功");
            }
        }
        #endregion
    }
}
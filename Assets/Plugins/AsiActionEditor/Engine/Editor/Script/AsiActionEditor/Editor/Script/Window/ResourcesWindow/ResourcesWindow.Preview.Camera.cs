using System.Collections.Generic;
using System.IO;
using AsiActionEngine.RunTime;
using AsiTimeLine.Editor;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class ResourcesWindow
    {
        public Dictionary<string, EditorCameraWarp> CameraWarps => mCameraWarps;
        public GameObject PreCamera => mPreCamera;
        public CameraControl PreCamControl => mPreCamControl;

        private Vector2 mCamScroll;
        private GameObject mPreCamera;
        private CameraControl mPreCamControl;
        private EditorCameraWarp mCurCameraWarp;
        private EditorCameraWarp mPreCameraWarp;
        private Dictionary<string, EditorCameraWarp> mCameraWarps = new Dictionary<string, EditorCameraWarp>();
        private List<EditorCameraWarp> camWarpList = new List<EditorCameraWarp>();

        public void InitCameraGUI()
        {
            mCameraWarps.Clear();
            camWarpList.Clear();
            LoadAllCam();
        }
        private void DrawCameraGroupGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("预览", EditorStyles.toolbarButton))
                {
                    CreactPreviewCamera();
                }
                
                if (GUILayout.Button("复制", EditorStyles.toolbarButton))
                {
                    
                }

                if (GUILayout.Button("新建", EditorStyles.toolbarButton))
                {

                    CreactCameraWarp("Default");

                }

                if (GUILayout.Button("删除", EditorStyles.toolbarButton))
                {
                    DestroyCam(mCurCameraWarp.Name);
                    InitCameraGUI();
                }

                if (GUILayout.Button("保存", EditorStyles.toolbarButton))
                {
                    SaveCameraWarp();
                }
            }

            using (var _scroll = new GUILayout.ScrollViewScope(mCamScroll))
            {
                mCamScroll = _scroll.scrollPosition;
                foreach (var _camInfo in mCameraWarps)
                {
                    bool _isSelect = mCameraWarps[_camInfo.Key] == InspectorWindow.Instance.CurSelectProperty;
                    bool _isPreview = false;
                    if (mPreCameraWarp is not null)
                    {
                        _isPreview = mCameraWarps[_camInfo.Key] == mPreCameraWarp;
                    }
                    using (new GUIColorScope(Color.gray, _isSelect))
                    {
                        // mGUIStyle.normal.textColor = _isPreview ? Color.red : Color.white;
                        string _buttonName = _isPreview ? $"{_camInfo.Key}   (Preview)" : _camInfo.Key;
                        if (GUILayout.Button(_buttonName))
                        {
                            mCurCameraWarp = mCameraWarps[_camInfo.Key];
                            
                            InspectorWindow.Instance.UpdateConfig(_camInfo.Value.ModelPath);
                            
                            OnSelectCamWarp(mCurCameraWarp);
                        }
                    }
                }
            }
        }

        public void CreactPreviewCamera(string _camera = "")
        {
            EditorCameraWarp _target = mCurCameraWarp;

            if (!string.IsNullOrEmpty(_camera))
            {
                if (!mCameraWarps.TryGetValue(_camera, out _target))
                {
                    EngineDebug.LogError($"不存在此相机: <color=#ffcc00>{_camera}</color>");
                    return;
                }
            }
            
            if (_target is null)
            {
                EditorUtility.DisplayDialog("警告", "请先选择预览对象", "我知道了");
                return;
            }
            
            Transform _LookPoint = null;
            // if (mPreCameraWarp != _target)
            {
                if (mRole == null)
                {
                    // EditorUtility.DisplayDialog("警告", "请先创建预览角色", "我知道了");
                    EngineDebug.LogError("请先创建预览角色");
                    return;
                }
                if (mRole.TryGetComponent(out CharacterConfig _config))
                {
                    if (!_config.HelpPointDic.TryGetValue(ECharacteLimbType.Cam_Main, out _LookPoint))
                    {
                        EditorUtility.DisplayDialog("警告", "当前CharacterConfig组件未配置 Cam_Main", "我知道了");
                        return;
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("警告", "预览角色未挂载CharacterConfig组件", "我知道了");
                    return;
                }
                
                List<CameraControl> _model_olds = MiscFunctions.FindObjects<CameraControl>();
                foreach (var _control in _model_olds)
                {
                    // DestroyImmediate(_control.gameObject);
                    Undo.DestroyObjectImmediate(_control.gameObject);
                }

                //todo: 相机加载
                GameObject _loading = AssetDatabase.LoadAssetAtPath<GameObject>(_target.ModelPath);
                mPreCamera = Object.Instantiate(_loading);
                Undo.RegisterCreatedObjectUndo(mPreCamera,"创建相机单位");
                if (mPreCamera.TryGetComponent(out CameraControl _cameraControl))
                {
                    mPreCamControl = _cameraControl;
                    _cameraControl.OnInit(_LookPoint, mUnit.ActionStateMachine, _target.DefaultCamID);
                    
                    if (_cameraControl.allCinemachine.Length > _target.DefaultCamID)
                    {
                        _cameraControl.allCinemachine[_target.DefaultCamID].enabled = true;
                    }
                    else
                    {
                        EngineDebug.LogWarning($"默认相机配置错误  ID:{_target.DefaultCamID}");
                    }
                }

                if (string.IsNullOrEmpty(_camera))
                {
                    Selection.activeGameObject = mPreCamera;
                    EditorGUIUtility.PingObject(mPreCamera);
                }

                mPreCameraWarp = _target;
            }
        }

        public bool CurCamIsPreCam()
        {
            return mCurCameraWarp == mPreCameraWarp;
        }

        public void SetPreCamera(CameraControl _mPreCamera)
        {
            mPreCamControl = _mPreCamera;
        }

        private void DestroyCam(string _name)
        {
            if(string.IsNullOrEmpty(_name))
                return;
            
            string _path = ActionWindowMain.ActionEditorFuntion.GetCamEditorDataPath(_name);
            AssetDatabase.DeleteAsset(_path);

            _path = string.Format(ActionWindowMain.ActionEditorFuntion.GetRunTimeDataPath(), "Camera", _name);
            if (!AssetDatabase.DeleteAsset(_path))
            {
                EngineDebug.Log($"删除失败: {_path}");
            }
            // _path = ActionWindowMain.ActionEditorFuntion.GetCamDataPath(_name);
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
        
        public void CamReName(string _from, string _to)
        {
            int _findID = -1;
            for (int i = 0; i < camWarpList.Count; i++)
            {
                string _name = camWarpList[i].Name;
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
                EditorCameraWarp _unitWarp = camWarpList[_findID];

                if (!ActionWindowMain.ActionEditorFuntion.SaveCameraData(_unitWarp, _to))
                {
                    EngineDebug.LogError($"命名修改失败: {_from} to {_to}");
                    return;
                }
                DestroyCam(_unitWarp.Name);
                _unitWarp.Name = _to;
                InitCameraGUI();
                AssetDatabase.Refresh();
            }
            else
            {
                EngineDebug.LogError($"命名修改失败!!!");
            }
        }
        
        public void CamReID(int _from, int _to)
        {
            int _findID = -1;
            for (int i = 0; i < camWarpList.Count; i++)
            {
                int _name = camWarpList[i].ID;
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
                EditorCameraWarp _unitWarp = camWarpList[_findID];
                _unitWarp.ID = _to;
            }
            else
            {
                EngineDebug.LogError($"ID修改失败!!!");
            }
        }

        #region 相机数据存取

        private void LoadAllCam()
        {
            DirectoryInfo dir = new DirectoryInfo(ActionWindowMain.ActionEditorFuntion.GetCamEditorDataPath());
            FileInfo[] fileInfo = dir.GetFiles("*");
            foreach (var VARIABLE in fileInfo)
            {
                string[] _name = VARIABLE.Name.Split('.');
                if (_name[^1] == "json")
                {
                    EditorCameraWarp _cameraWarp = LoadCameraWarp(_name[0]);
                    _cameraWarp.Name = _name[0];
                    camWarpList.Add(_cameraWarp);
                }
            }
            camWarpList.Sort((x, y) => { return x.ID.CompareTo(y.ID);});
            foreach (var _cameraWarp in camWarpList)
            {
                // _cameraWarp.Name = 
                mCameraWarps.Add(_cameraWarp.Name, _cameraWarp);
            }
        }

        private void CreactCameraWarp(string _name)
        {
            int _warpId = 0;
            string _warpName = _name;

            if (mCurCameraWarp is not null)
            {
                _warpId = mCurCameraWarp.ID + 1;
                _warpName = mCurCameraWarp.Name;
            }
            
            for (int i = 0; i < mCameraWarps.Count; i++)
            {
                bool _isFind = false;
                foreach (var VARIABLE in mCameraWarps)
                {
                    if (_warpId == VARIABLE.Value.ID)
                    {
                        _isFind = true;
                        break;
                    }
                }

                if (_isFind)
                {
                    _warpId++;
                }
                else
                {
                    break;
                }
            }

            _warpName += $"_{_warpId}";
            EditorCameraWarp _cameraWarp = new EditorCameraWarp(_warpId, _warpName);

            if (!ActionWindowMain.ActionEditorFuntion.SaveCameraData(_cameraWarp, _warpName))
            {
                EditorUtility.DisplayDialog("警告", "相机数据储存失败\n客户端未实现相机数据的存储方法", "我知道了");
            }

            InitCameraGUI();
        }
        
        
        public void SaveCameraWarp()
        {
            foreach (var _cameraWarp in mCameraWarps)
            {
                ActionWindowMain.ActionEditorFuntion.SaveCameraData(_cameraWarp.Value, _cameraWarp.Key);
            }
            AssetDatabase.Refresh();
        }

        private EditorCameraWarp LoadCameraWarp(string _name)
        {
            if (!ActionWindowMain.ActionEditorFuntion.LoadCameraData(out EditorCameraWarp _cameraWarp,_name))
            {
                string _debug =
                    "相机数据读取失败" + "\n" +
                    "客户端未实现相机数据的读取方法" + "\n" +
                    $"ID: {_name}" + "\n" +
                    "";
                EditorUtility.DisplayDialog("警告", _debug, "我知道了");
                return null;
            }
            return _cameraWarp;
        }
        

        #endregion
    }
}
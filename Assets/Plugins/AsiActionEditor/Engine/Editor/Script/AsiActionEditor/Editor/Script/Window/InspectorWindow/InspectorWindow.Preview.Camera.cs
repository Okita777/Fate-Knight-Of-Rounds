using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
#if Cinemachine
using Cinemachine;
#endif
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class InspectorWindow
    {
        private GameObject mCurCamera;
        private CameraControl mCurCameraControl;
        private List<Transform> _cameraControlList = new List<Transform>();
        private List<Transform> _cameraControlList_Re = new List<Transform>();
#if Cinemachine
        private List<CinemachineVirtualCameraBase> _cinemachineList = new List<CinemachineVirtualCameraBase>();
#endif
        private Dictionary<string, List<Behaviour>> mCinemachine = new Dictionary<string, List<Behaviour>>();
        private GUIStyle mCineGUI = new GUIStyle();

        // private int m_CamNewID;
        // private string m_CamNewName;

        private void DrawCameraWarpGUI(EditorCameraWarp _cameraWarp)
        {
            //头部绘制
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("序号：", GUILayout.Width(60));
                // m_newID = EditorGUILayout.IntField(m_newID);
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    int _newID = EditorGUILayout.DelayedIntField(_cameraWarp.ID);
                    if (_check.changed)
                    {
                        ResourcesWindow.Instance.CamReID(_cameraWarp.ID, _newID);
                        ResourcesWindow.Instance.Repaint();
                    }
                }
                // m_CamNewID = EditorGUILayout.IntField("序号：", m_CamNewID);
                // bool _isChange = m_CamNewID != _cameraWarp.ID;
                // using (new GUIColorScope(Color.red, _isChange))
                // {
                //     if (_isChange)
                //     {
                //         if (Event.current.type == EventType.KeyDown)
                //         {
                //             if (Event.current.keyCode == KeyCode.Return)
                //             {
                //                 ResourcesWindow.Instance.CamReID(_cameraWarp.ID, m_CamNewID);
                //                 ResourcesWindow.Instance.Repaint();
                //             }
                //         }
                //     }
                //
                //     if (GUILayout.Button("应用",GUILayout.Width(39)))
                //     {
                //         ResourcesWindow.Instance.CamReID(_cameraWarp.ID, m_CamNewID);
                //         ResourcesWindow.Instance.Repaint();
                //     }
                // }
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("名称：", GUILayout.Width(60));
                // m_newName = EditorGUILayout.TextField(m_newName);
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    string _newName = EditorGUILayout.DelayedTextField(_cameraWarp.Name);
                    if (_check.changed)
                    {
                        ResourcesWindow.Instance.CamReName(_cameraWarp.Name, _newName);
                        ResourcesWindow.Instance.Repaint();
                    }
                }
                // m_CamNewName = EditorGUILayout.TextField("名称：",m_CamNewName);
                // bool _isChange = m_CamNewName != _cameraWarp.Name;
                // using (new GUIColorScope(Color.red, _isChange))
                // {
                //     if (_isChange)
                //     {
                //         if (Event.current.type == EventType.KeyDown)
                //         {
                //             if (Event.current.keyCode == KeyCode.Return)
                //             {
                //                 ResourcesWindow.Instance.CamReName(_cameraWarp.Name, m_CamNewName);
                //                 ResourcesWindow.Instance.Repaint();
                //             }
                //         }
                //     }
                //
                //     if (GUILayout.Button("应用",GUILayout.Width(39)))
                //     {
                //         ResourcesWindow.Instance.CamReName(_cameraWarp.Name, m_CamNewName);
                //         ResourcesWindow.Instance.Repaint();
                //     }
                // }
            }
            // _cameraWarp.ID = EditorGUILayout.IntField("序号：",_cameraWarp.ID);
            // _cameraWarp.Name = EditorGUILayout.TextField("名称：",_cameraWarp.Name);
            
            GUILayout.Space(10);
            using (new GUIColorScope(Color.cyan))
            {
                GUILayout.Label("资产数据:");
            }
            using (new GUILayout.HorizontalScope())
            {
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    DrawEditorAttribute.Draw(_cameraWarp);
                    if (_check.changed)
                    {
                        //todo: 相机加载
                        mCurCamera = AssetDatabase.LoadAssetAtPath<GameObject>(_cameraWarp.ModelPath);
                        if (!mCurCamera.TryGetComponent(out CameraControl _cameraControl))
                        {
                            mCurCamera = null;
                            _cameraWarp.ModelPath = String.Empty;
                            EditorUtility.DisplayDialog("警告", "当前对象未检测到父类为 [CameraControl] 的组件，请检测对象", "我知道了");
                        }
                        needInit = true;
                    }
                }
            }
            
            if (!InitCameraWarp(_cameraWarp))
            {
                return;
            }


            GUILayout.Space(10);
            if (mCurCamera is not null)
            {
                if (GUILayout.Button("将场景相机配置至此"))
                {
                    SetConfig(_cameraWarp);
                }
                
                GUILayout.Space(10);
                if (mCurCameraControl is not null)
                {
                    DrawCamList();
                }
            }
            // else
            // {
            //     using (new GUIColorScope(Color.red))
            //     {
            //         GUILayout.Label("未配置预制体!!!");
            //     }
            // }
        }

        private bool InitCameraWarp(EditorCameraWarp _cameraWarp)
        {
            if (!needInit)
            {
                return true;
            }

            // //初始化赋值
            // m_CamNewID = _cameraWarp.ID;
            // m_CamNewName = _cameraWarp.Name;
            
            mCurCamera = null;
            mCurCameraControl = null;
            if (!string.IsNullOrEmpty(_cameraWarp.ModelPath))
            {
                //todo：相机预制体加载
                mCurCamera = AssetDatabase.LoadAssetAtPath<GameObject>(_cameraWarp.ModelPath);
                // EngineDebug.Log($"读取了: {mCurCamera is not null}");
                if (mCurCamera is null)
                {
                    return false;
                }
                mCurCamera.TryGetComponent(out mCurCameraControl);
            }

            mCineGUI.normal.textColor = Color.white;

            InitDic();
            
            needInit = false;
            return true;
        }

        private void SetConfig(EditorCameraWarp _cameraWarp)
        {
            if (mCurCameraControl is not null)
            {
                if (ResourcesWindow.Instance.PreCamera is not null)
                {
                    bool _isSelectCam = false;
                    
                    CameraControl _cameraControl = ResourcesWindow.Instance.PreCamControl;
                    GameObject _selectObj = Selection.activeGameObject;
                    if (_selectObj is not null && _selectObj.TryGetComponent(out _cameraControl))
                    {
                        if (!EditorUtility.DisplayDialog("警告", "是否将当前 场景中选中的相机 数据覆盖至当前 列表选中的相机 ？", "确定", "取消"))
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (!ResourcesWindow.Instance.CurCamIsPreCam())
                        {
                            if (!EditorUtility.DisplayDialog("警告", "是否将当前预览相机数据覆盖至当前选择相机", "确定", "取消"))
                            {
                                return;
                            }
                        }
                    }


                    // Transform _defaultTraget = _cameraControl.defaultLookTarget;
                    // // Transform _curTraget = null;
                    // if (!_defaultTraget)
                    // {
                    //     EditorUtility.DisplayDialog("警告", "未配置默认目标", "我知道了");
                    //     return;
                    // }//找到预览相机的默认注视对象
                    //
                    // _cameraControlList.Clear();
                    // _cameraControlList_Re.Clear();
                    //
                    // _cameraControlList.AddRange(_cameraControl.GetComponentsInChildren<Transform>(true));
                    // _cameraControlList.RemoveAt(0);
                    // // EngineDebug.Log("第一个层级对象: " + _cameraControlList[0].name);
                    // //转化cinemachine目标
                    // foreach (var _cinemachine in _cameraControl.GetComponentsInChildren<CinemachineVirtualCameraBase>(true))
                    // {
                    //     Transform _followTrans = _cinemachine.Follow;
                    //     if (!_cameraControlList.Contains(_followTrans))
                    //     {
                    //         _cameraControlList_Re.Add(_followTrans);
                    //         _cinemachine.Follow = _defaultTraget;
                    //     }
                    //     
                    //     Transform _lockTrans = _cinemachine.LookAt;
                    //     if (!_cameraControlList.Contains(_lockTrans))
                    //     {
                    //         _cameraControlList_Re.Add(_lockTrans);
                    //         _cinemachine.LookAt = _defaultTraget;
                    //     }
                    // }
                    //
                    // string _path = ActionWindowMain.GetAssetPathToResources(_cameraWarp.ModelPath,"prefab");
                    // PrefabUtility.CreatePrefab(_path, _cameraControl.gameObject);
                    PrefabUtility.CreatePrefab(_cameraWarp.ModelPath, _cameraControl.gameObject);
                    //
                    // //还原
                    // int _Re_Id = 0;
                    // foreach (var _cinemachine in _cameraControl.GetComponentsInChildren<CinemachineVirtualCameraBase>(true))
                    // {
                    //     Transform _followTrans = _cinemachine.Follow;
                    //     if (_followTrans == _defaultTraget)
                    //     {
                    //         _cinemachine.Follow = _cameraControlList_Re[_Re_Id];
                    //         _Re_Id++;
                    //     }
                    //     
                    //     Transform _lockTrans = _cinemachine.LookAt;
                    //     if (_lockTrans == _defaultTraget)
                    //     {
                    //         _cinemachine.LookAt = _cameraControlList_Re[_Re_Id];
                    //         _Re_Id++;
                    //     }
                    // }
                }
                
#if Cinemachine
                _cinemachineList.Clear();
                Transform _transform = mCurCamera.transform;
                for (int i = 0; i < _transform.childCount; i++)
                {
                    foreach (var _cinemachine in _transform.GetChild(i).GetComponentsInChildren<CinemachineVirtualCameraBase>(true))
                    {
                        _cinemachineList.Add(_cinemachine);
                        // _cinemachine.gameObject.SetActive(true);
                    }
                }
                mCurCameraControl.allCinemachine = _cinemachineList.ToArray();
#endif
            }
            EditorUtility.SetDirty(mCurCamera);
            AssetDatabase.SaveAssetIfDirty(mCurCamera);
            // ResourcesWindow.Instance.CreactPreviewCamera();
            
            InitDic();
        }

        private void DrawCamList()
        {
            foreach (var _key in mCinemachine.Keys)
            {
                using (new GUIColorScope(Color.green))
                {
                    GUILayout.Label(_key);
                }

                foreach (var _behaviour in mCinemachine[_key])
                {
                    string[] _typeName = _behaviour.GetType().ToString().Split('.');
                    GUILayout.Label(
                        "           <color=#696969>Cinemachine:  </color>" +
                        _behaviour.name +
                        $"    [<color=#FFCC00> {_typeName[1]} </color>]",
                        mCineGUI
                    );
                }
                
                GUILayout.Space(10);
            }
        }

        private void InitDic()
        {
            // UpdateConfig();
            mCinemachine.Clear();
            if (mCurCameraControl is null || mCurCameraControl.allCinemachine is null) return;
            foreach (var _cinemachine in mCurCameraControl.allCinemachine)
            {
                string _parentName = _cinemachine.transform.parent.name;
                if (mCinemachine.ContainsKey(_parentName))
                {
                    mCinemachine[_parentName].Add(_cinemachine);
                }
                else
                {
                    List<Behaviour> _list = new List<Behaviour>();
                    _list.Add(_cinemachine);
                    mCinemachine.Add(_parentName, _list);
                }
            }
        }

        public void UpdateConfig(string _path)
        {
#if Cinemachine
            GameObject _loadCam = Resources.Load<GameObject>(_path);
            if (_loadCam != null && _loadCam.TryGetComponent(out CameraControl _cameraControl))
            {
                if (_cameraControl is null) _cameraControl = mCurCameraControl;
                if (_cameraControl is null) return;
                _cameraControl.allCinemachine =
                    _cameraControl.GetComponentsInChildren<CinemachineVirtualCameraBase>(true);
                EditorUtility.SetDirty(_cameraControl);
                AssetDatabase.SaveAssets();
                
                // EngineDebug.LogWarning(AssetDatabase.GetAssetPath(_cameraControl));
            }
#endif
        }
    }
}
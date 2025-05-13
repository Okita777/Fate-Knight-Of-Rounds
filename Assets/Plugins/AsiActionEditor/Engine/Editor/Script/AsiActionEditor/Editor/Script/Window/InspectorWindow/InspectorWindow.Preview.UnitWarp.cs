using System;
using System.Collections.Generic;
using System.IO;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class InspectorWindow
    {
        private GameObject mModel;
        private ActionEngineSetting mEngineSetting = null;
        private ReorderableList ActionLables;
        private ReorderableList ActionTypes;
        private int mSelectCamID = 0;
        private int mSelectActionStateID;
        private int mSelectMenuID = 0;
        private int mSlectTypeListID = -1;
        private string mTypeListReName = String.Empty;
        private List<string> mActionState = new List<string>();
        private List<string> mCameras = new List<string>();

        private string mNewActionName;
        private void DrawUnitWarp(EditorUnitWarp _unitWarp)
        {
            //头部绘制
            if (!InitUnitWarp(_unitWarp))
            {
                return;
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("序号：", GUILayout.Width(60));
                // m_newID = EditorGUILayout.IntField(m_newID);
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    int _newID = EditorGUILayout.DelayedIntField(_unitWarp.ID);
                    if (_check.changed)
                    {
                        ResourcesWindow.Instance.UnitReID(_unitWarp.ID, _newID);
                        ResourcesWindow.Instance.Repaint();
                    }
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("名称：", GUILayout.Width(60));
                // m_newName = EditorGUILayout.TextField(m_newName);
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    string _newName = EditorGUILayout.DelayedTextField(_unitWarp.Name);
                    if (_check.changed)
                    {
                        ResourcesWindow.Instance.UnitReName(_unitWarp.Name, _newName);
                        ResourcesWindow.Instance.Repaint();
                    }
                }
            }

            //资产数据
            GUILayout.Space(20);
            using (new GUIColorScope(Color.cyan))
            {
                GUILayout.Label("资产数据: ");
            }
            using (new GUILayout.HorizontalScope())
            {
                using (var a = new EditorGUI.ChangeCheckScope())
                {
                    DrawEditorAttribute.Draw(_unitWarp, new []{"ModelPath"});
                    // _model = (GameObject)EditorGUILayout.ObjectField(mModel, typeof(GameObject), false);
                    if (a.changed)
                    {
                        GameObject _model = Resources.Load<GameObject>(_unitWarp.ModelPath);

                        if (_model is not null)
                        {
                            if (_model.TryGetComponent(out Animator _animator))
                            {
                                Action _setCallback = () =>
                                {
                                    needInit = true;
                                    // mModel = _model;
                                    // _unitWarp.modelPath = AssetDatabase.GetAssetPath(mModel);
                                };
            
                                if (!_model.TryGetComponent(out Unit _value))
                                {
                                    bool _createUnit = EditorUtility.DisplayDialog(
                                        "警告",
                                        "模型格式错误，未挂载 Unit 相关脚本, 是否自动挂载",
                                        "OK",
                                        "关闭"
                                    );
                                    if (_createUnit)
                                    {
                                        _model.AddComponent<Unit>();
                                        EditorUtility.SetDirty(_model);
                                        _setCallback();
                                    }
                                }
                                else
                                {
                                    _setCallback();
                                }
                            }
                            else
                            {
                                EditorUtility.DisplayDialog(
                                    "警告",
                                    "模型格式错误，请确认是否挂载Animator",
                                    "OK"
                                );
                                _unitWarp.ModelPath = string.Empty;
                            }
                        }
                    }
                }
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("相机单位： ",GUILayout.Width(80));
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    mSelectCamID = EditorGUILayout.Popup(mSelectCamID, mCameras.ToArray());
                    if (_check.changed)
                    {
                        _unitWarp.CameID = ResourcesWindow.Instance.CameraWarps[mCameras[mSelectCamID]].Name;
                    }
                }
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("编辑器设置文件: ", GUILayout.Width(100));
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    mEngineSetting = 
                        (ActionEngineSetting)EditorGUILayout.ObjectField(mEngineSetting, typeof(ActionEngineSetting),false);
                    if (_check.changed)
                    {
                        _unitWarp.SettingPath = AssetDatabase.GetAssetPath(mEngineSetting);
                    }
                }
            }



            //Action组选择 新建 复制
            GUILayout.Space(20);
            using (new GUIColorScope(Color.cyan))
            {
                GUILayout.Label("角色Action组: ");
            }
            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                using (new GUILayout.HorizontalScope())
                {
                    mSelectActionStateID = 
                        EditorGUILayout.Popup(mSelectActionStateID, mActionState.ToArray());
                    using (new GUIColorScope(Color.red))
                    {
                        if (GUILayout.Button("X", GUILayout.Width(25)))
                        {
                            if (mSelectActionStateID < mActionState.Count)
                            {
                                if (EditorUtility.DisplayDialog(
                                        "警告", "你正在尝试删除Action组，确定继续吗", "确定", "取消"))
                                {
                                    AssetDatabase.DeleteAsset(
                                        ActionWindowMain.ActionEditorFuntion.GetActionEditorDataPath(
                                            mActionState[mSelectActionStateID])
                                    );

                                    //将绝对路径转换为项目路径
                                    string _path = string.Format(ActionWindowMain.ActionEditorFuntion.GetRunTimeDataPath(),"Action",mActionState[mSelectActionStateID]);
                                    AssetDatabase.DeleteAsset(_path);
                                    // string _path = ActionWindowMain.ActionEditorFuntion.GetActionDataPath(
                                    //     mActionState[mSelectActionStateID]);
                                    // if (ResourcesWindow.Instance.GetPorjectPathTo(out string _fullPath,_path))
                                    // {
                                    //     AssetDatabase.DeleteAsset(_fullPath);
                                    // }
                                    // else
                                    // {
                                    //     AssetDatabase.DeleteAsset(ActionWindowMain.GetAssetPathToResources(_path));
                                    // }

                                    needInit = true;
                                    GUI.changed = true;
                                }
                            }
                        }
                    }
                }

                if (_check.changed)
                {
                    _unitWarp.Action = mActionState[mSelectActionStateID];
                }
            }
            
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("新建",GUILayout.Width(50)))
                {
                    CreateActionState(ref mNewActionName, _unitWarp);
                }
                
                if (GUILayout.Button("复制",GUILayout.Width(50)))
                {
                    CreateActionState(ref mNewActionName, _unitWarp, true);
                }
                mNewActionName = EditorGUILayout.TextField(mNewActionName);
            }
            
            //是否为玩家
            GUILayout.Space(20);
            using (new GUIColorScope(Color.cyan))
            {
                GUILayout.Label("角色类型: ");
            }
            _unitWarp.IsPlayer = EditorGUILayout.ToggleLeft("玩家", _unitWarp.IsPlayer);
            
            GUILayout.Space(15);
            DrawEditorAttribute.Draw(_unitWarp,new []{"GValueSetting"});
            
            //以下数据全部归属于Action组
            GUILayout.Space(15);
            GUIStyle _style = new GUIStyle();
            _style.normal.textColor = Color.cyan;
            GUILayout.Label("基础Action参数配置: ",_style);
            GUILayout.Label("<color=#FFCC00>(此类数据来源于Action组)</color>",_style);
            string _ActionGroupName = ResourcesWindow.Instance.ActionGroupName;
            if (_ActionGroupName == _unitWarp.Action)
            {
                if (GUILayout.Button("保存Action参数配置"))
                {
                    ResourcesWindow.Instance.SaveActionStateInfo(_ActionGroupName, true);
                }
                
                GUILayout.Space(5);
                GUILayout.Label("初始化时执行的Action");
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Default层： ", GUILayout.Width(65));
                    ResourcesWindow.Instance.SetStartAction
                        (EditorGUILayout.TextField(ResourcesWindow.Instance.StartAction));
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Limb层： ", GUILayout.Width(65));
                    ResourcesWindow.Instance.SetStartAction_Limb
                        (EditorGUILayout.TextField(ResourcesWindow.Instance.StartAction_Limb));
                }
                // using (new GUILayout.HorizontalScope())
                // {
                //     GUILayout.Label("Upper层： ", GUILayout.Width(65));
                //     ResourcesWindow.Instance.SetStartAction_Upper
                //         (EditorGUILayout.TextField(ResourcesWindow.Instance.StartAction_Upper));
                // }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Script层： ", GUILayout.Width(65));
                    ResourcesWindow.Instance.SetStartAction_Script
                        (EditorGUILayout.TextField(ResourcesWindow.Instance.StartAction_Script));
                }
                
                GUILayout.Space(5);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("受击Action： ", GUILayout.Width(65));
                    ResourcesWindow.Instance.SetHitAction
                        (EditorGUILayout.TextField(ResourcesWindow.Instance.HitAction));
                }

                using (new GUIColorScope(Color.grey))
                {
                    GUILayout.Label("警告：受击Action并不直接播放受击动画！！");
                    GUILayout.Label("而是在受击时调用受击Action下的打断轨！！");
                    GUILayout.Space(5);
                }

                mSelectMenuID = GUILayout.Toolbar(mSelectMenuID, new[] { "ActionType", "ActionLable" });
                if (mSelectMenuID == 0)
                {
                    ActionTypes.DoLayoutList();
                }
                else if (mSelectMenuID == 1)
                {
                    ActionLables.DoLayoutList();
                }
            }
            else
            {
                GUILayout.Label("请点击预览以加载当前Action组数据");
            }
        }

        private bool InitUnitWarp(EditorUnitWarp _unitWarp)
        {
            if (!needInit)
            {
                return true;
            }

            //加载模型资源路径
            // mModel = AssetDatabase.LoadAssetAtPath<GameObject>(_unitWarp.ModelPath);
            mEngineSetting =  AssetDatabase.LoadAssetAtPath<ActionEngineSetting>(_unitWarp.SettingPath);
            
            //加载Action文件
            InitActionState(_unitWarp);
            
            //Action标签列表
            ActionLables = new ReorderableList(ResourcesWindow.Instance.ActionLable, null, false, true, true, true);
            ActionLables.drawElementCallback = (rect, index, active, focused) =>
            {
                rect.width -= 20;
                rect.x += 20;
                ResourcesWindow.Instance.ActionLable[index] =
                    EditorGUI.TextField(rect, ResourcesWindow.Instance.ActionLable[index]);
                // _unitWarp.ActionLabel[index] = EditorGUI.TextField(rect, _unitWarp.ActionLabel[index]);
            };
            ActionLables.drawHeaderCallback = rect =>
            {
                GUI.Label(rect, "Action标签列表");
            };
            ActionLables.onAddCallback = list =>
            {
                list.list.Add("Default");
                // list.index
            };
            ActionLables.onRemoveCallback = list =>
            {
                list.list.RemoveAt(list.index);
                GUI.changed = true;
            };

            // //初始化参数
            // m_UnitNewID = _unitWarp.ID;
            // m_UnitNewName = _unitWarp.Name;
            
            //初始化相机列表
            ResourcesWindow.Instance.InitCameraGUI();
            int i = 0;
            mSelectCamID = -1;
            mCameras.Clear();
            foreach (var _cameraWarpKey in ResourcesWindow.Instance.CameraWarps)
            {
                mCameras.Add(_cameraWarpKey.Key);
                if (_cameraWarpKey.Value.Name == _unitWarp.CameID)
                {
                    mSelectCamID = i;
                }
                i++;
            }

            mSlectTypeListID = -1;
            ActionTypes = new ReorderableList(ResourcesWindow.Instance.ActionType, null, true, true, true, true);
            ActionTypes.drawElementCallback = (rect, index, active, focused) =>
            {
                if (mSlectTypeListID == index)
                {
                    int _width = 80;
                    Rect _reName = new Rect(rect);
                    _reName.width -= _width;
                    mTypeListReName = EditorGUI.TextField(_reName, mTypeListReName);
                    _reName.x += _reName.width;
                    _reName.width = _width;

                    bool _ischange = mTypeListReName != ResourcesWindow.Instance.ActionType[index];
                    using (new GUIColorScope(Color.red,_ischange))
                    {
                        if (GUI.Button(_reName, "应用更名"))
                        {
                            if (_ischange)
                            {
                                bool _isReName = ActionEngineReName.ReActionTypeName
                                    (ResourcesWindow.Instance.ActionType[index], mTypeListReName);
                                if (_isReName)
                                {
                                    ResourcesWindow.Instance.ActionType[index] = mTypeListReName;
                                }
                            }
                        }
                    }
                }
                else
                {
                    EditorGUI.TextField(rect, ResourcesWindow.Instance.ActionType[index]);
                }
            };
            ActionTypes.drawHeaderCallback = rect =>
            {
                GUI.Label(rect, "Action状态列表");
            };
            ActionTypes.onAddCallback = list =>
            {
                list.list.Add("Default");
                // list.index
            };
            ActionTypes.onRemoveCallback = list =>
            {
                string _name = ResourcesWindow.Instance.ActionType[list.index];
                if(EditorUtility.DisplayDialog("警告",$"确定要删除 {_name} 吗","确定","取消"))
                {
                    list.list.RemoveAt(list.index);
                    mSlectTypeListID = -1;
                    GUI.changed = true;
                }

            };
            ActionTypes.onSelectCallback = list =>
            {
                mSlectTypeListID = list.index;
                mTypeListReName = ResourcesWindow.Instance.ActionType[mSlectTypeListID];
            };
            
            needInit = false;
            return true;
        }
        
        private void CreateActionState(ref string _name, UnitWarp _unitWarp, bool _isCopy = false)
        {
            if (!string.IsNullOrEmpty(_name))
            {
                if (!mActionState.Contains(_name))
                {
                    ResourcesWindow.Instance.SaveActionStateInfo(_name);
                    _unitWarp.Action = _name;
                    //mSelectActionStateID = mActionState.Count;
                    needInit = true;
                    GUI.changed = true;
                }
            }
            _name = string.Empty;
        }

        //初始化
        private void InitActionState(UnitWarp _unitWarp)
        {
            mActionState.Clear();
            // DirectoryInfo dir = new DirectoryInfo(MotionEngineConst.EditorActionSavePath());
            DirectoryInfo dir = new DirectoryInfo(ActionWindowMain.ActionEditorFuntion.GetActionEditorDataPath());
            FileInfo[] fileInfo = dir.GetFiles("*");
            foreach (var VARIABLE in fileInfo)
            {
                string[] _name = VARIABLE.Name.Split('.');
                if (_name[^1] == "json")
                    mActionState.Add(_name[0]);
            }

            //从unit储存的名字寻找序号
            bool _isFind = false;
            for (int i = 0; i < mActionState.Count; i++)
            {
                if (mActionState[i] == _unitWarp.Action)
                {
                    mSelectActionStateID = i;
                    _isFind = true;
                }
            }

            if (!_isFind)
            {
                mSelectActionStateID = 0;
                if (mActionState.Count > 0)
                {
                    _unitWarp.Action = mActionState[0];
                }
                else
                {
                    _unitWarp.Action = string.Empty;
                }
            }
            

        }
    }
}
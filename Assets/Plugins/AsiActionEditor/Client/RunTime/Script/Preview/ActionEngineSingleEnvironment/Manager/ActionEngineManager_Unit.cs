using System;
using System.Collections.Generic;
using System.IO;
using AsiActionEngine.RunTime;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace AsiTimeLine.RunTime
{
    public partial class ActionEngineManager_Unit
    {
        #region Instance

        private static ActionEngineManager_Unit _instance;
        public static ActionEngineManager_Unit Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new ActionEngineManager_Unit();
                }

                return _instance;
            }
        }

        #endregion
        private List<Unit> mUnits = new List<Unit>();
        // private Dictionary<string, int> mUnitWarpInfo = new Dictionary<string, int>();//角色列表数据
        private Dictionary<int, UnitWarp> mUnitWarpInfo = new Dictionary<int, UnitWarp>();//角色列表数据
        private Dictionary<string, ActionStateInfo> mActionInfo = new Dictionary<string, ActionStateInfo>();//角色行为列表数据

        public List<Unit> Units => mUnits;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init() => OnStart();
        /// <summary>
        /// 每帧执行
        /// </summary>
        /// <param name="_deltatime">每帧间隔时间</param>
        public void Update(float _deltatime) => OnUpdate(_deltatime);
        /// <summary>
        /// 每帧执行（Late）
        /// </summary>
        /// <param name="_deltatime">每帧间隔时间</param>
        public void LateUpdate(float _deltatime) => OnLateUpdate(_deltatime);
        /// <summary>
        /// 添加单位
        /// </summary>
        /// <param name="_unit"></param>
        public void AddUnit(Unit _unit) => OnAddUnit(_unit);

        // public void LoadUnitWarpInfo(UnitWarpInfo _unitWarpInfo) => OnLoadUnitWarpInfo(_unitWarpInfo);
        

        #region PublicFunction

        /// <summary>
        /// 获取单位的封装数据
        /// </summary>
        /// <param name="_name">单位名称</param>
        /// <param name="_loadCallback">加载结束后的回调</param>
        /// <returns></returns>
        public void GetUnitWarp(int _name, Action<UnitWarp> _loadCallback) => OnGetUnitWarp(_name, _loadCallback);
        public void GetActionList(string _name, Action<ActionStateInfo> _loadCallback) => OnGetActionList(_name, _loadCallback);

        public void GetActionList_Editor(string _name, Action<ActionStateInfo> _loadCallback) => OnGetActionList_Editor(_name, _loadCallback);
        #endregion 

        #region Function
        private void OnStart()
        {
            //初始化列表和字典
            mUnits.Clear();
            mUnitWarpInfo.Clear();
            mActionInfo.Clear();
            mCameraPool.Clear();
            mUnitPool.Clear();
            mCameraWarps.Clear();
            
            CreactTimeLineUpdateMode();
        }
        
        private void OnUpdate(float _deltatime)
        {
            foreach (var _unit in mUnits)
            {
                _unit.OnUpdate(_deltatime);
            }
        }
        
        private void OnLateUpdate(float _deltatime)
        {
            foreach (var _unit in mUnits)
            {
                _unit.OnLateUpdate(_deltatime);
            }
        }

        private void OnAddUnit(Unit _unit)
        {
            mUnits.Add(_unit);
        }

        
        private void OnGetUnitWarp(int _id, Action<UnitWarp> _loadCallback)
        {
            if (mUnitWarpInfo.TryGetValue(_id, out var _unitWarp))
            {
                _loadCallback(_unitWarp);
            }
            else
            {
                OnLoadUnitWarpInfo(_id, _loadCallback);
            }
        }

        private List<Action<ActionStateInfo>> _loadCallbacks = new List<Action<ActionStateInfo>>();
        private void OnGetActionList(string _name, Action<ActionStateInfo> _loadCallback)
        {
            if (mActionInfo.ContainsKey(_name))
            {
                _loadCallback(mActionInfo[_name]);
            }
            else
            {
                _loadCallbacks.Add(_loadCallback);//异步加载结束前收集所有回调
                if (_loadCallbacks.Count > 1) return;//申请过一次就别再申请了
                //如果字典中不存在这个行为列表，则自行加载
                ActionEnginLoadData.Instance.LoadUnitAction(_name, (list =>
                    {
                        mActionInfo.Add(_name,list);
                        foreach (var VARIABLE in _loadCallbacks)
                        {
                            VARIABLE(list);
                        }
                        _loadCallbacks.Clear();
                    })
                );
            }
        }
        private void OnGetActionList_Editor(string _name, Action<ActionStateInfo> _loadCallback)
        {
            if (mActionInfo.ContainsKey(_name))
            {
                mActionInfo.Remove(_name);
                ActionEnginLoadData.Instance.LoadUnitAction(_name, (list =>
                    {
                        mActionInfo.Add(_name,list);
                        _loadCallback(list);
                    })
                );
                EngineDebug.LogWarning("加载Json");
            }
            else
            {
                EngineDebug.DisplayDialog("警告", "未加载过此Action", "我知道了");
            }
            // else
            // {
            //     //如果字典中不存在这个行为列表，则自行加载
            //     OnLoadUnitAction(_name, (list =>
            //         {
            //             mActionInfo.Add(_name,list);
            //             _loadCallback(list);
            //         })
            //     );
            // }
        }
        private void OnLoadUnitWarpInfo(int _id, Action<UnitWarp> _unitWarpInfo)
        {
            if (mUnitWarpInfo.Count < 1)
            {
                // Object[] _allObj =
                //     UnityEditor.AssetDatabase.LoadAllAssetsAtPath(ActionEngineRuntimePath.Instance.UnitPath()); 
                // Resources.LoadAll(ActionEngineRuntimePath.Instance.UnitPath());
                // string _debug = "";
                // foreach (var VARIABLE in _allObj)
                // {
                //     _debug += "Action名字: " + VARIABLE.name + "\n";
                // }
                // EngineDebug.Log(ActionEngineRuntimePath.Instance.UnitPath());
                // EngineDebug.Log(_debug);

                // EngineDebug.Log(((TextAsset)_allObj[0]).text);
                
                // foreach (var VARIABLE in _allObj)
                // {
                //     if(VARIABLE is TextAsset _textAsset)
                //     {
                //         string _str = _textAsset.text;
                //         UnitWarp _unit = new UnitWarp(0, "null", new GValue_Setting());
                //         JsonUtility.FromJsonOverwrite(_str, _unit);
                //
                //         //将加载好的Unit数据添加到字典
                //         mUnitWarpInfo.Add(_unit.ID, _unit);
                //     }
                // }
                DirectoryInfo dir = new DirectoryInfo(ActionEngineRuntimePath.Instance.UnitPath());
                FileInfo[] fileInfo = dir.GetFiles("*");

#if UNITY_EDITOR
                string _DebugString = "<color=#FFCC00>检测到重复ID的Unit！！！</color>\n";
                bool _iscf = false;
#endif
                
                foreach (var VARIABLE in fileInfo)
                {
                    if (VARIABLE.Name.Split('.')[^1] == ActionEngineRuntimePath.SuffixesP)
                    {
                        ActionEnginLoadData.Instance.LoadUnit(VARIABLE.Name, (value) =>
                        {
#if UNITY_EDITOR
                            if (mUnitWarpInfo.ContainsKey(value.ID))
                            {
                                _DebugString += value.Name + $" ({mUnitWarpInfo[value.ID].Name})" + "、 ";
                                _iscf = true;
                            }else
#endif
                            mUnitWarpInfo.Add(value.ID, value);
                        });
                    }
                }
#if UNITY_EDITOR
                if (_iscf)
                {
                    EngineDebug.LogError(_DebugString);
                }
#endif
                
                if (mUnitWarpInfo.TryGetValue(_id, out UnitWarp _unitWarpv))
                {
                    _unitWarpInfo(_unitWarpv);
                }
                else
                {
                    EngineDebug.Log($"Unit数据加载失败:  {_id}");
                }
            }
            else
            {
                if(mUnitWarpInfo.TryGetValue(_id, out UnitWarp _unitWarp))
                {
                    _unitWarpInfo(_unitWarp);
                }
            }
        }

        // private void OnLoadUnitAction(string _name, Action<ActionStateInfo> _loadCallback)
        // {
        //     LoadingData.Instance.LoadUnitAction(_name, (value) =>
        //     {
        //         _loadCallback(value);
        //     });
        //     // string _path = ActionEngineRuntimePath.Instance.ActionPath(_name);
        //     //
        //     // TextAsset _textAsset = Resources.Load<TextAsset>(_path);
        //     // if (_textAsset is null)
        //     // {
        //     //     EngineDebug.LogWarning($"单位数据加载出错， \n路径: <color=#FFCC00>{_path}</color>");
        //     // }
        //     // else
        //     // {
        //     //     string _str = _textAsset.text;
        //     //     ActionStateInfo _info = new ActionStateInfo(null, _name);
        //     //     JsonUtility.FromJsonOverwrite(_str, _info);
        //     //     _loadCallback(_info);
        //     // }
        // }
        
        // private void OnLoad
        #endregion

        #region TimeLineUpdate

        private void CreactTimeLineUpdateMode()
        {
#if UNITY_EDITOR
            GameObject _TimeLineObj = new GameObject();
            _TimeLineObj.name = "TimeLineObj";
            _TimeLineObj.AddComponent<ActionTimeLineUpdate>();
#endif
        }
        

        #endregion
    }
}
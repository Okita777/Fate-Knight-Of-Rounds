using System;
using System.Collections.Generic;
using System.IO;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class ResourcesWindow
    {
        private int mSelectActionType_filter;
        private string mSelectActionState = string.Empty;
        private string mActionGroupName = string.Empty;
        private string mStartAction;
        private string mStartAction_Limb;
        private string mStartAction_Upper;
        private string mStartAction_Script;
        private string mHitAction;
        private List<int> mActionIDList = new List<int>();
        private List<string> mActionType = new List<string>();
        private List<string> mActionLable = new List<string>();
        public List<EditorActionState> mActionState;
        private List<EditorActionState> mActionState_filter = new List<EditorActionState>();
        private Dictionary<int, EditorActionState> mActionStateDic = new Dictionary<int, EditorActionState>();
        private List<EditorActionState> mTemplate;
        private int mTemplateID = 0;

        public bool ActionIsChange = false;
        public ActionStatePart ActionStatePart;
        public EditorActionStateInfo ActionInfo;
        public string ActionGroupName => mActionGroupName;
        public string StartAction => mStartAction;
        public string StartAction_Limb => mStartAction_Limb;
        public string StartAction_Upper => mStartAction_Upper;
        public string StartAction_Script => mStartAction_Script;

        public string HitAction => mHitAction;
        public List<string> ActionType => mActionType;
        public List<string> ActionLable => mActionLable;
        public List<EditorActionState> AllActionState => mActionState;
        public Dictionary<int, EditorActionState> ActionStateDic => mActionStateDic;
        private void InitActionStateGUI()
        {
            mActionState = null;
            mSelectActionState = string.Empty;
            mActionGroupName = "null";
            mSelectActionType_filter = -2;
            // mTemplateID = 0;
            mActionStateDic.Clear();
            // EngineDebug.LogWarning("初始化了");
        }
        private void DrwaActionStateGUI()
        {
            //头部绘制
            using (new GUILayout.HorizontalScope(GUILayout.Width(position.width - 60)))
            {
                if (GUILayout.Button("复制 ", EditorStyles.toolbarButton))
                {
                    if (mActionState == null)
                    {
                        return;
                    }

                    ActionOnChange();
                    CreactNewState(GetEditorActionStateToSelect());
                    ActionListSort();
                }
                if (GUILayout.Button("新建^", EditorStyles.toolbarButton))
                {
                    if (Event.current.button == 1)
                    {
                        CreactNewActionMenu(Event.current);
                        // EngineDebug.LogWarning("鼠标右键");
                    }
                    else
                    {
                        if (mActionState == null)
                        {
                            return;
                        }
                    
                        if (mTemplateID == 0)
                        {
                            int _id = GetNewActionID();
                            CreactNewState();
                        }
                        else
                        {
                            CreactNewState(mTemplate[mTemplateID - 1]);
                        }
                        ActionOnChange();
                    }
                    ActionListSort();
                }
                if (GUILayout.Button("删除", EditorStyles.toolbarButton))
                {
                    if (mActionState == null)
                    {
                        return;
                    }
                    
                    if (mOnSelectActionStateID < mActionState.Count)
                    {
                        if (mSelectActionType_filter > -1)
                        {
                            mActionState_filter.Remove(GetEditorActionStateToSelect());
                        }
                        mActionState.Remove(GetEditorActionStateToSelect());
                    }
                    ActionOnChange();
                }
            }
            GUILayout.Space(2);
            using (new GUILayout.HorizontalScope(GUILayout.Width(position.width - 60)))
            {
                // if (GUILayout.Button("排序 ", EditorStyles.toolbarButton))
                // {
                //     ActionOnChange();
                //     ActionListSort();
                // }

                //筛选
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    mSelectActionType_filter = EditorGUILayout.Popup(mSelectActionType_filter,
                        mActionType.ToArray(), EditorStyles.toolbarPopup);
                    if (_check.changed)
                    {
                        mActionState_filter.Clear();
                        foreach (var _actionState in mActionState)
                        {
                            if (_actionState.EditorActionType.Contains(mActionType[mSelectActionType_filter]))
                            {
                                mActionState_filter.Add(_actionState);
                            }
                        }
                    }
                }

                if (mSelectActionType_filter > -1)
                {
                    if (GUILayout.Button("取消过滤", EditorStyles.toolbarButton))
                    {
                        mSelectActionType_filter = -1;
                    }
                }

            }

            Rect _saveRect = position;
            _saveRect.width = 60;
            _saveRect.x = position.width - _saveRect.width;
            _saveRect.y = EditorGUIUtility.singleLineHeight;
            _saveRect.height = 46;

            using (new GUIColorScope(Color.red, ActionIsChange))
            {
                if (GUI.Button(_saveRect,"保存"))
                {
                    SaveActionState();
                }
            }

            if (mActionState == null)
            {
                return;
            }
            //Action列表

            Rect _rect = new Rect(position);
            float _HeadHeight = 65;
            _rect.y = _HeadHeight;
            _rect.x = 0;
            _rect.height -= _HeadHeight;
            List<EditorActionState> _actionStates = mSelectActionType_filter < 0 ? mActionState : mActionState_filter;
            ActionButtonList.DrawAction(_rect, _actionStates, (EditorActionState _actionState) =>
            {
                // if (Application.isPlaying)
                // {
                //     EditorUtility.DisplayDialog("警告", "运行时暂不能切换动画", "我知道了");
                // }
                // else
                {
                    int _selectID = mActionState.IndexOf(_actionState);
                    OnSelectActionState(_selectID, false, true);
                    AsiActionEngineEditorUpdate.UpdateActionSelectID(_selectID);
                }
            }, mOnSelectActionStateID);
        }
        private Vector2 mActionScroll;

        public void ActionListSort()
        {
            if (mActionState == null)
            {
                return;
            }
            mActionState.Sort((x, y) => { return x.ID.CompareTo(y.ID); });
            if (mSelectActionType_filter > -1)
            {
                mActionState_filter.Sort((x, y) => { return x.ID.CompareTo(y.ID); });
            }
        }
        
        public void SetActionStateName(string _actionName, bool _setIndex = true)
        {
            mSelectActionState = _actionName;

            //_setIndex是仅提供给RuntimeTimeline使用的
            if (_setIndex)
            {
                mOnSelectActionStateID = 0;
                InitActionStateGUI();
            }
        }

        public void SetStartAction(string _name)
        {
            mStartAction = _name;
        }
        
        public void SetStartAction_Limb(string _name)
        {
            mStartAction_Limb = _name;
        }
        
        public void SetStartAction_Upper(string _name)
        {
            mStartAction_Upper = _name;
        }
        
        public void SetStartAction_Script(string _name)
        {
            mStartAction_Script = _name;
        }
        public void SetHitAction(string _name)
        {
            mHitAction = _name;
        }

        public int GetIDToActionType(string _actonType)
        {
            if (mActionType.Contains(_actonType))
            {
                return mActionType.IndexOf(_actonType);
            }

            return -1;
        }

        public void ActionOnChange(bool _change = true, string _undoName = "")
        {
            if (_change)
            {
                AsiActionEngineEditorUpdate.SetActionUndo(_undoName);
            }
            if (ActionIsChange != _change)
            {
                ResourcesWindow.Instance.Repaint();
                ActionIsChange = _change;
            }
        }
        
        public void UpdateActionStateDic()
        {
            mActionStateDic.Clear();
            foreach (var item in mActionState)
            {
                if (!mActionStateDic.TryAdd(item.ID, item))
                {
                    //EngineDebug.LogError
                }
            }
        }

        private int GetNewActionID()
        {
            if (mActionState.Count < 1)
            {
                return 0;
            }
            if(mOnSelectActionStateID >= mActionState.Count)
            {
                return 0;
            }
            mActionIDList.Clear();
            foreach (var _actionState in mActionState)
            {
                mActionIDList.Add(_actionState.ID);
            }

            int findID = mActionState[mOnSelectActionStateID].ID;
            for (int i = mOnSelectActionStateID; i < mActionState.Count; i++)
            {
                // int _id = [findID].ID + 1;
                findID++;
                if (!mActionIDList.Contains(findID))
                {
                    return findID;
                }
            }
            ActionListSort();
            return mActionState[^1].ID + 1;
        }

        private void CreactNewState(EditorActionState _actionState = null)
        {
            int _id = GetNewActionID();
            EditorActionState _newState = null;
            if (_actionState == null || string.IsNullOrEmpty(_actionState.Name))
            {
                _newState = new EditorActionState(_id, $"Action {_id}");
            }
            else
            {
                _newState = _actionState.Clone(_id, _actionState.Name + "_" + _id);
                mActionState.Add(_newState);
                mActionState_filter.Add(_newState);
                return;
            }

            if (mSelectActionType_filter > -1)
            {
                _newState.EditorActionType = mActionType[mSelectActionType_filter];
                string[] _names = mActionType[mSelectActionType_filter].Split('/');
                string _myName = _names[0];
                for (int i = 1; i < _names.Length; i++)
                {
                    _myName += $"_{_names[i]}";
                }

                _newState.Name = $"{_myName} {_id}";
                mActionState_filter.Add(_newState);
            }

            mActionState.Add(_newState);
        }

        #region 数据存取
        public void LoadActionStateInfo(string _actionName)
        {
            mActionState = new List<EditorActionState>();

            //尝试加载客户端序列化的数据
            if (ActionWindowMain.ActionEditorFuntion.LoadActionData(out EditorActionStateInfo _actionInfo, _actionName))
            {
                LoadActionState(_actionInfo, _actionName);
                return;
            }
            
            string _path = MotionEngineConst.EditorActionSavePath(_actionName);
            if (!File.Exists(_path))
            {
                _actionName = string.Empty;
                return;
            }
            
            if (!string.IsNullOrEmpty(_actionName))
            {
                string _str = File.ReadAllText(_path);
                EditorActionStateInfo _LoadInfo = new EditorActionStateInfo(null, _actionName);
                JsonUtility.FromJsonOverwrite(_str, _LoadInfo);
                LoadActionState(_LoadInfo, _actionName);
            }
        }

        public void LoadActionState(EditorActionStateInfo _actionInfo, string _actionName)
        {
            ActionInfo = _actionInfo;
            if (_actionInfo.mActionState != null)
            {
                mActionGroupName = _actionName;
                mActionState = _actionInfo.mActionState;
                mStartAction = _actionInfo.mDefaultAction;
                mStartAction_Limb = _actionInfo.mDefaultAction_Limb;
                mStartAction_Upper = _actionInfo.mDefaultAction_Upper;
                mStartAction_Script = _actionInfo.mDefaultAction_Script;
                mHitAction = _actionInfo.mHitAction;
                mActionType = _actionInfo.mActionType;
                mActionLable = _actionInfo.mActionLable;
                mTemplate = _actionInfo.mTemplates;
                // mTemplateID = _actionInfo.mTemplateID;
                UpdateActionStateDic();
            }
        }

        public void SaveActionState()
        {
            ActionIsChange = false;
            if (!string.IsNullOrEmpty(mSelectActionState))
            {
                SaveActionStateInfo(mSelectActionState, true);
                UpdateActionStateDic();
                if (Application.isPlaying)
                {//如果在运行中，让角色重新加载Action列表
                    TimeLineWindow.Instance.RuntimeSaveAndUpdate();
                }
            }
            else
            {
                EngineDebug.LogWarning("未设定Action保存名称");
            }
        }
        public void SaveActionStateInfo(string _actionName, bool isCopy = false)
        {
            //正式序列化之前先清算一波
            ActionSaveFlishEvent.Run();
            if (isCopy)
            {
                CheckDuplicateName _checkDuplicateName = new CheckDuplicateName();
                foreach (var VARIABLE in mActionState)
                {
                    _checkDuplicateName.OnCheck(VARIABLE.Name);
                }

                if (_checkDuplicateName.IsDuplicate())
                {
                    EditorUtility.DisplayDialog("保存出错！！！",_checkDuplicateName.GetDuplicateName(),"我知道了");
                }
                else
                {
                    EditorActionStateInfo _saveData = new EditorActionStateInfo(mActionState,_actionName, mStartAction,mStartAction_Limb,mStartAction_Upper,mStartAction_Script, mHitAction,
                        mActionType, mActionLable);
                    _saveData.mTemplates = mTemplate;
                    // _saveData.mTemplateID = mTemplateID;
                    
                    //客户端保存序列化参数
                    if (ActionWindowMain.ActionEditorFuntion.SaveActionData(_saveData, _actionName))
                    {
                        ActionSaveFlishEvent.Run();
                        AssetDatabase.Refresh();
                        return;
                    }
                    
                    string _str = JsonUtility.ToJson(_saveData);
                    File.WriteAllText(MotionEngineConst.EditorActionSavePath(_actionName),_str);
                    RunTimeDataManager.SaveActionState(_saveData, _actionName);
                    EngineDebug.Log($"成功储存  [ <color=#FFF100>{_actionName}</color> ]  Action数据");
                }
            }
            else
            {
                EditorActionStateInfo _saveData = new EditorActionStateInfo(null,_actionName);
                // _saveData.mTemplate = mTemplate;
                
                //客户端保存序列化参数
                if (ActionWindowMain.ActionEditorFuntion.SaveActionData(_saveData, _actionName))
                {
                    ActionSaveFlishEvent.Run();
                    AssetDatabase.Refresh();
                    return;
                }
                
                string _str = JsonUtility.ToJson(_saveData);
                File.WriteAllText(MotionEngineConst.EditorActionSavePath(_actionName),_str);
                RunTimeDataManager.SaveActionState(_saveData, _actionName);
                EngineDebug.Log($"成功新建  [ <color=#00BBFF>{_actionName}</color> ]  Action数据 ");
            }
            ActionSaveFlishEvent.Run();
        }

        public EditorActionStateInfo CloneEditorActionStateInfo()
        {
            return new EditorActionStateInfo(mActionState,mSelectActionState, mStartAction,mStartAction_Limb,
                mStartAction_Upper,mStartAction_Script, mHitAction, mActionType, mActionLable);
        }
        #endregion

        #region 回调

        private void CreactNewActionMenu(Event _event)
        {
            GenericMenu _menu = new GenericMenu();

            mTemplateID = Mathf.Clamp(mTemplateID, 0, mTemplate.Count);
            string _deleteName = _event.control ? " (删除)" : "";
            
            string _name = mTemplateID == 0 ? "> 默认模板 <" : "默认模板";
            _menu.AddItem(new GUIContent(_name), false, () => { mTemplateID = 0; });

            for (int i = 0; i < mTemplate.Count; i++)
            {
                _name = (mTemplateID == (i + 1) ? $"> {mTemplate[i].Name} <" : mTemplate[i].Name) + _deleteName;
                int _selectID = i + 1;
                _menu.AddItem(new GUIContent(_name), false, () =>
                {
                    if (_event.control)
                    {
                        mTemplate.RemoveAt(_selectID - 1);
                    }
                    else
                    {
                        mTemplateID = _selectID;
                    }
                });
            }

            _menu.AddItem(new GUIContent("设置模板"), false, () =>
            {
                // if (ActionStateDic.TryGetValue(mOnSelectActionStateID, out EditorActionState _actionState))
                if(mOnSelectActionStateID>-1 && mOnSelectActionStateID<AllActionState.Count)
                {
                    EditorActionState _actionState = AllActionState[mOnSelectActionStateID];
                    mTemplate.Add(_actionState.Clone(_actionState.ID, _actionState.Name));
                    EditorUtility.DisplayDialog("通知", $"已设置 【{_actionState.Name}】 为模板", "我知道了");
                    ActionIsChange = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("警告", "请选择任意一Action再设置模板", "我知道了");
                }
            });
            if (!_event.control) _menu.AddDisabledItem(new GUIContent("ctrl+右键点击“新建^”可删除模板"));
            _menu.ShowAsContext();
        }

        #endregion
    }
}
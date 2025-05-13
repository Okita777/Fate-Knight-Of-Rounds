using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEngine;
using UnityEngine.Serialization;
using File = System.IO.File;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public class EditorUnitWarpInfo
    {
        public List<EditorUnitWarp> mUnitWarp;
        public EditorUnitWarpInfo(List<EditorUnitWarp> _unitWarps)
        {
            mUnitWarp = _unitWarps;
        }
    }
    
    [System.Serializable]
    public class EditorActionStateInfo
    {
        public List<EditorActionState> mActionState;
        // public int mTemplateID = 0;
        // public EditorActionState mTemplate = null;
        public List<EditorActionState> mTemplates = new List<EditorActionState>();
        public string mActionGroupName;
        public string mDefaultAction;
        public string mDefaultAction_Limb;
        public string mDefaultAction_Upper;
        public string mDefaultAction_Script;

        public string mHitAction;
        public List<string> mActionType;
        public List<string> mActionLable;

        public EditorActionStateInfo(List<EditorActionState> _actionStates, 
            string _mActionGroupName, string _mDefaultAction = "", string _mDefaultAction_Limb = "", 
            string _mDefaultAction_Upper = "", string _mDefaultAction_Script = "", string _mHitAction = "",
            List<string> _mActionType = null, List<string> _mActionLable = null)
        {
            mActionGroupName = _mActionGroupName;
            if (_actionStates == null)
            {
                mActionState = new List<EditorActionState>();
                mDefaultAction = string.Empty;
                mDefaultAction_Limb  = string.Empty;
                mDefaultAction_Upper  = string.Empty;
                mDefaultAction_Script  = string.Empty;
                mHitAction = string.Empty;
                List<string> _actionType = new List<string>();
                _actionType.Add("Idle");
                _actionType.Add("Move");
                _actionType.Add("Jump");
                _actionType.Add("UseItem");
                _actionType.Add("Interact");
                _actionType.Add("Attack");
                _actionType.Add("Hit");
                _actionType.Add("Defence");
                _actionType.Add("Dead");

                mActionType = _actionType;
                mActionLable = new List<string>();
            }
            else
            {
                // mActionState = _actionStates;
                mActionState = new List<EditorActionState>();
                foreach (EditorActionState _actionState in _actionStates)
                {
                    mActionState.Add(_actionState.Clone(_actionState.ID,_actionState.Name));
                }
                mDefaultAction = _mDefaultAction;
                mDefaultAction_Limb = _mDefaultAction_Limb;
                mDefaultAction_Upper = _mDefaultAction_Upper;
                mDefaultAction_Script = _mDefaultAction_Script;
                mHitAction = _mHitAction;
                mActionType = new List<string>();
                foreach (var VARIABLE in _mActionType)
                {
                    mActionType.Add(VARIABLE);
                }
                mActionLable = new List<string>();
                foreach (var VARIABLE in _mActionLable)
                {
                    mActionLable.Add(VARIABLE);
                }
                // mActionLable = _mActionLable;
            }
        }

        public EditorActionStateInfo Clone()
        {
            return new EditorActionStateInfo(mActionState, mActionGroupName, mDefaultAction, mDefaultAction_Limb,
                mDefaultAction_Upper, mDefaultAction_Script, mHitAction, mActionType, mActionLable);
        }
        
        public ActionStateInfo GetActionStateInfo()
        {
            List<ActionState> _actionStates = new List<ActionState>();
            if (this.mActionState != null)
            {
                foreach (var VARIABLE in this.mActionState)
                {
                    _actionStates.Add(VARIABLE.GetActionState());
                }
            }
            ActionStateInfo _info = new ActionStateInfo(_actionStates, mActionGroupName);
            _info.mDefaultAction = this.mDefaultAction;
            _info.mDefaultAction_Limb = this.mDefaultAction_Limb;
            _info.mDefaultAction_Upper = this.mDefaultAction_Upper;
            _info.mDefaultAction_Script = this.mDefaultAction_Script;

            _info.mHitAction = this.mHitAction;
            _info.mActionType = this.mActionType;
            _info.mActionLable = this.mActionLable;
            return _info;
        }
    }

    public class RunTimeDataManager
    {
        public static void SaveUnit(EditorUnitWarpInfo _editorUnitWarp, string _name)
        {
            string _savePath = MotionEngineRuntimePath.Instance.UnitPath(_name);

            string _str = JsonUtility.ToJson(UnitWarpInfo(_editorUnitWarp));
            File.WriteAllText(_savePath, _str);
        }

        public static void SaveActionState(EditorActionStateInfo _actionState, string _name)
        {
            string _savePath = MotionEngineRuntimePath.Instance.ActionPath(_name);

            string _str = JsonUtility.ToJson(_actionState.GetActionStateInfo());
            File.WriteAllText(_savePath, _str);
        }

        public static UnitWarpInfo UnitWarpInfo(EditorUnitWarpInfo _editorUnitWarp)
        {
            List<UnitWarp> _unitWarps = new List<UnitWarp>();
            if (_editorUnitWarp != null && _editorUnitWarp.mUnitWarp != null)
            {
                foreach (var _unitWarp in _editorUnitWarp.mUnitWarp)
                {
                    _unitWarps.Add(_unitWarp.GetUnitWarp());
                }
            }
            UnitWarpInfo _info = new UnitWarpInfo(_unitWarps);
            return _info;
        }

    }

}
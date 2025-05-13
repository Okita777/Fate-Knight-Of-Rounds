using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class UnitWarpInfo
    {
        public List<UnitWarp> mUnitWarp;
        public UnitWarpInfo(List<UnitWarp> _unitWarps)
        {
            mUnitWarp = _unitWarps;
        }
    }
    
    [System.Serializable]
    public class ActionStateInfo
    {
        public readonly string ActionGroupName = String.Empty;
        public List<ActionState> mActionState;
        public string mDefaultAction;
        public string mDefaultAction_Limb;
        public string mDefaultAction_Upper;
        public string mDefaultAction_Script;
        public string mHitAction;
        public List<string> mActionType;
        public List<string> mActionLable;
        public ActionStateInfo(List<ActionState> _actionStates, string _name)
        {
            mActionState = _actionStates;
            ActionGroupName = _name;
        }
    }

}
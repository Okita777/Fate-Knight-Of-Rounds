using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public class ActionEngineReName
    {
        public static void ReName(string _oldName,string _newName)
        {
            List<EditorActionState> _actionStates = ResourcesWindow.Instance.AllActionState;

            if (string.IsNullOrEmpty(_oldName))
            {
                EditorUtility.DisplayDialog("警告", "重命名失败，命名不能为空", "我知道了");
                return;
            }

            if (_oldName == _newName)
            {
                return;
            }
            
            //检查新命名是否合规
            foreach (var _actionState in _actionStates)
            {
                if (_actionState.Name == _newName)
                {
                    EditorUtility.DisplayDialog("警告", "重命名失败，命名有重复", "我知道了");
                    return;
                }
            }
            
            ResourcesWindow.Instance.GetEditorActionStateToSelect().Name = _newName;

            //开始替换命名
            foreach (var _actionState in _actionStates)
            {
                // _actionState.Name = SetName(_actionState.Name, _oldName, _newName);
                _actionState.DefaultAction = SetName(_actionState.DefaultAction, _oldName, _newName);

                List<ActionTrackGroup> _AllEventTrackGroup = _actionState.AllEventTrackGroup;
                if (_AllEventTrackGroup == null)
                {
                    return;
                }
                foreach (var _trackGroup in _AllEventTrackGroup)
                {
                    foreach (var _actionTrack in _trackGroup.CurActiontTrack)
                    {
                        if (_actionTrack is EventTrack _eventTrack)
                        {
                            foreach (var _interrup in _eventTrack.CurInterrup)
                            {
                                if (_interrup.ActionInterrupt.ActionName == _oldName)
                                {
                                    //轨道的跳转对象
                                    _interrup.ActionInterrupt.ActionName = _newName;
                                }
                            } //轨道下的所有打断事件
                        }//事件轨
                        else if (_actionTrack is InterrupGroupTrack _interrupGroup)
                        {
                            for (int i = 0; i < _interrupGroup.InterrupLock.Count; i++)
                            {
                                if (_interrupGroup.InterrupLock[i] == _oldName)
                                {
                                    _interrupGroup.InterrupLock[i] = _newName;
                                }
                            }
                            if (_interrupGroup.InterrupOffset.ContainsKey(_oldName))
                            {
                                _interrupGroup.InterrupOffset.Add(_newName, _interrupGroup.InterrupOffset[_oldName]);
                                _interrupGroup.InterrupOffset.Remove(_oldName);
                            }
                        }//打断组轨
                    }//轨道组下的所有轨道
                }
            }

        }
        
        public static void ReID(int _oldID,int _newID)
        {
            List<EditorActionState> _actionStates = ResourcesWindow.Instance.AllActionState;
            
            if (_oldID == _newID)
            {
                return;
            }
            
            //先检查新命名是否合规
            foreach (var _actionState in _actionStates)
            {
                if (_actionState.ID == _newID)
                {
                    EditorUtility.DisplayDialog("警告", "重命名失败，ID序号有重复", "我知道了");
                    return;
                }
            }
            
            ResourcesWindow.Instance.GetEditorActionStateToSelect().ID = _newID;

            //开始替换命名
            foreach (var _actionState in _actionStates)
            {
                List<ActionTrackGroup> _AllEventTrackGroup = _actionState.AllEventTrackGroup;
                if (_AllEventTrackGroup == null)
                {
                    return;
                }
                foreach (var _trackGroup in _AllEventTrackGroup)
                {
                    foreach (var _actionTrack in _trackGroup.CurActiontTrack)
                    { 
                        if (_actionTrack is InterrupGroupTrack _interrupGroupTrack)
                        {
                            if (_interrupGroupTrack.ActionStateID == _oldID)
                            {
                                _interrupGroupTrack.ActionStateID = _newID;
                            }
                        }//更替打断组ID
                    }//轨道组下的所有轨道
                }
            }
        }

        public static bool ReActionTypeName(string _oldName, string _newName)
        {
            if (string.IsNullOrEmpty(_oldName))
            {
                EditorUtility.DisplayDialog("警告", "重命名失败，命名不能为空", "我知道了");
                return false;
            }

            if (ResourcesWindow.Instance.ActionType.Contains(_newName))
            {
                EditorUtility.DisplayDialog("警告", "重命名失败，命名有重复", "我知道了");
                return false;
            }
            
            List<EditorActionState> _actionStates = ResourcesWindow.Instance.AllActionState;
            if (_actionStates == null)
            {
                EditorUtility.DisplayDialog("警告", "重命名失败，请点击预览以加载Action列表", "我知道了");
                return false;
            }
            foreach (var _actionState in _actionStates)
            {
                if (_actionState.EditorActionType == _oldName)
                {
                    _actionState.EditorActionType = _newName;
                }
            }

            return true;
        }

        #region LocalFunction
        private static string SetName(string _nowName, string _oldName, string _newName)
        {
            if (_nowName == _oldName)
            {
                _nowName = _newName;
            }
            return _nowName;
        }
        
        private static int SetID(int _nowName, int _oldName, int _newName)
        {
            if (_nowName == _oldName)
            {
                _nowName = _newName;
            }
            return _nowName;
        }
        #endregion

    }
}
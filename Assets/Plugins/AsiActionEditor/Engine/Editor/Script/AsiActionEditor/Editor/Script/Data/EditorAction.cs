using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public class EditorActionState : ActionState
    {
        //单独储存于Editor的层级数据  用以还原
        public int EditorAnimaLayer = 0;
        public int EditorAnimaID = 0;
        public string EditorActionType = "Idle";

        public EditorActionEvent EditorAnimEvent;
        public List<ActionTrackGroup> AllEventTrackGroup = null;
        public EditorActionInterrupt EditorActionInterrupt;

        //todo 避免频繁实例化而做的临时修改
        private ActionState _actionState = new ActionState();
        private List<ActionEvent> _actionEvents = new List<ActionEvent>();
        private List<ActionInterrupt> _actionInterrupts_OnHit = new List<ActionInterrupt>();
        private List<ActionInterrupt> _actionInterrupts_BeHit = new List<ActionInterrupt>();
        private List<ActionInterrupt> _actionInterrupts = new List<ActionInterrupt>();
        private List<ActionInterruptGroup> _actionInterruptGroups = new List<ActionInterruptGroup>();
        public EditorActionState(int _id, string _name)
        {
            ID = _id;
            Name = _name;

            TotalTime = 2 * RunTime.MotionEngineConst.TimeDoubling;

            EditorActionInterrupt = new EditorActionInterrupt();
        }

        public EditorActionState Clone(int _id, string _name = "")
        {
            EditorActionState _curActionState =
                new EditorActionState(_id, string.IsNullOrEmpty(_name) ? (Name + "_copy") : _name);
            
            //复制总时长和动画事件轨道
            _curActionState.TotalTime = mTotalTime;
            if (EditorAnimEvent != null && EditorAnimEvent.EventData != null)
            {
                _curActionState.EditorAnimEvent = EditorAnimEvent.Clone(); 
            }
            else
            {
                _curActionState.EditorAnimEvent = null;
            }

            //复制动画参数和各个轨道
            _curActionState.EditorActionType = EditorActionType;
            _curActionState.AllEventTrackGroup = new List<ActionTrackGroup>();
            if (AllEventTrackGroup is not null)
            {
                foreach (var _actionTrack in AllEventTrackGroup)
                {
                    _curActionState.AllEventTrackGroup.Add(_actionTrack.Clone());
                }
            }
            _curActionState.EditorActionInterrupt = new EditorActionInterrupt();
            EditorActionInterrupt.CopyCondition(_curActionState.EditorActionInterrupt);
            _curActionState.TotalTime = mTotalTime;
            
            //复制基础参数
            _curActionState.DefaultAction = mDefaultAction;
            _curActionState.OffsetTime = mOffsetTime;
            _curActionState.MixTime = mMixTime;
            _curActionState.AnimaLayer = mAnimaLayer;
            _curActionState.EditorAnimaLayer = EditorAnimaLayer;
            
            return _curActionState;
        }

        public ActionState GetActionState()
        {
            if (_actionState == null) _actionState = new ActionState();
            if (_actionEvents == null) _actionEvents = new List<ActionEvent>();
            if (_actionInterrupts == null) _actionInterrupts = new List<ActionInterrupt>();
            if (_actionInterrupts_OnHit == null) _actionInterrupts_OnHit = new List<ActionInterrupt>();
            if (_actionInterrupts_BeHit == null) _actionInterrupts_BeHit = new List<ActionInterrupt>();
            if (_actionInterruptGroups == null) _actionInterruptGroups = new List<ActionInterruptGroup>();

            
            _actionState.ID = ID;
            _actionState.Name = Name;
            _actionState.AnimaLayer = AnimaLayer;
            _actionState.TotalTime = TotalTime;
            _actionState.MixTime = MixTime;
            _actionState.OffsetTime = OffsetTime;
            _actionState.ActionLable = ResourcesWindow.Instance.GetIDToActionType(EditorActionType);
            if (DefaultAction == "Null") _actionState.DefaultAction = string.Empty;
            else _actionState.DefaultAction = DefaultAction;

            if (AnimaLayer < 4)
            {
                if (EditorAnimEvent is not null)
                    _actionState.AnimEvent = EditorAnimEvent.GetRunTimeData();
                else
                    _actionState.AnimEvent = null;
            }
            else
            {
                _actionState.AnimEvent = null;
                // EditorAnimEvent = null; 
            }

            _actionEvents.Clear();
            _actionInterrupts.Clear();
            _actionInterrupts_OnHit.Clear();
            _actionInterrupts_BeHit.Clear();
            _actionInterruptGroups.Clear();

            if (AllEventTrackGroup == null)
            {
                _actionState.EventList = _actionEvents;
                _actionState.InterruptList = _actionInterrupts;
                _actionState.InterruptGroupList = _actionInterruptGroups;
                return _actionState;
            }//如果轨道组是空的话  则直接给个空值回去
            foreach (var _trackGroup in AllEventTrackGroup)
            {
                foreach (var _actionTrack in _trackGroup.CurActiontTrack)
                {
                    if (_actionTrack is EventTrack _eventTrack)
                    {
                        foreach (var _event in _eventTrack.CurEventDisplay)
                        {
                            //todo: 有朝一日检查一下序列化文件  这个是解决的， 检查是否有序列化Editor的
                            _actionEvents.Add(_event.MainEvent.GetRunTimeData());
                        } //轨道下的所有事件

                        foreach (var _interrup in _eventTrack.CurInterrup)
                        {
                            bool _isDefaultInterrup = true;
                            foreach (var _interrupt in _interrup.ActionInterrupt.InterruptConditionList)
                            {
                                if (ActionWindowMain.GetConditionType(_interrupt.InterruptType, out var _type))
                                {
                                    if (_type == EInterruptTypeInternal.EIT_CheckBeHit)
                                    {
                                        // EngineDebug.Log($"收集到受击轨道: {_interrup.ActionInterrupt.ActionName}");
                                        _actionInterrupts_BeHit.Add(_interrup.ActionInterrupt);
                                        _isDefaultInterrup = false;
                                        break;
                                    }
                                    else if (_type == EInterruptTypeInternal.EIT_CheckOnHit)
                                    {
                                        // EngineDebug.Log($"收集到命中轨道: {_interrup.ActionInterrupt.ActionName}");
                                        _actionInterrupts_OnHit.Add(_interrup.ActionInterrupt);
                                        _isDefaultInterrup = false;
                                        break;
                                    }
                                }
                            }//查找所有的条件

                            if (_isDefaultInterrup)
                            {
                                _actionInterrupts.Add(_interrup.ActionInterrupt);
                            }
                        } //轨道下的所有打断事件
                    }
                    else if (_actionTrack is InterrupGroupTrack _interrupGroupTrack)
                    {
                        List<IActionTrack> _actionTracks = GetActiontTrack(_interrupGroupTrack.GetAction());
                        _actionInterruptGroups.Add(new ActionInterruptGroup(
                            _interrupGroupTrack.ActionStateID,
                            _interrupGroupTrack.ActionOffset,
                            GetInterrupHide(_actionTracks, _interrupGroupTrack),
                            GetInterrupOffset(_actionTracks, _interrupGroupTrack)
                        ));
                    }
                }//轨道组下的所有轨道
            }//所有轨道组

            _actionState.EventList = _actionEvents;
            _actionState.InterruptList = _actionInterrupts;
            _actionState.InterruptList_OnHit = _actionInterrupts_OnHit;
            _actionState.InterruptList_BeHit = _actionInterrupts_BeHit;
            _actionState.InterruptGroupList = _actionInterruptGroups;
            
            return _actionState;
        }

        private List<bool> GetInterrupHide(List<IActionTrack> _actionTracks, InterrupGroupTrack _group)
        {
            List<bool> _value = new List<bool>();
            
            foreach (var _actionTrack in _actionTracks)
            {
                if (_actionTrack is EventTrack _eventTrack)
                {
                    foreach (var _interrup in _eventTrack.CurInterrup)
                    {
                        if (_group.InterrupLock.Contains(_interrup.ActionInterrupt.ActionName))
                        {
                            _value.Add(true);
                        }
                        else
                        {
                            _value.Add(false);
                        }
                    } //轨道下的所有打断事件
                }
            }
            
            return _value;
        }

        private List<int> GetInterrupOffset(List<IActionTrack>  _actionTracks, InterrupGroupTrack _group)
        {
            List<int> _value = new List<int>();
            
            foreach (var _actionTrack in _actionTracks)
            {
                if (_actionTrack is EventTrack _eventTrack)
                {
                    foreach (var _interrup in _eventTrack.CurInterrup)
                    {
                        if (_group.InterrupOffset.ContainsKey(_interrup.ActionInterrupt.ActionName))
                        {
                            _value.Add(_group.InterrupOffset[_interrup.ActionInterrupt.ActionName]);
                        }
                        else
                        {
                            _value.Add(0);
                        }
                    } //轨道下的所有打断事件
                }
            }
            
            return _value;
        }

        private List<IActionTrack> GetActiontTrack(EditorActionState _actionState)
        {
            List<IActionTrack> _actionTracks = new List<IActionTrack>();

            if (_actionState != null && _actionState.AllEventTrackGroup != null)
            {
                foreach (var _trackGroup in _actionState.AllEventTrackGroup)
                {
                    foreach (var _actionTrack in _trackGroup.CurActiontTrack)
                    {
                        _actionTracks.Add(_actionTrack);
                    }
                }
            }
            
            return _actionTracks;
        }
        
        
    }
}
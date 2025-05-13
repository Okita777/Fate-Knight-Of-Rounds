using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public class EditorActionEvent : ActionEvent
    {
        public int EventTrackType = 0;//0默认轨道， 1首尾有虚线
        // public List<Vector2> BluePrintPos = new List<Vector2>();
        public NodeEdiDataDic NodeEdiDataDic = new NodeEdiDataDic();
        public EditorActionEvent(IActionEventData _eventData, int _Duration = RunTime.MotionEngineConst.TimeDoubling, 
            int _EventTrackType = 0)
        {
            EventData = _eventData;
            Duration = _Duration;
            EventTrackType = _EventTrackType;
        }

        public EditorActionEvent Clone()
        {
            // EditorActionEvent _actionEvent = new EditorActionEvent(EventData.Clone(null), Duration, EventTrackType);
            EditorActionEvent _actionEvent = null;
            if (EventData == null)
            {
                _actionEvent = new EditorActionEvent(null, Duration, EventTrackType);
            }
            else
            {
                // _actionEvent = ActionEventCreact.Creact(EventData.GetEvenType());
                // _actionEvent.Duration = Duration;
                // _actionEvent.EventTrackType = EventTrackType; 

                _actionEvent = new EditorActionEvent(EventData.Clone(EventData.Creact()), Duration, EventTrackType);
            }

            _actionEvent.TriggerTime = mTriggerTime;
            _actionEvent.EditorInheritable = mEditorInheritable;
            _actionEvent.Inheritable = mInheritable;
            _actionEvent.EventTrackType = EventTrackType;
            _actionEvent.NodeEdiDataDic = NodeEdiDataDic;
            return _actionEvent;
        }

        public ActionEvent GetRunTimeData()
        {
            ActionEvent _ActionEvent = new ActionEvent();
            // ActionEvent _ActionEvent = this;
            _ActionEvent.TriggerTime = mTriggerTime;
            _ActionEvent.Duration = mDuration;
            _ActionEvent.ForceInit = mForceInit;
            _ActionEvent.Inheritable = mInheritable;
            _ActionEvent.EditorInheritable = mEditorInheritable;

            if (EventData is not null)
                _ActionEvent.EventData = EventData.Clone(EventData.Creact()); //复制_actionEventData下的参数
            else 
                _ActionEvent.EventData = null;

            return _ActionEvent;
        }

    }
}
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class InspectorWindow
    {
        public void SelectProperty(IProperty _property, bool _isMarkUndo = true)
        {
            bool _isSelectLast = mSelectProperty == _property;
            mSelectProperty = _property;
            needInit = true;
            Repaint();
            if (!_isSelectLast)
            {
                TimeLineWindow.Instance.UpdateTimeToNow();
                // ScenceDraw.Instance.OnUpdateDraw();
                if(_isMarkUndo)AsiActionEngineEditorUpdate.ChangeSelectActionEvent(GetEventSelectID());
            }
        }

        public AsiActionEventSelectID GetEventSelectID()
        {
            int _type = -1;
            int _id = ResourcesWindow.Instance.mOnSelectActionStateID;
            int _trackGroupID = 0;
            int _trackID = 0;
            int _eventID = 0;
            IProperty _selectProperty = InspectorWindow.Instance.CurSelectProperty;
            if (_selectProperty is null)
                return new AsiActionEventSelectID(-1, 0, 0, 0, 0);

            if (_selectProperty is EditorUnitWarp _unitWarp)
            {
                _type = 1;
            }
            else if (_selectProperty is ItemWarp _weaponWarp)
            {
                _type = 2;
            }
            else if (_selectProperty is EditorActionState _editorActionState)
            {
                _type = 3;
            }
            else if (_selectProperty is EditorActionEvent _editorActionEvent)
            {
                EditorActionState _actionState = ResourcesWindow.Instance.GetEditorActionStateToSelect();
                foreach (ActionTrackGroup _actionTrackGroup in _actionState.AllEventTrackGroup)
                {
                    _trackID = 0;
                    foreach (IActionTrack _actionTrack in _actionTrackGroup.CurActiontTrack)
                    {
                        if (_actionTrack is EventTrack _eventTrack)
                        {
                            _eventID = 0;
                            foreach (EventDisplay _eventDisplay in _eventTrack.CurEventDisplay)
                            {
                                if (_eventDisplay.MainEvent == _editorActionEvent)
                                {
                                    return new AsiActionEventSelectID(4, _id, _trackGroupID, _trackID, _eventID);
                                }

                                _eventID++;
                            }
                        }
                        _trackID++;
                    }
                    _trackGroupID++;
                }
            }
            else if (_selectProperty is EditorActionInterrupt _editorActionInterrupt)
            {
                EditorActionState _actionState = ResourcesWindow.Instance.GetEditorActionStateToSelect();
                foreach (ActionTrackGroup _actionTrackGroup in _actionState.AllEventTrackGroup)
                {
                    _trackID = 0;
                    foreach (IActionTrack _actionTrack in _actionTrackGroup.CurActiontTrack)
                    {
                        if (_actionTrack is EventTrack _eventTrack)
                        {
                            _eventID = 0;
                            foreach (ActionInterrupDisplay _interrupDisplay in _eventTrack.CurInterrup)
                            {
                                if (_interrupDisplay.ActionInterrupt == _editorActionInterrupt)
                                {
                                    return new AsiActionEventSelectID(5, _id, _trackGroupID, _trackID, _eventID);
                                }

                                _eventID++;
                            }
                        }

                        _trackID++;
                    }

                    _trackGroupID++;
                }
            }
            else if (_selectProperty is EditorCameraWarp _cameraWarp)
            {
                _type = 6;
            }
            else if (_selectProperty is EditorEngineGValuePart _gValue)
            {
                _type = 7;
            }
            
            return new AsiActionEventSelectID(_type, _id, _trackGroupID, _trackID, _eventID);
        }
    }
}
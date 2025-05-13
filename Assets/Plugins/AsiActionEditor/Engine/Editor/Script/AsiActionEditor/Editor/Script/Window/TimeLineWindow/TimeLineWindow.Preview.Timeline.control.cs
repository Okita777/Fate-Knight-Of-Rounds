using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.DrawData;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class TimeLineWindow
    {
        public int mLastTimeLineTime = 0;
        
        private MouseActionInfo mTimeLineControlCallBack_dwon;
        private MouseActionInfo mTimeLineControlCallBack_move;
        private MouseActionInfo mTimeLineControlCallBack_up;
        
        private bool mTimeControlIsDraw = false;
        private bool mIsPlaying = false;
        private bool mChangeKeep = true;

        private int mTimeLineStartTime;
        private int mNowTime = 0;
                
        private float mUpdateTotalTime;
        private float mLastTime = 0f;
        
        public int NowTime
        {
            get { return mNowTime; }
        }

        public void InitTimeLineControl()
        {
            mTimeLineControlCallBack_dwon = new MouseActionInfo(OnLoadTimeControlEvent, MotionEngineConst.Priority_TimeControl);
            mTimeLineControlCallBack_move = new MouseActionInfo(OnMoveTimeControlHandle, MotionEngineConst.Priority_TimeControl);
            mTimeLineControlCallBack_up = new MouseActionInfo(UnLoadTimeControlEvent, MotionEngineConst.Priority_TimeControl);
            
            // mNowTime = 0;
            if (!mChangeKeep)
            {
                mPreviweState = EPreviweState.Pause;
            }
            mTimeLineStartTime = 0;
            mUpdateTotalTime = 0;
            UpdateTime(0);
        }
        
        private void DrawTimeLineControl(Rect _rect)
        {
            float _timeToPos = GetPosToTime(mNowTime);
            mTimeControlIsDraw = _timeToPos >= TimelineRect.x;
            if (mTimeControlIsDraw) mTimeControlIsDraw = _timeToPos <= TimelineRect.x + TimelineRect.width;
            if (mTimeControlIsDraw)
            {
                OnLoadMouseAction(EMouseEvent.MouseDwonLeft,mTimeLineControlCallBack_dwon);
                Rect _timeControlRect = new Rect(_rect);
                _timeControlRect.y += 3f;
                _timeControlRect.height -= 3f;
                _timeControlRect.width = MotionEngineConst.GUI_TimeLineControlWidth;
                _timeControlRect.x = _timeToPos - MotionEngineConst.GUI_TimeLineControlWidth * 0.5f;
                EditorGUI.DrawRect(_timeControlRect, EngineSetting.colorTimeLineControl);

                Vector2 _starPos = new Vector2(_timeToPos, _timeControlRect.y + _timeControlRect.height);
                Vector2 _endPos = new Vector2(_timeToPos, TimelineRect.height + TimelineRect.y);
                Handles.color = EngineSetting.colorTimeLineControl;
                Handles.DrawLine(_starPos,_endPos);
            }

            UpdateEditorTimeLine();
            UpdateBakeAttackBox();//烘焙攻击盒时
        }

        private void UpdateEditorTimeLine()
        {
            if(mIsPlayGame && !mIsEditor)return;

            bool isPlaying = mPreviweState == EPreviweState.Play;
            if (mIsPlaying != isPlaying)
            {
                //初始化
                if (isPlaying)
                {
                    mLastTime = Time.realtimeSinceStartup;
                    if (mNowTime >= mActionTotalTime)
                    {
                        mTimeLineStartTime = 0;
                    }
                    else
                    {
                        mTimeLineStartTime = mNowTime;
                    }
                    mUpdateTotalTime = 0;
                }
                mIsPlaying = isPlaying;
            }

            //每帧更新 TimeLine 状态 
            if (isPlaying)
            {
                float _editorDeltaTime = Time.realtimeSinceStartup - mLastTime;
                mUpdateTotalTime += _editorDeltaTime * mPlaySpeed * RunTime.MotionEngineConst.TimeDoubling;
                int _updateNowTime = mTimeLineStartTime + Mathf.RoundToInt(mUpdateTotalTime);

                //播放到末尾了
                if (_updateNowTime > mActionTotalTime)
                {
                    if (mIsLoop)
                    {
                        mTimeLineStartTime -= mActionTotalTime;
                    }
                    else
                    {
                        mPreviweState = EPreviweState.Pause;
                        _updateNowTime = mActionTotalTime;
                    }
                }
                
                UpdateTime(_updateNowTime);
                
                mLastTime = Time.realtimeSinceStartup;
            }
        }
        
        private void UpdateTime(int _time)
        {
            // if (!AsiEditorUpdate_Donece.Instance.DrawTimeLine)
            // {
            //     AsiEditorUpdate_Donece.Instance.DrawTimeLine = true;
            // }
            // else
            // {
            //     return;
            // }
            
            if (needInit)
            {
                return;
            }
            
            mNowTime = _time;
            mTimeViewer.SetTime(_time);

            if (mIsPlayGame)
            {
                //是否正在运行中编辑
                if (mIsEditor)
                {
                    // if (ActionWindowMain.ScenceDraw_Runtime)
                    // {
                    //     //绘制场景图形
                    //     OnScenseDraw(_time);
                    // }
                }
                else
                {
                    return;
                }
            }
            else
            {
                // if (ActionWindowMain.ScenceDraw_Editor)
                // {
                //     //绘制场景图形
                //     OnScenseDraw(_time);
                // } 
            }


            
            // if(mIsPlayGame && !mIsEditor)return;
            GUI.changed = true;
            //动画事件轨
            if (!(SelectActionState.EditorAnimEvent == null || SelectActionState.EditorAnimEvent.EventData == null))
            {
                EventUpdate.Instance.EditorEventUpdate(_time, SelectActionState.EditorAnimEvent);
            }

            if (SelectActionState.AllEventTrackGroup is not null)
            {
                EventUpdate.Instance.Init();
                foreach (var _trackGroup in SelectActionState.AllEventTrackGroup)
                {
                    foreach (var _actionTrack in _trackGroup.CurActiontTrack)
                    {
                        if (_actionTrack is EventTrack _eventTrack)
                        {
                            //所有事件轨
                            if (_eventTrack.IsPreview)
                            {
                                foreach (EventDisplay _eventDisplay in _eventTrack.CurEventDisplay)
                                {
                                    EventUpdate.Instance.EditorEventUpdate(_time, _eventDisplay.MainEvent);
                                }
                            }
                        }
                    }
                }
            }

            mLastTimeLineTime = _time;
        }

        public void OnScenseDraw(int _time)
        {
            ScenceDraw.Instance.DrawBoxInit();

            if ((ActionWindowMain.ScenceDraw_Interrupt || ActionWindowMain.ScenceDraw_Event))
            {
                if (ActionWindowMain.ScenceDraw_Interrupt && InspectorWindow.Instance.CurSelectProperty is EditorActionInterrupt _interrupt)
                {
                    //选择跳转轨时只绘制它
                    if (ResourcesWindow.Instance.TryGetCharacterConfig(out CharacterConfig config))
                    {
                        if (_interrupt.InterruptConditionList is not null)
                        {
                            foreach (IInterruptCondition VARIABLE in _interrupt.InterruptConditionList)
                            {
                                VARIABLE.EditorDraw(config, new ActionMachineTime(0, _time, _interrupt.TriggerTime, _interrupt.Duration));
                            }
                        }
                    }
                }
                else if (ActionWindowMain.ScenceDraw_Event &&InspectorWindow.Instance.CurSelectProperty is EditorActionEvent _actionEvent)
                {
                    //选择事件轨时只绘制它
                    if (ResourcesWindow.Instance.TryGetCharacterConfig(out CharacterConfig config))
                    {
                        _actionEvent.EventData.EditorDraw(config, ResourcesWindow.Instance.ActionStatePart, 
                            new ActionMachineTime(0, _time, _actionEvent.TriggerTime, _actionEvent.Duration));
                    }
                    // VARIABLE.EditorDraw(config, new ActionMachineTime(0, 1, 0, -1));
                }
                else if (SelectActionState.AllEventTrackGroup is not null)
                {
                    EventUpdate.Instance.Init();
                    foreach (var _trackGroup in SelectActionState.AllEventTrackGroup)
                    {
                        foreach (var _actionTrack in _trackGroup.CurActiontTrack)
                        {
                            if (_actionTrack is EventTrack _eventTrack)
                            {
                                //所有事件轨
                                if (_eventTrack.IsPreview)
                                {
                                    foreach (EventDisplay _eventDisplay in _eventTrack.CurEventDisplay)
                                    {
                                        EventUpdate.Instance.EditorEventDraw(_time, _eventDisplay.MainEvent);
                                    }

                                    foreach (ActionInterrupDisplay _interrupDisplay in _eventTrack.CurInterrup)
                                    {
                                        EventUpdate.Instance.EditorConditionDraw(_time,
                                            _interrupDisplay.ActionInterrupt);
                                    }
                                }
                            }
                            else if (ActionWindowMain.ScenceDraw_Interrupt && _actionTrack is InterrupGroupTrack _interrupGroupTrack)
                            {
                                if (_interrupGroupTrack.mAllInterrup is not null)
                                {
                                    foreach (InterrupGroupTrackDisplay _groupTrackDisplay in _interrupGroupTrack
                                                 .mAllInterrup)
                                    {
                                        if (!_groupTrackDisplay.Change_hide)
                                        {
                                            int _oT = _interrupGroupTrack.ActionOffset + _groupTrackDisplay.OffsetTime;
                                            EventUpdate.Instance.EditorConditionDraw(_time - _oT,
                                                _groupTrackDisplay.ActionInterrupt);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void UpdateTimeToNow()
        {
            UpdateTime(mNowTime);
        }
        
        //回调
        private bool OnLoadTimeControlEvent(Event _event)
        {
            if (mHeadrInteractRect.Contains(_event.mousePosition))
            {
                mPreviweState = EPreviweState.Pause;
                UpdateTime(GetTimeToPos(_event.mousePosition.x));
                OnLoadMouseAction(EMouseEvent.MouseDrag, mTimeLineControlCallBack_move);
                OnLoadMouseAction(EMouseEvent.MouseUp, mTimeLineControlCallBack_up);
                return true;
            }
            return false;
        }

        private bool OnMoveTimeControlHandle(Event _event)
        {
            int _mNowTime = GetTimeToPos(_event.mousePosition.x);
            _mNowTime = Mathf.Clamp(_mNowTime, 0, mActionTotalTime);
            UpdateTime(_mNowTime);
            // mNowTime = GetTimeToPos(_event.mousePosition.x);
            // mNowTime = Mathf.Clamp(mNowTime, 0, mActionTotalTime);
            // mTimeViewer.SetTime(mNowTime);
            return false;
        }

        private bool UnLoadTimeControlEvent(Event _event)
        {
            UnLoadMouseAction(EMouseEvent.MouseDwonLeft,mTimeLineControlCallBack_dwon);
            UnLoadMouseAction(EMouseEvent.MouseDrag, mTimeLineControlCallBack_move);
            UnLoadMouseAction(EMouseEvent.MouseUp, mTimeLineControlCallBack_up);
            return false;
        }
    }
}
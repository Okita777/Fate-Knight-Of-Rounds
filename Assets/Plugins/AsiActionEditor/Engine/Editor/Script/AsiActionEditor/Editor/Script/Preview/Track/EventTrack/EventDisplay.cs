using System;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public class EventDisplay
    {
        [SerializeField] private EditorActionEvent mEvent;//
        
        [NonSerialized] private EventTrack mEventTrack;//[NonSerialized]ASD
        [NonSerialized] private TimeLineWindow mWindow;
        private Rect mEventTrackRect;
        private Rect meventRect;
        private MouseActionInfo mMouseActionInfo_l;
        private MouseActionInfo mMouseActionInfo_r;
        private MouseActionInfo mMouseActionInfo_Move;
        private MouseActionInfo mMouseActionInfo_MoveEnd;
        private EventManipulator mManipulator_l;
        private EventManipulator mManipulator_r;
        private bool mDrawHeadManiPulator = false;
        private bool mDrawFootManiPulator = false;
        private bool mIsDraw = false;
        private float mStartPos = 0f;
        private float mEndPos = 0f;
        private Action<EventDisplay> mDeleteCallback = null;

        public EditorActionEvent MainEvent => mEvent;
        public Rect EventTrackRect  => mEventTrackRect ;
        
        public bool IsDraw
        {
            get
            {
                return mIsDraw;
            }
            set
            {
                if (mIsDraw != value)
                {
                    mIsDraw = value;
                    if (mIsDraw)
                    {
                        OnLoadEvent();
                    }
                    else
                    {
                        UnLoadEvent();
                        mManipulator_l.IsDraw = false;
                        mManipulator_r.IsDraw = false;
                    }
                }
            }
        }


        public EventDisplay(EditorActionEvent _event, EventTrack _track = null, Action<EventDisplay> _deleteCallback = null)
        {
            mWindow = TimeLineWindow.Instance;
            mEvent = _event;
            mEventTrack = _track;
            mDeleteCallback = _deleteCallback;

            mMouseActionInfo_r = new MouseActionInfo(OnMenu, MotionEngineConst.Priority_EventUnit);
            mMouseActionInfo_l = new MouseActionInfo(OnConEvent, MotionEngineConst.Priority_EventUnit);
            mMouseActionInfo_Move = new MouseActionInfo(OnMoveEvent, MotionEngineConst.Priority_EventUnit);
            mMouseActionInfo_MoveEnd = new MouseActionInfo(OnMoveEnd, MotionEngineConst.Priority_EventUnit);
            mManipulator_l = new EventManipulator(SetEventRectHead, SetEventRectInit);
            mManipulator_r = new EventManipulator(SetEventRectFoot);
        }

        public void Draw(Rect _rect)
        {
            mEventTrackRect = _rect;
            if(mEvent == null)return;
            
            mStartPos = mWindow.GetPosToTime(mEvent.TriggerTime);
            mEndPos = mStartPos + MotionEngineConst.GUI_EventTrackMinWidth;
            if (mEvent.Duration != 0)
            {
                //在真实末端长度大于最小长度时才应用真实长度
                float _realEndPos = mWindow.GetPosToTime(mEvent.TriggerTime + mEvent.Duration);
                if(_realEndPos - mStartPos > MotionEngineConst.GUI_EventTrackMinWidth)
                {
                    mEndPos = _realEndPos;
                }
            }


            bool _isDrawHead = mStartPos < _rect.x + _rect.width;
            bool _isDrawFoot = mEvent.Duration < 0 || (mEndPos > _rect.x);

            IsDraw = (_isDrawHead && _isDrawFoot);

            if (IsDraw)
            {
                bool _drawHeadManiPulator = mStartPos > _rect.x;
                bool _drawFootManiPulator = false;
                
                mDrawHeadManiPulator = _drawHeadManiPulator && (mEvent.Duration > 0);
                mDrawFootManiPulator = _drawFootManiPulator;

                //绘制主身
                meventRect = new Rect(_rect);

                if (_drawHeadManiPulator)
                {
                    meventRect.x = mStartPos;
                }

                float _timeRangeEnd = _rect.x + _rect.width;
                float _eventWidth = _timeRangeEnd - _rect.x;
                if (mEndPos < _timeRangeEnd && mEvent.Duration >= 0)
                {
                    _eventWidth = mEndPos - meventRect.x;
                    _drawFootManiPulator = true;
                    mDrawFootManiPulator = mEvent.Duration > 0;//单帧不能拥有控制柄
                }//绘制页脚
                else
                {
                    if (!_drawHeadManiPulator)
                    {//没有绘制页头的话  就绘制最长轨道
                        _eventWidth = _rect.width;
                    }
                    else
                    {
                        _eventWidth = _rect.width + _rect.x - mStartPos;
                    }
                }

                meventRect.width = _eventWidth;

                Color _eventTrackColor = mEventTrack == null
                    ? mWindow.EngineSetting.colorEventTrackNull
                    : mEventTrack.MainColor;
                Color _color = mWindow.IsSelection(mEvent) ? mWindow.EngineSetting.colorSelection : _eventTrackColor;
                EditorGUI.DrawRect(meventRect, _color);

                
                //绘制标题
                if (_eventWidth > MotionEngineConst.GUI_EventRenderTitleMinWidth)
                {
                    DrawTitle(ActionEventDescripted.Instance.GetTitle(MainEvent));
                    
                    //绘制详情信息
                    string _detailed = ActionEventDescripted.Instance.GetDetailed(MainEvent);
                    if (_drawFootManiPulator && !string.IsNullOrEmpty(_detailed))
                    {
                        DrawDetailed(_detailed);
                    }
                }
                else
                {
                    //绘制详情信息
                    string _detailed = ActionEventDescripted.Instance.GetDetailed(MainEvent);
                    if (_drawFootManiPulator)
                    {
                        if (string.IsNullOrEmpty(_detailed))
                        {
                            DrawDetailed(ActionEventDescripted.Instance.GetTitle(MainEvent));
                        }
                        else
                        {
                            DrawDetailed(_detailed);
                        }
                    }
                }

                //绘制辨识符号  单帧
                if (mEvent.Duration == 0)
                {
                    float _symbolHeight = 5f;
                    Rect _symbol = new Rect(meventRect);
                    _symbol.y += _symbol.height - _symbolHeight;
                    _symbol.height = _symbolHeight;
                    EditorGUI.DrawRect(_symbol, Color.black * 0.5f);
                }
                
                //绘制虚线
                if (mEvent.EventTrackType == 1)
                {
                    Rect _timeLineRect = TimeLineWindow.Instance.TimelineRect;
                    Vector2 _startPos = new Vector2(0, _timeLineRect.y - 20f);
                    Vector2 _endPos = new Vector2(0, _timeLineRect.y + _timeLineRect.height);
                    Handles.color = _eventTrackColor;
                    if (mDrawHeadManiPulator)
                    {
                        _startPos.x = meventRect.x;
                        _endPos.x = meventRect.x;
                        Handles.DrawDottedLine(_startPos, _endPos, 2);
                    }
                    if (mDrawFootManiPulator)
                    {
                        float _posX = meventRect.x + meventRect.width;
                        _startPos.x = _posX;
                        _endPos.x = _posX;
                        Handles.DrawDottedLine(_startPos, _endPos, 2);
                    }
                }
                
                //绘制操作柄
                DrawManipulator(meventRect);
            }
        }

        public void DrawManipulator(Rect _rect)
        {
            mManipulator_l.IsDraw = mDrawHeadManiPulator;
            mManipulator_r.IsDraw = mDrawFootManiPulator;

            mManipulator_l.Draw(_rect.x, _rect.y, _rect.height);
            mManipulator_r.Draw(_rect.x + _rect.width, _rect.y, _rect.height);
        }

        private void DrawTitle(string _title)
        {
            GUI.Box(meventRect, _title, mWindow.EngineSetting.fontTrack_Title);
        }
        private void DrawDetailed(string _detailed)
        {
            Rect _detailedRect = new Rect(meventRect);
            _detailedRect.x += _detailedRect.width + 5f;
            _detailedRect.width = 300f;
            GUI.Label(_detailedRect, _detailed, mWindow.EngineSetting.fontTrack_Detailed);
        }


        #region 回调

        private int mInitEndTime;
        private void SetEventRectInit()
        {
            mInitEndTime = mEvent.TriggerTime + mEvent.Duration;
        }
        private void SetEventRectHead(float _pos)
        {
            int _time = Mathf.Max(0, mWindow.GetTimeToPos(_pos));
            mEvent.TriggerTime = _time;
            mEvent.Duration = mInitEndTime - mEvent.TriggerTime;
            
            TimeLineWindow.Instance.UpdateTimeToNow();
            // Debug.Log($"EndPos: {meventRect.x + meventRect.width}");
        }
        private void SetEventRectFoot(float _pos)
        {
            int _time = mWindow.GetTimeToPos(_pos);
            mEvent.Duration = _time - mEvent.TriggerTime;
            
            TimeLineWindow.Instance.UpdateTimeToNow();
        }
        private void OnLoadEvent()
        {
            // EngineDebug.Log("装载回调");
            mWindow.OnLoadMouseAction(EMouseEvent.MouseDwonRight, mMouseActionInfo_r);
            mWindow.OnLoadMouseAction(EMouseEvent.MouseDwonLeft, mMouseActionInfo_l);
        }

        private void UnLoadEvent()
        {
            // EngineDebug.Log("卸载回调");
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDwonRight, mMouseActionInfo_r);
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDwonLeft, mMouseActionInfo_l);
        }
        
        private float mMousePos = 0;
        private int mEventStartPos = 0;
        private bool OnConEvent(Event _event)
        {
            Vector2 _mousePos = mWindow.MousePosToViewPos(_event.mousePosition);

            if ((meventRect).Contains(_mousePos))
            {
                mWindow.OnLoadMouseAction(EMouseEvent.MouseUp, mMouseActionInfo_MoveEnd);
                mWindow.OnLoadMouseAction(EMouseEvent.MouseDrag, mMouseActionInfo_Move);
                mMousePos = _mousePos.x - mStartPos;
                mWindow.OnSelection(mEvent);
                mEventStartPos = mEvent.TriggerTime;
                return true;
            }
            return false;
        }

        private bool OnMoveEvent(Event _event)
        {
            mEvent.TriggerTime = mWindow.GetTimeToPos(_event.mousePosition.x - mMousePos);

            if (mEventTrack != null)//mEventTrack!= null && mEvent.Duration >= 0
            {
                mEvent.TriggerTime = Mathf.Max(0, mEvent.TriggerTime);
            }
            // Debug.Log($"mEvent.TriggerTime: {mEvent.TriggerTime}");
            TimeLineWindow.Instance.UpdateTimeToNow();

            return true;
        }
        private bool OnMoveEnd(Event _event)
        {
            mWindow.UnLoadMouseAction(EMouseEvent.MouseUp, mMouseActionInfo_MoveEnd);
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDrag, mMouseActionInfo_Move);
            if (mEvent.TriggerTime != mEventStartPos) ResourcesWindow.Instance.ActionOnChange();
            return true;
        }
        
        private bool OnMenu(Event _event)
        {
            Vector2 _mousePos = mWindow.MousePosToViewPos(_event.mousePosition);

            if ((meventRect).Contains(_mousePos))
            {
                CreactMenu();
                return true;
            }
            return false;
        }
        private void CreactMenu()
        {
            GenericMenu _menu = new GenericMenu();
            _menu.AddItem(new GUIContent("删除事件"),false, () =>
            {
                if (mDeleteCallback != null)
                {
                    IsDraw = false;
                    mDeleteCallback(this);
                }
            });
            _menu.AddItem(new GUIContent("复制事件"),false, () =>
            {
                // if (mDeleteCallback != null)
                // {
                //     IsDraw = false;
                //     mDeleteCallback(this);
                // }
                ActionSaveFlishEvent.Run();
                ActionWindowMain.CopyActionEven = mEvent.Clone();
            });

            if (ActionWindowMain.CopyActionEven == null)
            {
                _menu.AddDisabledItem(new GUIContent("粘贴参数(未复制过)"));
            }
            else
            {
                if (ActionWindowMain.CopyActionEven.EventData.GetEvenType() == mEvent.EventData.GetEvenType())
                {
                    _menu.AddItem(new GUIContent("粘贴参数"), false, () =>
                    {
                        ActionSaveFlishEvent.Run();
                        mEvent.EventData = ActionWindowMain.CopyActionEven.EventData.Clone(
                            ActionWindowMain.CopyActionEven.EventData.Creact());
                    });
                }
                else
                {
                    _menu.AddDisabledItem(new GUIContent("粘贴参数(类型不同)"));
                }
            }

            // using (new GUIColorScope(Color.cyan))
            {
                _menu.AddItem(new GUIContent(">- 编辑事件 -<"), false, () =>
                {
                    string _path = "";
                    string[] _guid = AssetDatabase.FindAssets(mEvent.EventData.GetType().Name);
                    string[] _nowPath = AssetDatabase.GUIDToAssetPath(_guid[0]).Split('/');
                    for (int i = 0; i < _nowPath.Length-1; i++)
                        _path += (_nowPath[i] + "/");
                    _path += _nowPath[^1];
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(_path, 10);
                    // EngineDebug.LogWarning($"跳转事件类型: {mEvent.EventData.GetType().Name} \n最终路径: {_path}");
                });
            }
            _menu.ShowAsContext();
        }

        #endregion
        
    }
}
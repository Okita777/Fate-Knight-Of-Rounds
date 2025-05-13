using System;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public class ActionInterrupDisplay
    {
        [SerializeField] private EditorActionInterrupt mActionInterrupt;
        
        [NonSerialized] private EventTrack mEventTrack;
        [NonSerialized] private TimeLineWindow mWindow;
        private Rect mEventTrackRect;
        private Rect meventRect;
        private MouseActionInfo mMouseActionInfo_l;
        private MouseActionInfo mMouseActionInfo_r;
        private MouseActionInfo mMouseActionInfo_Move;
        private MouseActionInfo mMouseActionInfo_MoveEnd;
        private EventManipulator mManipulator_l;
        private EventManipulator mManipulator_c;
        private EventManipulator mManipulator_r;
        private bool mDrawHeadManiPulator = false;
        private bool mDrawBodyManiPulator = false;
        private bool mDrawFootManiPulator = false;
        private bool mIsDraw = false;

        private Action<ActionInterrupDisplay> mDeleteCallback = null;
        
        public EditorActionInterrupt ActionInterrupt => mActionInterrupt;


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
                        // Debug.Log("安装AA");
                        OnLoadEvent();
                    }
                    else
                    {
                        // Debug.Log("卸载AA");
                        UnLoadEvent();

                    }
                }
            }
        }
        
        public ActionInterrupDisplay(EditorActionInterrupt _event, EventTrack _track = null, Action<ActionInterrupDisplay> _deleteCallback = null)
        {
            mWindow = TimeLineWindow.Instance;
            mActionInterrupt = _event;
            mEventTrack = _track;
            mDeleteCallback = _deleteCallback;

            mMouseActionInfo_r = new MouseActionInfo(OnMenu, MotionEngineConst.Priority_EventUnit);
            mMouseActionInfo_l = new MouseActionInfo(OnConEvent, MotionEngineConst.Priority_EventUnit);
            mMouseActionInfo_Move = new MouseActionInfo(OnMoveEvent, MotionEngineConst.Priority_EventUnit);
            mMouseActionInfo_MoveEnd = new MouseActionInfo(OnMoveEnd, MotionEngineConst.Priority_EventUnit);
            
            mManipulator_l = new EventManipulator(SetEventRectHead, SetEventRectInit);
            mManipulator_c = new EventManipulator(SetEventRectBody);
            mManipulator_r = new EventManipulator(SetEventRectFoot);
        }
        
        public void Draw(Rect _rect)
        {
            
            mEventTrackRect = _rect;
            if(mActionInterrupt == null)return;
            
            float _startPos = mWindow.GetPosToTime(mActionInterrupt.TriggerTime);
            float _endPos = _startPos + MotionEngineConst.GUI_EventTrackMinWidth;
            if (mActionInterrupt.Duration != 0)
            {
                //在真实末端长度大于最小长度时才应用真实长度
                float _realEndPos = mWindow.GetPosToTime(mActionInterrupt.TriggerTime + mActionInterrupt.Duration);
                if(_realEndPos - _startPos > MotionEngineConst.GUI_EventTrackMinWidth)
                {
                    _endPos = _realEndPos;
                }
            }


            bool _isDrawHead = _startPos < _rect.x + _rect.width;
            bool _isDrawFoot = mActionInterrupt.Duration < 0 || (_endPos > _rect.x);

            IsDraw = (_isDrawHead && _isDrawFoot);

            if (IsDraw)
            {
                bool _drawHeadManiPulator = _startPos > _rect.x;
                bool _drawBodyManiPulator = false;
                bool _drawFootManiPulator = false;
                
                mDrawHeadManiPulator = _drawHeadManiPulator && (mActionInterrupt.Duration != 0);
                mDrawBodyManiPulator = _drawBodyManiPulator;
                mDrawFootManiPulator = _drawFootManiPulator;

                //绘制主身
                meventRect = new Rect(_rect);

                if (_drawHeadManiPulator)
                {
                    meventRect.x = _startPos;
                }

                float _timeRangeEnd = _rect.x + _rect.width;
                float _eventWidth = _timeRangeEnd - _rect.x;
                if (_endPos < _timeRangeEnd && mActionInterrupt.Duration >= 0)
                {
                    _eventWidth = _endPos - meventRect.x;
                    _drawFootManiPulator = true;
                    mDrawFootManiPulator = mActionInterrupt.Duration > 0;//单帧不能拥有控制柄
                }//绘制页脚
                else
                {
                    if (!_drawHeadManiPulator)
                    {//没有绘制页头的话  就绘制最长轨道
                        _eventWidth = _rect.width;
                    }
                    else
                    {
                        _eventWidth = _rect.width + _rect.x - _startPos;
                    }
                }
                meventRect.width = _eventWidth;

                //绘制跳转区块
                Color _eventTrackColor = mEventTrack.MainColor;
                Color _color = mWindow.IsSelection(mActionInterrupt) ? 
                    mWindow.EngineSetting.colorSelection : _eventTrackColor;
                EditorGUI.DrawRect(meventRect, _color);
                
                //绘制预输入区块
                float _bodyPos = 0;
                if (mActionInterrupt.ExecuteTime > 0)
                {
                    _bodyPos = mWindow.GetPosToTime(mActionInterrupt.ExecuteTime + mActionInterrupt.TriggerTime);
                    if (_bodyPos > meventRect.x)
                    {
                        Rect _PreRect = new Rect(meventRect);
                        _PreRect.width = _bodyPos - _PreRect.x;
                        EditorGUI.DrawRect(_PreRect, Color.black * 0.35f);

                        //是否绘制中间的控制柄
                        mDrawBodyManiPulator = _PreRect.width > MotionEngineConst.GUI_EventTrackMinWidth;
                    }
                }

                //绘制标题
                if (_eventWidth > MotionEngineConst.GUI_EventRenderTitleMinWidth)
                {
                    string _title = "动画跳转: " + mActionInterrupt.ActionName +
                                    "\n融合时间：" + mActionInterrupt.CrossFadeTime +
                                    "     剪切时间:" + mActionInterrupt.OffsetTime;
                    GUI.Box(meventRect, _title, mWindow.EngineSetting.fontTrack_Title);
                }
                else
                {
                    Rect _detailedRect = new Rect(meventRect);
                    _detailedRect.x += _detailedRect.width + 5;
                    _detailedRect.width = 300;
                    string _title = "动画跳转: " + mActionInterrupt.ActionName +
                                    "\n融合时间：" + mActionInterrupt.CrossFadeTime +
                                    "     剪切时间:" + mActionInterrupt.OffsetTime;
                    GUI.Box(_detailedRect, _title, mWindow.EngineSetting.fontTrack_Detailed);
                }
                
                //绘制辨识符号  单帧
                if (mActionInterrupt.Duration == 0)
                {
                    float _symbolHeight = 5f;
                    Rect _symbol = new Rect(meventRect);
                    _symbol.y += _symbol.height - _symbolHeight;
                    _symbol.height = _symbolHeight;
                    EditorGUI.DrawRect(_symbol, Color.black * 0.5f);
                }
                
                //绘制操作柄
                DrawManipulator(meventRect, _bodyPos);
            }
        }
        
        public void DrawManipulator(Rect _rect,float pos_c)
        {
            mManipulator_l.IsDraw = mDrawHeadManiPulator;
            mManipulator_c.IsDraw = mDrawBodyManiPulator;
            mManipulator_r.IsDraw = mDrawFootManiPulator;
            
            mManipulator_l.Draw(_rect.x, _rect.y, _rect.height);
            mManipulator_c.Draw(pos_c, _rect.y, _rect.height);
            mManipulator_r.Draw(_rect.x + _rect.width, _rect.y, _rect.height);
        }
        
        #region 回调
        private int mInitEndTime;
        private int mInitStartTime;
        private int mInitExecuteTime;
        private void SetEventRectInit()
        {
            mInitEndTime = mActionInterrupt.TriggerTime + mActionInterrupt.Duration;
            mInitStartTime = mActionInterrupt.TriggerTime;
            mInitExecuteTime = mActionInterrupt.ExecuteTime;
        }
        private void SetEventRectHead(float _pos)
        {
            int _time = Mathf.Max(0, mWindow.GetTimeToPos(_pos));
            mActionInterrupt.TriggerTime = _time;
            if (mActionInterrupt.Duration > -1)
            {
                mActionInterrupt.Duration = mInitEndTime - mActionInterrupt.TriggerTime;
            }

            if (mActionInterrupt.ExecuteTime > 0)
            {
                mActionInterrupt.ExecuteTime = mInitExecuteTime + mInitStartTime - _time;
            }
        }
        private void SetEventRectBody(float _pos)
        {
            int _time = mWindow.GetTimeToPos(_pos);
            mActionInterrupt.ExecuteTime = _time - mActionInterrupt.TriggerTime;
            if (mActionInterrupt.ExecuteTime < 0 ||
                (mActionInterrupt.ExecuteTime >= mActionInterrupt.Duration && mActionInterrupt.Duration > 0))
            {
                mActionInterrupt.ExecuteTime = 0;
            }
        }
        private void SetEventRectFoot(float _pos)
        {
            int _time = mWindow.GetTimeToPos(_pos);
            mActionInterrupt.Duration = _time - mActionInterrupt.TriggerTime;
            if (mActionInterrupt.ExecuteTime >= mActionInterrupt.Duration)
            {
                mActionInterrupt.ExecuteTime = 0;
            }
        }
        
        private void OnLoadEvent()
        {
            mWindow.OnLoadMouseAction(EMouseEvent.MouseDwonRight, mMouseActionInfo_r);
            mWindow.OnLoadMouseAction(EMouseEvent.MouseDwonLeft, mMouseActionInfo_l);
        }

        private void UnLoadEvent()
        {
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDwonRight, mMouseActionInfo_r);
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDwonLeft, mMouseActionInfo_l);
            mManipulator_l.IsDraw = false;
            mManipulator_c.IsDraw = false;
            mManipulator_r.IsDraw = false;
        }

        private float mMousePos = 0;
        private int mStartPos = 0;
        private bool OnConEvent(Event _event)
        {
            Vector2 _mousePos = mWindow.MousePosToViewPos(_event.mousePosition);

            if ((meventRect).Contains(_mousePos))
            {
                mWindow.OnLoadMouseAction(EMouseEvent.MouseUp, mMouseActionInfo_MoveEnd);
                mWindow.OnLoadMouseAction(EMouseEvent.MouseDrag, mMouseActionInfo_Move);
                mMousePos = _mousePos.x - mWindow.GetPosToTime(mActionInterrupt.TriggerTime);
                mStartPos = mActionInterrupt.TriggerTime;
                mWindow.OnSelection(mActionInterrupt);
                return true;
            }
            return false;
        }

        private bool OnMoveEvent(Event _event)
        {
            mActionInterrupt.TriggerTime = Mathf.Max(0, mWindow.GetTimeToPos(_event.mousePosition.x-mMousePos));
            return true;
        }
        private bool OnMoveEnd(Event _event)
        {
            mWindow.UnLoadMouseAction(EMouseEvent.MouseUp, mMouseActionInfo_MoveEnd);
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDrag, mMouseActionInfo_Move);
            if (mActionInterrupt.TriggerTime != mStartPos) ResourcesWindow.Instance.ActionOnChange();
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
            _menu.ShowAsContext();
        }
        

        #endregion
    }
}
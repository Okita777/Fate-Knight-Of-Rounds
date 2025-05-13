using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public class EventTrack : IActionTrack
    {
        [SerializeField] private List<EventDisplay> mCurEventDisplay;
        [SerializeField] private List<ActionInterrupDisplay> mCurInterrup = new List<ActionInterrupDisplay>();
        [SerializeField] private string mTrackName;
        [SerializeField] private Color mColor;
        [SerializeField] private bool mIsPreview = false;

        [NonSerialized] private ActionEngineSetting mEngineSetting;
        [NonSerialized] public Rect mTrackRect;
        private TimeLineWindow mWindow => TimeLineWindow.Instance;
        private GUIContent mIcon = null;
        private Rect mPreviewButtonRect;
        private bool mIsDraw = false;
        private float mDrawStart;
        private float mDrawEnd;
        private MouseActionInfo mMouseActionInfo;
        private MouseActionInfo mPreviewButton;

        private Action<EventTrack> mDeleteCallback;
        
        // private List<int> mEventTypes;
        private List<string> mEventTypes;
        public Color MainColor => mColor;
        public List<EventDisplay> CurEventDisplay => mCurEventDisplay;
        public List<ActionInterrupDisplay> CurInterrup => mCurInterrup;
        public string TrackName => mTrackName;
        public bool IsPreview => mIsPreview;

        public bool IsDraw
        {
            get { return mIsDraw; }
            set
            {
                if (mIsDraw != value)
                {
                    if (value)
                    {
                        OnLoadEvent();
                    }
                    else
                    {
                        UnloadEvent();
                    }
                    mIsDraw = value;
                }
            }
        }
        

        public void Draw(Rect _rect, float _DrawStart, float _DrawEnd)
        {

            // _DrawStart += 60f;
            // _DrawEnd -= 60f;
            mDrawStart = _DrawStart;
            mDrawEnd = _DrawEnd;
            bool _IsDraw = _rect.y < _DrawEnd;
            if (_IsDraw) _IsDraw = (_rect.y + _rect.height) > _DrawStart;
            IsDraw = _IsDraw;
            if (IsDraw)
            {
                //背景绘制
                float _posY = (MotionEngineConst.GUI_EventGroupHeight - MotionEngineConst.GUI_EventTrackHeight) * 0.5f;
                _rect.height = MotionEngineConst.GUI_EventTrackHeight;
                _rect.y += _posY;
                _rect.x += MotionEngineConst.GUI_EventTrackRetraction;
                EditorGUI.DrawRect(_rect, mEngineSetting.colorEventTrack);

                //头部绘制
                _rect.width = MotionEngineConst.GUI_EventTrackHeader;
                mColor = EditorGUI.ColorField(_rect, new GUIContent(""), mColor, false, false, false);
                
                //图标绘制
                float _iconSize = MotionEngineConst.GUI_EventTrackIconSize;
                float _DifferenceHalf = (MotionEngineConst.GUI_EventTrackHeight - _iconSize) * 0.5f;
                Rect _trackIcon = new Rect(_rect.x + MotionEngineConst.GUI_EventTrackHeader + _DifferenceHalf,
                    _rect.y + _DifferenceHalf, _iconSize, _iconSize);
                GUI.Box(_trackIcon, mIcon, GUIStyle.none);
            
                //绘制轨道名
                float _trackNamePosX = _trackIcon.x + _trackIcon.width + _DifferenceHalf;
                float _trackNameWidth = MotionEngineConst.GUI_TimeHeadWindth - _trackNamePosX - _rect.height-_DifferenceHalf;
                Rect _trackNameRect = new Rect(_trackNamePosX, _trackIcon.y, _trackNameWidth, _trackIcon.height);
                using (new GUIColorScope(mEngineSetting.colorWhite))
                {
                    mTrackName = EditorGUI.TextField(_trackNameRect, mTrackName, mEngineSetting.fontDefault);
                }

                //绘制预览提示图标
                mPreviewButtonRect = new Rect(_trackNameRect.x + _trackNameRect.width + _DifferenceHalf, _rect.y,
                    _rect.height, _rect.height);
                GUIContent _previewIcon = mIsPreview
                    ? mEngineSetting.ExGUI.eventTrackIcon_Preview
                    : mEngineSetting.ExGUI.eventTrackIcon_PreviewOff;
                GUI.Label(mPreviewButtonRect, _previewIcon);


                //轨道的Rect
                mTrackRect = mWindow.GetEvenTrackRect(_rect.y, _rect.height);
                // EditorGUI.DrawRect(mTrackRect, Color.black);

                //绘制Action事件
                foreach (var VARIABLE in mCurEventDisplay)
                {
                    VARIABLE.Draw(mTrackRect);
                }

                foreach (var VARIABLE in mCurInterrup)
                {
                    VARIABLE.Draw(mTrackRect);
                }
            }
        }

        public float GetHeight()
        {
            return MotionEngineConst.GUI_EventGroupHeight;
        }

        public void OnLoadEvent()
        {
            mWindow.OnLoadMouseAction(EMouseEvent.MouseDwonRight, mMouseActionInfo);
            mWindow.OnLoadMouseAction(EMouseEvent.MouseDwonLeft, mPreviewButton);
            mWindow.OnLoadEventTrack(CheckMouseEvent);

            // EngineDebug.Log($"装载事件: {mTrackName}");
        }

        public void UnloadEvent()
        {
            //释放子事件
            foreach (var VARIABLE in mCurEventDisplay)
                VARIABLE.IsDraw = false;
            foreach (var VARIABLE in mCurInterrup)
                VARIABLE.IsDraw = false;
            
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDwonRight, mMouseActionInfo);
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDwonLeft, mPreviewButton);
            mWindow.UnLoadEventTrack(CheckMouseEvent);

            // EngineDebug.Log($"卸载事件: {mTrackName}");
        }

        public IActionTrack Clone()
        {
            List<EventDisplay> _EventDisplayList = new List<EventDisplay>();
            List<ActionInterrupDisplay> _CurInterrup = new List<ActionInterrupDisplay>();
            
            EventTrack _eventTrack = new EventTrack(mEngineSetting, mIcon, mDeleteCallback,
                mEventTypes, mColor, this);

            foreach (var _eventDisplay in mCurEventDisplay)
            {
                _EventDisplayList.Add(new EventDisplay(_eventDisplay.MainEvent.Clone(), this, DeleteDisplayEvent));
            }
            foreach (var _actionInterrup in mCurInterrup)
            {
                _CurInterrup.Add(new ActionInterrupDisplay(_actionInterrup.ActionInterrupt.CloneEditor(), this, DeleteDisplayEvent));
            }
            
            _eventTrack.mCurEventDisplay = _EventDisplayList;
            _eventTrack.mCurInterrup = _CurInterrup;
            
            return _eventTrack;
        }
        private bool OnConTextClick(Event _event)
        {
            Vector2 _mousePos = mWindow.MousePosToViewPos(_event.mousePosition);
            if (mTrackRect.Contains(_mousePos))
            {
                if (_mousePos.y < mDrawStart)
                {
                    return false;
                }
                mMenuMousePosx = _mousePos.x;
                CustomTrackMenu();
                return true;
            }
            return false;
        }


        #region 回调

        private bool CheckMouseEvent(Event _event, out EventTrack _eventTrack)
        {
            Vector2 _mousePos = mWindow.MousePosToViewPos(_event.mousePosition);
            if (mTrackRect.Contains(_mousePos))
            {
                mMenuMousePosx = _mousePos.x;

                Rect _rect = new Rect(mTrackRect);
                _rect.y -= mWindow.MousePosToViewPos();
                _rect.width += 400;
                _rect.x -= 200;
                EditorGUI.ObjectField(_rect, (MonoScript)null, typeof(MonoScript));
                _eventTrack = this;
                return true;
            }

            _eventTrack = null;
            return false;
        }
        
        public void OnDrawSelectRect(Event _event, int _totalTime)
        {
            Rect _rect = new Rect(mTrackRect);
            _rect.y -= mWindow.MousePosToViewPos();
            _rect.x = _event.mousePosition.x;
            _rect.width = _totalTime;
            EditorGUI.DrawRect(_rect, Color.red * 0.6f);
        }
        private bool OnPreviewButton(Event _event)
        {
            Vector2 _mousePos = mWindow.MousePosToViewPos(_event.mousePosition);
            if (mPreviewButtonRect.Contains(_mousePos))
            {
                mIsPreview = !mIsPreview;
                TimeLineWindow.Instance.UpdateTimeToNow();
                EditorWindow.GetWindow<SceneView>().Repaint();
                return true;
            }
            return false;
        }

        private float mMenuMousePosx = 0f;
        private void CustomTrackMenu()
        {
            GenericMenu _menu = new GenericMenu();
            foreach (var VARIABLE in mEventTypes)
            {
                if(ActionWindowMain.GetActionEventID(VARIABLE, out int _eventID)){
                    _menu.AddItem(new GUIContent(ActionWindowMain.EventTypesDes[_eventID]), false, () =>
                    {
                        // CreactTrackEvent(ActionWindowMain.EventTypes.IndexOf(VARIABLE) , mMenuMousePosx);
                        CreactTrackEvent(_eventID, mMenuMousePosx);
                        ResourcesWindow.Instance.ActionOnChange();
                    });
                }
            }
            _menu.AddDisabledItem(new GUIContent(mTrackName));

            _menu.AddItem(new GUIContent("删除轨道"), false, () =>
            {
                IsDraw = false;
                mDeleteCallback(this);
                ResourcesWindow.Instance.ActionOnChange();
            });

            if (ActionWindowMain.CopyActionEven == null)
            {
                _menu.AddDisabledItem(new GUIContent("粘贴事件"));
            }
            else
            {
                _menu.AddItem(new GUIContent("粘贴事件"), false, () =>
                {
                    ResourcesWindow.Instance.ActionOnChange();
                    mCurEventDisplay.Add(new EventDisplay(ActionWindowMain.CopyActionEven.Clone(), this, DeleteDisplayEvent));
                    // IsDraw = false;
                    // mDeleteCallback(this);
                });
            }
            
            _menu.ShowAsContext();
        }

        private void DeleteDisplayEvent(EventDisplay _event)
        {
            ResourcesWindow.Instance.ActionOnChange();
            mCurEventDisplay.Remove(_event);
        }
        private void DeleteDisplayEvent(ActionInterrupDisplay _event)
        {
            ResourcesWindow.Instance.ActionOnChange();
            mCurInterrup.Remove(_event);
        }
        public void CreactTrackEvent(int _evenType, float _MousePosX)
        {
            ResourcesWindow.Instance.ActionOnChange();

            // EngineDebug.Log(
            //     "_evenType: " + _evenType + "\n" +
            //     "(int)EEvenTypeInternal: " + (-(int)EEvenTypeInternal.EET_Interrupt) + "\n" +
            //     "EventType_m.Length: " + ActionWindowMain.EventType_m.Length + "\n" +
            //     // "_evenType: " + _evenType + "\n" +
            //     ""
            // );
            if (_evenType < ActionWindowMain.EventType_m.Length)
            {
                _evenType *= -1;
            }
            else
            {
                _evenType -= ActionWindowMain.EventType_m.Length;
            }
            
            int _TriggerTime = (mWindow.GetTimeToPos(_MousePosX));
            int _Duration = 1 * RunTime.MotionEngineConst.TimeDoubling;
            if (_evenType == -(int)EEvenTypeInternal.EET_Interrupt)
            {
                EditorActionInterrupt _actionInterrupt = ActionEventCreact.CreactInterrupt(_evenType);
                _actionInterrupt.TriggerTime = _TriggerTime;
                _actionInterrupt.Duration = _Duration;
                mCurInterrup.Add(new ActionInterrupDisplay(_actionInterrupt,this,DeleteDisplayEvent));
            }
            else
            {
                EditorActionEvent _actionEvent = ActionEventCreact.Creact(_evenType);
                if (_actionEvent == null)
                {
                    EditorUtility.DisplayDialog("警告", "事件创建错误，详情请看 控制台 面版的Debug", "我知道了");
                    return;
                }
                _actionEvent.TriggerTime = _TriggerTime;

                // if (!mIsPreview)
                // {
                //     mIsPreview = EventUpdate.Instance.EditorEventUpdate(0, _actionEvent);
                // }
                mIsPreview = true;
                mCurEventDisplay.Add(new EventDisplay(_actionEvent, this, DeleteDisplayEvent));
            }
        }
        #endregion

        #region 重构函数
        public EventTrack(
            ActionEngineSetting _engineSetting, 
            GUIContent _icon, 
            Action<EventTrack> _deleCallback,
            List<string> _evenTypes,
            Color _color, 
            EventTrack _eventTrack = null,
            string _trackName = ""
        ) 
        {
            if (_eventTrack == null)
            {
                mCurEventDisplay = new List<EventDisplay>();
                mCurInterrup = new List<ActionInterrupDisplay>();
                mTrackName = _trackName;
                mColor = _color;
                mIsPreview = true;
            }
            else
            {
                List<EventDisplay> _EventDisplayList = new List<EventDisplay>();
                List<ActionInterrupDisplay> _CurInterrup = new List<ActionInterrupDisplay>();
                foreach (var VARIABLE in _eventTrack.CurEventDisplay)
                {
                    _EventDisplayList.Add(new EventDisplay(VARIABLE.MainEvent, this, DeleteDisplayEvent));
                }
                foreach (var VARIABLE in _eventTrack.CurInterrup)
                {
                    _CurInterrup.Add(new ActionInterrupDisplay(VARIABLE.ActionInterrupt, this, DeleteDisplayEvent));
                }
                mCurEventDisplay = _EventDisplayList;
                mCurInterrup = _CurInterrup;
                mTrackName = _eventTrack.TrackName;
                mColor = _eventTrack.MainColor;
                mIsPreview = _eventTrack.IsPreview;
            }
            mEngineSetting = _engineSetting;
            mIcon = _icon;
            mDeleteCallback = _deleCallback;
            mEventTypes = _evenTypes;
            // mWindow = TimeLineWindow.Instance;
            mMouseActionInfo = new MouseActionInfo(OnConTextClick, MotionEngineConst.Priority_EventTrack);
            mPreviewButton = new MouseActionInfo(OnPreviewButton, MotionEngineConst.Priority_EventUnit);
        }
        #endregion

    }
}
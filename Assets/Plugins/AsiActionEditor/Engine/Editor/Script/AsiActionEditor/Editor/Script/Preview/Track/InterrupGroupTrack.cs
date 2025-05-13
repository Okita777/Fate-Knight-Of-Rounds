using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using AsiActionEngine.RunTime;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public class InterrupGroupTrack : IActionTrack
    {
        [SerializeField] private int mActionStateID;
        [SerializeField] private int mActionOffset;
        [SerializeField] private Color mColor = Color.black;
        [SerializeField] private List<string> mInterrupLock = new List<string>();
        [SerializeField] private InterrupOffset mInterrupOffset = new InterrupOffset();//Dictionary<string,float>
        
        [NonSerialized] private ActionEngineSetting mEngineSetting;
        [NonSerialized] private TimeLineWindow mWindow;
        [NonSerialized] public List<InterrupGroupTrackDisplay> mAllInterrup = new List<InterrupGroupTrackDisplay>();
        private Rect mTrackRect;
        private Rect mPreviewButtonRect;
        private Rect mMainEvent;
        private MouseActionInfo mInterrupUnFold;
        private MouseActionInfo mTrackMenu;
        private EventManipulator mManipulator;
        private Action<IActionTrack> mDeleteCallback;
        private Action mReorderableListChange;
        private float mDrawStart;
        private float mDrawEnd;
        private bool mIsOpen = false;
        private bool mIsDraw = false;
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
        public int ActionStateID
        {
            get { return mActionStateID; }
            set { mActionStateID = value; }
        }

        public int ActionOffset => mActionOffset; 
        public List<string> InterrupLock => mInterrupLock;
        public InterrupOffset InterrupOffset => mInterrupOffset;

        public InterrupGroupTrack(int _name, InterrupGroupTrack _InterrupGroupTrack,
            Action<IActionTrack> _deleCallback, Action _ReorderableListChange)
        {
            if (_InterrupGroupTrack == null)
            {
                mActionStateID = _name;
            }
            else
            {
                mActionStateID = _InterrupGroupTrack.ActionStateID;
                mActionOffset = _InterrupGroupTrack.ActionOffset;
                mColor = _InterrupGroupTrack.mColor;
                mInterrupLock = _InterrupGroupTrack.InterrupLock;
                mInterrupOffset = _InterrupGroupTrack.mInterrupOffset;
            }
            mDeleteCallback = _deleCallback;
            mReorderableListChange = _ReorderableListChange;
            mWindow = TimeLineWindow.Instance;
            mEngineSetting = TimeLineWindow.Instance.EngineSetting;
            mInterrupUnFold = new MouseActionInfo(OnPreviewButton, MotionEngineConst.Priority_EventTrack);
            mTrackMenu = new MouseActionInfo(OnConTextClick, MotionEngineConst.Priority_EventTrack);
            mManipulator = new EventManipulator(SetTrackTrigger, SetTrackTriggerInit);
        }

        public void Draw(Rect _rect, float _DrawStart, float _DrawEnd)
        {
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
                GUI.Box(_trackIcon, mEngineSetting.ExGUI.eventTrackIcon_Interruo, GUIStyle.none);

                //绘制轨道名
                float _trackNamePosX = _trackIcon.x + _trackIcon.width + _DifferenceHalf;
                float _trackNameWidth = MotionEngineConst.GUI_TimeHeadWindth - _trackNamePosX - _rect.height - _DifferenceHalf;
                Rect _trackNameRect = new Rect(_trackNamePosX, _trackIcon.y, _trackNameWidth, _trackIcon.height);

                bool _isValue = GetAction() == null;
                string _disName = _isValue ? "打断组丢失" : "(打断组) " + GetAction().Name;
                GUI.Label(
                    _trackNameRect,
                    string.Format("<color=#{0}>{1}</color>", _isValue ? "CC3300" : "FFCC33", _disName),
                    mEngineSetting.fontDefault
                );

                //绘制预览提示图标
                mPreviewButtonRect = new Rect(_trackNameRect.x + _trackNameRect.width + _DifferenceHalf, _rect.y,
                    _rect.height, _rect.height);
                GUIContent _previewIcon = mIsOpen
                    ? mEngineSetting.ExGUI.interrupGroupTrack_Unfold
                    : mEngineSetting.ExGUI.interrupGroupTrack_Onfold;
                GUI.Label(mPreviewButtonRect, _previewIcon);
                Rect _intertrackIcon = new Rect(_rect);
                if (mIsOpen)
                {
                    Rect _interrupTrack = new Rect(mTrackRect);
                    Rect _interrupTrackBack = new Rect(mTrackRect);
                    float _heightspace = _interrupTrack.height + 2;
                    float _retraction = mTrackRect.x - _rect.x - MotionEngineConst.GUI_EventTrackHeader;
                    _retraction -= _iconSize * 2.5f;
                    _interrupTrackBack.x -= _retraction;
                    _interrupTrackBack.width += _retraction;

                    _intertrackIcon = new Rect(_interrupTrackBack);
                    _intertrackIcon.x -= _interrupTrackBack.height;
                    _intertrackIcon.width = _interrupTrackBack.height;
                    foreach (var item in GetInterrup())
                    {
                        _interrupTrack.y += _heightspace;
                        _interrupTrackBack.y += _heightspace;
                        _intertrackIcon.y += _heightspace;
                        bool _tarckIsDraw = _interrupTrack.y < _DrawEnd;
                        if (_tarckIsDraw) _tarckIsDraw = _interrupTrack.y + _interrupTrack.height > _DrawStart;
                        if (_tarckIsDraw)
                        {
                            EditorGUI.DrawRect(_interrupTrackBack, mEngineSetting.colorEventTrack);
                            item.Draw(_interrupTrack);
                            GUI.Label(_intertrackIcon, mEngineSetting.ExGUI.eventTrackIcon_InterrupTarck);
                            GUI.Label(_interrupTrackBack, $"<color=#FFCC33>{item.ActionInterrupt.ActionName}</color>",
                                mEngineSetting.fontDefault);
                            //绘制忽略层显示效果
                            if (item.Change_hide)
                            {
                                EditorGUI.DrawRect(_interrupTrack, Color.black * 0.3f);
                            }
                        }
                    }
                }
                //轨道的Rect
                mTrackRect = mWindow.GetEvenTrackRect(_rect.y, _rect.height);
                mManipulator.Draw(mTrackRect.x, mTrackRect.y, mTrackRect.height, mTrackRect.width);
                Rect _trackRect = new Rect(mTrackRect);
                _trackRect.width = 5f;
                _trackRect.x = mWindow.GetPosToTime(mActionOffset);
                if (_trackRect.x >= mTrackRect.x)
                {
                    _trackRect.height += 2f;
                    EditorGUI.DrawRect(_trackRect, mColor);
                    if (mIsOpen)
                    {
                        Handles.color = mColor;
                        float _lineHeight = _intertrackIcon.y + _intertrackIcon.height;
                        Vector2 _linePosX_S = new Vector2(_trackRect.position.x, _trackRect.position.y);
                        Vector2 _linePosX_E = new Vector2(_trackRect.position.x, _lineHeight);

                        Handles.DrawLine(_linePosX_S, _linePosX_E);
                    }
                }
            }
        }

        public EditorActionState GetAction()
        {
            if (mActionStateID < 0)
            {
                return null;
            }

            if(ResourcesWindow.Instance.ActionStateDic.TryGetValue(mActionStateID, out var _editorActionState))
            {
                return _editorActionState;
            }
            // mActionStateID = -1;
            return null;
        }
        private List<InterrupGroupTrackDisplay> GetInterrup()
        {
            if(mAllInterrup.Count < 1)
            {
                EditorActionState _editorActionState = GetAction();
                if (_editorActionState != null)
                {//更新打断组的列表
                    foreach (var AllEventTrackGroup in _editorActionState.AllEventTrackGroup)
                    {
                        foreach (var _actionTrack in AllEventTrackGroup.CurActiontTrack)
                        {
                            if (_actionTrack is EventTrack _eventTrack)
                            {
                                foreach (var _interrup in _eventTrack.CurInterrup)
                                {
                                    mAllInterrup.Add(new InterrupGroupTrackDisplay(_interrup.ActionInterrupt,
                                        _eventTrack.MainColor, this));
                                } //轨道下的所有打断事件
                            }
                        }
                    }
                }
            }
            return mAllInterrup;
        }

        public float GetHeight()
        {
            if (mIsOpen)
            {
                return GetInterrup().Count * MotionEngineConst.GUI_EventGroupHeight + MotionEngineConst.GUI_EventGroupHeight;
            }
            return MotionEngineConst.GUI_EventGroupHeight;
        }
        
        public IActionTrack Clone()
        {
            InterrupGroupTrack _eventTrack = new InterrupGroupTrack(mActionStateID, this, mDeleteCallback,
                mReorderableListChange);
            _eventTrack.mInterrupLock = new List<string>();
            _eventTrack.mInterrupLock.AddRange(mInterrupLock);
            _eventTrack.mInterrupOffset = new InterrupOffset();
            foreach (var _key in mInterrupOffset)
            {
                _eventTrack.mInterrupOffset.Add(_key.Key, _key.Value);
            }
            
            return _eventTrack;
        }
        
        #region 回调
        private float mInitOffsetPos;
        private void SetTrackTriggerInit()
        {
            int _time = mWindow.GetTimeToPos(Event.current.mousePosition.x);
            mInitOffsetPos = Event.current.mousePosition.x - mWindow.GetPosToTime(mActionOffset);
        }
        private void SetTrackTrigger(float _posX)
        {
            int _time = mWindow.GetTimeToPos(_posX - mInitOffsetPos);
            mActionOffset = Mathf.Max(0, _time);
        }

        private void GetEventRect(Rect _rect)
        {
            Rect _main = new Rect(_rect);
            List<InterrupGroupTrackDisplay> _interrup = GetInterrup();
        }
        
        public void OnLoadEvent()
        {
            // if(mInterrupUnFold is null){Debug.LogError("null"); return;}
            // if(mTrackMenu is null){Debug.LogError("null"); return;}

            mWindow.OnLoadMouseAction(EMouseEvent.MouseDwonLeft, mInterrupUnFold);
            mWindow.OnLoadMouseAction(EMouseEvent.MouseDwonRight, mTrackMenu);
            mManipulator.IsDraw = true;
        }

        public void UnloadEvent()
        {
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDwonLeft, mInterrupUnFold);
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDwonRight, mTrackMenu);

            mManipulator.IsDraw = false;
            foreach (var VARIABLE in mAllInterrup)
            {
                VARIABLE.IsDraw = false;
            }
        }

        private bool OnPreviewButton(Event _event)
        {
            Vector2 _mousePos = mWindow.MousePosToViewPos(_event.mousePosition);
            if (mPreviewButtonRect.Contains(_mousePos))
            {
                mIsOpen = !mIsOpen;
                if (!mIsOpen)
                {
                    foreach (var VARIABLE in mAllInterrup)
                    {
                        VARIABLE.IsDraw = false;
                    }
                }
                mReorderableListChange();
                return true;
            }
            return false;
        }

        private bool OnConTextClick(Event _event)
        {
            Vector2 _mousePos = mWindow.MousePosToViewPos(_event.mousePosition);
            if ((mTrackRect).Contains(_mousePos))
            {
                if (_mousePos.y < mDrawStart)
                {
                    return false;
                }
                CustomTrackMenu();
                return true;
            }
            return false;
        }
        private void CustomTrackMenu()
        {
            GenericMenu _menu = new GenericMenu();

            if (GetAction() != null)
            {
                _menu.AddDisabledItem(new GUIContent("(打断组)" + GetAction().Name));
            }

            _menu.AddItem(new GUIContent("删除轨道"), false, () =>
            {
                ResourcesWindow.Instance.ActionOnChange();
                IsDraw = false;
                mDeleteCallback(this);
            });
            _menu.AddItem(new GUIContent("解组"), false, () =>
            {
                ResourcesWindow.Instance.ActionOnChange();
                //todo 将组内打断轨实例化到当前Action下
            });
            _menu.AddItem(new GUIContent("解组(忽略重复轨道)"), false, () =>
            {
                ResourcesWindow.Instance.ActionOnChange();
                //todo 将组内打断轨实例化到当前Action下
            });
            _menu.ShowAsContext();
        }
        #endregion
    }
}
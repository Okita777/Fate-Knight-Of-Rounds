using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public class InterrupGroupTrackDisplay
    {
        private EditorActionInterrupt mActionInterrupt;
        private TimeLineWindow mWindow;
        private InterrupGroupTrack mInterrupGroupTrack;
        private ActionEngineSetting mEngineSetting;
        private Rect mEventTrackRect;
        private Rect meventRect;
        private Color mColor;
        private MouseActionInfo mMouseActionInfo;

        private EventManipulator mManipulator;

        private bool mDrawHeadManiPulator = false;
        private bool mDrawBodyManiPulator = false;
        private bool mDrawFootManiPulator = false;
        private bool mChange_move = false;
        private bool mChange_hide = false;
        private bool mIsDraw = false;
        private int mTriggerTime = 0;
        private float mStartPos;
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

        public bool Change_hide => mChange_hide;
        public int OffsetTime
        {
            get
            {
                string _name = mActionInterrupt.ActionName;
                if (mInterrupGroupTrack.InterrupOffset.TryGetValue(_name, out int _time))
                {
                    return _time;
                }
                return 0;
            }
        }
        public InterrupGroupTrackDisplay(EditorActionInterrupt _ActionInterrupt, Color _Color,
            InterrupGroupTrack _mInterrupGroupTrack)
        {
            mWindow = TimeLineWindow.Instance;
            mActionInterrupt = _ActionInterrupt;
            mColor = _Color;
            mInterrupGroupTrack = _mInterrupGroupTrack;
            mEngineSetting = mWindow.EngineSetting;
            mMouseActionInfo = new MouseActionInfo(OnConTextClick, MotionEngineConst.Priority_EventTrack);
            mManipulator = new EventManipulator(OnDrage,OnDrageInit);
        }

        public void Draw(Rect _rect)
        {
            mEventTrackRect = _rect;
            if (mActionInterrupt == null) return;
            
            //检查修改内容
            if (string.IsNullOrEmpty(mActionInterrupt.ActionName))
            {
                mChange_hide = false;
                mChange_move = false;
            }
            else
            {
                mChange_hide = mInterrupGroupTrack.InterrupLock.Contains(mActionInterrupt.ActionName);
                mChange_move = mInterrupGroupTrack.InterrupOffset.ContainsKey(mActionInterrupt.ActionName);
            }

            int _trigerOffset = mInterrupGroupTrack.ActionOffset;
            if (mChange_move)
            {
                _trigerOffset += mInterrupGroupTrack.InterrupOffset[mActionInterrupt.ActionName];
            }

            mTriggerTime = mActionInterrupt.TriggerTime + _trigerOffset;
            mStartPos = mWindow.GetPosToTime(mTriggerTime);
            float _endPos = mStartPos + MotionEngineConst.GUI_EventTrackMinWidth;
            if (mActionInterrupt.Duration != 0)
            {
                //在真实末端长度大于最小长度时才应用真实长度
                float _realEndPos = mWindow.GetPosToTime(mTriggerTime + mActionInterrupt.Duration );
                if (_realEndPos - mStartPos > MotionEngineConst.GUI_EventTrackMinWidth)
                {
                    _endPos = _realEndPos;
                }
            }
            
            //绘制操作记录回调
            float _callBackIconInterval = _rect.height - 5f;
            Rect _callBackIcon = new Rect(_rect);
            _callBackIcon.width = _callBackIconInterval;
            _callBackIcon.x -= MotionEngineConst.GUI_EventTrackIconSize * 1.5f;
            GUI.Label(_callBackIcon,mEngineSetting.ExGUI.eventTrackIcon_Child);
            //绘制【已忽视】图标
            if (mChange_hide)
            {
                _callBackIcon.x -= _callBackIconInterval;
                if (GUI.Button(_callBackIcon, ""))
                {
                    mInterrupGroupTrack.InterrupLock.Remove(mActionInterrupt.ActionName);
                    ResourcesWindow.Instance.ActionOnChange();

                }
                GUI.Label(_callBackIcon, mEngineSetting.ExGUI.eventTrackIcon_IsLock);
            }
            //绘制【触发帧已被修改】图标
            if (mChange_move)
            {
                mChange_move = mInterrupGroupTrack.InterrupOffset[mActionInterrupt.ActionName] != 0;
                if (mChange_move)
                {
                    _callBackIcon.x -= _callBackIconInterval;
                    if (GUI.Button(_callBackIcon, ""))
                    {
                        mInterrupGroupTrack.InterrupOffset.Remove(mActionInterrupt.ActionName);
                        ResourcesWindow.Instance.ActionOnChange();

                    }
                    GUI.Label(_callBackIcon, mEngineSetting.ExGUI.eventTrackIcon_IsMove);
                }
            }
            
            bool _isDrawHead = mStartPos < _rect.x + _rect.width;
            bool _isDrawFoot = mActionInterrupt.Duration < 0 || (_endPos > _rect.x);

            IsDraw = (_isDrawHead && _isDrawFoot);

            if (IsDraw)
            {
                bool _drawHeadManiPulator = mStartPos > _rect.x;
                bool _drawBodyManiPulator = false;
                bool _drawFootManiPulator = false;

                mDrawHeadManiPulator = _drawHeadManiPulator && (mActionInterrupt.Duration != 0);
                mDrawBodyManiPulator = _drawBodyManiPulator;
                mDrawFootManiPulator = _drawFootManiPulator;

                //绘制主身
                meventRect = new Rect(_rect);

                if (_drawHeadManiPulator)
                {
                    meventRect.x = mStartPos;
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
                        _eventWidth = _rect.width + _rect.x - mStartPos;
                    }
                }
                meventRect.width = _eventWidth;

                //绘制跳转区块
                Color _eventTrackColor = mColor;
                Color _color = mWindow.IsSelection(mActionInterrupt) ?
                    mWindow.EngineSetting.colorSelection : _eventTrackColor;
                EditorGUI.DrawRect(meventRect, _color);

                //绘制预输入区块
                float _bodyPos = 0;
                if (mActionInterrupt.ExecuteTime > 0)
                {
                    _bodyPos = mWindow.GetPosToTime(mActionInterrupt.ExecuteTime + mTriggerTime);
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
                string _interrupName = "[打断组] 动画跳转: ";
                string _animaInterrup = mChange_move ? $"<color=#FF0000>{_interrupName}</color>" : _interrupName;
                if (_eventWidth > MotionEngineConst.GUI_EventRenderTitleMinWidth)
                {
                    string _title = _animaInterrup + mActionInterrupt.ActionName +
                                    "\n融合时间：" + mActionInterrupt.CrossFadeTime +
                                    "     剪切时间:" + mActionInterrupt.OffsetTime;
                    GUI.Box(meventRect, _title, mWindow.EngineSetting.fontTrack_Title);
                }
                else
                {
                    Rect _detailedRect = new Rect(meventRect);
                    _detailedRect.x += _detailedRect.width + 5;
                    _detailedRect.width = 300;
                    string _title = _animaInterrup + mActionInterrupt.ActionName +
                                    "\n融合时间：" + mActionInterrupt.CrossFadeTime +
                                    "     剪切时间:" + mActionInterrupt.OffsetTime;
                    GUI.Box(_detailedRect, _title, mWindow.EngineSetting.fontTrack_Detailed);
                }

                //绘制操作柄
                mManipulator.Draw(_rect.x, _rect.y, _rect.height, _rect.width);
            }
        }

        #region 回调
        private void OnLoadEvent()
        {
            mWindow.OnLoadMouseAction(EMouseEvent.MouseDwonRight, mMouseActionInfo);
            mManipulator.IsDraw = true;
        }
        private void UnLoadEvent()
        {
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDwonRight, mMouseActionInfo);
            mManipulator.IsDraw = false;
        }
        
        private bool OnConTextClick(Event _event)
        {
            Vector2 _mousePos = mWindow.MousePosToViewPos(_event.mousePosition);
            if (mEventTrackRect.Contains(_mousePos))
            {
                CustomTrackMenu();
                return true;
            }
            return false;
        }
        private void CustomTrackMenu()
        {
            GenericMenu _menu = new GenericMenu();

            _menu.AddDisabledItem(new GUIContent("子轨道" + mActionInterrupt.ActionName));

            if (mChange_hide)
            {
                _menu.AddItem(new GUIContent("取消忽视"), false, () =>
                {
                    mInterrupGroupTrack.InterrupLock.Remove(mActionInterrupt.ActionName);
                    ResourcesWindow.Instance.ActionOnChange();

                });
            }
            else
            {
                _menu.AddItem(new GUIContent("忽视轨道"), false, () =>
                {
                    mInterrupGroupTrack.InterrupLock.Add(mActionInterrupt.ActionName);
                    ResourcesWindow.Instance.ActionOnChange();

                });  
            }
            
            if (mChange_move)
            {
                _menu.AddItem(new GUIContent("还原轨道位置"), false, () =>
                {
                    mInterrupGroupTrack.InterrupOffset.Remove(mActionInterrupt.ActionName);
                    ResourcesWindow.Instance.ActionOnChange();

                });
            }

            _menu.ShowAsContext();
        }

        private float InitTimePos;
        private void OnDrageInit()
        {
            string _name = mActionInterrupt.ActionName;
            if (string.IsNullOrEmpty(_name))
            {
                return;
            }
            if (!mInterrupGroupTrack.InterrupOffset.ContainsKey(_name))
            {
                mInterrupGroupTrack.InterrupOffset.Add(_name, 0);//mStartPos
            }

            int _triggerTime = mInterrupGroupTrack.ActionOffset + mActionInterrupt.TriggerTime;
            float _trggerPos = mWindow.GetPosToTime(mTriggerTime - _triggerTime);
            InitTimePos = Event.current.mousePosition.x - _trggerPos;
        }
        private void OnDrage(float _posX)
        {
            ResourcesWindow.Instance.ActionOnChange();
            string _name = mActionInterrupt.ActionName;
            if (string.IsNullOrEmpty(_name))
            {
                return;
            }
            int _time = mWindow.GetTimeToPos(_posX - InitTimePos);
            int _minTime = -mActionInterrupt.TriggerTime - mInterrupGroupTrack.ActionOffset - mActionInterrupt.ExecuteTime;
            mInterrupGroupTrack.InterrupOffset[_name] = Mathf.Max(_time, _minTime);
        }
        #endregion
        
    }
}

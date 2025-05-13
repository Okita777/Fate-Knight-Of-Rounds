using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.Editor
{
    public partial class TimeLineWindow
    {
        private Vector2 mSrollPos;
        private Rect mSrollRect2;
        private EventDisplay mAnimEvent = null;

        private Rect mSrollRect
        {
            get { return mSrollRect2; }
            set
            {
                mSrollRect2 = value;
            }
        }
        private float mSrollScopeHeight;
        private float mSrollScopePosY;
        private MouseActionInfo mMouseAction;
        private EventDisplay mEventDisplay;
        
        public List<string> ActionTrackGroupUnfold = new List<string>();
        public List<ActionTrackGroup> mCurEventTrackGroup => SelectActionState.AllEventTrackGroup;
        private void InitScrollView()
        {
            //创建可视化 EventTrackGroup 数据
            if (SelectActionState == null) SelectActionState = new EditorActionState(0, "Preview");
            List<ActionTrackGroup> _CurEventTrackGroup = new List<ActionTrackGroup>();
            foreach (var _trackEvents in EngineSetting.mTrackEvents)
            {
                //尝试从ActionState寻找轨道数据
                bool _isFindGrounp = false;
                if (SelectActionState != null && SelectActionState.AllEventTrackGroup != null)
                {
                    foreach (var _eventTrackGroup in SelectActionState.AllEventTrackGroup)
                    {
                        if (_eventTrackGroup.GroupName == _trackEvents.trackGroupName)
                        {
                            _CurEventTrackGroup.Add(new ActionTrackGroup(
                                EngineSetting,
                                _trackEvents.trackIcon,
                                _trackEvents.trackName,
                                _trackEvents.trackColor,
                                _trackEvents.evenTypes,
                                _eventTrackGroup
                            ));
                            _isFindGrounp = true;
                            break;
                        }
                    }
                }

                //从ActionState中没读取到轨道数据，新建数据
                if (!_isFindGrounp)
                {
                    _CurEventTrackGroup.Add(new ActionTrackGroup(
                        EngineSetting,
                        _trackEvents.trackIcon,
                        _trackEvents.trackName,
                        _trackEvents.trackColor,
                        _trackEvents.evenTypes,
                        null,
                        _trackEvents.trackGroupName
                    ));
                }
            }
            SelectActionState.AllEventTrackGroup = _CurEventTrackGroup;

            mMouseAction = new MouseActionInfo(OnConClick, MotionEngineConst.Priority_BackGround);
        }

        public void InitTrackGroupInfo()
        {
            InitScrollView();
        }

        public void DrawScrollview()
        {
            float _poseY = EditorGUIUtility.singleLineHeight + 27f;
            mSrollRect = new Rect(0, _poseY, position.width, position.height - 20f - _poseY);

            if (mSrollRect.height > 0)
            {
                mSrollScopePosY = mSrollRect.y;
                mManipulator.OnLoadMouseEvent(EMouseEvent.MouseDwonLeft, mMouseAction);
                using (new GUI.GroupScope(mSrollRect))
                {
                    Rect _rollRect = new Rect(mSrollRect);
                    _rollRect.position = Vector2.zero;
                    Rect _viewRect = new Rect(_rollRect);
                    _viewRect.width -= 16f;
                    _viewRect.height = mSrollScopeHeight;
                    mSrollScopeHeight = MotionEngineConst.GUI_AnimEventHeight;
                    using (var _srollScope = new GUI.ScrollViewScope(_rollRect,mSrollPos,_viewRect,false,true))
                    {
                        mSrollPos = _srollScope.scrollPosition;
                        
                        DrawAnimEvents(_rollRect);

                        //定义绘制范围
                        float _drawStart = mSrollPos.y;
                        float _drawEnd   = mSrollRect.height + mSrollPos.y;
            
                        // _drawStart += 60f;
                        // _drawEnd -= 60f;
                        
                        foreach (var VARIABLE in mCurEventTrackGroup)
                        {
                            //超出Rect范围处不绘制
                            bool _isDraw = mSrollScopeHeight < _drawEnd;
            
                            //未进入Rect范围处不绘制
                            if (_isDraw)
                            {
                                float _drawEndPos = (mSrollScopeHeight + VARIABLE.GetHeight());
                                _isDraw = _drawEndPos > _drawStart;
                            }
                            mSrollScopeHeight += VARIABLE.Draw(mSrollScopeHeight, _isDraw, _drawStart, _drawEnd);
                        }
                    }
                }
            }
            Rect _bilibiliIDRect = new Rect(mSrollRect.position.x, mSrollRect.position.y + mSrollRect.height,
                MotionEngineConst.GUI_TimeHeadWindth, 20);
            if (EditorGUI.LinkButton(_bilibiliIDRect, "  作者:Asitir Bug反馈、教程查看点这"))
            {
                Application.OpenURL("https://space.bilibili.com/4671627");
            }
        }

        public Vector2 MousePosToViewPos(Vector2 _mousePos)
        {
            _mousePos.y += (mSrollPos.y - mSrollRect.y);
            return _mousePos;
        }
        public float MousePosToViewPos()
        {
            return (mSrollPos.y - mSrollRect.y);
        }
        private bool OnConClick(Event _event)
        {
            if (mSrollRect.Contains(_event.mousePosition))
            {
                OnSelection(null);
                return true;
            }
            return false;
        }

        //绘制动画轨道
        private void DrawAnimEvents(Rect _rect)
        {
            //绘制轨道
            _rect.x = MotionEngineConst.GUI_TimeHeadWindth;
            _rect.height = MotionEngineConst.GUI_AnimEventHeight - 2f;
            _rect.width -= MotionEngineConst.GUI_SrocllviewWidth + _rect.x;
            EditorGUI.DrawRect(_rect, EngineSetting.colorEventTrack);

            //绘制头部
            Rect _rectHead = new Rect(_rect);
            _rectHead.x = 0;
            _rectHead.width = MotionEngineConst.GUI_TimeHeadWindth;
            EditorGUI.DrawRect(_rectHead, EngineSetting.colorEventTrackGroup);

            //绘制头部描述
            using (new GUIStyleScope(EngineSetting.fontDefault, TextAnchor.MiddleCenter))
            {
                GUI.Box(_rectHead, "AnimationClip", EngineSetting.fontDefault);
            }

            //绘制动画事件
            if (mAnimEvent != null)
            {
                mAnimEvent.Draw(_rect);
            }
        }
    }
}
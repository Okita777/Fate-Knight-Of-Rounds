using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;

namespace AsiActionEngine.Editor
{
    public class EventManipulator
    {
        private MouseActionInfo mHanding_Select;
        private MouseActionInfo mHanding_Move;
        private MouseActionInfo mHanding_UnLoad;

        private Action<float> mGetPosToManipulator;
        private Action mManipulatorInit;
        
        private Rect mHandingRect;
        private bool mIsDraw;
        public bool IsDraw
        {
            get { return mIsDraw; }
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
                    }
                }
            }
        }

        private TimeLineWindow mWindow;

        public EventManipulator(Action<float> _GetPosToManipulator, Action _ManipulatorInit = null)
        {
            mGetPosToManipulator = _GetPosToManipulator;
            mManipulatorInit = _ManipulatorInit;
            mWindow = TimeLineWindow.Instance;

            mHanding_Select = new MouseActionInfo(OnClickEvent, MotionEngineConst.Priority_EventHanding);
            mHanding_Move = new MouseActionInfo(OnDragEvent, MotionEngineConst.Priority_EventHanding);
            mHanding_UnLoad = new MouseActionInfo(OnDragEnd, MotionEngineConst.Priority_EventHanding);
        }

        public void Draw(float _posX, float _posY, float _height,float _width = -1)
        {
            if (!IsDraw || mWindow.OnDragManipulator) return;
            if (_width <= 0)
            {
                _width = MotionEngineConst.GUI_EventDisplayManipulatorWeight;
                mHandingRect = new Rect(_posX - _width * 0.5f, _posY, _width, _height);
            }
            else
            {
                mHandingRect = new Rect(_posX, _posY, _width, _height);
            }

            EditorGUIUtility.AddCursorRect(mHandingRect, MouseCursor.SplitResizeLeftRight);
            // EditorGUI.DrawRect(mHandingRect, Color.black * 0.5f);
        }

        private int mEventEndTime;
        private bool OnClickEvent(Event _event)
        {
            Vector2 _pos = mWindow.MousePosToViewPos(_event.mousePosition);
            if (mHandingRect.Contains(_pos))
            {
                mWindow.OnLoadMouseAction(EMouseEvent.MouseDrag, mHanding_Move);
                mWindow.OnLoadMouseAction(EMouseEvent.MouseUp, mHanding_UnLoad);
                if (mManipulatorInit != null) mManipulatorInit();
                mWindow.OnDragManipulator = true;
                return true;
            }
            return false;
        }
        private bool OnDragEvent(Event _event)
        {
            mGetPosToManipulator(_event.mousePosition.x);
            return true;
        }
        private bool OnDragEnd(Event _event)
        {
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDrag, mHanding_Move);
            mWindow.UnLoadMouseAction(EMouseEvent.MouseUp, mHanding_UnLoad);
            ResourcesWindow.Instance.ActionOnChange();
            mWindow.OnDragManipulator = false;
            return true;
        }

        private void OnLoadEvent()
        {
            mWindow.OnLoadMouseAction(EMouseEvent.MouseDwonLeft, mHanding_Select);
        }
        private void UnLoadEvent()
        {
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDwonLeft, mHanding_Select);
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class TimeLineWindow
    {
        public EPreviweState mPreviweState = EPreviweState.Pause;

        public void DrawPervivewGUI()
        {
            Rect _ManipulatorRect = ManipulatorRect();
            mManipulator.Update(_ManipulatorRect, Event.current);
            EngineSetting.Update();
            Update_ShortcutKey(mTimelineRect);
            DrawSelect();
            DrawShortcutBar();
            DrawScrollview();
            DrawTimeline();
            mManipulator.OnDraw();
        }

        private Rect ManipulatorRect()
        {
            Rect _ManipulatorRect = new Rect(mTimelineRect);
            float _refer = MotionEngineConst.GUI_EventTrackIconSize * 1.5f;
            _ManipulatorRect.x -= _refer;
            _ManipulatorRect.width += _refer;
            return _ManipulatorRect;
        }
    }
}
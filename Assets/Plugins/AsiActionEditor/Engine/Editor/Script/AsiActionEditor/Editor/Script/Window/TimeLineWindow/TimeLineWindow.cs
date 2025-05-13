using System;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class TimeLineWindow : EditorWindow
    {
        #region Instance
        private static TimeLineWindow mInstance;
        public static TimeLineWindow Instance
        {
            get
            {
                if (!mInstance)
                {
                    mInstance = GetWindow<TimeLineWindow>();
                    mInstance.titleContent = new GUIContent("ActionTimeLine");
                }

                return mInstance;
            }
        }
        #endregion

        public float TimeScale
        {
            get { return mPlaySpeed; }
            set { mPlaySpeed = value; }
        }

        public static bool needInit = true;

        public ActionEngineSetting EngineSetting
        {
            get { return ActionWindowMain.EngineSetting; }
        }

        private WindowManipulator mManipulator;
        
        private void OnGUI()
        {

            
            if (!Init())
            {
                return;
            }

            // if (Application.isPlaying)
            // {
            //     
            // }
            // else
            {
                DrawPervivewGUI();
            }
        }
        public WindowManipulator Manipulator
        {
            get { return mManipulator; }
        }

        #region 初始化
        private bool Init()
        {
            if (!needInit) {return true;}
            
            if (!EngineSetting)
            {
                return false;
            }

            if (ActionWindowMain.ActionEditorFuntion is null)
            {
                return false;
            }

            needInit = false;

            mManipulator = new WindowManipulator();
            SelectActionState = null;
            mAnimEvent = null;
            
            InitScrollView();
            InitTimeLine();
            return true;
        }
        #endregion

        #region 注册回调

        public void OnLoadMouseAction(EMouseEvent _eventType, MouseActionInfo _even)
        {
            mManipulator.OnLoadMouseEvent(_eventType, _even);
        }

        public void UnLoadMouseAction(EMouseEvent _eventType, MouseActionInfo _even)
        {
            mManipulator.UnLoadMouseEvent(_eventType, _even);
        }

        public void OnLoadEventTrack(onDrawEventTrack _eventTrack)
        {
            mManipulator.OnLoadEventTrack(_eventTrack);
        }
        public void UnLoadEventTrack(onDrawEventTrack _eventTrack)
        {
            mManipulator.UnLoadEventTrack(_eventTrack);
        }
        #endregion
    }
}
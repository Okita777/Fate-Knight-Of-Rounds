using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class TimeLineWindow
    {
        private float mPlaySpeed = 1.0f;

        private bool mIsLoop = true;
        private void DrawShortcutBar()
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                using (new GUIColorScope(mIsLoop? Color.gray : Color.white))
                {
                    if (GUILayout.Button("Loop", EditorStyles.toolbarButton))
                    {
                        mIsLoop = !mIsLoop;
                    }
                }
  
                if (GUILayout.Button(EngineSetting.ExGUI.setpBack, EditorStyles.toolbarButton))
                {
                    if (mIsPlayGame)
                        SetActionEditorState(true);
                    
                    mPreviweState = EPreviweState.Pause;
                    int _time = Mathf.Max(mTimeViewer.UpFramTime(mNowTime),0);
                    mNowTime = GetTimeToAbsorpyion(_time, true);
                    UpdateTimeToNow();
                }

                bool isPlaying = mPreviweState == EPreviweState.Play;
                GUIContent playStateContent = isPlaying ? EngineSetting.ExGUI.pause : EngineSetting.ExGUI.play;
                if (GUILayout.Button(playStateContent, EditorStyles.toolbarButton))
                {
                    if (mIsPlayGame)
                        SetActionEditorState(true);
                    
                    mPreviweState = isPlaying ? EPreviweState.Pause : EPreviweState.Play;
                }
  
                if (GUILayout.Button(EngineSetting.ExGUI.nextBack, EditorStyles.toolbarButton)) 
                {
                    if (mIsPlayGame)
                        SetActionEditorState(true);
                    
                    mPreviweState = EPreviweState.Pause;
                    int _time = Mathf.Min(mTimeViewer.NextFramTime(mNowTime), mActionTotalTime);
                    mNowTime = GetTimeToAbsorpyion(_time, true);
                    UpdateTimeToNow();
                }
                
                if (GUILayout.Button(EngineSetting.ExGUI.stop, EditorStyles.toolbarButton)) 
                {
                    if (mIsPlayGame)
                        SetActionEditorState(true);
                    
                    mPreviweState = EPreviweState.Pause;
                    UpdateTime(0);
                }

                if (mIsPlayGame && mIsEditor)
                {
                    using (new GUIColorScope(Color.red))
                    {
                        if (GUILayout.Button("返回游戏模式", EditorStyles.toolbarButton))
                        {
                            SetActionEditorState(false);
                        }
                    }
                }
                
                GUILayout.Space(20);
                mPlaySpeed = GUILayout.HorizontalSlider(mPlaySpeed, 0.1f, 2.0f, GUILayout.Width(150));
                mPlaySpeed = (int)(100 * mPlaySpeed);
                mPlaySpeed /= 100;
                GUILayout.Label(mPlaySpeed.ToString("0.00"));
                GUILayout.FlexibleSpace();

                // if (GUILayout.Button(EngineSetting.ExGUI.opinonsIcon, EditorStyles.toolbarButton))
                // {
                //     EditorIcons.EditorIconsOpen();
                // }

                if (EditorGUILayout.DropdownButton(new GUIContent("Draw"), FocusType.Passive,
                        EditorStyles.toolbarDropDown))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Editor时绘制"),ActionWindowMain.ScenceDraw_Editor, () =>
                    {
                        ActionWindowMain.ScenceDraw_Editor = !ActionWindowMain.ScenceDraw_Editor;
                        if (ActionWindowMain.ScenceDraw_Editor) PlayerPrefs.SetInt("ScenceDraw_Editor", 1);
                        else PlayerPrefs.SetInt("ScenceDraw_Editor", 0);
                        PlayerPrefs.Save();
                        TimeLineWindow.Instance.UpdateTimeToNow();
                    });
                    menu.AddItem(new GUIContent("RunTime时绘制"),ActionWindowMain.ScenceDraw_Runtime, () =>
                    {
                        ActionWindowMain.ScenceDraw_Runtime = !ActionWindowMain.ScenceDraw_Runtime;
                        if (ActionWindowMain.ScenceDraw_Runtime) PlayerPrefs.SetInt("ScenceDraw_Runtime", 1);
                        else PlayerPrefs.SetInt("ScenceDraw_Runtime", 0);
                        PlayerPrefs.Save();
                        TimeLineWindow.Instance.UpdateTimeToNow();
                    });

                    if (ActionWindowMain.ScenceDraw_Runtime || ActionWindowMain.ScenceDraw_Editor)
                    {
                        menu.AddItem(new GUIContent("绘制事件轨图形"), ActionWindowMain.ScenceDraw_Event, () =>
                        {
                            ActionWindowMain.ScenceDraw_Event = !ActionWindowMain.ScenceDraw_Event;
                            if (ActionWindowMain.ScenceDraw_Event) PlayerPrefs.SetInt("ScenceDraw_Event", 1);
                            else PlayerPrefs.SetInt("ScenceDraw_Event", 0);
                            PlayerPrefs.Save();
                            TimeLineWindow.Instance.UpdateTimeToNow();
                        });
                        menu.AddItem(new GUIContent("绘制跳转轨图形"), ActionWindowMain.ScenceDraw_Interrupt, () =>
                        {
                            ActionWindowMain.ScenceDraw_Interrupt = !ActionWindowMain.ScenceDraw_Interrupt;
                            if (ActionWindowMain.ScenceDraw_Interrupt) PlayerPrefs.SetInt("ScenceDraw_Interrupt", 1);
                            else PlayerPrefs.SetInt("ScenceDraw_Interrupt", 0);
                            PlayerPrefs.Save();
                            TimeLineWindow.Instance.UpdateTimeToNow();
                        });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("绘制事件轨图形"));
                        menu.AddDisabledItem(new GUIContent("绘制跳转轨图形"));
                    }

                    menu.ShowAsContext();
                }
                
                if (GUILayout.Button(EngineSetting.ExGUI.InputModule, EditorStyles.toolbarButton))
                {
                    InputModuleWindow.OpenWindow();
                }
                GUIContent _timeLock = mTimeViewer.TimeAbsorpyion
                    ? EngineSetting.ExGUI.lockIcon_On
                    : EngineSetting.ExGUI.lockIcon_Off;
                if (GUILayout.Button(_timeLock, EditorStyles.toolbarButton))
                {
                    mTimeViewer.SetTimeAbsorpyion(!mTimeViewer.TimeAbsorpyion);
                }
                if (GUILayout.Button(EngineSetting.ExGUI.opinonsEngineColor, EditorStyles.toolbarButton))
                {
                    Selection.activeObject = EngineSetting;
                }
                if (EditorGUILayout.DropdownButton(EngineSetting.ExGUI.opinons, FocusType.Passive,
                        EditorStyles.toolbarDropDown))
                {
                    GenericMenu menu = new GenericMenu();
                    // menu.AddDisabledItem(new GUIContent("帧率"));
                    menu.AddItem(new GUIContent("帧率/60"),mTimeViewer.FrameRate == 60, () =>
                    {
                        mTimeViewer.ChangeRate(60);
                    });
                    menu.AddItem(new GUIContent("帧率/30"),mTimeViewer.FrameRate == 30, () =>
                    {
                        mTimeViewer.ChangeRate(30);
                    });
                    // menu.AddItem(new GUIContent("15"),mTimeViewer.FrameRate == 15, () =>
                    // {
                    //     mTimeViewer.ChangeRate(15);
                    // });
                    menu.AddItem(new GUIContent("切换Action时保持播放"), mChangeKeep, () =>
                    {
                        // mTimeViewer.ChangeRate(15);
                        mChangeKeep = !mChangeKeep;
                    });
                    menu.ShowAsContext();
                }
            }
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public class GUIStyleExtension
    {
        //快捷菜单栏
        public GUIContent setpBack { get; private set; }
        public GUIContent nextBack { get; private set; }
        public GUIContent play { get; private set; }
        public GUIContent loop { get; private set; }
        public GUIContent pause { get; private set; }
        public GUIContent stop { get; private set; }
        public GUIContent opinons { get; private set; }
        public GUIContent opinonsIcon { get; private set; }
        public GUIContent opinonsEngineColor { get; private set; }
        public GUIContent lockIcon_On { get; private set; }
        public GUIContent lockIcon_Off { get; private set; }
        public GUIContent InputModule { get; private set; }

        public GUIContent eventGroupHeaderOff { get; private set; }
        public GUIContent eventGroupHeaderOpen { get; private set; }
        public GUIContent eventGroupHeaderCreact { get; private set; }
        
        public GUIContent eventTrackIcon_Anima { get; private set; }
        public GUIContent eventTrackIcon_Interruo { get; private set; }
        public GUIContent eventTrackIcon_InterrupTarck { get; private set; }
        public GUIContent eventTrackIcon_Child { get; private set; }
        public GUIContent eventTrackIcon_IsMove { get; private set; }
        public GUIContent eventTrackIcon_IsLock { get; private set; }

        public GUIContent eventTrackIcon_Preview { get; private set; }
        public GUIContent eventTrackIcon_PreviewOff { get; private set; }
        public GUIContent interrupGroupTrack_Onfold { get; private set; }
        public GUIContent interrupGroupTrack_Unfold { get; private set; }



        public void Init()
        {
            setpBack = EditorGUIUtility.IconContent("Animation.PrevKey", "上一帧");
            nextBack = EditorGUIUtility.IconContent("Animation.NextKey", "下一帧");
            play = EditorGUIUtility.IconContent("Animation.Play", "播放");
            pause = EditorGUIUtility.IconContent("d_PauseButton", "暂停");
            loop = EditorGUIUtility.IconContent("d_RotateTool", "循环播放");
            stop = EditorGUIUtility.IconContent("d_PreMatQuad", "停止");
            opinons = EditorGUIUtility.IconContent("_Popup", "设置");
            opinonsIcon = EditorGUIUtility.IconContent("d_GridLayoutGroup Icon", "图标列表");
            opinonsEngineColor = EditorGUIUtility.IconContent("d_ColorPicker.CycleSlider", "设置动编颜色");
            lockIcon_On =  EditorGUIUtility.IconContent("IN LockButton on@2x", "开启帧吸附");
            lockIcon_Off =  EditorGUIUtility.IconContent("IN LockButton@2x", "关闭帧吸附");
            InputModule = EditorGUIUtility.IconContent("d_StandaloneInputModule Icon", "输入系统");

            eventGroupHeaderOff = EditorGUIUtility.IconContent("d_scrollright", "关闭组列表");
            eventGroupHeaderOpen = EditorGUIUtility.IconContent("d_scrolldown", "展开组列表");
            eventGroupHeaderCreact = EditorGUIUtility.IconContent("d_CreateAddNew", "创建事件轨道");

            eventTrackIcon_Anima = EditorGUIUtility.IconContent("AnimationClip Icon", "动画轨");
            eventTrackIcon_Interruo = EditorGUIUtility.IconContent("BlendTree Icon", "打断轨组");
            eventTrackIcon_InterrupTarck = EditorGUIUtility.IconContent("Animator Icon", "打断轨组子级");
            eventTrackIcon_Child = EditorGUIUtility.IconContent("NetworkTransformChild Icon", "打断轨组子级2");
            eventTrackIcon_IsMove = EditorGUIUtility.IconContent("d_Grid.MoveTool", "打断轨组子级位移");
            eventTrackIcon_IsLock = EditorGUIUtility.IconContent("d_SceneViewVisibility", "打断轨组子级锁定");
            eventTrackIcon_Preview = EditorGUIUtility.IconContent("d_scenevis_visible_hover", "预览");
            eventTrackIcon_PreviewOff = EditorGUIUtility.IconContent("d_SceneViewVisibility", "关闭预览");

            interrupGroupTrack_Unfold = EditorGUIUtility.IconContent("SpriteRenderer Icon", "关闭预览");
            interrupGroupTrack_Onfold = EditorGUIUtility.IconContent("Sprite Icon", "开启预览");//RectTransform Icon
        }
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using AsiActionEngine.RunTime;
using UnityEngine.Serialization;

namespace AsiActionEngine.Editor
{
    [CreateAssetMenu(fileName = "WindowSetting", menuName = "AsiMotionEngine/Settings", order = 3)]
    public class ActionEngineSetting : ScriptableObject
    {
        public ActionEngineSetting()
        {
            if (mTrackEvents.Count < 1)
            {
                mTrackEvents.Add(new TrackEvents("镜头组","镜头轨道"));
                mTrackEvents.Add(new TrackEvents("攻击盒","攻击轨道"));
                mTrackEvents.Add(new TrackEvents("特效组","特效轨道"));
                mTrackEvents.Add(new TrackEvents("音效组","音效轨道"));
                mTrackEvents.Add(new TrackEvents("动作组","动作轨道"));
                mTrackEvents.Add(new TrackEvents("单位事件","事件"));
            }
        }
        [Header("功能色")]
        public Color colorWhite = Color.white;
        // public Color colorGroup;
        // public Color colorBlack;
        public Color colorSelection = Color.white;

        [Header("字体颜色")] 
        public Color colorFontDefault = Color.white;
        public Color colorFontTrack_Title = Color.black;
        public Color colorFontTrack_Detailed = Color.white;

        
         // 功能色BoxGroup
        // [Header("功能色")]
        // public Color colorPreInput;

        // 轨道通用功能色
        [Header("轨道通用功能色")]
        public Color colorTimeLineControl = new Color(1,0.6f,0,0.6f);
        public Color colorEventTrackGroup = new Color(0.16f,0.16f,0.16f,1);
        public Color colorEventTrack = new Color(1,1,1,0.027f);
        public Color colorEventTrackNull = new Color(0.19f,0.19f,0.19f,1);

        [HideInInspector]
        public GUIStyle fontDefault;
        [HideInInspector]
        public GUIStyle fontTrack_Title;
        [HideInInspector]
        public GUIStyle fontTrack_Detailed;
        
        public GUIStyleExtension ExGUI = new GUIStyleExtension();
        // public TrackDictionary trackGroupEvents = new TrackDictionary();
        public List<TrackEvents> mTrackEvents = new List<TrackEvents>();
        public void Init()
        {
            ExGUI.Init();
            InitValue();
        }

        public void Update()
        {
            UpdateValue();
        }

        private void InitValue()
        {
            fontDefault = new GUIStyle();
            fontTrack_Title = new GUIStyle();
            fontTrack_Detailed = new GUIStyle();

            fontDefault.alignment = TextAnchor.MiddleLeft;
            fontTrack_Title.alignment = TextAnchor.MiddleCenter;
            fontTrack_Detailed.alignment = TextAnchor.MiddleLeft;
            
            fontTrack_Title.fontSize = 10;
            fontTrack_Detailed.fontSize = 10;
            
            
        }

        private void UpdateValue()
        {
            fontDefault.normal.textColor = colorFontDefault;
            fontTrack_Title.normal.textColor = colorFontTrack_Title;
            fontTrack_Detailed.normal.textColor = colorFontTrack_Detailed;
        }
    }
}
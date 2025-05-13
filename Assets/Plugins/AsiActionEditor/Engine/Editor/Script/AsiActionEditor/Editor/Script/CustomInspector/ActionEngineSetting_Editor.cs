using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AsiActionEngine.RunTime;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UIElements;

namespace AsiActionEngine.Editor
{

    [CustomEditor(typeof(ActionEngineSetting))]
    public class ActionEngineSetting_Editor : UnityEditor.Editor
    {
        private int mDisplayType;
        private bool isChange = false;
        private bool isInit = true;
        private ActionEngineSetting main => target as ActionEngineSetting;
        private ReorderableList mReorderableList;
        private List<bool> mTrackStates;

        public bool IsInit => isInit;

        private void OnEnable()
        {
            isInit = true;
        }

        private void OnInit()
        {
            if(!isInit)return;
            main.Init();
            mTrackStates = new List<bool>();
            for (int i = 0; i < main.mTrackEvents.Count; i++)
                mTrackStates.Add(false);
            mEngineSetting = ActionWindowMain.EngineSetting;
            // mTracks = main.trackGroupEvents.Keys.ToList();
            mReorderableList = new ReorderableList(main.mTrackEvents, null, true, true, true, true);
            mReorderableList.drawElementCallback = DrawListBody;
            mReorderableList.drawHeaderCallback = DrawListHead;
            mReorderableList.elementHeightCallback = GetElementHeight;
            isInit = false;
        }

        public override void OnInspectorGUI()
        {
            OnInit();

            mDisplayType = GUILayout.Toolbar(mDisplayType, new[] { "基本外观设置", "轨道组设定" });

            if (mDisplayType == 0)
                DrawAppearance();
            else if(mDisplayType == 1)
                DrawTranckGroup();
            
            if (GUILayout.Button("保存配置"))
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);

                if (!TimeLineWindow.needInit)
                    TimeLineWindow.Instance.Manipulator.Clear();
                TimeLineWindow.Instance.InitTrackGroupInfo();
                ActionWindowMain.UpdateMainWindow();
            }
        }

        private void DrawAppearance()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                ActionWindowMain.UpdateMainWindow();
                // EditorUtility.SetDirty(target);
                isChange = true;
            }
        }


        private Color trackUnfold = Color.green;
        private Color trackOnfold = Color.blue;
        private ActionEngineSetting mEngineSetting;
        private void DrawTranckGroup()
        {
            mReorderableList.DoLayoutList();


        }

        private void DrawListHead(Rect rect)
        {
            rect.height += 2;
            rect.y -= 2;
            GUI.Box(rect, "动作编辑器轨道设置");
        }
        private void DrawListBody(Rect _rect, int _index, bool _active, bool _focused)
        {
            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                main.mTrackEvents[_index].Draw(_rect, _index);

                if (_check.changed)
                {
                    EditorUtility.SetDirty(main);
                }
            }
        }

        private float GetElementHeight(int _index)
        {
            return main.mTrackEvents[_index].GetHeight();
        }
        
    }
}

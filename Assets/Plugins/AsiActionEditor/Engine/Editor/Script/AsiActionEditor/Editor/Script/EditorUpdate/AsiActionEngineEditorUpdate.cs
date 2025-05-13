using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using UnityEditor.ShortcutManagement;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public struct AsiActionEventSelectID
    {
        public int type;
        public int actionID;
        public int trackGroupID;
        public int trackID;
        public int eventID;

        public AsiActionEventSelectID(int type, int actionID, int trackGroupID, int trackID, int eventID)
        {
            this.type = type;
            this.actionID = actionID;
            this.trackGroupID = trackGroupID;
            this.trackID = trackID;
            this.eventID = eventID;
        }

        public AsiActionEventSelectID Clone()
        {
            return new AsiActionEventSelectID(type, actionID, trackGroupID, trackID, eventID);
        }

        public bool SelectActionEvent()
        {
            // Debug.LogWarning("type: " + type);
            if (type == 1)
            {
                Debug.LogWarning("暂未实现 Unit 属性的撤销显示，请静待群主更新");
                return true;
            }else if (type == 2)
            {
                Debug.LogWarning("暂未实现 Item 属性的撤销显示，请静待群主更新");
                return true;

            }
            if (type == 3)
            {
                TimeLineWindow.Instance.Repaint();
                ResourcesWindow.Instance.OnSelectActionState(actionID, false, false);
                ResourcesWindow.Instance.Repaint();
                // Debug.LogWarning("暂未实现 Unit 属性的撤销显示，请静待群主更新");
                return true;

            }else if (type == 4)
            {
                //事件
                EditorActionState _actionState = ResourcesWindow.Instance.GetEditorActionStateToSelect();
                if (_actionState.AllEventTrackGroup.Count > trackGroupID)
                {
                    ActionTrackGroup _actionTrackGroup = _actionState.AllEventTrackGroup[trackGroupID];
                    if (_actionTrackGroup.CurActiontTrack.Count > trackID)
                    {
                        IActionTrack _actionTrack = _actionTrackGroup.CurActiontTrack[trackID];
                        if (_actionTrack is EventTrack _eventTrack)
                        {
                            if (_eventTrack.CurEventDisplay.Count > eventID)
                            {
                                TimeLineWindow.Instance.OnSelection(_eventTrack.CurEventDisplay[eventID].MainEvent,false);
                                TimeLineWindow.Instance.Repaint();
                                return true;
                            }
                        }
                    }
                }

            }else if (type == 5)
            {
                //打断轨
                EditorActionState _actionState = ResourcesWindow.Instance.GetEditorActionStateToSelect();
                if (_actionState.AllEventTrackGroup.Count > trackGroupID)
                {
                    ActionTrackGroup _actionTrackGroup = _actionState.AllEventTrackGroup[trackGroupID];
                    if (_actionTrackGroup.CurActiontTrack.Count > trackID)
                    {
                        IActionTrack _actionTrack = _actionTrackGroup.CurActiontTrack[trackID];
                        if (_actionTrack is EventTrack _eventTrack)
                        {
                            if (_eventTrack.CurInterrup.Count > eventID)
                            {
                                TimeLineWindow.Instance.OnSelection(_eventTrack.CurInterrup[eventID].ActionInterrupt,false);
                                TimeLineWindow.Instance.Repaint();
                                return true;
                            }
                        }

                    }

                }

            }else if (type == 6)
            {
                Debug.LogWarning("暂未实现 Camera 属性的撤销显示，请静待群主更新");
                return true;

            }else if (type == 7)
            {
                Debug.LogWarning("暂未实现 GValue 属性的撤销显示，请静待群主更新");
                return true;

            }
            return false;
        }
    }
    [InitializeOnLoad]
    public class AsiActionEngineEditorUpdate : AssetModificationProcessor
    {
        #region SaveCallBack
        //保存场景时触发
        static public void OnWillSaveAssets(string[]names)
        {
            if (ResourcesWindow.Instance.mSelectmenuID == 0)
            {
                //保存单位窗口
                ResourcesWindow.Instance.SaveUnitInfo();
            }else if (ResourcesWindow.Instance.mSelectmenuID == 1)
            {
                //保存ActionState窗口
                ResourcesWindow.Instance.SaveActionState();

            }else if (ResourcesWindow.Instance.mSelectmenuID == 2)
            {
                //保存GValue窗口
                ResourcesWindow.Instance.SaveGValueInfo();
            }else if (ResourcesWindow.Instance.mSelectmenuID == 3)
            {
                //保存子弹窗口
            }else if (ResourcesWindow.Instance.mSelectmenuID == 4)
            {
                //保存道具窗口
            }else if (ResourcesWindow.Instance.mSelectmenuID == 5)
            {
                //保存相机窗口
                ResourcesWindow.Instance.SaveCameraWarp();
            }
        }

        #endregion

        #region Property
        public static EditorEgineActionStates mEditorEgineActionStates;//SO对象
        private static int mActionStateInputID = 0;//操作ID，用来检测用户是否有撤销操作
        private static string mUndoName = "";
        private static double mUndoTime = 0;
        private static bool mLastOnSelect = false;
        // private static bool mIsUpdateTimeSelect = false;
        #endregion

        #region Init
        static AsiActionEngineEditorUpdate()
        {
            //todo: 暂时关闭了撤销
            return;
            mUndoTime = -1;
            //创建撤销
            if (!File.Exists(MotionEngineConst.EditorActionStateUndo))
            {
                ScriptableObject _action = ScriptableObject.CreateInstance(typeof(EditorEgineActionStates));//MotionEngineConst.EditorActionStateUndo,
                _action.name = "AsiActionStateUndo";
                AssetDatabase.CreateAsset(_action, MotionEngineConst.EditorActionStateUndo);
                mEditorEgineActionStates = _action as EditorEgineActionStates;
                mEditorEgineActionStates.mInputID = 0;
                mActionStateInputID = 0;
            }
            else
            {
                mEditorEgineActionStates = AssetDatabase.LoadAssetAtPath<EditorEgineActionStates>(MotionEngineConst.EditorActionStateUndo);
                if (mEditorEgineActionStates == null)
                {
                    ScriptableObject _action = ScriptableObject.CreateInstance(typeof(EditorEgineActionStates));//MotionEngineConst.EditorActionStateUndo,
                    _action.name = "AsiActionStateUndo";
                    AssetDatabase.CreateAsset(_action, MotionEngineConst.EditorActionStateUndo);
                    mEditorEgineActionStates = _action as EditorEgineActionStates;
                    mEditorEgineActionStates.mInputID = 0;
                    mActionStateInputID = 0;
                    // Debug.LogError("读取失败"); 
                    // return;
                }
                else
                {
                    mEditorEgineActionStates.mInputID = 0;
                    mActionStateInputID = 0;
                }
            }
            EditorApplication.update += Update; 
        }
        #endregion

        #region Update
        private static void Update()
        {

            if (mActionStateInputID != mEditorEgineActionStates.mInputID)
            {
                // if (mActionStateInputID > mEditorEgineActionStates.mInputID)
                // {
                //     Debug.Log("撤销了"); 
                //     // UpdateActionState();
                //
                // }
                // else
                // {
                //     Debug.Log("取消撤销了");
                //     // UpdateActionState();
                // }
                mActionStateInputID = mEditorEgineActionStates.mInputID;

                if (ResourcesWindow.Instance.ActionGroupName ==
                    mEditorEgineActionStates.mEditorActionStateInfo.mActionGroupName)
                {
                    if (mEditorEgineActionStates.mOnlySelected && mLastOnSelect)
                    {
                        // Debug.Log("回到上一个选择");
                        mEditorEgineActionStates.mAsiActionEventSelectID.SelectActionEvent();
                    }
                    else
                    {
                        // Debug.Log("回到上一组数据");
                        UpdateActionState();
                    }

                    mLastOnSelect = mEditorEgineActionStates.mOnlySelected;
                }
                else
                {
                    //无效撤销
                }
            }

            if (mUndoTime > 0)
            {
                if (EditorApplication.timeSinceStartup - mUndoTime > 0.6f)
                {
                    mUndoTime = -1;
                    UpdateActionUndo();
                    // Debug.Log("打印");
                }
            }
        }
        #endregion

        private static void UpdateActionState()
        {
            // List<EditorActionState> _actionStates =
            //     ResourcesWindow.Instance.mActionState = new List<EditorActionState>();
            // ResourcesWindow.Instance.ActionIsChange = true;
            // foreach (var VARIABLE in mEditorEgineActionStates.mEditorActionStateInfo.mActionState)
            // {
            //     _actionStates.Add(VARIABLE.Clone(VARIABLE.ID, VARIABLE.Name));
            // }
            // ResourcesWindow.Instance.mActionState = _actionStates;
            ResourcesWindow.Instance.LoadActionState(mEditorEgineActionStates.mEditorActionStateInfo.Clone(),
                mEditorEgineActionStates.mEditorActionStateInfo.mActionGroupName);
            // ResourcesWindow.Instance.UpdateActionStateDic();
            ResourcesWindow.Instance.OnSelectActionState(mEditorEgineActionStates.mAsiActionEventSelectID.actionID,
                false, false);
            if (!mEditorEgineActionStates.mAsiActionEventSelectID.SelectActionEvent())
            {
                Debug.LogWarning("选择失败");
            }
        }

        public static void UpdateActionSelectID(int _id)
        {
            //todo: 暂时关闭了撤销
            return;
            if (_id != mEditorEgineActionStates.mAsiActionEventSelectID.actionID)
            {
                Undo.RecordObject(mEditorEgineActionStates, "ActionEngin/ActionStateInfo SelectAction"); 
                AsiActionEventSelectID _SelectID =new AsiActionEventSelectID(3,_id,0,0,0);
                mEditorEgineActionStates.mOnlySelected = true;
                mLastOnSelect = true;
                mEditorEgineActionStates.mAsiActionEventSelectID = _SelectID;
                MarkUndo();
                EditorUtility.SetDirty(mEditorEgineActionStates);
            }
        }

        public static void ChangeSelectActionEvent(AsiActionEventSelectID _selectID)
        {
            //todo: 暂时关闭了撤销
            return;
            // Debug.Log("写入Type：" + _selectID.type);

            Undo.RecordObject(mEditorEgineActionStates, "ActionEngin/ActionStateInfo SelectActionEvent");
            mEditorEgineActionStates.mOnlySelected = true;
            mLastOnSelect = true;
            MarkUndo();
            mEditorEgineActionStates.mAsiActionEventSelectID = _selectID;
            EditorUtility.SetDirty(mEditorEgineActionStates);
        }

        private static void MarkUndo()
        {
            mEditorEgineActionStates.mInputID++;
            mActionStateInputID = mEditorEgineActionStates.mInputID;
        }

        public static void SetActionUndo(string _undoName = "")
        {
            //延迟记录  避免短期多次注册
            mUndoTime = EditorApplication.timeSinceStartup;
            mUndoName = _undoName;
        }

        private static void UpdateActionUndo()
        {
            Undo.RecordObject(mEditorEgineActionStates, "ActionEngin/ActionStateInfo  " + mUndoName);
            mEditorEgineActionStates.mOnlySelected = false;
            mLastOnSelect = false;
            mEditorEgineActionStates.mEditorActionStateInfo = ResourcesWindow.Instance.CloneEditorActionStateInfo();
            // ScenceDraw.Instance.OnUpdateDraw();
            MarkUndo();
            EditorUtility.SetDirty(mEditorEgineActionStates);
        }
    }
}
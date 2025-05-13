using System;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class TimeLineWindow
    {
        private IProperty mSelection = null;
        private bool mOnDragManipulator = false;

        public EditorActionState SelectActionState { get; private set; }

        public bool OnDragManipulator
        {
            get { return mOnDragManipulator; }
            set { mOnDragManipulator = value; }
        }

        public void OnSelection(IProperty _target, bool _isMarkUndo = true)
        {
            mSelection = _target;
            if (_target == null)
            {
                var _actionState = ResourcesWindow.Instance.AllActionState;
                int _id = ResourcesWindow.Instance.mOnSelectActionStateID;
                if (_actionState != null && _id < _actionState.Count)
                {
                    InspectorWindow.Instance.SelectProperty(_actionState[_id],_isMarkUndo);
                }
                else
                {
                    InspectorWindow.Instance.SelectProperty(null,_isMarkUndo); 
                }
            }
            else
            {
                InspectorWindow.Instance.SelectProperty(_target,_isMarkUndo);
            }

            ScenceDraw.Instance.OnUpdateDraw();
            mOnDragManipulator = false;
        }

        public bool IsSelection(IProperty _target)
        {
            return mSelection != null && mSelection == _target;
        }

        public void OnChangeAction(EditorActionState _action, bool _isPlay = true)
        {
            if (_action.EditorAnimEvent != null && _action.EditorAnimEvent.EventData != null)
            {
                if (SelectActionState == _action)
                {
                    if (_isPlay)
                        mPreviweState = mPreviweState == EPreviweState.Play ? EPreviweState.Pause : EPreviweState.Play;
                }
                else
                {
                    SelectActionState = _action;
                    InitTimeLine();
                }
            }
            else
            {
                if (_action.EditorAnimEvent != null && _action.EditorAnimEvent.EventData == null)
                {
                    _action.EditorAnimEvent = null;
                } //数据加载时 EditorAnimEvent 被强制序列化导致数据错误  故此做判断规避

                SelectActionState = _action;
                InitTimeLine();
            }

            InitScrollView();
            mManipulator = new WindowManipulator();
        }

        private void DrawSelect()
        {
            if (mOnDragManipulator)
            {
                EditorGUIUtility.AddCursorRect(TimelineRect, MouseCursor.SplitResizeLeftRight);
            }
        }
    }
}
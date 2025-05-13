using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public class ActionTrackGroup
    {
        [SerializeField] private string mGroupName;
        [SerializeReference] private List<IActionTrack> mCurActiontTrack = new List<IActionTrack>();
        
        [NonSerialized] private ActionEngineSetting mEngineSetting;
        [NonSerialized] private TimeLineWindow mWindow;
        [NonSerialized] private ReorderableList mEventList;
        private string mTrackName;
        private float mDrawStart;
        private float mDrawEnd;
        private Color mColor;
        private Rect mButtonRect;
        private GUIContent mIcon;
        private MouseActionInfo mCreateEventTrack;
        private MouseActionInfo mCreateOtherTrack;
        private List<string> mEventTypes;
        private bool listChange = false;

        private bool mIsOpen
        {
            get
            {
                return mWindow.ActionTrackGroupUnfold.Contains(mGroupName);
            }
            set
            {
                
                if (value)
                {
                    if (!mWindow.ActionTrackGroupUnfold.Contains(mGroupName))
                    {
                        mWindow.ActionTrackGroupUnfold.Add(mGroupName);
                    }    
                }
                else
                {
                    if (mWindow.ActionTrackGroupUnfold.Contains(mGroupName))
                    {
                        mWindow.ActionTrackGroupUnfold.Remove(mGroupName);
                    }  
                }
            }
        }

        private bool mIsDraw = false;
        public string GroupName => mGroupName;
        public List<IActionTrack> CurActiontTrack => mCurActiontTrack;


        public float Draw(float _rollScrollHeight, bool _isDraw, float _DrawStart, float _DrawEnd)
        {
            mDrawStart = _DrawStart;
            mDrawEnd = _DrawEnd;
            //float _CurEventTrackHeight = mIsOpen ? (mCurActiontTrack.Count * MotionEngineConst.GUI_EventGroupHeight) : 0;
            float _CurEventTrackHeight = 0;
            if (mIsOpen)
            {
                foreach (var item in mCurActiontTrack)
                {
                    _CurEventTrackHeight += item.GetHeight();
                }
            }
            if (_isDraw)
            {
                if (!mIsDraw)
                {
                    OnLoadCallBack();

                    mIsDraw = true;
                }
                //绘制头部 -> 下拉图标
                Rect _iconRect = new Rect(0, _rollScrollHeight, MotionEngineConst.GUI_EventGroupHeaderHeight,
                    MotionEngineConst.GUI_EventGroupHeaderHeight);
                GUIContent _groupHeaderGUI =
                    mIsOpen ? mEngineSetting.ExGUI.eventGroupHeaderOpen : mEngineSetting.ExGUI.eventGroupHeaderOff;
                GUI.Label(_iconRect, _groupHeaderGUI,mEngineSetting.fontDefault);

                //绘制头部 -> 下拉触发按钮
                Rect _groupRect = new Rect(0, _rollScrollHeight, MotionEngineConst.GUI_TimeHeadWindth - _iconRect.width,
                    MotionEngineConst.GUI_EventGroupHeaderHeight);
                if (GUI.Button(_groupRect, "", EditorStyles.label))
                {
                    mIsOpen = !mIsOpen;
                    if (!mIsOpen)
                    {
                        foreach (var VARIABLE in mCurActiontTrack)
                            VARIABLE.IsDraw = false;
                    }
                }

                //绘制头部 -> 颜色块
                _groupRect.x += _iconRect.width;
                Rect _GroupRect = new Rect(_groupRect.x, _groupRect.y, _groupRect.width,
                    _groupRect.height + _CurEventTrackHeight);
                EditorGUI.DrawRect(_GroupRect, mEngineSetting.colorEventTrackGroup);

                //绘制头部 -> 轨道组名
                using (new GUIColorScope(mEngineSetting.colorWhite))
                {
                    GUI.Label(_groupRect, mGroupName + $"  <color=#FFF100>{mEventList.count}</color>", mEngineSetting.fontDefault);
                }

                //绘制头部 -> 轨道添加按钮
                _iconRect.x = _groupRect.width;
                _iconRect.y += 2f;
                mButtonRect = _iconRect;
                GUI.Button(mButtonRect, mEngineSetting.ExGUI.eventGroupHeaderCreact, EditorStyles.iconButton);

                //展开轨道组
                if (mIsOpen)
                {
                    _GroupRect.x = 0;
                    _GroupRect.y += MotionEngineConst.GUI_EventGroupHeaderHeight - 7f;
                    _GroupRect.height -= MotionEngineConst.GUI_EventGroupHeaderHeight;
                    _GroupRect.width = TimeLineWindow.Instance.position.width;
                    mEventList.DoList(_GroupRect);
                    if (listChange)
                    {
                        mEventList.list = mCurActiontTrack;
                        listChange = false;
                    }
                }
            }
            else
            {
                if (mIsDraw)
                {
                    UnLoadCallBack();
                    mIsDraw = false;
                }
            }
            return MotionEngineConst.GUI_EventGroupHeaderHeight + MotionEngineConst.GUI_EventGroupHeaderInterval + _CurEventTrackHeight;
        }

        public float GetHeight()
        {
            float _CurEventTrackHeight =  0;
            if (mIsOpen)
            {
                foreach (var _actionTrack in mCurActiontTrack)
                {
                    _CurEventTrackHeight += _actionTrack.GetHeight();
                }
            }
            return MotionEngineConst.GUI_EventGroupHeaderHeight + MotionEngineConst.GUI_EventGroupHeaderInterval + _CurEventTrackHeight;
        }

        public ActionTrackGroup Clone()
        {
            ActionTrackGroup _actionTrack = new ActionTrackGroup(mEngineSetting, mIcon, mTrackName, mColor, 
                mEventTypes, null, mGroupName);
            
            List<IActionTrack> _eventTracks = new List<IActionTrack>();
            foreach (var VARIABLE in CurActiontTrack)
            {
                // if (VARIABLE is not null)
                {
                    _eventTracks.Add(CreactEventTrack(
                        mEngineSetting,
                        mIcon,
                        _actionTrack.DeleteElement,
                        mEventTypes,
                        mColor,
                        VARIABLE.Clone()
                    )); 
                }
            }

            _actionTrack.mCurActiontTrack = _eventTracks;
            return _actionTrack;
        }
        private void DrawEventTrack(Rect _rect, int _index, bool _isActive, bool _ISFocused)
        {
            mCurActiontTrack[_index].Draw(_rect, mDrawStart, mDrawEnd);
        }

        //创建轨道
        private IActionTrack CreactEventTrack(
            ActionEngineSetting _engineSetting, 
            GUIContent _icon, 
            Action<IActionTrack> _deleCallback,
            List<string> _evenTypes,
            Color _color, 
            IActionTrack _actionTrack = null,
            string _trackName = ""
        )
        {
            if (_actionTrack != null)
            {
                if (_actionTrack is EventTrack _eventTrack)
                {
                    return new EventTrack(
                        _engineSetting,
                        _icon,
                        _deleCallback,
                        _evenTypes,
                        _color,
                        _eventTrack
                    );
                }else if(_actionTrack is InterrupGroupTrack _interrupTrack)
                {
                    return new InterrupGroupTrack(
                        _interrupTrack.ActionStateID,
                        _interrupTrack, 
                        DeleteElement,
                        ReorderableListChange
                    );
                }
            }
            
            return new EventTrack(
                _engineSetting,
                _icon,
                _deleCallback,
                _evenTypes,
                _color,
                null,
                _trackName
            );
        }

        private float GetElementHeight(int _index)
        {
            return mCurActiontTrack[_index].GetHeight() - 2f;
        }

        private void ReorderableListChange()
        {
            listChange = true;
            mEventList.ClearSelection();
            mEventList.list = null;
            
            foreach (var _actionTrack in TimeLineWindow.Instance.SelectActionState.AllEventTrackGroup)
            {
                foreach (var VARIABLE in _actionTrack.CurActiontTrack)
                {
                    VARIABLE.IsDraw = false;
                }

            }//UnityEditor不会自主刷新，这里手动刷新下回调
            // EngineDebug.Log("触发刷新");
        }

        #region 重构函数
        public ActionTrackGroup(
            ActionEngineSetting _engineSetting, 
            GUIContent _icon,
            string _trackName,
            Color _color,
            List<string> _eventTypes,
            ActionTrackGroup actionTrackGroup = null,
            string _groupName = ""
        )
        {
            if (actionTrackGroup != null)
            {
                mGroupName = actionTrackGroup.GroupName;
                List<IActionTrack> _eventTracks = new List<IActionTrack>();
                foreach (var VARIABLE in actionTrackGroup.CurActiontTrack)
                {
                    _eventTracks.Add(CreactEventTrack(
                        _engineSetting, 
                        _icon, 
                        DeleteElement,
                        _eventTypes,
                        _color, 
                        VARIABLE
                    ));
                }
                mCurActiontTrack = _eventTracks;
            }
            else
            {
                mGroupName = _groupName;
            }
            mEngineSetting = _engineSetting;
            mIcon = _icon;
            mTrackName = _trackName;
            mColor = _color;
            mEventTypes = _eventTypes;
            mWindow = TimeLineWindow.Instance;
            
            mEventList = new ReorderableList(mCurActiontTrack, null, true, false, false, false);
            mEventList.showDefaultBackground = false;
            mEventList.multiSelect = false;
            mEventList.drawElementCallback = DrawEventTrack;
            mEventList.drawNoneElementCallback = (Rect _rect) => { };
            mEventList.elementHeightCallback = GetElementHeight;
            //mEventList.onReorderCallback = OnChangeTrackCallBack;
            mCreateEventTrack = new MouseActionInfo(CreateEventTrack, MotionEngineConst.Priority_EventUnit);
            mCreateOtherTrack = new MouseActionInfo(CreateOtherTarck, MotionEngineConst.Priority_EventUnit);
        }
        

        #endregion

        #region 回调

        private void OnLoadCallBack()
        {
            // EngineDebug.Log($"装载事件: {mGroupName}");
            mWindow.OnLoadMouseAction(EMouseEvent.MouseDwonLeft, mCreateEventTrack);
            mWindow.OnLoadMouseAction(EMouseEvent.MouseDwonRight, mCreateOtherTrack);
        }

        private void UnLoadCallBack()
        {
            // EngineDebug.Log($"卸载事件: {mGroupName}");
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDwonLeft, mCreateEventTrack);
            mWindow.UnLoadMouseAction(EMouseEvent.MouseDwonRight, mCreateOtherTrack);
        }

        private bool CreateEventTrack(Event _event)
        {
            Vector2 _mousePos = mWindow.MousePosToViewPos(_event.mousePosition);

            if (mButtonRect.Contains(_mousePos))
            {
                ResourcesWindow.Instance.ActionOnChange();
                
                mIsOpen = true;
                mCurActiontTrack.Add(CreactEventTrack(
                    mEngineSetting, 
                    mIcon, 
                    DeleteElement,
                    mEventTypes,
                    mColor, 
                    null,
                    mTrackName
                ));
                return true;
            }
            return false;
        }

        private bool CreateOtherTarck(Event _event)
        {
            Vector2 _mousePos = mWindow.MousePosToViewPos(_event.mousePosition);

            if (mButtonRect.Contains(_mousePos))
            {
                CustomTrackMenu();
                return true;
            }
            return false;
        }

        private void CustomTrackMenu()
        {

            GenericMenu _menu = new GenericMenu();

            foreach (var _curAction in ResourcesWindow.Instance.AllActionState)
            {
                string _name = "(打断组)" + _curAction.EditorActionType + "/" + _curAction.Name;
                _menu.AddItem(new GUIContent(_name), false, () =>
                {
                    mIsOpen = true;
                    mCurActiontTrack.Add(new InterrupGroupTrack(_curAction.ID, null, 
                        DeleteElement, ReorderableListChange));
                    ResourcesWindow.Instance.ActionOnChange();
                });
            }
            _menu.ShowAsContext();

        }
        private void DeleteElement(IActionTrack _eventTrack)
        {
            if (!mCurActiontTrack.Contains(_eventTrack))
            {
                EngineDebug.LogError("根本不在List列表啊   ");
                return;
            }
            
            ResourcesWindow.Instance.ActionOnChange();
            mCurActiontTrack.Remove(_eventTrack);
        }
        #endregion
    }
}
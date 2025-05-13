using AsiActionEngine.RunTime;

namespace AsiActionEngine.Editor
{
    public partial class ResourcesWindow
    {
        public int mSelectmenuID { get; private set; }
        public int mOnSelectUnitID { get; private set; }
        public int mOnSelectActionStateID { get; private set; }
        
        public void OnSelectMenu(int _id)
        {
            mSelectmenuID = _id;
            if (_id < 0)
            {
                mSelectmenuID = 0;
                EngineDebug.LogWarning($"不存在负ID的菜单");
            }
            else if (_id >= mMenu.Length)
            {
                mSelectmenuID = mMenu.Length - 1;
                EngineDebug.LogWarning($"不存在ID为【{_id}】 的菜单");
            }

            //在菜单选择时初始化
            InitUnitGUI();
            InitGValueGUI();
            InitCameraGUI();
            System.GC.Collect();

            // if (mSelectmenuID == 0)
            // {
            //     InitUnitGUI();
            // }else if (mSelectmenuID == 1)
            // {
            //     InitGValueGUI();
            // }
            // else if (mSelectmenuID == 2)
            // {
            //     InitGValueGUI();
            // }
            // else if (mSelectmenuID == 5)
            // {
            //     InitCameraGUI();
            // }
        }

        public void OnSelectUnit(int _id)
        {
            mOnSelectUnitID = _id;
            InspectorWindow.Instance.SelectProperty(mUnitWarp[_id]);
        }

        public EditorUnitWarp GetSelectUnit()
        {
            return mUnitWarp[mOnSelectUnitID];
        }
        
        public void OnSelectGValue(EditorEngineGValuePart _gValue)
        {
            CurSclect_GValue = _gValue;
            // mOnSelectActionStateID = mActionState.IndexOf(_actionState);
            InspectorWindow.Instance.SelectProperty(_gValue);
            // TimeLineWindow.Instance.OnChangeAction(mActionState[mOnSelectActionStateID]);
            // TimeLineWindow.Instance.Repaint();
        }
        public void OnSelectActionState(int _id, bool _isMarkUndo = true, bool _isPlay = true)
        {
            mOnSelectActionStateID = _id;
            InspectorWindow.Instance.SelectProperty(mActionState[_id],_isMarkUndo);
            TimeLineWindow.Instance.OnChangeAction(mActionState[_id], _isPlay);
            TimeLineWindow.Instance.Repaint();
        }
        public void OnSelectActionState(EditorActionState _actionState, bool _isMarkUndo = true)
        {
            mOnSelectActionStateID = mActionState.IndexOf(_actionState);
            OnSelectActionState(mOnSelectActionStateID,_isMarkUndo);
        }

        public void OnSelectCamWarp(EditorCameraWarp _cameraWarp)
        {
            InspectorWindow.Instance.SelectProperty(_cameraWarp);
        }

        public EditorActionState GetEditorActionStateToSelect()
        {
            // EngineDebug.Log("选择");
            // return mSelectActionState;
            if(mActionState == null || mOnSelectActionStateID >= mActionState.Count)
            {
                return null;
            }
            return mActionState[mOnSelectActionStateID];
        }

        public string GetActionToCurrName()
        {
            return mActionState[mOnSelectActionStateID].Name;
        }
        
        public string GetUnitToCurrName()
        {
            return mUnitWarp[mOnSelectUnitID].Name;
        }
    }
}
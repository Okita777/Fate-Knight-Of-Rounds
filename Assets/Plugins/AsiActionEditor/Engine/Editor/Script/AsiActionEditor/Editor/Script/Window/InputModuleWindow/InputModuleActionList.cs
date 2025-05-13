using System.Collections.Generic;
using System.IO;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public class InputActionList
    {
        #region MyRegion
        private static InputActionList _Instance;
        public static InputActionList Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new InputActionList();
                    _Instance.LoadActionList();
                }
                return _Instance;
            }
        }
        #endregion

        private List<string> mActionList = new List<string>();

        public List<string> ActionList => mActionList;

        public void SetActionList(List<string> _actionList)
        {
            mActionList = _actionList;
        }

        public void SaveActionList(List<string> _actionList = null)
        {
            if (_actionList != null)
            {
                mActionList = _actionList;
            }
            // EngineDebug.Log("保存InputAction配置列表成功");

            // string _path = RunTime.MotionEngineRuntimePath.Instance.InputActionListPath();
            string _path = ActionWindowMain.ActionEditorFuntion.GetInputActionPath();
            // _path = ActionWindowMain.GetAssetPathToResources(_path);

            InputActionListInfo _actionListInfo = new InputActionListInfo();
            _actionListInfo.ActionList = mActionList;
            string _str = JsonUtility.ToJson(_actionListInfo);
            // Debug.Log("保存路径：" + _path);
            File.WriteAllText(_path,_str);
        }
        
        private void LoadActionList()
        {
            string _path = ActionWindowMain.ActionEditorFuntion.GetInputActionPath();

            // string _str = File.ReadAllText(_path);
            // Debug.Log("读取路径：" + _path);

            string _str = AssetDatabase.LoadAssetAtPath<TextAsset>(_path).text;
            InputActionListInfo _actionListInfo = new InputActionListInfo();
            JsonUtility.FromJsonOverwrite(_str, _actionListInfo);
            _Instance.mActionList = new List<string>();
            if (_actionListInfo is not null)
            {
                _Instance.mActionList.AddRange(_actionListInfo.ActionList);
            }
        }
        
        
    }

    [System.Serializable]
    public class InputActionListInfo
    {
        public List<string> ActionList = new List<string>();
    }
}
using AsiTimeLine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiTimeLine.Editor
{
    public partial class ActionEngineConst
    {
        private const string EditorResourcesPath =  "Assets/Editor/ActionEditorConfig/";
        private const string EditorSetting = EditorResourcesPath + "Setting/WindowSetting.asset";//编辑器界面配置数据
        private const string EditorDataPath = EditorResourcesPath + "Data/";//编辑器资产路径
        public const string EditorDataPartPath = EditorDataPath + "{0}/{1}.json";//编辑器配置路径
        
        public static readonly string RunTimeSavePath = ActionEngineRuntimePath.Instance.DataPath + "{0}/{1}.byte";//Resources下Json资源保存路径
        public static string EditorUnitSavePath(string _name = "")
        {
            if(string.IsNullOrEmpty(_name))
            {
                return EditorDataPath + "Unit";
            }
            return EditorDataPath + $"Unit/{_name}.json";
        }
        public static string EditorActionSavePath(string _name = "")
        {
            if(string.IsNullOrEmpty(_name))
            {
                return EditorDataPath + "Action";
            }
            return EditorDataPath + $"Action/{_name}.json";
        }
        public static string EditorGValueSavePath(string _name = "")
        {
            if(string.IsNullOrEmpty(_name))
            {
                return EditorDataPath + "GValue";
            }
            return EditorDataPath + $"GValue/{_name}.json";
        }
        public static string EditorItemSavePath(string _name = "")
        {
            if(string.IsNullOrEmpty(_name))
            {
                return EditorDataPath + "Item";
            }
            return EditorDataPath + $"Item/{_name}.json";
        }
        
        public static string EditorCamSavePath(string _name = "")
        {
            if(string.IsNullOrEmpty(_name))
            {
                return EditorDataPath + "Camera";
            }
            return EditorDataPath + $"Camera/{_name}.json";
        }
    }
}
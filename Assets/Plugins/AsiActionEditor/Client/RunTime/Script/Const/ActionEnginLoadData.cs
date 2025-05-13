using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AsiActionEngine.RunTime;
using UnityEngine;
using Object = UnityEngine.Object;
using TextAsset = UnityEngine.TextAsset;

namespace AsiTimeLine.RunTime
{
    public class ActionEnginLoadData
    {
        #region Instance
        private static ActionEnginLoadData _instance;

        public static ActionEnginLoadData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ActionEnginLoadData();
                }
                return _instance;
            }
        }
        #endregion

        private string Suffixes => ActionEngineRuntimePath.Suffixes;
        public bool LoadInputInfo(Action<InputModuleInfo> _action)
        {
            string _path = ActionEngineRuntimePath.Instance.InputModulePath();

            string _str = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(_path).text;
            InputModuleInfo mInputModuleInfo = new InputModuleInfo(null, 0);
            JsonUtility.FromJsonOverwrite(_str, mInputModuleInfo);
            _action(mInputModuleInfo);
            return true;
        }

        public bool LoadGValue(Action<EngineGValue> _action)
        {
            string _path = ActionEngineRuntimePath.Instance.GValuePath("GValue" + Suffixes);
            using (FileStream _fileStream = new FileStream(_path, FileMode.Open))
            {
                BinaryFormatter _binaryFormatter = new BinaryFormatter();
                _action((EngineGValue)_binaryFormatter.Deserialize(_fileStream));
            }

            return true;
        }

        public bool LoadUnitAction(string _name, Action<ActionStateInfo> _action)
        {
            string _path = ActionEngineRuntimePath.Instance.ActionPath(_name + Suffixes);
            using (FileStream _fileStream = new FileStream(_path, FileMode.Open))
            {
                BinaryFormatter _binaryFormatter = new BinaryFormatter();
                _action((ActionStateInfo)_binaryFormatter.Deserialize(_fileStream));
            }

            return true;
        }

        public bool LoadUnit(string _name, Action<UnitWarp> _action)
        {
            string _path = ActionEngineRuntimePath.Instance.UnitPath(_name);
            // EngineDebug.Log("尝试加载路径：" + _path);
            using (FileStream _fileStream = new FileStream(_path, FileMode.Open))
            {
                BinaryFormatter _binaryFormatter = new BinaryFormatter();
                _action((UnitWarp)_binaryFormatter.Deserialize(_fileStream));
            }
            return true;
        }

        public bool LoadCameraWarp(string _name, Action<CameraWarp> _action)
        {
            string _path = ActionEngineRuntimePath.Instance.CameraPath(_name + Suffixes);
            using (FileStream _fileStream = new FileStream(_path, FileMode.Open))
            {
                BinaryFormatter _binaryFormatter = new BinaryFormatter();
                _action((CameraWarp)_binaryFormatter.Deserialize(_fileStream));
            }
            return true;
        }

        public bool LoadAudioClip(string _name, Action<AudioClipDicList> _action)
        {
            string _path = ActionEngineRuntimePath.Instance.AssetDataPath(_name + Suffixes);
            if (File.Exists(_path))
            {
                using (FileStream _fileStream = new FileStream(_path, FileMode.Open))
                {
                    BinaryFormatter _binaryFormatter = new BinaryFormatter();
                    _action((AudioClipDicList)_binaryFormatter.Deserialize(_fileStream));
                }
            }
            else
            {
                _action(new AudioClipDicList());
            }

            return true;
        }
        
        //同步加载
        public T LoadObject<T>(string _path) where T : Object
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(_path);
#endif
            return null;
        }
        public bool LoadObject<T>(string _path, Action<T> _action) where T : Object
        {
#if UNITY_EDITOR
            _action(UnityEditor.AssetDatabase.LoadAssetAtPath<T>(_path));
            return true;
#endif
            return false;

        }
        public bool LoadObjectAsync<T>(string _path, Action<T> _action) where T : Object
        {
#if UNITY_EDITOR
            _action(UnityEditor.AssetDatabase.LoadAssetAtPath<T>(_path));
            return true;
#endif
            return false;
        }
    }
}
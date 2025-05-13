using System;
using System.IO;
using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using AsiTimeLine.RunTime;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using MotionEngineConst = AsiActionEngine.Editor.MotionEngineConst;

namespace AsiTimeLine.Editor
{
    public class SaveData
    {
        private static string mRunTimeSavePath => ActionEngineConst.RunTimeSavePath;//Resources下Json资源保存路径
        private const bool mSaveToJson = true;//保存数据文件为Json  否则保存为二进制文件
        
        #region 加载数据 是Editor下的资源加载 直接同步加载即可
        static public bool LoadActionData(out EditorActionStateInfo _editorActionState, string _actionName)
        {
            string _path = ActionEngineConst.EditorActionSavePath(_actionName);
            if (!File.Exists(_path))
            {
                _editorActionState = null;
                EngineDebug.LogWarning($"客户端加载 Action 数据失败，\n加载路径：{_path}");
                return false;
            }
            

            string _str = File.ReadAllText(_path);
            EditorActionStateInfo _LoadInfo = new EditorActionStateInfo(null, _actionName);
            JsonUtility.FromJsonOverwrite(_str, _LoadInfo);
            _editorActionState = _LoadInfo;
            return true;
        }
        
        
        static public bool LoadUnitData(out EditorUnitWarp _editorActionState, string _unitName)
        {
            //加载音频资源
            ActionEngineManager_AudioClip.Instance.Init();
            
            string _path = ActionEngineConst.EditorUnitSavePath(_unitName);
            if (!File.Exists(_path))
            {
                EngineDebug.Log($"客户端加载 Unit 数据失败，\n加载路径：{_path}");
                _editorActionState = null;
                return false;
            }

            if (mSaveToJson)
            {
                string _str = File.ReadAllText(_path);
                EditorUnitWarp _LoadInfo = new EditorUnitWarp(0,"null",new GValue_Setting());
                JsonUtility.FromJsonOverwrite(_str, _LoadInfo);
                _editorActionState = _LoadInfo;
                return true;
            }
            else
            {
                //加载为二进制数据
                using (FileStream _fileStream = new FileStream(_path, FileMode.Open))
                {
                    BinaryFormatter _binaryFormatter = new BinaryFormatter();
                    _editorActionState = (EditorUnitWarp) _binaryFormatter.Deserialize(_fileStream);
                    return true;
                }
            }
        }
        
        static public bool LoadGValueData(out EditorEngineGValue _editorEngineGValue, string _gvalueName)
        {
            string _path = ActionEngineConst.EditorGValueSavePath(_gvalueName);
            if (!File.Exists(_path))
            {
                EngineDebug.Log($"客户端加载 GValue 数据失败，\n加载路径：{_path}");
                _editorEngineGValue = new EditorEngineGValue();
                return false;
            }
            
            string _str = File.ReadAllText(_path);
            EditorEngineGValue _LoadInfo = new EditorEngineGValue();
            JsonUtility.FromJsonOverwrite(_str, _LoadInfo);
            _editorEngineGValue = _LoadInfo;
            return true;
        }
        
        static public bool LoadCameraWarp(out EditorCameraWarp _editorCameraWarp, string _name)
        {
            string _path = ActionEngineConst.EditorCamSavePath(_name);
            if (!File.Exists(_path))
            {
                EngineDebug.Log($"客户端加载 Camera 数据失败，\n加载路径：{_path}");
                _editorCameraWarp = null;
                return false;
            }
            
            string _str = File.ReadAllText(_path);
            EditorCameraWarp _LoadInfo = new EditorCameraWarp(0,"");
            JsonUtility.FromJsonOverwrite(_str, _LoadInfo);
            _editorCameraWarp = _LoadInfo;
            return true;
        }

        static public bool LoadGValueState(out EditorEngineGValue _editorGValue, string _gvalueName)
        {
            string _path = ActionEngineConst.EditorGValueSavePath(_gvalueName);
            if (!File.Exists(_path))
            {
                EngineDebug.Log($"客户端加载 GValue 数据失败，\n加载路径：{_path}");
                _editorGValue = null;
                return false;
            }
            string _str = File.ReadAllText(_path);
            EditorEngineGValue _LoadInfo = new EditorEngineGValue();
            JsonUtility.FromJsonOverwrite(_str, _LoadInfo);
            _editorGValue = _LoadInfo;
            return true;
        }
        #endregion


        #region 保存数据
        static public bool SaveActionData(EditorActionStateInfo _editorActionState, string _actionName)
        {

            // //保存Runtime数据
            // string _savePath = ActionEngineRuntimePath.Instance.ActionPath(_actionName);
            // _savePath = string.Format(mRunTimeSavePath, _savePath);
            //
            // _str = JsonUtility.ToJson(_editorActionState.GetActionStateInfo());
            // File.WriteAllText(_savePath, _str);

            // try
            {
                ActionStateInfo _actionState = _editorActionState.GetActionStateInfo();
                using (FileStream _fileStream = new FileStream(string.Format(mRunTimeSavePath,"Action", _actionName), FileMode.Create))
                {
                    BinaryFormatter _binaryFormatter = new BinaryFormatter();
                    _binaryFormatter.Serialize(_fileStream, _actionState);
                }
            
                string _str = JsonUtility.ToJson(_editorActionState);
                File.WriteAllText(ActionEngineConst.EditorActionSavePath(_actionName),_str);
                EngineDebug.Log($"成功储存  [ <color=#FFF100>{_actionName}</color> ]  Action数据");
            }
            // catch (Exception e)
            // {
            //     EditorUtility.DisplayDialog("警告", "序列化失败，可能存在无法进行序列或者未标记的变量，请查看【Console】面板报错信息", "OK");
            //     Console.WriteLine(e);
            //     throw;
            // }
            

            return true;
        }
        static public bool SaveUnitData(EditorUnitWarp _editorUnitWarp, string _unitName)
        {
            if (mSaveToJson)
            {
                string _str = JsonUtility.ToJson(_editorUnitWarp);
                File.WriteAllText(ActionEngineConst.EditorUnitSavePath(_unitName),_str);
                
                // //保存Runtime数据
                // string _savePath = ActionEngineRuntimePath.Instance.UnitPath(_unitName);
                // _savePath = string.Format(mSavePath, _savePath);
                //
                // // _str = JsonUtility.ToJson(RunTimeDataManager.UnitWarpInfo(_editorUnitWarp));
                // _str = JsonUtility.ToJson(_editorUnitWarp);
                // File.WriteAllText(_savePath, _str);
            }
            else
            {
                //序列化到二进制
                using (FileStream _fileStream = new FileStream(ActionEngineConst.EditorUnitSavePath(_unitName), FileMode.Create))
                {
                    BinaryFormatter _binaryFormatter = new BinaryFormatter();
                    _binaryFormatter.Serialize(_fileStream, _editorUnitWarp);
                }
            }

            using (FileStream _fileStream = new FileStream(string.Format(mRunTimeSavePath,"Unit", _unitName), FileMode.Create))
            {
                BinaryFormatter _binaryFormatter = new BinaryFormatter();
                _binaryFormatter.Serialize(_fileStream, _editorUnitWarp.GetUnitWarp());
            }
            // EngineDebug.Log($"成功储存  [ <color=#FFF100>{_unitName}</color> ]  Unit数据 ");
            
            return true;
        }
        static public bool SaveCameraWarp(EditorCameraWarp _editorCameraWarp, string _name)
        {
            string _str = JsonUtility.ToJson(_editorCameraWarp);
            File.WriteAllText(ActionEngineConst.EditorCamSavePath(_name),_str);
            
            // //保存Runtime数据
            // string _savePath = ActionEngineRuntimePath.Instance.CameraPath(_name);
            // _savePath = string.Format(mRunTimeSavePath, _savePath);
            //
            // _str = JsonUtility.ToJson(_editorCameraWarp.GetCameraWarp());
            // File.WriteAllText(_savePath, _str);
            
            using (FileStream _fileStream = new FileStream(string.Format(mRunTimeSavePath,"Camera", _name), FileMode.Create))
            {
                BinaryFormatter _binaryFormatter = new BinaryFormatter();
                _binaryFormatter.Serialize(_fileStream, _editorCameraWarp.GetCameraWarp());
            }
            
            EngineDebug.Log($"成功储存  [ <color=#FFF100>{_name}</color> ]  Camera数据 ");
            return true;
        }
        static public bool SaveGValueData(EditorEngineGValue _engineGValue, string _gvalueName)
        {
            string _str = JsonUtility.ToJson(_engineGValue);
            File.WriteAllText(ActionEngineConst.EditorGValueSavePath(_gvalueName),_str);
            
            // //保存Runtime数据
            // string _savePath = ActionEngineRuntimePath.Instance.GValuePath(_gvalueName);
            // _savePath = string.Format(mRunTimeSavePath, _savePath);
            //
            // _str = JsonUtility.ToJson(_engineGValue.GetEngineGValue());
            // File.WriteAllText(_savePath, _str);
            
            using (FileStream _fileStream = new FileStream(string.Format(mRunTimeSavePath,"GValue", _gvalueName), FileMode.Create))
            {
                BinaryFormatter _binaryFormatter = new BinaryFormatter();
                _binaryFormatter.Serialize(_fileStream, _engineGValue.GetEngineGValue());
            }
            
            EngineDebug.Log($"成功储存  [ <color=#FFF100>{_gvalueName}</color> ]  GValue数据 ");
            return true;
        }

        static public bool SaveAssetData(object _audioClipDicList, string _name)
        {
            using (FileStream _fileStream = new FileStream(string.Format(mRunTimeSavePath,"AssetData", _name), FileMode.Create))
            {
                BinaryFormatter _binaryFormatter = new BinaryFormatter();
                _binaryFormatter.Serialize(_fileStream, _audioClipDicList);
            }
            
            EngineDebug.Log($"成功储存AssetData:  [ <color=#FFF100>{_name}</color> ]  数据 ");
            return true;
        }
        #endregion

    }
}
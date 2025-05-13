using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    [InitializeOnLoad]
    public class ActionWindowMain
    {
        public ActionWindowMain()
        {
            //初始化时读取数据
            ScenceDraw_Editor = PlayerPrefs.GetInt("ScenceDraw_Editor", 1) > 0;
            ScenceDraw_Runtime = PlayerPrefs.GetInt("ScenceDraw_Runtime", 1) > 0;
            ScenceDraw_Event = PlayerPrefs.GetInt("ScenceDraw_Event", 1) > 0;
            ScenceDraw_Interrupt = PlayerPrefs.GetInt("ScenceDraw_Interrupt", 1) > 0;
        }
        
        private static ActionEngineSetting mEngineSetting;
        
        //复制的事件数据
        public static EditorActionEvent CopyActionEven = null;
        public static ActionEngineSetting EngineSetting
        {
            get
            {
                if (!mEngineSetting)
                {
                    mEngineSetting =
                        AssetDatabase.LoadAssetAtPath<ActionEngineSetting>(MotionEngineConst.EditorSetting);
                    mEngineSetting?.Init();
                }

                return mEngineSetting;
            }
            set
            {
                if (value != null)
                {
                    mEngineSetting = value;
                    mEngineSetting.Init();
                }
                else
                {
                    mEngineSetting =
                        AssetDatabase.LoadAssetAtPath<ActionEngineSetting>(MotionEngineConst.EditorSetting);
                    mEngineSetting.Init();
                }
            }
        }

        // public static int SenceDrawSetting = 0;
        public static bool ScenceDraw_Editor = true;
        public static bool ScenceDraw_Runtime = true;
        public static bool ScenceDraw_Event = true;
        public static bool ScenceDraw_Interrupt = true;
        public static AsiActionEditorFuntion ActionEditorFuntion;
        public static string[] EventType;
        public static string[] ConditionType;
        public static string[] EventType_m;
        public static string[] ConditionType_m;
        public static List<string> EventTypesDes = new List<string>();
        public static List<string> EventTypes = new List<string>();
        public static List<string> ConditionTypesDes = new List<string>();
        public static List<string> ConditionTypes = new List<string>();
        private static List<string> _EnumName = new List<string>();

        public static void MainWindow()
        {
            TimeLineWindow.needInit = true;
            ResourcesWindow.needInit = true;
            TimeLineWindow.Instance.Show();
            ResourcesWindow.Instance.Show();
            InspectorWindow.Instance.Show();
            
            ResourcesWindow.Instance.ActionOnChange(false);
        }

        public static void InitWindow(EditorActionData _editorActionData)
        {
            ResourcesWindow.Instance.ActionOnChange(false);

            ActionEditorFuntion = _editorActionData.AsiActionEditorFuntion;
            EventType = _editorActionData.EventDataTypeDes;
            ConditionType = _editorActionData.ConditionTypeDes;

            _EnumName.Clear();
            for (int i = 0; i < 99999; i++)
            {
                EEvenTypeInternal _en = (EEvenTypeInternal)i;
                string _enumName = _en.ToString();
                if (_enumName.Split('_').Length < 2)
                {
                    break;
                }
                
                _EnumName.Add(EnumUtinity.GetDescription2(_en));
            }
            EventType_m = _EnumName.ToArray();
            
            _EnumName.Clear();
            for (int i = 0; i < 99999; i++)
            {
                EInterruptTypeInternal _en = (EInterruptTypeInternal)i;
                string _enumName = _en.ToString();
                if (_enumName.Split('_').Length < 2)
                {
                    break;
                }
                
                _EnumName.Add(EnumUtinity.GetDescription2(_en));
            }
            ConditionType_m = _EnumName.ToArray();
            
            EventTypesDes.AddRange(EventType_m);
            EventTypesDes.AddRange(EventType);
            EventTypes.AddRange(Enum.GetNames(typeof(EEvenTypeInternal)));
            EventTypes.AddRange(_editorActionData.EventDataType);
            
            ConditionTypesDes.AddRange(ConditionType_m);
            ConditionTypesDes.AddRange(ConditionType);
            ConditionTypes.AddRange(Enum.GetNames(typeof(EInterruptTypeInternal)));
            ConditionTypes.AddRange(_editorActionData.ConditionType);
        }

        public static void UpdateMainWindow()
        {
            TimeLineWindow.Instance.Repaint();
        }

        public static bool GetConditionType(int _typeID, out EInterruptTypeInternal _interruptType)
        {
            if (_typeID < 0)
            {
                _interruptType = (EInterruptTypeInternal)(-_typeID);
                return true;
            }

            _interruptType = EInterruptTypeInternal.EIT_InValid;
            return false;
        }
        
        public static string GetAssetPathToResources(string _ResourcePath, string _suffix = "json")
        {
            if (string.IsNullOrEmpty(_suffix))
            {
                return $"Assets/Resources/{_ResourcePath}";

            }
            return $"Assets/Resources/{_ResourcePath}.{_suffix}";
        }

        public static bool GetActionEventID(string _ActionName, out int _EventID)
        {
            if (EventTypes.Contains(_ActionName))
            {
                _EventID = EventTypes.IndexOf(_ActionName);
                return true;
            }
            _EventID = 0;
            return false;
        }
    }

    public struct EditorActionData
    {
        //传入事件枚举
        public string[] EventDataTypeDes;
        public string[] EventDataType;

        //传入跳转条件枚举
        public string[] ConditionTypeDes;
        public string[] ConditionType;

        //执行函数
        public AsiActionEditorFuntion AsiActionEditorFuntion;

        public EditorActionData(
            string[] eventDataTypeDes,
            string[] conditionTypeDes,
            string[] eventDataType,
            string[] conditionType,
            AsiActionEditorFuntion _asiActionEditorFuntion
        )
        {
            EventDataTypeDes = eventDataTypeDes;
            ConditionTypeDes = conditionTypeDes;
            EventDataType = eventDataType;
            ConditionType = conditionType;
            AsiActionEditorFuntion = _asiActionEditorFuntion;
        }
    }

    public abstract class AsiActionEditorFuntion
    {
        /// <summary>
        /// Editor下，Timeline更新时初始化调用
        /// </summary>
        public abstract void TimeLineInit();

        /// <summary>
        /// Editor下，切换Action时调用
        /// </summary>
        public abstract void ChangeAction();
        
        /// <summary>
        /// Editor下，Timeline更新时调用的函数
        /// </summary>
        /// <param name="_deltaTime"></param>
        /// <param name="_actionEvent"></param>
        public abstract bool TimeLineUpdate(int _time, EditorActionEvent _actionEvent);

        /// <summary>
        /// 绘制事件的属性面板
        /// </summary>
        /// <param name="_actionEvent"></param>
        public abstract void DrawEventDate(EditorActionEvent _actionEvent, bool _isInit);

        /// <summary>
        /// 绘制事件的属性面板
        /// </summary>
        /// <param name="_actionEvent"></param>
        public abstract bool DrawEventHelpWindows(EditorActionEvent _actionEvent, out Action _drawCallback);
        
        /// <summary>
        /// 绘制跳转条件面板
        /// </summary>
        /// <param name="_actionCondition"></param>
        public abstract void DrawCondition(IInterruptCondition _actionCondition, bool _isInit);

        /// <summary>
        /// 绘制事件头部描述
        /// </summary>
        /// <param name="_actionEvent"></param>
        /// <returns></returns>
        public abstract string DrawEventTitle(EditorActionEvent _actionEvent, bool _isInit);

        /// <summary>
        /// 绘制事件参数描述
        /// </summary>
        /// <param name="_actionEvent"></param>
        /// <returns></returns>
        public abstract string DrawEventDetailed(EditorActionEvent _actionEvent, bool _isInit);

        /// <summary>
        /// 实例化事件
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public abstract EditorActionEvent CreactActionEvent(int _id);

        /// <summary>
        /// 实例化条件
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public abstract IInterruptCondition CreactCondition(int _id);

        public abstract void ActionUnitPreview(ActionPreviewMark _previewMark, GameObject _target);

        public abstract string GetActionEditorDataPath(string _name = "");
        public abstract string GetUnitEditorDataPath(string _name = "");
        public abstract string GetItemEditorWarpPath(string _name = "");
        public abstract string GetCamEditorDataPath(string _name = "");
        // public abstract string GetActionDataPath(string _name = "");
        // public abstract string GetUnitDataPath(string _name = "");
        // public abstract string GetItemWarpPath(string _name = "");
        // public abstract string GetCamDataPath(string _name = "");
        public abstract string GetRunTimeDataPath();
        public abstract string GetInputActionPath();
        public abstract string GetInputModule();

        //攻击信息
        public abstract IAttackInfo AttackInfo();
        // //受击回调
        // public abstract void BeHit(IAttackInfo _attackInfo);
        
        /// <summary>
        /// 保存Action数据
        /// </summary>
        /// <param name="_editorActionState">要储存的Action数据</param>
        /// <returns>储存成功</returns>
        public virtual bool SaveActionData(EditorActionStateInfo _editorActionState, string _name)
        {
            return false;
        }
        public virtual bool SaveCameraData(EditorCameraWarp _cameraWarp, string _name)
        {
            return false;
        }
        public virtual bool SaveUnitData(EditorUnitWarp _saveData, string _name)
        {
            return false;
        }
        public virtual bool SaveItemData(string _name)
        {
            return false;
        }
        public virtual bool SaveGValueData(EditorEngineGValue _cameraWarp, string _name)
        {
            return false;
        }

        public abstract IProperty BluePrint_Create(string name);
        public abstract string BluePrint_Name(string name);

        /// <summary>
        /// 加载Action数据
        /// </summary>
        /// <param name="_editorActionState">加载对象</param>
        /// <returns>加载成功</returns>
        public virtual bool LoadActionData(out EditorActionStateInfo _editorActionState, string _name)
        {
            _editorActionState = null;
            return false;
        }
        public virtual bool LoadGValueData(out EditorEngineGValue _editorActionState, string _gvalueName)
        {
            _editorActionState = null;
            return false;
        }
        public virtual bool LoadCameraData(out EditorCameraWarp _cameraWarp, string _name)
        {
            _cameraWarp = null;
            return false;
        }
        public virtual bool LoadUnitData(out EditorUnitWarp _saveData, string _name)
        {
            _saveData = null;
            return false;
        }
        public virtual bool LoadItemData(string _name)
        {
            return false;
        }

        public virtual bool ReLoadAcrionList(string _GroupName)
        {
            return false;
        }


    }
}
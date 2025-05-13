using System;
using AsiActionEditor_Ex.RunTime;
using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using AsiTimeLine.RunTime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AsiTimeLine.Editor
{
    public class ActionEngineEvent : AsiActionEditorFuntion
    {
        public override void TimeLineInit() => EditorEventUpdate.OnInit();
        
        public override void ChangeAction() => EditorEventUpdate.OnChangeAction();

        public override bool TimeLineUpdate(int _time, EditorActionEvent _actionEvent)
            => EditorEventUpdate.OnUpdate(_time, _actionEvent);

        public override void DrawEventDate(EditorActionEvent _actionEvent, bool _isInit)
            => DrawInspector.DrawEvent(_actionEvent, _isInit);

        public override bool DrawEventHelpWindows(EditorActionEvent _actionEvent, out Action _drawCallBack)
            => DrawInspectorHelpWindow.DrawEventHelpWindows(_actionEvent, out _drawCallBack);
        
        public override void DrawCondition(IInterruptCondition _actionCondition, bool _isInit)
            => DrawInspector.DrawCondition(_actionCondition, _isInit);

        public override string DrawEventTitle(EditorActionEvent _actionEvent, bool _isInit)
            => DrawInspector.GetTitle(_actionEvent, _isInit);
        
        public override string DrawEventDetailed(EditorActionEvent _actionEvent, bool _isInit)
            => DrawInspector.GetDetailed(_actionEvent, _isInit);

        public override EditorActionEvent CreactActionEvent(int _id)
            => Editor.CreactActionEvent.Events(_id);

        public override IInterruptCondition CreactCondition(int _id)
            => Editor.CreactActionEvent.Condition(_id);


        public override string BluePrint_Name(string name) => DrawBluePrintName.GetName(name);
        public override IProperty BluePrint_Create(string name)=>CreactBluePrint.CreateBluePrint(name);
        
        public override bool LoadActionData(out EditorActionStateInfo _editorActionState, string _actionName)
            => Editor.SaveData.LoadActionData(out _editorActionState, _actionName);

        public override bool LoadUnitData(out EditorUnitWarp _saveData, string _unitName)
            => Editor.SaveData.LoadUnitData(out _saveData, _unitName);

        public override bool LoadCameraData(out EditorCameraWarp _saveData, string _name)
            => Editor.SaveData.LoadCameraWarp(out _saveData, _name);

        public override bool LoadGValueData(out EditorEngineGValue _editorActionState, string _gvalueName)
            => Editor.SaveData.LoadGValueState(out _editorActionState, _gvalueName);


        public override bool SaveActionData(EditorActionStateInfo _editorActionState, string _actionName)
            => Editor.SaveData.SaveActionData(_editorActionState, _actionName);

        public override bool SaveGValueData(EditorEngineGValue _editorActionState, string _gvalueName)
            => Editor.SaveData.SaveGValueData(_editorActionState, _gvalueName);



        public override bool SaveUnitData(EditorUnitWarp _saveData, string _unitName)
            => Editor.SaveData.SaveUnitData(_saveData, _unitName);

        public override bool SaveCameraData(EditorCameraWarp _cameraWarp, string _name)
            => Editor.SaveData.SaveCameraWarp(_cameraWarp, _name);


        public override string GetActionEditorDataPath(string _name = "") => ActionEngineConst.EditorActionSavePath(_name);
        public override string GetUnitEditorDataPath(string _name = "") => ActionEngineConst.EditorUnitSavePath(_name);
        public override string GetItemEditorWarpPath(string _name = "") => ActionEngineConst.EditorItemSavePath(_name);
        public override string GetCamEditorDataPath(string _name = "") => ActionEngineConst.EditorCamSavePath(_name);

        public override string GetRunTimeDataPath() => ActionEngineConst.RunTimeSavePath;

        // public override string GetActionDataPath(string _name = "") => ActionEngineRuntimePath.Instance.ActionPath(_name);
        // public override string GetUnitDataPath(string _name = "") => ActionEngineRuntimePath.Instance.UnitPath(_name);
        // public override string GetItemWarpPath(string _name = "") => ActionEngineRuntimePath.Instance.ItemPath(_name);
        // public override string GetCamDataPath(string _name = "") => ActionEngineRuntimePath.Instance.CameraPath(_name);
        public override string GetInputModule() => ActionEngineRuntimePath.Instance.InputModulePath();
        public override string GetInputActionPath() => ActionEngineRuntimePath.Instance.InputActionListPath();

        public override bool ReLoadAcrionList(string _GroupName) => EditorEventUpdate.ReLoadActionData(_GroupName);


        //战斗数据交互
        public override IAttackInfo AttackInfo() => new AttackInfo();

        //单位预览
        public override void ActionUnitPreview(ActionPreviewMark _previewMark, GameObject _target)
        {
            var _unitPreview = _target.AddComponent<UnitEditorPreview>();

            _unitPreview.IsPlayer = _previewMark.IsPlayer;
            _unitPreview.ActionName = _previewMark.ActionName;
            _unitPreview.DefaltCamera = _previewMark.DefaltCamera;
            _unitPreview.DefaltWeapon = _previewMark.DefaltWeapon;
            
            _unitPreview.CamOffsetPos = _previewMark.CamOffsetPos;
            _unitPreview.CamRotSpeed = _previewMark.CamRotSpeed;
            
            Object.DestroyImmediate(_previewMark);
        }
    }
}
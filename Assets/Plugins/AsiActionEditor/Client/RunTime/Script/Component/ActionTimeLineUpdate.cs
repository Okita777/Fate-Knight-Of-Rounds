using System;
#if UNITY_EDITOR
using System.Collections.Generic;
using AsiActionEngine.RunTime.DrawData;
using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using UnityEditor;
#endif
using UnityEngine;

namespace AsiTimeLine.RunTime
{

#if UNITY_EDITOR
    public class ActionTimeLineUpdate : MonoBehaviour
    {
        private GameObject mSelect = null;
        private float deltaTime = 0.0f;
        private void Start()
        {
            TimeLineWindow.Instance.RunTimeStart();
            mSelect = null;
            Selection.activeGameObject = null;
        }

        private void Update()
        {
            deltaTime = Time.deltaTime;

            if (ResourcesWindow.Instance.ActionStatePart is null)
            {
                if (ActionEngineManager_Input.Instance.Player is not null)
                {
                    ResourcesWindow.Instance.ActionStatePart =
                        ActionEngineManager_Input.Instance.Player.ActionStateMachine.AllActionStatePart[0];
                }
            }
            
            GameObject _nowObj = Selection.activeGameObject;
            if (mSelect != _nowObj)
            {
                if (_nowObj == null) return;
                if (_nowObj.TryGetComponent(out Unit _unit))
                {
                    //切换到该Unit单位
                    TimeLineWindow.Instance.RunTimeUpdateUnit(_unit);
                    //注册当前预览的相机
                    ResourcesWindow.Instance.SetPreCamera(ActionEngineManager_Input.Instance.CurCamera);
                    ResourcesWindow.Instance.SetRole(_nowObj);
                    //注册Action状态机
                    ResourcesWindow.Instance.ActionStatePart = _unit.ActionStateMachine.AllActionStatePart[0];
                }

                mSelect = _nowObj;
            }
            
            //鼠标输入
            if (Input.GetMouseButtonDown(2))
            {
                ActionEngineManager_Input.Instance.SetMoseDisPlay(!Cursor.visible);
                TimeLineWindow.Instance.SetActionEditorState(Cursor.visible);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ActionEngineManager_Input.Instance.SetMoseDisPlay(Cursor.visible);
                TimeLineWindow.Instance.SetActionEditorState(Cursor.visible);
            }
            //每帧更新TimeLine窗口
            TimeLineWindow.Instance.RuntimeUpdateFrame(Time.deltaTime);
            onDraw = true;
        }

        private bool onDraw = false;
        private void OnDrawGizmos()
        {
            if (!onDraw)
            {
                EngineDrawListData.Instance.Draw(0);
                return;
            }
            onDraw = false;
            //运行时不绘制
            if (!ActionWindowMain.ScenceDraw_Runtime) return;
            if(!ActionWindowMain.ScenceDraw_Event && !ActionWindowMain.ScenceDraw_Interrupt)return;

            // if (!TimeLineWindow.Instance.IsEditor)
            {
                //运行中非编辑时执行所有单位的绘制
                // EngineDrawListData.Instance.Init();
                // EngineDebug.LogWarning("绘制的单位有几个: " + ActionEngineManager_Unit.Instance.Units.Count);
                foreach (Unit _unit in ActionEngineManager_Unit.Instance.Units)
                {
                    bool isDrawEditor = _unit == TimeLineWindow.Instance.SelectUnit;

                    foreach (ActionStatePart _statePart in _unit.ActionStateMachine.AllActionStatePart)
                    {
                        if (_unit.ActionStateMachine.TryGetComponent(out CharacterConfig _config))
                        {
                            if (ActionWindowMain.ScenceDraw_Event && _statePart.CurrentActionState is not null)
                            {
                                if (!isDrawEditor)
                                {
                                    foreach (ActionEvent _event in _statePart.CurrentActionEvents)
                                    {
                                        _event.EventData.EditorDraw(_config, ResourcesWindow.Instance.ActionStatePart, 
                                            new ActionMachineTime(Time.deltaTime, _statePart.ElapsedTime,
                                                _event.TriggerTime, _event.Duration));
                                    }
                                }
                                else
                                {
                                    if (ResourcesWindow.Instance.ActionStateDic.TryGetValue(
                                            _statePart.CurrentActionState.ID, out EditorActionState _actionState))
                                    {
                                        foreach (ActionTrackGroup _actionTrackGroup in _actionState.AllEventTrackGroup)
                                        {
                                            foreach (IActionTrack _actionTrack in _actionTrackGroup.CurActiontTrack)
                                            {
                                                if (_actionTrack is EventTrack _eventTrack)
                                                {
                                                    foreach (EventDisplay _eventDisplay in _eventTrack.CurEventDisplay)
                                                    {
                                                        EditorActionEvent _actionEvent = _eventDisplay.MainEvent;
                                                        _actionEvent.EventData.EditorDraw(_config,
                                                            ResourcesWindow.Instance.ActionStatePart,
                                                            new ActionMachineTime(Time.deltaTime,
                                                                _statePart.ElapsedTime, _actionEvent.TriggerTime,
                                                                _actionEvent.Duration));
                                                    }
                                                }
                                            }
                                        }
                                        // _events = _actionState.EventList;
                                        // EngineDebug.Log("事件轨数量: " + _events.Count);
                                    }
                                    else
                                    {
                                        EngineDebug.LogError("不存在？？");
                                        continue;
                                    }
                                }
                            }

                            if (ActionWindowMain.ScenceDraw_Interrupt)
                            {
                                foreach (ActionInterrupt _interrupt in _statePart.CurActionInterrupt)
                                {
                                    foreach (IInterruptCondition _condition in _interrupt.InterruptConditionList)
                                    {
                                        _condition.EditorDraw(_config,
                                            new ActionMachineTime(Time.deltaTime, _statePart.ElapsedTime,
                                                _interrupt.TriggerTime, _interrupt.Duration));
                                    }
                                }
                            }
                        }
                    }
                }
                EngineDrawListData.Instance.Draw(deltaTime);
            }
        }
    }
#endif

}
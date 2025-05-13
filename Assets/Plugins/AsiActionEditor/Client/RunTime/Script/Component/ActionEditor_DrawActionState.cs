using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    public class ActionEditor_DrawActionState : MonoBehaviour
    {
        private class SActionStateDrawData
        {
            public ActionState actionState;
            public int jumpStateIndex;
            public float jumpTime;
            public bool isCur;
            public List<string> actionStateNames;

            public float setAnimTime;
            public float setAnimTimeNow;
            public float setAnimTime_2;
            public float setAnimTimeNow_2;
            public SActionStateDrawData(ActionState actionState, int jumpStateIndex, float jumpTime, bool isCur,
                List<string> actionStateNames)
            {
                this.actionState = actionState;
                this.jumpStateIndex = jumpStateIndex;
                this.jumpTime = jumpTime;
                this.isCur = isCur;
                this.actionStateNames = actionStateNames;

                setAnimTime = -1;
                setAnimTimeNow = -1;
                setAnimTime_2 = -1;
                setAnimTimeNow_2 = -1;
            }

            public void Clone(SActionStateDrawData other)
            {
                this.actionState = other.actionState;
                this.jumpStateIndex = other.jumpStateIndex;
                this.jumpTime = other.jumpTime;
                this.isCur = other.isCur;
                this.actionStateNames.Clear();
                this.actionStateNames.AddRange(other.actionStateNames);
                this.setAnimTime = other.setAnimTime;
                this.setAnimTimeNow = other.setAnimTimeNow;
                this.setAnimTime_2 = other.setAnimTime_2;
                this.setAnimTimeNow_2 = other.setAnimTimeNow_2;
            }

            public void StartAnim(float animTime)
            {
                setAnimTimeNow += animTime;
                if (isCur)
                {
                    setAnimTime_2 = 0.5f;
                    setAnimTimeNow_2 = setAnimTime_2;
                }
            }

            public float AnimTime(float updateTime)
            {
                if (setAnimTimeNow > 0)
                {
                    setAnimTimeNow -= updateTime;
                    if (setAnimTimeNow > 0)
                        return setAnimTimeNow / setAnimTime;
                }
                return 0;
            }
            
            public float AnimTime2(float updateTime)
            {
                if (setAnimTimeNow_2 > 0)
                {
                    setAnimTimeNow_2 -= updateTime;
                    if (setAnimTimeNow_2 > 0)
                        return setAnimTimeNow_2 / setAnimTime_2;
                }
                return 0;
            }
        }

        //绘制UI的配置
        private const int childHeight = 20; //成员高度
        private const int interal = 3; //成员间隔
        private const int windowsWidth = 200;
        private const int windowsWidthInteral = 10;

        public int m_CheckLayerID = 0;
        public int m_DrawHistory = 4;
        public float m_AnimTime = 0.2f;
        public float drawHeight = 0;


        private Unit unit;
        private bool isActive;
        private SActionStateDrawData[] actionStates;
        private int ActionStateJumpCount = 0;
        private float ActionLastTime = 0;
        private ActionState lastActionState = null;
        private ActionStatePart statePart;
        private List<string> stateNames = new List<string>();

        private void Start()
        {
            actionStates = new SActionStateDrawData[m_DrawHistory + 2];
            for (int i = 0; i < actionStates.Length; i++)
            {
                actionStates[i] = new SActionStateDrawData(null, -1, -1, false, new List<string>());
                actionStates[i].setAnimTime = m_AnimTime;
            }

            isActive = TryGetComponent(out unit);
            if (!isActive)
            {
                EngineDebug.LogWarning("未获取到unit组件");
            }
        }

        private void OnGUI()
        {
            if (!isActive) return;
            //获取当前Action
            statePart = unit.ActionStateMachine.AllActionStatePart[m_CheckLayerID];
            ActionState _curAction = statePart.CurrentActionState;

            //检查当前Action变化
            if (lastActionState != _curAction)
            {
                ActionStateJumpCount++;
                ChangeActionState(_curAction);
                lastActionState = _curAction;
            }

            //更新当前经过的时间
            ActionLastTime = statePart.ElapsedTime;

            //
            float deltaTime = Time.deltaTime;

            //绘制
            for (int i = 0; i < actionStates.Length; i++)
            {
                if (ActionStateJumpCount > i)
                {
                    float _pos_X = (actionStates.Length - 2 - i) * (windowsWidth + windowsWidthInteral);
                    DrawAction(actionStates[i], statePart, new Vector2((_pos_X), drawHeight), windowsWidth, deltaTime);
                }
                else
                {
                    break;
                }
            }
            // DrawAction(actionStates[0], statePart, new Vector2(windowsWidth * m_DrawHistory,0), windowsWidth);
        }

        private void DrawAction(SActionStateDrawData _curAction, ActionStatePart _statePart, Vector2 _pos, float _width,
            float _deltaTime)
        {
            Color color = GUI.color;

            int actionCount = _curAction.actionStateNames.Count;

            int mainHeight = childHeight + interal;

            //X轴位置
            _pos.x += _width * (_curAction.AnimTime(_deltaTime));

            //头部高度
            float _headHeight = 20;
            GUI.color = Color.blue;
            GUI.Label(new Rect(_pos.x, _pos.y, _width, mainHeight), $"  层级({m_CheckLayerID})  " + _curAction.actionState.Name);
            GUI.color = color;


            //获得宽高
            Vector2 windowSize = new Vector2(_width, actionCount * mainHeight + _headHeight);

            //绘制背景
            float pintAnim = _curAction.AnimTime2(_deltaTime) * 50;
            GUI.Box(new Rect(_pos.x - pintAnim, _pos.y, windowSize.x + pintAnim*2, windowSize.y + pintAnim), "");

            //绘制TimeLine
            if (_curAction.isCur)
            {
                float time = ActionLastTime > 0 ? ActionLastTime / _curAction.actionState.TotalTime : 0;
                GUI.Box(
                    new Rect(_pos.x + _width * time, _pos.y + _headHeight, 0,
                        actionCount * mainHeight),
                    "");
            }
            else
            {
                if (_curAction.jumpTime > -0.001f)
                {
                    GUI.Box(
                        new Rect(_pos.x + _width * _curAction.jumpTime, _pos.y + _headHeight, 0,
                            actionCount * mainHeight),
                        "");
                }
            }

            //绘制组内成员
            for (int i = 0; i < actionCount; i++)
            {
                if (i == _curAction.jumpStateIndex)
                {
                    GUI.color = Color.red;
                    Vector2 _childPos = new Vector2(_pos.x, mainHeight * i + 20);
                    GUI.Box(new Rect(_childPos.x, _pos.y + _childPos.y, _width, mainHeight),
                        _curAction.actionStateNames[i]);
                    GUI.color = color;
                }
                else
                {
                    Vector2 _childPos = new Vector2(_pos.x, mainHeight * i + 20);
                    GUI.Box(new Rect(_childPos.x, _pos.y + _childPos.y, _width, mainHeight),
                        _curAction.actionStateNames[i]);
                }
            }
            // Handles.
        }

        private void ChangeActionState(ActionState newActionState)
        {
            for (int i = actionStates.Length - 1; i > 0; i--)
            {
                actionStates[i].Clone(actionStates[i - 1]);
            }

            stateNames.Clear();
            //获取当前所有打断轨
            foreach (ActionInterrupt actionState in newActionState.InterruptList)
            {
                stateNames.Add(actionState.ActionName);
            }
            
            foreach (ActionInterruptGroup interruptGroup in newActionState.InterruptGroupList)
            {
                if (statePart.ActionStates.TryGetValue(interruptGroup.ActionID, out ActionState actionState))
                {
                    stateNames.Add("打断组：" + actionState.Name);
                    if (actionState.InterruptList.Count != interruptGroup.ActionHide.Count)
                    {
                        foreach (var _target in actionState.InterruptList)
                        {
                            stateNames.Add(_target.ActionName);
                        }
// #if UNITY_EDITOR
//                         EngineDebug.LogWarning("序列化可能存在错误，打断组配置丢失");
// #endif
                        continue;
                    }
                    for (int i = 0; i < actionState.InterruptList.Count; i++)
                    {
                        if (!interruptGroup.ActionHide[i])
                        {
                            stateNames.Add(actionState.InterruptList[i].ActionName);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(newActionState.DefaultAction))
            {
                stateNames.Add("结束后默认衔接的动画：");
                stateNames.Add(newActionState.DefaultAction);
            }


            actionStates[1].isCur = false;
            actionStates[1].jumpTime =
                ActionLastTime > 0 ? ActionLastTime / actionStates[1].actionState.TotalTime : 0;
            actionStates[1].jumpStateIndex = -1;
            int findID = 0;
            foreach (string name in actionStates[1].actionStateNames)
            {
                if (name == newActionState.Name)
                {
                    actionStates[1].jumpStateIndex = findID;
                    break;
                }

                findID++;
            }

            actionStates[0].actionState = newActionState;
            actionStates[0].actionStateNames.Clear();
            actionStates[0].actionStateNames.AddRange(stateNames);
            actionStates[0].isCur = true;
            actionStates[0].jumpTime = -1;
            actionStates[0].jumpStateIndex = -1;

            foreach (SActionStateDrawData VARIABLE in actionStates)
            {
                VARIABLE.StartAnim(m_AnimTime);
            }
        }
    }
}
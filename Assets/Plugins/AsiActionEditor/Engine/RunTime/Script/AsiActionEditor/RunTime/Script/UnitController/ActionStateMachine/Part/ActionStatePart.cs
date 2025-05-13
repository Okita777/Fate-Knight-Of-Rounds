using System.Collections.Generic;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    // private delegate 
    public partial class ActionStatePart
    {
        public readonly int mCurActionLayer;
        
        public bool ActionEnble { get; private set; }//当前状态机是否在运行
        public float ElapsedTime;//当前状态已经经过的时间 毫秒
        public float ElapsedTime_last => mElapsedTime_last;//切换Action时，上一个Action的时间

        public ActionState CurrentActionState { get; private set; }//当前执行的主要事件
        public ActionState CurrentActionState_Last { get; private set; }//切换Action时，上一个Action
        
        public ActionStateMachine ActionStateMachine => mActionStateMachine;
        public List<ActionEvent> CurrentActionEvents => mCurrentActionEvents;
        public List<ActionInterrupt> CurActionInterrupt => mCurActionInterrupt;
        public Dictionary<int, ActionState> ActionStates => mActionStateMachine.ActionStates;
        public Dictionary<string, int> ActionStateID => mActionStateMachine.ActionStateID;
        
        /// <summary>
        /// 进入Action
        /// </summary>
        /// <param name="_action"></param>
        public void OnEnter(ActionState _action) => OnEnterState(_action);
        
        /// <summary>
        /// 逻辑更新(轨道事件、跳转事件、攻击判定等)
        /// </summary>
        /// <param name="_deltaTime">单帧耗时</param>
        public void OnUpdate(float _deltaTime) => OnUpdateState(_deltaTime);
        /// <summary>
        /// 逻辑更新(轨道事件、跳转事件、攻击判定等)
        /// </summary>
        /// <param name="_deltaTime">单帧耗时</param>
        public void OnLateUpdate(float _deltaTime) => OnLateUpdateState(_deltaTime);
        
        /// <summary>
        /// 动画更新(更新动画、IK、注视等角色相关表现)
        /// </summary>
        /// <param name="_deltaTime">单帧耗时</param>
        public void UpdateAnima(float _deltaTime) => OnUpdateAnim(_deltaTime);
        
        /// <summary>
        /// 强行切换角色当前行为状态
        /// </summary>
        /// <param name="_actionName">行为状态名称</param>
        /// <param name="_mixTime">融合时间(毫秒)）</param>
        /// <param name="_offsetTime">剪切时间 (毫秒)</param>
        public bool ChangeState(string _actionName, int _mixTime = 0, int _offsetTime = 0) =>
            OnChangeState(_actionName, _mixTime, _offsetTime);
        public bool ChangeState(ActionState _actionName, int _mixTime = 0, int _offsetTime = 0) =>
            OnChangeState(_actionName, _mixTime, _offsetTime);
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="_name">动画名称</param>
        /// <param name="_mixTime">混合时间</param>
        /// <param name="_layer">所在层级</param>
        /// <param name="_offsetTime">剪切时间</param>
        public void PlayAnim(string _name, float _mixTime, int _layer, float _offsetTime) =>
            OnPlayAnim(_name, _mixTime, _layer, _offsetTime);
        
        /// <summary>
        /// 设置状态机运行状态
        /// </summary>
        /// <param name="_enble"></param>
        public void SetActionEnble(bool _enble = true)
        {
            ActionEnble = _enble;
        }
        
        /// <summary>
        /// 获取状态层级
        /// </summary>
        /// <param name="_action">状态</param>
        /// <returns></returns>
        public string GetActionType(ActionState _action) =>
            mActionStateMachine.ActionStateInfo.mActionType[_action.ActionLable];
        public string GetActionType() => mActionStateMachine.ActionStateInfo.mActionType[CurrentActionState.ActionLable];
    }
}
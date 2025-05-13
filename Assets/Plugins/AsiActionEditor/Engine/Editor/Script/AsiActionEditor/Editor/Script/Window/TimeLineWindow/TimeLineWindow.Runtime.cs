using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class TimeLineWindow
    {
        public bool IsEditor => mIsEditor;
        public Unit SelectUnit => mSelectUnit;
        
        private bool mIsPlayGame => Application.isPlaying;//是否在播放状态
        private Unit mSelectUnit = null;
        private bool mIsEditor = false;

        private int mNowSelectUnitIndex = -1;

        private int mCurAction = -999;
        public void RunTimeStart()
        {
            mSelectUnit = null;
            mIsEditor = false;
        }
        
        public void SetActionEditorState(bool _state)
        {
            if (mIsEditor != _state)
            {
                if (_state)
                {//进入编辑模式
                    if(mSelectUnit == null)
                        return;
                    
                    mSelectUnit.ActionStateMachine.CurAnimator.speed = 0;
                    mSelectUnit.ActionStateMachine.TimeScale = 0;
                    mPreviweState = EPreviweState.Pause;
                    TimeLineWindow.Instance.Repaint();
                    mNowTime = (int)mSelectUnit.ActionStateMachine.AllActionStatePart[0].ElapsedTime;
                }
                else
                {//退出编辑模式
                    // ActionStateMachine _stateMachine = mSelectUnit.ActionStateMachine;
                    mSelectUnit.ActionStateMachine.CurAnimator.speed = 1;
                    mSelectUnit.ActionStateMachine.TimeScale = 1;
                    mSelectUnit.ActionStateMachine.ChangeAction(SelectActionState.Name, 0, NowTime);

                    mSelectUnit.ActionStateMachine.AllActionStatePart[0].ElapsedTime = NowTime;
                    mPreviweState = EPreviweState.Play;
                    
                    ActionStateMachine _stateMachine = mSelectUnit.ActionStateMachine;
                    //初始化时间轴位置
                    ResourcesWindow.Instance.InitAnimatorData(_stateMachine.CurAnimator);
                    int _nowPlayID = _stateMachine.AllActionStatePart[0].CurrentActionState.ID;
                    if (ResourcesWindow.Instance.ActionStateDic.TryGetValue(_nowPlayID, out EditorActionState _actionState))
                    {
                        ResourcesWindow.Instance.OnSelectActionState(_actionState);
                    }
                }
                mIsEditor = _state;
            }
        }
        
        //更新场景中选择的单位
        public void RunTimeUpdateUnit(Unit _unit)
        {
            if (mSelectUnit != _unit)
            {
                //获取加载所需信息
                ActionStateMachine _stateMachine = _unit.ActionStateMachine;
                int _unitID = _unit.UnitID;
                string _actionName = _stateMachine.ActionGroupName;

                //找到所属的 Unit 列表对象
                int _selectID = 0;
                bool _isFind = false;
                foreach (var VARIABLE in ResourcesWindow.Instance.UnitWarp)
                {
                    if (VARIABLE.ID == _unitID)
                    {
                        ResourcesWindow.Instance.OnSelectUnit(_selectID);
                        mNowSelectUnitIndex = _selectID;
                        _isFind = true;
                        break;
                    }
                    _selectID++;
                }

                //检查是否有找到对象
                if (!_isFind)
                {
                    EngineDebug.LogWarning($"单位加载出错, ID： {_unitID}");
                    return;
                }
                mSelectUnit = _unit;
                
                //加载Editor动作列表
                ResourcesWindow.Instance.SetActionStateName(_actionName);
                ResourcesWindow.Instance.LoadActionStateInfo(_actionName);
                ResourcesWindow.Instance.SetActionStateName(_actionName, false);

                //初始化时间轴位置
                ResourcesWindow.Instance.InitAnimatorData(_stateMachine.CurAnimator);
                int _nowPlayID = _stateMachine.AllActionStatePart[0].CurrentActionState.ID;
                if (ResourcesWindow.Instance.ActionStateDic.TryGetValue(_nowPlayID, out EditorActionState _actionState))
                {
                    ResourcesWindow.Instance.OnSelectActionState(_actionState);
                }

            }
            else
            {
                ResourcesWindow.Instance.OnSelectUnit(mNowSelectUnitIndex);
            }
            //重新绘制面板
            ResourcesWindow.Instance.Repaint();
        }

        //跟随角色状态更新TimeLine
        public void RuntimeUpdateFrame(float _deltaTime)
        {
            if (mIsEditor)
            {
                if(mSelectUnit is not null) mSelectUnit.ActionStateMachine.AllActionStatePart[0].ElapsedTime = NowTime;
                return;
            }

            if (mSelectUnit == null)
            {
                mIsEditor = false;
                return;
            }
            ActionStateMachine _stateMachine = mSelectUnit.ActionStateMachine;

            //更新时间轴
            UpdateTime((int)_stateMachine.AllActionStatePart[0].ElapsedTime);

            //绘制场景图形
            OnEditorScenceDraw(_stateMachine);
            
            //检查动作跳转
            int _nowPlayActionID = _stateMachine.AllActionStatePart[0].CurrentActionState.ID;
            if (mCurAction != _nowPlayActionID)
            {
                if (ResourcesWindow.Instance.ActionStateDic.TryGetValue(_nowPlayActionID,
                        out EditorActionState _actionState))
                {
                    ResourcesWindow.Instance.OnSelectActionState(_actionState);
                    ResourcesWindow.Instance.Repaint();
                }
                mCurAction = _nowPlayActionID;
            }
            
            TimeLineWindow.Instance.Repaint();
        }
        
        //Runtime时绘制
        public void OnEditorScenceDraw(ActionStateMachine _stateMachine)
        {
            // foreach (ActionStatePart _actionStatePart in _stateMachine.AllActionStatePart)
            // {
            //     if (_actionStatePart.ActionEnble)
            //     {
            //         // ActionState _actionState = _actionStatePart.CurrentActionState;
            //         foreach (ActionInterrupt _interrupt in _actionStatePart.CurActionInterrupt)
            //         {
            //             foreach (IInterruptCondition _condition in _interrupt.InterruptConditionList)
            //             {
            //                 _condition.EditorDraw();
            //             }
            //         }
            //     }
            // }
        }
        
        //保存时更新Runtime内的数据
        public void RuntimeSaveAndUpdate()
        {
            if(mSelectUnit == null)return;

            ActionWindowMain.ActionEditorFuntion.ReLoadAcrionList(mSelectUnit.ActionStateMachine.ActionGroupName);
            // //重新生成 ActionStateMachine
            // int ElapsedTime = (int)mSelectUnit.ActionStateMachine.AllActionStatePart[0].ElapsedTime;
            // if (mSelectUnit.TryGetComponent(out ActionPreviewMark _mark))
            // {
            //     _mark.ReLoadActionInfo();
            // }
            // ActionStateMachine _stateMachine = mSelectUnit.ActionStateMachine;
            // _stateMachine.TimeScale = mIsEditor ? 0 : 1;
            //
            // _stateMachine.ChangeAction(SelectActionState.Name, 0, 0);
            // _stateMachine.AllActionStatePart[0].ElapsedTime = ElapsedTime;
            // // EngineDebug.LogWarning($"当前Action名称: {SelectActionState.Name}");
            //
            // UpdateTime(ElapsedTime);
        }
    }
}
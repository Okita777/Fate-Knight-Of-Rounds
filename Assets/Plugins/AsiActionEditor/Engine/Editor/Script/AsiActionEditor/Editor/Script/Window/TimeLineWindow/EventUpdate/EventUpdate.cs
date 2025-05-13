using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class EventUpdate
    {
        #region Instance

        private static EventUpdate _Instance;
        public static EventUpdate Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new EventUpdate();
                }
                return _Instance;
            }
        }

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            ActionWindowMain.ActionEditorFuntion.TimeLineInit();
        }
        
        /// <summary>
        /// 在Editor下执行事件
        /// </summary>
        /// <param name="_time">当前时间</param>
        /// <param name="_actionEvent">执行的事件</param>
        /// <returns></returns>
        public bool EditorEventUpdate(int _time, EditorActionEvent _actionEvent)
        {
            IActionEventData _eventData = _actionEvent.EventData;
            bool _flag = true;

            if (_eventData is Event_PlayAnim _eventPlayAnim)
            {
                Update_EventPlayAnim(_time, _eventPlayAnim, _actionEvent);
            }//动画更新
            // else if(_eventData is Event_AttackBox _eventAttackBox)
            // {
            //     Update_EventAttackBox(_time, _eventAttackBox, _actionEvent);
            // }//攻击盒绘制
            else
            {
                _flag = ActionWindowMain.ActionEditorFuntion.TimeLineUpdate(_time, _actionEvent);
            }

            return _flag;
        }

        public void EditorEventDraw(int _time, EditorActionEvent _actionEvent)
        {
            if (ResourcesWindow.Instance.TryGetCharacterConfig(out CharacterConfig config))
            {
                if (ActionWindowMain.ScenceDraw_Event)
                    _actionEvent.EventData.EditorDraw(config,ResourcesWindow.Instance.ActionStatePart,
                        new ActionMachineTime(1, _time, _actionEvent.TriggerTime, _actionEvent.Duration)); //调用内部自定绘制
            }
        }

        public void EditorConditionDraw(int _time, ActionInterrupt _actionInterrupt)
        {
            if (ResourcesWindow.Instance.TryGetCharacterConfig(out CharacterConfig config))
            {
                foreach (IInterruptCondition VARIABLE in _actionInterrupt.InterruptConditionList)
                {
                    if (ActionWindowMain.ScenceDraw_Interrupt)
                        VARIABLE.EditorDraw(config,
                            new ActionMachineTime(1, _time, _actionInterrupt.TriggerTime, _actionInterrupt.Duration));
                }
            }
        }


        #region Function
        private void Update_EventPlayAnim(int _time, Event_PlayAnim _eventPlayAnim, EditorActionEvent _actionEvent)
        {
            Animator _roleAnim = ResourcesWindow.Instance.RoleAnim;
            if (_roleAnim != null)
            {
                float playPropertion = 0;
                if (_time > _actionEvent.TriggerTime)
                {
                    playPropertion = (float)(_time - _actionEvent.TriggerTime) / _actionEvent.Duration;
                }

                // Debug.Log($"播放动画: {_eventPlayAnim.mAnimName}\n动画层级: {SelectActionState.AnimaLayer}");
                //非Script层才会播放动画
                EditorActionState _actionState = ResourcesWindow.Instance.GetEditorActionStateToSelect();
                if (_actionState.AnimaLayer < 4)
                {
                    _roleAnim.Play(_eventPlayAnim.AnimName, _actionState.AnimaLayer,
                        Mathf.Min(1, playPropertion));
                    _roleAnim.Update(0);
                }
            }
        }
        
        #endregion


    }
}
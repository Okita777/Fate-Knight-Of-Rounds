using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public class ActionEventCreact
    {
        public static EditorActionEvent Creact(int _id)
        {
            EditorActionEvent _action = null;

            if (_id < 0)
            {
                EEvenTypeInternal _eventType = (EEvenTypeInternal)(-_id);
                // EditorActionEvent _actionEvent = null; Inheritable
                switch (_eventType)
                {
                    case EEvenTypeInternal.EET_AttackBox:
                        _action = CreacteAttackBox();
                        break;

                    case EEvenTypeInternal.EET_DTD_PlayAnim:
                        _action = new EditorActionEvent(new Event_PlayAnim());
                    break;
                    case EEvenTypeInternal.EET_RayCast:
                        _action = new EditorActionEvent(new Event_Cast_Ray(),500);
                        break;
                    case EEvenTypeInternal.EET_SetGFloat:
                        _action = new EditorActionEvent(new Event_SetGFloat(),500);
                        break;
                    case EEvenTypeInternal.EET_SetGInt:
                        _action = new EditorActionEvent(new Event_SetGInt(),500);
                        break;
                    case EEvenTypeInternal.EET_SetGPoint:
                        _action = new EditorActionEvent(new Event_SetGPoint(),500);
                        break;
                    default:
                        EngineDebug.LogError($"未写明 [<color=#FFCC00>{_eventType}</color>] 的实例化，请告知程序");
                        break;
                }
            }
            else
            {
                _action = ActionWindowMain.ActionEditorFuntion.CreactActionEvent(_id);
                // if (_actionEvent is not null)
                //     return _actionEvent;
            }
            
            ResourcesWindow.Instance.ActionOnChange();
            return _action;
        }

        #region CreactEditorAction
        
        private static EditorActionEvent CreacteAttackBox()
        {
            GameObject _role = ResourcesWindow.Instance.GetRole();
            if (_role != null && _role.TryGetComponent(out CharacterConfig _config))
            {
                Event_AttackBox _eventAttackBox = new Event_AttackBox();
                _eventAttackBox.AttackInfo = ActionWindowMain.ActionEditorFuntion.AttackInfo();
                
                EditorActionEvent _actionEvent = new EditorActionEvent(_eventAttackBox,500,1);
                _actionEvent.EditorInheritable = true;
                _actionEvent.Inheritable = true;
                return _actionEvent;
            }

            EditorUtility.DisplayDialog("警告", "请给对象挂载CharacterConfig组件", "我知道了");
            return null;
        }
        
        private static EditorActionEvent CreatAction(IActionEventData _event, bool _Single = false)
        {
            return new EditorActionEvent(_event, _Single ? 0 : AsiActionEngine.RunTime.MotionEngineConst.TimeDoubling);
        }
        private static EditorActionEvent CreatAction(IActionEventData _event, int _Duration)
        {
            return new EditorActionEvent(_event, _Duration);
        }

        #endregion

        
        public static EditorActionInterrupt CreactInterrupt(int _id)
        {
            ResourcesWindow.Instance.ActionOnChange();

            _id *= -1;
            EEvenTypeInternal _eventType = (EEvenTypeInternal)_id;
            switch ((EEvenTypeInternal)_eventType)
            {
                case EEvenTypeInternal.EET_Interrupt:
                    return new EditorActionInterrupt();
            }
            
            return null;
        }

        public static IInterruptCondition CreactCondition(int _id)
        {
            ResourcesWindow.Instance.ActionOnChange();

            EInterruptTypeInternal _interruptType = (EInterruptTypeInternal)(-_id);
            if (_id < 0)
            {
                switch (_interruptType)
                {
                    case EInterruptTypeInternal.EIT_CheckInput:
                        return new CheckCostomKey();
                    case EInterruptTypeInternal.EIT_CheckMove:
                        return new CheckMoveState();
                    case EInterruptTypeInternal.EIT_CheckOnHit:
                        return new CheckOnHit();
                    case EInterruptTypeInternal.EIT_CheckBeHit:
                        return new CheckBeHit();
                    case EInterruptTypeInternal.EIT_CheckWeightRange:
                        return new CheckWeightRange();
                    default:
                        EngineDebug.LogError("未配置实例化!! 请告知TA  " + _interruptType);
                        return null;
                }
            }
            else
            {
                return ActionWindowMain.ActionEditorFuntion.CreactCondition(_id);
            }
        }
    }
}
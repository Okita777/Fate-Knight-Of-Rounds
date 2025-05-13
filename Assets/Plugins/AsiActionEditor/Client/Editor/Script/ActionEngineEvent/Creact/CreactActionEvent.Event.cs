using AsiActionEditor_Ex.RunTime;
using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using AsiTimeLine.RunTime;

namespace AsiTimeLine.Editor
{
    public partial class CreactActionEvent
    {
        public static EditorActionEvent Events(int _id)
        {
            EEvenType _eventType = (EEvenType)_id;
            switch (_eventType)
            {
                //Camera
                case EEvenType.EET_CameraChange:
                    return CreatAction(new Event_CameraChange(),-100);
                case EEvenType.EET_CameraShake:
                    return CreatAction(new Event_CameraShake());
                
                //FindTarget
                case EEvenType.EET_Event_FindTargetToSphere:
                    return CreatAction(new Event_FindTargetToSphere());

                //Misc
                case EEvenType.EET_ActionDebug:
                    return CreatAction(new Event_ActionDebug(),true);//单帧
                case EEvenType.EET_Partocle:
                    return CreatAction(new Event_PlayParticle(),true);//单帧
                case EEvenType.EET_SetGValue:
                    return CreatActionInheritable(new Event_SetGValue(),true);//单帧, 可继承
                case EEvenType.EET_SetGvalue_Transform:
                    return CreatAction(new Event_SetGvalue_Transform(),true);//单帧
                case EEvenType.EET_RaycastHit:
                    return CreatActionInheritable(new Event_RaycastHit());//可继承
                case EEvenType.EET_Audio:
                    return CreatAction(new Event_PlayAudio(),true);//单帧
                
                //Anima
                case EEvenType.EET_SetAnimFloat:
                    return CreatActionInheritable(new Event_SetAnimFloat(),true);//单帧, 可继承
                case EEvenType.EET_InteractBarrier: 
                    return CreatAction(new Event_InteractBarrier());
                case EEvenType.EET_RootWeight:
                    return CreatActionInheritable(new Event_RootWeight());//可继承
                case EEvenType.EET_TargetingMove:
                    return CreatActionInheritable(new Event_TargetingMove());//可继承
                case EEvenType.EET_TowBoneIK:
                    return CreatActionInheritable(new Event_TowBoneIK());//可继承
                case EEvenType.EET_SetPointData:
                    return CreatActionInheritable(new Event_SetPointData());//可继承
                
                //ItemInteract
                case EEvenType.EET_Attach:
                    return CreatAction(new Event_Attach(),true);//单帧
                
                //Unit
                // case EEvenType.EET_CharacterMove:
                //     return CreatActionInheritable(new Event_CharacterMove());//可继承
                case EEvenType.EET_CharacterGravity:
                    return CreatAction(new Event_CharacterGravity(),true);//单帧
                case EEvenType.EET_CharacterAddForce:
                    return CreatAction(new Event_CharacterAddForce(),true);//单帧
                case EEvenType.EET_CharacterOnMove:
                    return CreatActionInheritable(new Event_CharacterOnMove());//可继承
                case EEvenType.EET_UnitRot:
                    return CreatActionInheritable(new Event_UnitRot());//可继承
                case EEvenType.EET_TimeScale:
                    return CreatActionInheritable(new Event_TimeScale());//可继承
                case EEvenType.EET_ChangeUnitLayer:
                    return CreatActionInheritable(new Event_ChangeUnitLayer(), true);//可继承
                
                case EEvenType.EET_SoftLock:
                    return CreatActionInheritable(new Event_SoftLock());//可继承
                
                //动画状态标签管理
                case EEvenType.EET_Lable:
                    return CreatActionInheritable(new Event_Lable());//可继承
                case EEvenType.EET_Lable_Delay:
                    return CreatActionInheritable(new Event_Lable_Delay());//可继承
            }
            
            EngineDebug.LogError($"客户端下，未写明 [<color=#FFCC00>{_eventType}</color>] 的实例化，请告知客户端程序");
            return null;
        }
        
        private static EditorActionEvent CreatAction(IActionEventData _event, bool _Single = false)
        {
            return new EditorActionEvent(_event, _Single ? 0 : 333);//AsiActionEngine.RunTime.MotionEngineConst.TimeDoubling
        }
        private static EditorActionEvent CreatAction(IActionEventData _event, int _Duration)
        {
            return new EditorActionEvent(_event, _Duration);
        }
        
        //创建可上下继承内部参数的事件轨道
        private static EditorActionEvent CreatActionInheritable(IActionEventData _event, bool _Single = false)
        {
            EditorActionEvent _actionEvent = CreatAction(_event, _Single);
            _actionEvent.EditorInheritable = true;
            _actionEvent.Inheritable = true;
            return _actionEvent;
        }
        private static EditorActionEvent CreatActionInheritable(IActionEventData _event, int _Duration)
        {
            EditorActionEvent _actionEvent = CreatAction(_event, _Duration);
            _actionEvent.EditorInheritable = true;
            _actionEvent.Inheritable = true;
            return _actionEvent;
        }
    }
}
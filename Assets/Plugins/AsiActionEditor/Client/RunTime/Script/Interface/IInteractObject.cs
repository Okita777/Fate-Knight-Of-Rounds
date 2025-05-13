using AsiActionEngine.RunTime;

namespace AsiTimeLine.RunTime
{
    public interface IInteractObject
    {
        string m_InteractName();//交互时显示的名字
        //float m_InteractRange();//交互范围
        bool m_AutoTrigger();//自动触发交互
        void OnTrigger(ActionStateMachine _actionState){}
        void OnDropped(ActionStateMachine _actionState){}

        bool OnDrawInteract(ActionStateMachine _actionState) { return false; }
    }
}
namespace AsiActionEngine.RunTime.Event_Extend
{
    public class Ex_AttackBox : StaticActionLogics
    {
        public event OnAttackData OnHit; //切换Action时调用
        public delegate void OnAttackData(IAttackInfo _attackInfo);

        public override void OnUpdate(ActionStateMachine _actionState)
        {
        }

        public void ExtrudEvent(IAttackInfo _attackInfo)
        {
            OnHit?.Invoke(_attackInfo);
        }
    }
}
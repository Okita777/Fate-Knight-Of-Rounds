using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public interface IAttackInfo
    {
        public IAttackInfo Clone();
        /// <summary>
        /// 受击时调用
        /// </summary>
        /// <param name="_attackInfo">攻击信息</param>
        /// <param name="_stateMachine">自身状态机</param>
        /// <param name="_attacker">攻击者</param>
        /// <param name="_hitPoint">攻击位置</param>
        public void BeHit(IAttackInfo _attackInfo, ActionStateMachine _stateMachine, Unit _attacker, Vector3 _hitPoint);
        public void OnHit(IAttackInfo _attackInfo, ActionStateMachine _stateMachine, Unit _hiter, Vector3 _hitPoint);
        public void OnHitStart(ActionStatePart _statePart);
        public void OnHitUpdate(ActionStatePart _statePart, Vector3 _point, Quaternion _rotate);
        public void OnHitEnd(ActionStatePart _statePart);

    }
}
using UnityEngine;

/// <summary>
/// 动作执行器，专门负责执行角色各种动作
/// 不直接做物理位移，调用 PhysicsHandler 实现移动/跳跃/闪避
/// 动作取消窗口、动作优先级、技能冷却检测放在 Controller 和 StateManager 里
/// </summary>
public class PlayerActionManager : MonoBehaviour
{
    private PlayerStateManager state;
    private PlayerAnimatorManager anim;
    private PlayerPhysicsHandler physics;


    void Awake()
    {
        state = GetComponent<PlayerStateManager>();
        anim = GetComponent<PlayerAnimatorManager>();
        physics = GetComponent<PlayerPhysicsHandler>();

    }

    /// <summary>
    /// 普通移动（调用 PhysicsHandler 实现位移）
    /// </summary>
    public void Move(Vector3 moveDir, float speed)
    {
        // TODO: 调用 physics.MoveCharacter(moveDir, speed)
    }

    /// <summary>
    /// 跳跃动作
    /// </summary>
    public void Jump()
    {
        // TODO: 判断跳跃条件
        // TODO: 调用 physics.ApplyJumpForce()
        // TODO: 播放跳跃动画
    }

    /// <summary>
    /// 闪避动作
    /// </summary>
    public void Dodge(Vector3 dodgeDir)
    {
        // TODO: 判断闪避条件
        // TODO: 调用 physics.DodgeDash(dodgeDir)
        // TODO: 播放闪避动画
        // TODO: 开启无敌帧
    }

    /// <summary>
    /// 攻击动作
    /// </summary>
    public void Attack()
    {
        // TODO: 判断攻击条件
        // TODO: 播放攻击动画
        // TODO: 更新 ComboIndex
        // TODO: 调用 ComboManager
    }

    /// <summary>
    /// 技能释放动作
    /// </summary>
    public void UseSkill(int skillId)
    {
        // TODO: 检查技能条件
        // TODO: 调用技能释放逻辑
        // TODO: 播放技能动画
    }

    /// <summary>
    /// Combo连招中断（被打断/超时重置）
    /// </summary>
    public void BreakCombo()
    {
        // TODO: 重置连击索引
        // TODO: 播放硬直动画或重置动作状态
    }
}

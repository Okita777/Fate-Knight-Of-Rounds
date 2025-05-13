using UnityEngine;

/// <summary>
/// 角色状态管理器，专门记录角色当前所有状态
/// 负责处理：
/// - 动作状态
/// - 受击状态
/// - 无敌帧
/// - 连击索引
/// - 动作取消窗口
/// - 技能释放状态
/// </summary>
public class PlayerStateManager : MonoBehaviour
{
    // 动作状态
    public bool IsGrounded { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsDodging { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsUsingSkill { get; private set; }
    public bool IsInvincible { get; private set; }

    // 动作取消窗口
    public bool CanCancelAction { get; private set; }

    // 连招索引
    public int ComboIndex { get; private set; }

    void Awake()
    {
        // 初始化状态
    }

    /// <summary>
    /// 重置所有动作状态（受击/死亡/动作结束时调用）
    /// </summary>
    public void ResetAllStates()
    {
        IsGrounded = false;
        IsJumping = false;
        IsDodging = false;
        IsAttacking = false;
        IsUsingSkill = false;
        IsInvincible = false;
        CanCancelAction = false;
        ComboIndex = 0;
    }

    /// <summary>
    /// 设置连击索引
    /// </summary>
    public void SetComboIndex(int index)
    {
        ComboIndex = index;
    }

    /// <summary>
    /// 重置连击
    /// </summary>
    public void ResetCombo()
    {
        ComboIndex = 0;
    }

    /// <summary>
    /// 开启无敌帧
    /// </summary>
    public void EnableInvincible()
    {
        IsInvincible = true;
    }

    /// <summary>
    /// 关闭无敌帧
    /// </summary>
    public void DisableInvincible()
    {
        IsInvincible = false;
    }

    /// <summary>
    /// 设置动作取消窗口（例如闪避取消攻击）
    /// </summary>
    public void SetCancelWindow(bool canCancel)
    {
        CanCancelAction = canCancel;
    }

    /// <summary>
    /// 地面检测状态同步
    /// </summary>
    public void SetGroundedState(bool isGrounded)
    {
        IsGrounded = isGrounded;
    }

    /// <summary>
    /// 设置跳跃状态
    /// </summary>
    public void SetJumpingState(bool isJumping)
    {
        IsJumping = isJumping;
    }

    /// <summary>
    /// 设置闪避状态
    /// </summary>
    public void SetDodgingState(bool isDodging)
    {
        IsDodging = isDodging;
    }

    /// <summary>
    /// 设置攻击状态
    /// </summary>
    public void SetAttackingState(bool isAttacking)
    {
        IsAttacking = isAttacking;
    }

    /// <summary>
    /// 设置技能释放状态
    /// </summary>
    public void SetSkillState(bool isUsingSkill)
    {
        IsUsingSkill = isUsingSkill;
    }

    // 后续要加受击、硬直、霸体、蓄力状态也可以继续往这里扩展
}

using UnityEngine;

/// <summary>
/// 角色动画管理器，专门负责统一管理 Animator 参数、动画事件、RootMotionHook
/// 不直接控制动作，所有动作由 ActionManager 调用
/// </summary>
public class PlayerAnimatorManager : MonoBehaviour
{
    private Animator animator;

    // 常用参数 Hash 缓存（正式项目写法）
    private int speedHash;
    private int isDodgingHash;
    private int comboIndexHash;
    private int isGroundedHash;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // 缓存参数Hash
        speedHash = Animator.StringToHash("Speed");
        isDodgingHash = Animator.StringToHash("IsDodging");
        comboIndexHash = Animator.StringToHash("ComboIndex");
        isGroundedHash = Animator.StringToHash("IsGrounded");
    }

    /// <summary>
    /// 设置移动速度参数
    /// </summary>
    public void SetSpeed(float value)
    {
        // TODO: animator.SetFloat(speedHash, value)
    }

    /// <summary>
    /// 设置闪避状态参数
    /// </summary>
    public void SetDodging(bool state)
    {
        // TODO: animator.SetBool(isDodgingHash, state)
    }

    /// <summary>
    /// 设置Combo连段索引
    /// </summary>
    public void SetComboIndex(int index)
    {
        // TODO: animator.SetInteger(comboIndexHash, index)
    }

    /// <summary>
    /// 设置地面状态参数
    /// </summary>
    public void SetIsGrounded(bool state)
    {
        // TODO: animator.SetBool(isGroundedHash, state)
    }

    /// <summary>
    /// 播放 Trigger 动画
    /// </summary>
    public void SetTrigger(string triggerName)
    {
        // TODO: animator.SetTrigger(triggerName)
    }

    /// <summary>
    /// 取消 Trigger
    /// </summary>
    public void ResetTrigger(string triggerName)
    {
        // TODO: animator.ResetTrigger(triggerName)
    }

    /// <summary>
    /// RootMotion 动画事件接管
    /// </summary>
    public void OnAnimatorMoveEvent()
    {
        // TODO: 将 RootMotion 位移传给 PhysicsHandler
    }

    /// <summary>
    /// 动画事件监听器
    /// </summary>
    public void OnAnimationEvent(string eventName)
    {
        // TODO: 处理攻击判定 / Combo窗口 / 特效触发等事件
    }

    /// <summary>
    /// 获取当前动画状态
    /// </summary>
    public AnimatorStateInfo GetCurrentStateInfo(int layer = 0)
    {
        return animator.GetCurrentAnimatorStateInfo(layer);
    }
}

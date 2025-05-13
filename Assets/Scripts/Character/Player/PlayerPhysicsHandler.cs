using UnityEngine;

/// <summary>
/// 角色物理移动管理器，专门负责自定义位移、重力、碰撞检测、SlopeLimit检测、DodgeDash
/// 彻底取代 CharacterController，手动控制 CapsuleCast、Raycast、地面检测、物理位移
/// </summary>
public class PlayerPhysicsHandler : MonoBehaviour
{
    [Header("基础参数")]
    public float moveSpeed = 6f;
    public float gravity = -9.8f;
    public float jumpForce = 10f;
    public float dodgeDistance = 3f;
    public float slopeLimit = 45f;
    public LayerMask groundLayer;

    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 moveDir;

    [Header("碰撞体设置")]
    public CapsuleCollider capsule;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    void Awake()
    {
        if (capsule == null)
            capsule = GetComponent<CapsuleCollider>();
    }

    void FixedUpdate()
    {
        ApplyGravity();
        CheckGrounded();
        MoveCharacter();
    }

    /// <summary>
    /// 位移调用接口，ActionManager 调用这里实现移动
    /// </summary>
    public void SetMoveDirection(Vector3 direction)
    {
        moveDir = direction;
    }

    /// <summary>
    /// 角色位移执行（物理移动）
    /// </summary>
    void MoveCharacter()
    {
        // TODO: transform.position += moveDir * moveSpeed * Time.fixedDeltaTime;
    }

    /// <summary>
    /// 重力控制
    /// </summary>
    void ApplyGravity()
    {
        // TODO: velocity.y += gravity * Time.fixedDeltaTime;
        // TODO: transform.position += velocity * Time.fixedDeltaTime;
    }

    /// <summary>
    /// 地面检测
    /// </summary>
    void CheckGrounded()
    {
        // TODO: isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    /// <summary>
    /// Slope坡度检测
    /// </summary>
    public bool CheckSlope(Vector3 moveDir)
    {
        // TODO: CapsuleCast 前方检测坡度角度，返回是否超过 slopeLimit
        return false;
    }

    /// <summary>
    /// 跳跃接口
    /// </summary>
    public void ApplyJumpForce()
    {
        // TODO: velocity.y = jumpForce;
    }

    /// <summary>
    /// 闪避/突进Dash接口
    /// </summary>
    public void DodgeDash(Vector3 dashDir, float dashSpeed, float duration)
    {
        // TODO: 协程 or DOTween 位移，带碰撞检测
    }
}

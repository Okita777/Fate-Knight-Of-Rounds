using UnityEngine;

/// <summary>
/// 角色相机管理器，自定义相机跟随、旋转、锁定、镜头事件控制器
/// </summary>
public class PlayerCameraController : MonoBehaviour
{
    [Header("目标")]
    public Transform target;

    [Header("相机参数")]
    public float followSpeed = 10f;
    public Vector3 followOffset = new Vector3(0, 3, -6);
    public float lookSensitivity = 2f;
    public float minYAngle = -30f;
    public float maxYAngle = 60f;

    [Header("锁定相关")]
    public Transform lockTarget;
    public float lockSpeed = 5f;
    public bool isLockedOn;

    private float currentYaw;
    private float currentPitch;

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (target == null) return;

        if (isLockedOn)
        {
            LockOnCamera();
        }
        else
        {
            FreeLookCamera();
        }
    }

    /// <summary>
    /// 自由视角相机
    /// </summary>
    void FreeLookCamera()
    {
        // TODO: 接收输入，调整 currentYaw 和 currentPitch，Clamp 限制
        // TODO: 相机位置插值跟随 target + offset
        // TODO: 相机旋转插值应用
    }

    /// <summary>
    /// 锁定视角相机
    /// </summary>
    void LockOnCamera()
    {
        // TODO: 计算锁定目标方向
        // TODO: 相机位置/旋转跟随 target 与 lockTarget 中间点
    }

    /// <summary>
    /// 接收视角旋转输入（由 InputManager 调用）
    /// </summary>
    public void RotateCamera(Vector2 lookInput)
    {
        currentYaw += lookInput.x * lookSensitivity;
        currentPitch -= lookInput.y * lookSensitivity;
        currentPitch = Mathf.Clamp(currentPitch, minYAngle, maxYAngle);
    }

    /// <summary>
    /// 开启锁定
    /// </summary>
    public void LockOn(Transform targetToLock)
    {
        lockTarget = targetToLock;
        isLockedOn = true;
    }

    /// <summary>
    /// 取消锁定
    /// </summary>
    public void Unlock()
    {
        lockTarget = null;
        isLockedOn = false;
    }

    /// <summary>
    /// 镜头过渡（比如大招视角、BOSS入场）
    /// </summary>
    public void PlayCameraTransition(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        // TODO: Coroutine 或 DOTween 实现镜头过渡
    }
}

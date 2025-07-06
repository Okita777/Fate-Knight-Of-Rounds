using Character;
using okita.Character;
using UnityEngine;

namespace okita.Player
{
    /// <summary>
    /// 玩家控制器核心模块。
    /// 负责角色移动、旋转、物理交互等底层控制逻辑，是所有控制流的汇聚点。
    /// 接收 PlayerInputManager 的输入状态、PlayerActionManager 的动作指令，
    /// 调用物理模块 PlayerPhysicsHandler 完成角色位移和碰撞，驱动 AnimatorManager 切换动画状态。
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        PlayerThirdViewCamera playerThirdViewCamera;
        PlayerOverHeadCamera playerOverHeadCamera;

        private void Start()
        {
            print("PlayerController Start");
            //启用玩家输入
            PlayerInputManager.Instance.gameObject.SetActive(false);
            PlayerInputManager.Instance.gameObject.SetActive(true);
            Cursor.visible = false; // 隐藏鼠标光标
            Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标光标到屏幕中心
                                                      //增加第三人称视角组件
            playerThirdViewCamera = GetComponent<PlayerThirdViewCamera>();
            playerOverHeadCamera = GetComponent<PlayerOverHeadCamera>();

            playerThirdViewCamera.true_T_FollowTarget = PlayerCameraManager.instance.CreateVirtualPoint(playerThirdViewCamera.TViewPoint);
            PlayerCameraManager.instance.OnInit(playerThirdViewCamera.true_T_FollowTarget, playerThirdViewCamera.true_T_FollowTarget, "ThirdPersonCamera");
        }

        private void Update()
        {
            //测试，按下t键盘，切换为俯视角，按下y键，切换为第三人称过肩视角
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (PlayerCameraManager.instance.GetCurrentCameraName() == "OverHeadCamera")
                {
                    return; // 如果已经是俯视角，则不切换
                }
                PlayerCameraManager.instance.RecycleVirtualPoint();
                playerOverHeadCamera.true_T_FollowTarget = PlayerCameraManager.instance.CreateVirtualPoint(playerOverHeadCamera.TViewPoint);
                PlayerCameraManager.instance.SwitchCamera(playerOverHeadCamera.true_T_FollowTarget, playerOverHeadCamera.true_T_FollowTarget, "OverHeadCamera");
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                if (PlayerCameraManager.instance.GetCurrentCameraName() == "ThirdPersonCamera")
                {
                    return; // 如果已经是第三人称视角，则不切换
                }
                PlayerCameraManager.instance.RecycleVirtualPoint();
                playerThirdViewCamera.true_T_FollowTarget = PlayerCameraManager.instance.CreateVirtualPoint(playerOverHeadCamera.TViewPoint);
                PlayerCameraManager.instance.SwitchCamera(playerThirdViewCamera.true_T_FollowTarget, playerThirdViewCamera.true_T_FollowTarget, "ThirdPersonCamera");
            }
        }
    }
}

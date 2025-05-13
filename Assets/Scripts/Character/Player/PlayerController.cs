// using UnityEngine;

// namespace Game.Player
// {
//     /// <summary>
//     /// 玩家控制器核心模块。
//     /// 负责角色移动、旋转、物理交互等底层控制逻辑，是所有控制流的汇聚点。
//     /// 接收 PlayerInputManager 的输入状态、PlayerActionManager 的动作指令，
//     /// 调用物理模块 PlayerPhysicsHandler 完成角色位移和碰撞，驱动 AnimatorManager 切换动画状态。
//     /// </summary>
//     public class PlayerController : MonoBehaviour
//     {
//         // 输入管理器
//         public PlayerInputManager inputManager { get; private set; }

//         // 动作管理器
//         public PlayerActionManager actionManager { get; private set; }

//         // 动画管理器
//         public PlayerAnimatorManager animatorManager { get; private set; }

//         // 连段管理器
//         public PlayerComboManager comboManager { get; private set; }

//         // 物理管理器
//         public PlayerPhysicsHandler physicsHandler { get; private set; }

//         // 相机控制器
//         public PlayerCameraController cameraController { get; private set; }

//         private void Awake()
//         {
//             inputManager = GetComponent<PlayerInputManager>();
//             actionManager = GetComponent<PlayerActionManager>();
//             animatorManager = GetComponent<PlayerAnimatorManager>();
//             comboManager = GetComponent<PlayerComboManager>();
//             physicsHandler = GetComponent<PlayerPhysicsHandler>();
//             cameraController = FindObjectOfType<PlayerCameraController>(); // 一般相机是单例
//         }

//         private void Update()
//         {
//             if (inputManager == null) return;

//             // 更新移动、旋转、跳跃、闪避等控制
//             HandleMovement();
//             HandleRotation();
//             HandleActions();
//         }

//         private void HandleMovement()
//         {
//             // TODO: 将 inputManager.MoveInput 和物理参数传递给 physicsHandler 执行物理位移
//         }

//         private void HandleRotation()
//         {
//             // TODO: 旋转逻辑，决定角色朝向（可根据相机方向 + 移动输入）
//         }

//         private void HandleActions()
//         {
//             // TODO: 监测 inputManager 的动作输入，调用 actionManager 执行动作
//         }

//         private void LateUpdate()
//         {
//             // TODO: 相机跟随等操作
//         }
//     }
// }

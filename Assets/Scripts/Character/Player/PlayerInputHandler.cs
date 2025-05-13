// using UnityEngine;
// using UnityEngine.InputSystem;

// namespace Game.Player
// {
//     /// <summary>
//     /// 玩家输入管理器，负责接收来自 Unity 新输入系统（Input System）的输入事件，
//     /// 并将输入状态缓存到属性，供其他模块（如 PlayerController、PlayerActionManager）读取使用。
//     /// 包含 Move、Look、Jump、Dash、Attack、Ultimate 等基础输入监听。
//     /// </summary>
//     public class PlayerInputManager : MonoBehaviour
//     {
//         private PlayerInput playerInput;

//         /// <summary> 移动输入（二维轴向） </summary>
//         public Vector2 MoveInput { get; private set; }

//         /// <summary> 镜头输入（二维轴向） </summary>
//         public Vector2 LookInput { get; private set; }

//         /// <summary> 跳跃按键是否按下 </summary>
//         public bool JumpPressed { get; private set; }

//         /// <summary> 闪避按键是否按下 </summary>
//         public bool DashPressed { get; private set; }

//         /// <summary> 普通攻击是否按下 </summary>
//         public bool AttackPressed { get; private set; }

//         /// <summary> 必杀技是否按下 </summary>
//         public bool UltimatePressed { get; private set; }

//         private void Awake()
//         {
//             playerInput = GetComponent<PlayerInput>();
//             if (playerInput == null)
//             {
//                 Debug.LogError("PlayerInput 组件缺失！");
//             }
//         }

//         private void OnEnable()
//         {
//             var actions = playerInput.actions;

//             actions["Move"].performed += OnMove;
//             actions["Move"].canceled += OnMove;

//             actions["Look"].performed += OnLook;
//             actions["Look"].canceled += OnLook;

//             actions["Jump"].performed += ctx => JumpPressed = true;
//             actions["Jump"].canceled += ctx => JumpPressed = false;

//             actions["Dash"].performed += ctx => DashPressed = true;
//             actions["Dash"].canceled += ctx => DashPressed = false;

//             actions["Attack"].performed += ctx => AttackPressed = true;
//             actions["Attack"].canceled += ctx => AttackPressed = false;

//             actions["Ultimate"].performed += ctx => UltimatePressed = true;
//             actions["Ultimate"].canceled += ctx => UltimatePressed = false;
//         }

//         private void OnDisable()
//         {
//             var actions = playerInput.actions;

//             actions["Move"].performed -= OnMove;
//             actions["Move"].canceled -= OnMove;

//             actions["Look"].performed -= OnLook;
//             actions["Look"].canceled -= OnLook;
//         }

//         private void OnMove(InputAction.CallbackContext context)
//         {
//             MoveInput = context.ReadValue<Vector2>();
//         }

//         private void OnLook(InputAction.CallbackContext context)
//         {
//             LookInput = context.ReadValue<Vector2>();
//         }

//         /// <summary>
//         /// 重置所有输入状态，防止卡键。
//         /// </summary>
//         public void ResetInput()
//         {
//             MoveInput = Vector2.zero;
//             LookInput = Vector2.zero;
//             JumpPressed = false;
//             DashPressed = false;
//             AttackPressed = false;
//             UltimatePressed = false;
//         }
//     }
// }

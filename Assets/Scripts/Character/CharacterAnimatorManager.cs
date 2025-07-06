using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

namespace okita.Character
{



    /// <summary>
    /// 角色动画管理器，专门负责统一管理 Animator 参数、动画事件、RootMotionHook
    /// 不直接控制动作，所有动作由 ActionManager 调用
    /// </summary>
    public class CharacterAnimatorManager : MonoBehaviour
    {
        private Animator animator;

        // Animator 参数 Hash 缓存

        void Awake()
        {
            animator = GetComponent<Animator>();

            // 缓存参数Hash
            if (animator == null)
            {
                Debug.LogError("Animator component is missing on the GameObject.");
                return;
            }

        }

        //播放动画
        public void PlayAnim(string animName, float fixTime = 0.1f, int layer = 0, float offsetTime = 0f)
        {
            // 确保动画存在
            if (!animator.HasState(layer, Animator.StringToHash(animName)))
            {
                Debug.LogWarning($"Animator does not have state '{animName}' on layer {layer}");
                return;
            }

            // 使用 CrossFadeInFixedTime 播放动画
            animator.CrossFadeInFixedTime(animName, fixTime, layer, offsetTime);
        }

        //设置动画播放速度
        public void SetSpeed(float speed)
        {
            if (animator != null)
            {
                animator.speed = speed;
            }
            else
            {
                Debug.LogWarning("Animator is not initialized.");
            }
        }

        //是否应用RootMotion
        public void SetApplyRootMotion(bool apply)
        {
            if (animator != null)
            {
                animator.applyRootMotion = apply;
            }
            else
            {
                Debug.LogWarning("Animator is not initialized.");
            }
        }

        #region 设置动画参数
        public void SetBool(string paramName, bool value)
        {
            if (animator != null)
            {
                animator.SetBool(paramName, value);
            }
            else
            {
                Debug.LogWarning("Animator is not initialized.");
            }
        }

        public void SetFloat(string paramName, float value)
        {
            if (animator != null)
            {
                animator.SetFloat(paramName, value);
            }
            else
            {
                Debug.LogWarning("Animator is not initialized.");
            }
        }

        public void SetInteger(string paramName, int value)
        {
            if (animator != null)
            {
                animator.SetInteger(paramName, value);
            }
            else
            {
                Debug.LogWarning("Animator is not initialized.");
            }
        }

        public void SetTrigger(string paramName)
        {
            if (animator != null)
            {
                animator.SetTrigger(paramName);
            }
            else
            {
                Debug.LogWarning("Animator is not initialized.");
            }
        }
        #endregion

        //获取当前动画信息
        public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layer = 0)
        {
            if (animator != null)
            {
                return animator.GetCurrentAnimatorStateInfo(layer);
            }
            else
            {
                Debug.LogWarning("Animator is not initialized.");
                return default;
            }
        }


        // 获取状态动画的播放进度
        public float GetStateAnimationProgress(string stateName)
        {
            // TODO: 返回状态动画的播放进度
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(stateName))
            {
                // 计算标准化时间
                return stateInfo.normalizedTime % 1f; // 确保返回值在0-1之间
            }
            Debug.LogWarning($"Animator does not have state '{stateName}'");
            return 0f;
        }


        // 检查状态动画是否播放完成
        public bool IsStateAnimationComplete(string stateName)
        {
            // TODO: 检查动画是否播放完成
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(stateName))
            {
                // 检查标准化时间是否接近1（即动画播放完成）
                return stateInfo.normalizedTime >= 1f;
            }
            Debug.LogWarning($"Animator does not have state '{stateName}'");
            return false;
        }


        // 强制跳转到动画的指定时间点
        public void SeekStateAnimation(string stateName, float normalizedTime)
        {
            // TODO: 跳转到动画的指定时间点
            if (animator != null && animator.HasState(0, Animator.StringToHash(stateName)))
            {
                // 获取当前状态信息
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName(stateName))
                {
                    // 设置标准化时间
                    animator.Play(stateName, 0, normalizedTime);
                }
                else
                {
                    Debug.LogWarning($"Animator is not in state '{stateName}'");
                }
            }
            else
            {
                Debug.LogWarning($"Animator does not have state '{stateName}'");
            }
        }
    }
}

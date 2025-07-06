using Cinemachine;
using okita.Character;
using okita.Player;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Character
{
    [System.Serializable]
    public class PlayerThirdViewCamera : MonoBehaviour
    {

        //第三人称相机
        [Header("Third Person Camera")]
        public Transform TViewPoint;
        public Transform true_T_FollowTarget;

        //水平方向旋转
        private float horizontalRotation = 0f; // 水平方向旋转

        //竖直方向旋转
        private float verticalRotation = 0f; // 竖直方向旋转

        //竖直方向最大旋转角度
        [SerializeField] private float verticalRotationMax = 50f; // 竖直方向最大旋转角度

        //竖直方向最小旋转角度
        [SerializeField] private float verticalRotationMin = -50f; // 竖直方向最小旋转角度

        //一个可调节的滑轨,方便在编辑器中调整旋转速度的大小
        [SerializeField]
        [Range(1f, 50f)]
        public float rotationSpeed = 10f; // 旋转速度

        private void Awake()
        {

        }

        private void Start()
        {

        }


        private void Update()
        {

        }

        private void LateUpdate()
        {

            UpdateThirdCamera();
        }

        public void UpdateThirdCamera()
        {
            if (true_T_FollowTarget != null)
            {
                true_T_FollowTarget.position = TViewPoint.position;
                horizontalRotation += PlayerInputManager.Instance.cameraInput.x * Time.deltaTime * rotationSpeed;
                verticalRotation -= PlayerInputManager.Instance.cameraInput.y * Time.deltaTime * rotationSpeed;
                verticalRotation = Mathf.Clamp(verticalRotation, verticalRotationMin, verticalRotationMax);
                true_T_FollowTarget.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
            }
        }
    }
}


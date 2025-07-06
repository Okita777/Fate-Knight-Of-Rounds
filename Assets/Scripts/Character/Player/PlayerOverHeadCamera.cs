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
    public class PlayerOverHeadCamera : MonoBehaviour
    {

        //俯视角相机
        [Header("OverHeadCamera")]
        public Transform TViewPoint;
        public Transform true_T_FollowTarget;

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

            UpdateOverHeadCamera();
        }

        public void UpdateOverHeadCamera()
        {   //忽略竖直方向的移动
            if (true_T_FollowTarget != null)
            {
                true_T_FollowTarget.position = new Vector3(TViewPoint.position.x, true_T_FollowTarget.position.y, TViewPoint.position.z);
            }
        }
    }
}


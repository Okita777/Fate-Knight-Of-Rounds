using System;
using System.Collections.Generic;
using Cinemachine;
using okita.Player;
using UnityEngine;

namespace okita.Character
{
    public class CameraConfig
    {
        public string cameraName;
        public Transform lookTarget;

        public Transform followTarget;

        public bool createVirtualLookPoint;

        public bool createVirtualFollowPoint;
    }

    public class PlayerCameraManager : MonoBehaviour
    {
        //单例
        public static PlayerCameraManager instance;


        [SerializeField] private List<CinemachineVirtualCamera> allCinemachine = new List<CinemachineVirtualCamera>();

        public Dictionary<string, int> cameraIndexDic = new Dictionary<string, int>();


        public Transform virtualPointRoot;

        private string lastCamName;

        private string currentCamName;

        private Transform lookTarget;

        private Transform followTarget;

        private GameObject currentVirtualPoint;

        private CinemachineBrain brain;

        private int defaultPriority = 10;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            //转换字典初始化
            for (int i = 0; i < allCinemachine.Count; i++)
            {
                if (!cameraIndexDic.ContainsKey(allCinemachine[i].name))
                {
                    cameraIndexDic.Add(allCinemachine[i].name, i);
                }
            }

            brain = Camera.main.GetComponent<CinemachineBrain>();
        }

        private void Start()
        {
            if (virtualPointRoot == null)
            {
                Debug.LogWarning("VirtualPointRoot is not set, creating a new GameObject.");
            }
        }

        private void Update()
        {
            if (brain.IsBlending)
            {
                // 如果正在切换相机，禁用输入
                PlayerInputManager.Instance.gameObject.SetActive(false);
            }
            else
            {
                // 切换完成后启用输入
                PlayerInputManager.Instance.gameObject.SetActive(true);
            }


        }

        public void OnInit(Transform _lookTarget, Transform _followTarget, string cameraName, int priority = 20)
        {
            lookTarget = _lookTarget;
            followTarget = _followTarget;

            if (GetCamera(cameraName, out int cameraIndex))
            {
                allCinemachine[cameraIndex].Follow = followTarget;
                allCinemachine[cameraIndex].LookAt = lookTarget;
                allCinemachine[cameraIndex].Priority = priority;
                currentCamName = cameraName;
                lastCamName = cameraName;
            }
            else
            {
                Debug.LogError("Camera not found: " + cameraName);
            }
        }

        public void SwitchCamera(Transform _lookTarget, Transform _followTarget, string cameraName, int priority = 20)
        {
            if (currentCamName == cameraName)
                return;

            if (GetCamera(currentCamName, out int currentCameraIndex))
            {
                allCinemachine[currentCameraIndex].Priority = defaultPriority; // 恢复默认优先级
                lastCamName = currentCamName;
            }

            if (GetCamera(cameraName, out int cameraIndex))
            {
                allCinemachine[cameraIndex].Follow = _followTarget;
                allCinemachine[cameraIndex].LookAt = _lookTarget;
                allCinemachine[cameraIndex].Priority = priority;
                currentCamName = cameraName;
            }
            else
            {
                Debug.LogError("Camera not found: " + cameraName);
            }
        }

        public bool GetCamera(string cameraName, out int cameraIndex)
        {
            if (cameraIndexDic.ContainsKey(cameraName))
            {
                int index = cameraIndexDic[cameraName];
                if (index >= 0 && index < allCinemachine.Count)
                {
                    cameraIndex = index;
                    return true;
                }
            }
            cameraIndex = -1;
            return false;
        }

        public Transform CreateVirtualPoint(Transform point)
        {
            //在virtualPointRoot下创建一个虚拟摄像机跟随点
            if (virtualPointRoot == null)
            {
                Debug.LogError("VirtualPointRoot is not set, cannot create virtual point.");
            }
            GameObject virtualPoint = new GameObject("VirtualLookPoint");
            virtualPoint.transform.SetParent(virtualPointRoot);
            virtualPoint.transform.position = point.position;
            virtualPoint.transform.rotation = point.rotation;
            currentVirtualPoint = virtualPoint;
            return virtualPoint.transform;
        }

        public Transform CreatePoint(Transform point, Transform root)
        {
            //在root下创建一个虚拟摄像机跟随点
            GameObject lookPoint = new GameObject("LookPoint");
            lookPoint.transform.SetParent(root);
            lookPoint.transform.position = point.position;
            lookPoint.transform.rotation = point.rotation;
            return lookPoint.transform;
        }

        public void ResetCamera()
        {
            //重置相机状态
            if (GetCamera(currentCamName, out int cameraIndex))
            {
                allCinemachine[cameraIndex].Priority = defaultPriority; // 恢复默认优先级s
            }
            currentCamName = null;
            lastCamName = null;
            lookTarget = null;
            followTarget = null;
        }

        //回收对应的虚拟的相机点位
        public void RecycleVirtualPoint()
        {
            if (currentVirtualPoint != null)
            {
                Destroy(currentVirtualPoint);
                currentVirtualPoint = null;
            }
            else
            {
                Debug.LogWarning("No virtual point to recycle.");
            }
        }

        //获取当前相机的名称
        public string GetCurrentCameraName()
        {
            return currentCamName;
        }
    }
}
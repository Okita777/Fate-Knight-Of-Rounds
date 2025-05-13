using System.Collections.Generic;
using UnityEngine;
#if Cinemachine
using Cinemachine;
#endif

namespace AsiActionEngine.RunTime
{
    /// <summary>
    /// 针对Cinemachine相机搭建的基类，不会包含任何相机效果
    /// </summary>
    public abstract class CameraControl : MonoBehaviour
    {
        [HideInInspector] public Transform lookTarget;
        
        [HideInInspector] public Transform defaultLookTarget;
        public ActionStateMachine stateMachine;
        public Behaviour[] allCinemachine = new Behaviour[0];//相机组

        private int lastCamID = -1;
        private float m_AmplitudeGain,m_FrequencyGain;
        private Vector3 m_PivotOffset;
        public bool isLock { get; private set; }
#if Cinemachine
        public CinemachineBasicMultiChannelPerlin perlin { get; private set; }
#endif

        public virtual void OnInit(Transform _lookTarget, ActionStateMachine _stateMachine, int _defaulCam)
        {
            OnReset();
            lookTarget = _lookTarget;
            stateMachine = _stateMachine;
#if Cinemachine
            foreach (var _behaviour in allCinemachine)
            {
                if (_behaviour is CinemachineVirtualCameraBase _cinemachine)
                {
                    _cinemachine.Follow = _lookTarget;
                    _cinemachine.LookAt = _lookTarget;
                }
            }
            
            allCinemachine[_defaulCam].gameObject.SetActive(false);
            allCinemachine[_defaulCam].gameObject.SetActive(true);

            lastCamID = _defaulCam;
#endif

        }

        public virtual void OnReset()
        {
#if Cinemachine
            foreach (var _behaviour in allCinemachine)
            {
                if (_behaviour is CinemachineVirtualCameraBase _cinemachine)
                {
                    _cinemachine.gameObject.SetActive(false);
                }
            }
#endif
        }
        
        public virtual void ChangeCam(int _id, Transform _camPoint)
        {
            isLock = false;
            if (lastCamID > -1)
            {
#if Cinemachine

                if (lastCamID != _id)
                {
                    if (allCinemachine[_id] is CinemachineVirtualCameraBase _cinemachine)
                    {
                        _cinemachine.Follow = _camPoint;
                        _cinemachine.LookAt = _camPoint;
                    }
                    allCinemachine[_id].gameObject.SetActive(true);
                    allCinemachine[lastCamID].gameObject.SetActive(false);

                    if (allCinemachine[lastCamID] is CinemachineVirtualCamera _cinemachineVirtualCameraold)
                    {
                        var _Perlin =_cinemachineVirtualCameraold.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                        if (_Perlin is not null)
                        {
                            _Perlin.m_AmplitudeGain = m_AmplitudeGain;
                            _Perlin.m_FrequencyGain = m_FrequencyGain;
                            _Perlin.m_PivotOffset = m_PivotOffset;
                        }
                    }
                    if (allCinemachine[_id] is CinemachineVirtualCamera _cinemachineVirtualCamera)
                    {
                        var _Perlin =_cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                        perlin = _Perlin;
                        if (_Perlin is not null)
                        {
                            m_AmplitudeGain = _Perlin.m_AmplitudeGain;
                            m_FrequencyGain = _Perlin.m_FrequencyGain;
                            m_PivotOffset = _Perlin.m_PivotOffset;
                        }
                    }
                    lastCamID = _id;

                }
                else
                {
                    if (allCinemachine[_id] is CinemachineVirtualCameraBase _cinemachine)
                    {
                        _cinemachine.Follow = _camPoint;
                        _cinemachine.LookAt = _camPoint;
                    }
                    allCinemachine[_id].gameObject.SetActive(false);
                    allCinemachine[_id].gameObject.SetActive(true);
                }
#endif
            }
        }
        public virtual void ChangeCam(int _id, Transform _camPoint, Transform _lookAt)
        {
            isLock = true;
            if (lastCamID > -1)
            {
#if Cinemachine
                if (lastCamID != _id)
                {
                    if (allCinemachine[_id] is CinemachineVirtualCameraBase _cinemachine)
                    {
                        _cinemachine.Follow = _camPoint;
                        _cinemachine.LookAt = _lookAt;
                    }
                    allCinemachine[_id].gameObject.SetActive(true);
                    allCinemachine[lastCamID].gameObject.SetActive(false);
                    
                    lastCamID = _id;
                }
                else
                {
                    if (allCinemachine[_id] is CinemachineVirtualCameraBase _cinemachine)
                    {
                        _cinemachine.Follow = _camPoint;
                        _cinemachine.LookAt = _lookAt;
                    }
                    allCinemachine[_id].gameObject.SetActive(false);
                    allCinemachine[_id].gameObject.SetActive(true);
                }
#endif
            }
        }
        public abstract void OnUpdate(float _deltaTime);
    }
}
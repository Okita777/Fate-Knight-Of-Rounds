using System;
#if Cinemachine
using Cinemachine;
#endif
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    public class CinemachineZoom : MonoBehaviour
    {
#if Cinemachine
        public CinemachineVirtualCamera[] cinemachines = new CinemachineVirtualCamera[0];
        public Vector2 zoomRange = Vector2.zero;
        public float zoomSpeed = 2;
        public float default_zoom;
        
        private float zoomDelta;
        private CinemachineFramingTransposer[] transposer = new CinemachineFramingTransposer[0];
        private void Start()
        {
            transposer = new CinemachineFramingTransposer[cinemachines.Length];
            for (int i = 0; i < cinemachines.Length; i++)
            {
                transposer[i] = cinemachines[i].GetCinemachineComponent<CinemachineFramingTransposer>();
            }
            // throw new NotImplementedException();
        }

        private void Update()
        {
            default_zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            default_zoom = Mathf.Clamp(default_zoom, zoomRange.x, zoomRange.y);
            if (default_zoom != zoomDelta)
            {

                foreach (var _cinemachine in transposer)
                {
                    _cinemachine.m_CameraDistance = default_zoom;
                }
                zoomDelta = default_zoom;
            }
        }
#endif

    }
}
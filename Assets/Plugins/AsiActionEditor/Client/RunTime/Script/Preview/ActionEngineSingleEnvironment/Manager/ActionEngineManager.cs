using System;
using System.Collections;
using System.IO;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    public class ActionEngineManager : MonoBehaviour
    {
        private void Awake()
        {
            ActionEngineManager_GValue.Instance.Init();
            ActionEngineManager_Input.Instance.Init();
            ActionEngineManager_Unit.Instance.Init();
        }

        private void Update()
        {
            float _deltaTime = Time.deltaTime;
            
            //所有单位和输入系统的主要逻辑
            ActionEngineManager_Unit.Instance.Update(_deltaTime);
            ActionEngineManager_Input.Instance.Update(_deltaTime);

            //异步加载回调
            ActionEngineResources.Instance.ChackCallBack(_deltaTime);
        }

        private void LateUpdate()
        {
            float _deltaTime = Time.deltaTime;
            ActionEngineManager_Unit.Instance.LateUpdate(_deltaTime);
            ActionEngineManager_Input.Instance.LateUpdate(_deltaTime);
        }
    }
}
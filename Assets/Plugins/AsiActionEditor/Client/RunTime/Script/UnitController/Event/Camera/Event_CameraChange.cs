using AsiActionEngine.RunTime;
using AsiTimeLine.RunTime;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEditor_Ex.RunTime
{
    [System.Serializable]
    public class Event_CameraChange : IActionEventData
    {
        [SerializeField] private int mEnterCam = 0;
        [SerializeField] private float mEnterTime = 1.0f;
        [SerializeField] private int mEnterPoint = (int)ECharacteLimbType.Cam_Main;
        // [SerializeField] private int mExitCam = 0;
        // [SerializeField] private int mExitCamPoint = (int)ECharacteLimbType.Cam_Main;
        [SerializeField] private int mActionLable = 0;
        [SerializeField] private bool mCheckLable = false;
        [SerializeField] private bool mIsContain = true;
        [SerializeField] private bool mIsLockCam = false;

        protected bool StateMachineLock = false;
        private bool lastLockState = false;

        #region Property

        [EditorProperty("进入时触发相机： ", EditorPropertyType.EEPT_Camera)]
        public int EnterCam
        {
            get { return mEnterCam; }
            set { mEnterCam = value; }
        }
        [EditorProperty("进入相机绑定点位： ", EditorPropertyType.EEPT_CharacteLimbType)]
        public int EnterPoint
        {
            get { return mEnterPoint; }
            set { mEnterPoint = value; }
        }
        
        [EditorProperty("相机进入时间： ", EditorPropertyType.EEPT_Float)]
        public float EnterTime
        {
            get { return mEnterTime; }
            set { mEnterTime = value; }
        }
        // [EditorProperty("退出时触发相机： ", EditorPropertyType.EEPT_Camera)]
        // public int ExitCam
        // {
        //     get { return mExitCam; }
        //     set { mExitCam = value; }
        // }
        // [EditorProperty("退出相机绑定点位： ", EditorPropertyType.EEPT_CharacteLimbType)]
        // public int ExitCamPoint
        // {
        //     get { return mExitCamPoint; }
        //     set { mExitCamPoint = value; }
        // }
        [EditorProperty("状态标签: ", EditorPropertyType.EEPT_ActionLable)]
        public int ActionLable
        {
            get { return mActionLable; }
            set { mActionLable = value; }
        }
        [EditorProperty("检查标签： ", EditorPropertyType.EEPT_Bool)]
        public bool CheckLable
        {
            get { return mCheckLable; }
            set { mCheckLable = value; }
        }

        [EditorProperty("包含： ", EditorPropertyType.EEPT_Bool)]
        public bool IsContain
        {
            get { return mIsContain; }
            set { mIsContain = value; }
        }
        
        [EditorProperty("为锁定相机： ", EditorPropertyType.EEPT_Bool)]
        public bool IsLockCam
        {
            get { return mIsLockCam; }
            set { mIsLockCam = value; }
        }
        #endregion
        
        public int GetEvenType() => (int)EEvenType.EET_CameraChange;
        public IActionEventData Creact() => new Event_CameraChange();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            ActionEngineManager_Input.Instance.CinemachineBrain.m_DefaultBlend =
                new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, mEnterTime);

            CameraControl _control = ActionEngineManager_Input.Instance.CurCamera;
            if (_control is not null)
            {
                if (!_isSingle)
                    lastLockState = _control.isLock;
                EnterCamInit(_actionState);
            }
        }

        
        public void Update(ActionStatePart _actionState, ActionMachineTime _actionTime)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;

            if (StateMachineLock != _stateMachine.IsLock)
            {//锁定状态变化时切换相机
                
                if (StateMachineLock && !_stateMachine.IsLock && !mIsLockCam)
                {
                    EnterCamInit(_actionState);
                    _stateMachine.SetMouseXY(_stateMachine.GetCharacterFor);
                    // EngineDebug.LogWarning("解除锁定");
                }
                
                if(mIsLockCam && _stateMachine.IsLock)
                {
                    // EngineDebug.Log("触发锁定");
                    EnterCamInit(_actionState);
                }

                StateMachineLock = _stateMachine.IsLock;
            }
        }

        public void Exit(ActionStatePart _actionState, bool _interruot)
        {
            // ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            // if(mIsLockCam != _stateMachine.IsLock)return;
            //
            // if (!ActionEngineManager_Input.Instance.IsPlayer(_actionState.ActionStateMachine.CurUnit))
            // {
            //     return;
            // }
            //
            // if (_stateMachine.TryGetComponent(out CharacterConfig _config))
            // {
            //     if (_config.HelpPointDic.TryGetValue((ECharacteLimbType)mEnterPoint, out Transform _camePoint))
            //     {
            //         CameraControl _control = ActionEngineManager_Input.Instance.CurCamera;
            //
            //         if (!mCheckLable)
            //         {
            //             if (lastLockState)
            //                 _control.ChangeCam(mEnterCam, _camePoint,_stateMachine.LockTransform);
            //             else
            //                 _control.ChangeCam(mEnterCam, _camePoint);
            //         }
            //         else
            //         {
            //             if (_stateMachine.CheckActionLable(mActionLable) == mIsContain)
            //             {
            //                 if (lastLockState)
            //                     _control.ChangeCam(mEnterCam, _camePoint,_stateMachine.LockTransform);
            //                 else
            //                     _control.ChangeCam(mEnterCam, _camePoint);
            //             }
            //         }
            //     }
            // }
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_CameraChange _event = _eventData as Event_CameraChange;

            _event.EnterCam = mEnterCam;
            _event.EnterPoint = mEnterPoint;
            _event.EnterTime = mEnterTime;

            // _event.ExitCam = mExitCam;
            // _event.ExitCamPoint = mExitCamPoint;
            _event.ActionLable = mActionLable;
            _event.CheckLable = mCheckLable;
            _event.IsContain = mIsContain;
            _event.IsLockCam = mIsLockCam;

            return _event;
        }

        private void EnterCamInit(ActionStatePart _actionState)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            if(mIsLockCam != _stateMachine.IsLock)return;
            
            if (!ActionEngineManager_Input.Instance.IsPlayer(_actionState.ActionStateMachine.CurUnit))
            {
                return;
            }

            if (_stateMachine.TryGetComponent(out CharacterConfig _config))
            {
                if (_config.HelpPointDic.TryGetValue((ECharacteLimbType)mEnterPoint, out Transform _camePoint))
                {
                    CameraControl _control = ActionEngineManager_Input.Instance.CurCamera;
                    if (!mCheckLable)
                    {
                        if (mIsLockCam)
                            _control.ChangeCam(mEnterCam, _camePoint,_stateMachine.LockTransform);
                        else
                            _control.ChangeCam(mEnterCam, _camePoint);

                    }
                    else
                    {
                        if (_stateMachine.CheckActionLable(mActionLable) == mIsContain)
                        {
                            if (mIsLockCam)
                                _control.ChangeCam(mEnterCam, _camePoint,_stateMachine.LockTransform);
                            else
                                _control.ChangeCam(mEnterCam, _camePoint);
                        }
                    }
                }
            }
        }
    }
}
using System;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.Graph;
using AsiActionEngine.RunTime.GraphVal;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_CharacterOnMove : IActionEventData
    {
        // [SerializeField] protected EMoveDirType mMoveDirType = EMoveDirType.Transform;
        // [SerializeField] protected GEnum mMoveDirType = new GEnum();
        // [SerializeField] protected EVector3 mMoveDir = new EVector3(0,0,1);
        // [SerializeField] protected EAngleType mAngleType;
        // [SerializeField] protected GFloat mMoveDirF = new GFloat();
        // [SerializeField] protected GFloat mMoveDirR = new GFloat();
        // [SerializeField] protected GFloat mMoveDirU = new GFloat();
        [SerializeField] protected GraphEvent_NoValue_Vector3 mNoveVelocity = new GraphEvent_NoValue_Vector3();
        [SerializeField] protected GFloat mLerpSpeed = new GFloat();
        [SerializeField] private bool mIsMoveInput = true;
        // [SerializeField] private SelectTransform mSelectTransform = new SelectTransform();

        #region Property
        [EditorProperty("移动输入中: ", EditorPropertyType.EEPT_Bool)]
        public bool IsMoveInput
        {
            get { return mIsMoveInput; }
            set { mIsMoveInput = value; }
        }
        // [EditorProperty("朝向类型: ", EditorPropertyType.EEPT_Enum)]
        // public EAngleType AngleType
        // {
        //     get { return mAngleType; }
        //     set { mAngleType = value; }
        // }
        [EditorProperty("移动速度: ", EditorPropertyType.EEPT_GraphValue)]
        public GraphEvent_NoValue_Vector3 NoveVelocity
        {
            get { return mNoveVelocity; }
            set { mNoveVelocity = value; }
        }
        // [EditorProperty("移动速度: ", EditorPropertyType.EEPT_Vector3)]
        // public EVector3 MoveDir
        // {
        //     get { return mMoveDir; }
        //     set { mMoveDir = value; }
        // }
        // [EditorProperty("移动速度(前): ", EditorPropertyType.EEPT_Float)]
        // public GFloat MoveDirF
        // {
        //     get { return mMoveDirF; }
        //     set { mMoveDirF = value; }
        // }
        // [EditorProperty("移动速度(右): ", EditorPropertyType.EEPT_Float)]
        // public GFloat MoveDirR
        // {
        //     get { return mMoveDirR; }
        //     set { mMoveDirR = value; }
        // }
        // [EditorProperty("移动速度(上): ", EditorPropertyType.EEPT_Float)]
        // public GFloat MoveDirU
        // {
        //     get { return mMoveDirU; }
        //     set { mMoveDirU = value; }
        // }
        [EditorProperty("过渡速度: ", EditorPropertyType.EEPT_Float)]
        public GFloat LerpSpeed
        {
            get { return mLerpSpeed; }
            set { mLerpSpeed = value; }
        }

        #endregion
        public int GetEvenType() => (int)EEvenType.EET_CharacterOnMove;
        public IActionEventData Creact() => new Event_CharacterOnMove();

        [NonSerialized] private Vector3 lerpDir;

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            // mMoveDirF.Init(_stateMachine);
            // mMoveDirR.Init(_stateMachine);
            // mMoveDirU.Init(_stateMachine);
            mLerpSpeed.Init(_stateMachine);

            if (_stateMachine.TryGetLogic(out Ex_Update_CharacterControl _characterControl))
            {
                lerpDir = _stateMachine.CurUnit.transform.forward;
                if (_isSingle)
                {
                    Vector3 dir = mNoveVelocity.value(_actionState, new ActionMachineTime(0,0,0,0));

                    _characterControl.CharacterMove = dir;
                }
            }
        }

        public void Update(ActionStatePart _actionState, ActionMachineTime _actionTime)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            Vector3 dir = mNoveVelocity.value(_actionState, _actionTime);
            if (IsMoveInput)
            {
                if (_stateMachine.IsMoveInput)
                {
                    if (_stateMachine.TryGetLogic(out Ex_Update_CharacterControl _characterControl))
                    {
                        if (LerpSpeed.value > 0)
                        {
                            lerpDir = Vector3.Lerp(lerpDir, dir, LerpSpeed.value * _actionTime.Deltatime);
                            _characterControl.CharacterVelocity = lerpDir;
                        }
                        else
                        {
                            lerpDir = dir;
                            _characterControl.CharacterVelocity = lerpDir;
                        }
                    }
                }
            }
            else
            {
                if (_stateMachine.TryGetLogic(out Ex_Update_CharacterControl _characterControl))
                {
                    if (LerpSpeed.value > 0)
                    {
                        lerpDir = Vector3.Lerp(lerpDir, dir, LerpSpeed.value * _actionTime.Deltatime);
                        _characterControl.CharacterVelocity = lerpDir;
                    }
                    else
                    {
                        lerpDir = dir;
                        _characterControl.CharacterVelocity = lerpDir;
                    }
                }
            }
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_CharacterOnMove _event = _eventData as Event_CharacterOnMove;
            _event.NoveVelocity = mNoveVelocity.Clone();

            // _event.AngleType = mAngleType;
            // // _event.MoveDir = mMoveDir;
            // _event.MoveDirF = (GFloat)mMoveDirF.Clone();
            // _event.MoveDirR = (GFloat)mMoveDirR.Clone();
            // _event.MoveDirU = (GFloat)mMoveDirU.Clone();
            _event.LerpSpeed = (GFloat)mLerpSpeed.Clone();

            _event.IsMoveInput = mIsMoveInput;

            return _event;
        }

        // private Vector3 GetDir(ActionStateMachine _stateMachine)
        // {
        //     Vector3 dir = new Vector3(mMoveDirR.value, mMoveDirU.value, mMoveDirF.value);
        //     EMoveDirType _dirType = (EMoveDirType)mMoveDirType.value;
        //     if (_dirType == EMoveDirType.Transform)
        //     {
        //         return _stateMachine.CurUnit.transform.TransformDirection(dir);
        //     }
        //     else if (_dirType == EMoveDirType.Camera)
        //     {
        //         return Quaternion.Euler(0, _stateMachine.GetCamPointRot().eulerAngles.y, 0) * dir;
        //     }
        //     else if (_dirType == EMoveDirType.inputDir_Cam)
        //     {
        //         Quaternion _dir = _stateMachine.CurUnit.transform.rotation;
        //         if (_stateMachine.PlayerInputMoveDir != Vector3.zero)
        //         {
        //             // float _moveDir = Quaternion.LookRotation(_stateMachine.PlayerInputMoveDir).eulerAngles.y;
        //             // _moveDir = _moveDir - _stateMachine.GetCamRot().eulerAngles.y;
        //             // if (_moveDir > 180) _moveDir -= 360;
        //             // if (_moveDir < -180) _moveDir += 360;
        //             // _dir = Quaternion.Euler(0, _moveDir, 0);
        //             if(_stateMachine.TryGetLogic(out Ex_GetAngleToType _getAngle))
        //             {
        //                 _dir = Quaternion.LookRotation(_getAngle.Get(_stateMachine, EAngleType.InputDir));
        //             }
        //         }
        //
        //         return _dir * dir;
        //     }
        //     return dir;
        // }

        // private Vector3 GetDir(ActionStateMachine _stateMachine, ActionStatePart _actionState, ActionMachineTime _actionTime)
        // {
        //     //mAngleType
        //     _stateMachine.TryGetStaticLogic(out Ex_GetAngleToType _getAngle);
        //     Vector3 dir = mNoveVelocity.value(_actionState, _actionTime);
        //     return Quaternion.LookRotation(_getAngle.Get(_stateMachine, mAngleType, _stateMachine.CurUnit.transform)) * dir;
        // }
    }
}
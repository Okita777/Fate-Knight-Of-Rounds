using System;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_RaycastHit : IActionEventData
    {
        [SerializeField] protected string mPartoclePath;
        [SerializeField] protected int mLayerMask = 0;
        [SerializeField] protected int mTargetPoint;
        [SerializeField] protected EVector3 mOffsetPos = new EVector3();
        [SerializeField] protected EVector3 mOffsetRot = new EVector3();
        [SerializeField] protected float mLength = 1;

        #region MyRegion
        [EditorProperty("粒子特效", EditorPropertyType.EEPT_GameObject)]
        public string PartoclePath
        {
            get { return mPartoclePath; }
            set { mPartoclePath = value; }
        }
        [EditorProperty("检测层级", EditorPropertyType.EEPT_LayerMask)]
        public int LayerMask
        {
            get { return mLayerMask; }
            set { mLayerMask = value; }
        }
        [EditorProperty("目标挂点", EditorPropertyType.EEPT_CharacteLimbType)]
        public int TargetPoint
        {
            get { return mTargetPoint; }
            set { mTargetPoint = value; }
        }
        [EditorProperty("位置偏移", EditorPropertyType.EEPT_Vector3)]
        public EVector3 OffsetPos
        {
            get { return mOffsetPos; }
            set { mOffsetPos = value; }
        }
        [EditorProperty("角度偏移", EditorPropertyType.EEPT_Vector3)]
        public EVector3 OffsetRot
        {
            get { return mOffsetRot; }
            set { mOffsetRot = value; }
        }
        [EditorProperty("长度", EditorPropertyType.EEPT_Float)]
        public float Length
        {
            get { return mLength; }
            set { mLength = value; }
        }
        #endregion

        [NonSerialized] private bool isValid = false;
        [NonSerialized] private float intervaltime = 0;
        [NonSerialized] private ActionEditor_Effects _effects;

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            intervaltime = 1;

            ActionEngineResources.Instance.LoadToObjectPool<ActionEditor_Effects>(mPartoclePath, (Component _obj) =>
            {
                if (_obj is ActionEditor_Effects _particle)
                {
                    isValid = true;
                    _effects = _particle;
                }
            }, 10, 5);
            if (_isSingle)
            {
                ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
                OnRaycastHit(_stateMachine);
            }
        }

        public void Update(ActionStatePart _actionState, ActionMachineTime _actionTime)
        {
            intervaltime += _actionTime.Deltatime;
            if (intervaltime > 0.02f)
            {
                OnRaycastHit(_actionState.ActionStateMachine);
                intervaltime = 0;
            }
        }

        public int GetEvenType() => (int)EEvenType.EET_RaycastHit;
        public IActionEventData Creact() => new Event_RaycastHit();

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_RaycastHit _event = _eventData as Event_RaycastHit;
            
            _event.PartoclePath = mPartoclePath;
            _event.LayerMask = mLayerMask;
            _event.TargetPoint = mTargetPoint;
            _event.OffsetPos = mOffsetPos;
            _event.OffsetRot = mOffsetRot;
            _event.Length = mLength;
            return _event;
        }

        private void OnRaycastHit(ActionStateMachine _stateMachine)
        {
            if(!isValid)return;
            
            ActionEngineResources.Instance.ResetLife(_effects, _effects.Life);
            if (_stateMachine.TryGetComponent(out CharacterConfig _characterConfig))
            {
                if(_characterConfig.HelpPointDic.TryGetValue((ECharacteLimbType)mTargetPoint, out Transform _transform))
                {
                    Vector3 _pos = _transform.TransformPoint(mOffsetPos.GetValue());
                    Quaternion _rot = _transform.rotation * Quaternion.Euler(mOffsetRot.GetValue());
                    // RaycastHit _hit;
                    if (Physics.Raycast(_pos, _rot * Vector3.forward, out RaycastHit _hit, mLength, mLayerMask))
                    {
                        Quaternion _effectRot =
                            Quaternion.LookRotation(_transform.position - _hit.point, _rot * Vector3.up);
                        _effects.transform.SetPositionAndRotation(_hit.point, _effectRot);
                        _effects.Play();
                    }
                }
            }
        }
    }
}
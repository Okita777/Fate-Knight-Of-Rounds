using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.Event_Extend;
using AsiTimeLine.RunTime;
using Cinemachine;
using UnityEngine;

namespace AsiActionEditor_Ex.RunTime
{
    [System.Serializable]
    public class Event_CameraShake : IActionEventData
    {
        public enum ETriggerTime
        {
            Any,
            OnHit
        }
        [SerializeField] private float mAmplitudeGain = 0.1f;
        [SerializeField] private float mFrequencyGain = 0.1f;
        [SerializeField] private EVector3 mPivotOffset = new EVector3();
        [SerializeField] private ETriggerTime mTriggerTime = ETriggerTime.Any;
        [SerializeField] private float mDuration = 0.2f;

        #region Property
        [EditorProperty("枢轴偏移： ", EditorPropertyType.EEPT_Vector3)]
        public EVector3 PivotOffset
        {
            get { return mPivotOffset; }
            set { mPivotOffset = value; }
        }
        [EditorProperty("振幅： ", EditorPropertyType.EEPT_Float)]
        public float AmplitudeGain
        {
            get { return mAmplitudeGain; }
            set { mAmplitudeGain = value; }
        }
        [EditorProperty("频率： ", EditorPropertyType.EEPT_Float)]
        public float FrequencyGain
        {
            get { return mFrequencyGain; }
            set { mFrequencyGain = value; }
        }
        [EditorProperty("触发时机： ", EditorPropertyType.EEPT_Enum)]
        public ETriggerTime TriggerTime
        {
            get { return mTriggerTime; }
            set { mTriggerTime = value; }
        }
        [EditorProperty("抖动持续时长： ", EditorPropertyType.EEPT_Float)]
        public float Duration
        {
            get { return mDuration; }
            set { mDuration = value; }
        }
        #endregion
        private float mAmplitudeGain_p = 0.1f;
        private float mFrequencyGain_p = 0.1f;
        private Vector3 mPivotOffset_p = Vector3.one;
        private CinemachineBasicMultiChannelPerlin _perlin;
        public int GetEvenType() => (int)EEvenType.EET_CameraShake;
        public IActionEventData Creact() => new Event_CameraShake();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            _perlin = ActionEngineManager_Input.Instance.CurCamera.perlin;
            if (_perlin is not null)
            {
                mAmplitudeGain_p = _perlin.m_AmplitudeGain;
                mFrequencyGain_p = _perlin.m_FrequencyGain;
                mPivotOffset_p = _perlin.m_PivotOffset;

                if (mTriggerTime == ETriggerTime.Any)
                {
                    _perlin.m_AmplitudeGain = mAmplitudeGain;
                    _perlin.m_FrequencyGain = mFrequencyGain;
                    _perlin.m_PivotOffset = mPivotOffset.GetValue();
                }

                if (!_isSingle)
                {
                    if (mTriggerTime == ETriggerTime.OnHit)
                    {
                        if (_stateMachine.TryGetStaticLogic(out Ex_AttackBox _attackBox))
                        {
                            _attackBox.OnHit += OnHitCallBack;
                        }
                    } 
                }
            }
        }

        public void Exit(ActionStatePart _actionState, bool _interruot)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;

            if (_perlin is not null)
            {
                if (mTriggerTime == ETriggerTime.OnHit)
                {
                    if (_stateMachine.TryGetStaticLogic(out Ex_AttackBox _attackBox))
                    {
                        _attackBox.OnHit -= OnHitCallBack;
                    }
                }
                _perlin.m_AmplitudeGain = mAmplitudeGain_p;
                _perlin.m_FrequencyGain = mFrequencyGain_p;
                _perlin.m_PivotOffset = mPivotOffset_p;
            }
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_CameraShake clone = _eventData as Event_CameraShake;
            clone.AmplitudeGain = mAmplitudeGain;
            clone.FrequencyGain = mFrequencyGain;
            clone.PivotOffset = mPivotOffset;
            clone.TriggerTime = mTriggerTime;
            clone.mDuration = Duration;

            return clone;
        }

        private void OnHitCallBack(IAttackInfo _attackInfo)
        {
            _perlin.m_AmplitudeGain = mAmplitudeGain;
            _perlin.m_FrequencyGain = mFrequencyGain;
            _perlin.m_PivotOffset = mPivotOffset.GetValue();
        }
    }
}
using System;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_TowBoneIK : IActionEventData
    {
        [SerializeField] protected int m_IkTargetID;
        [SerializeField] protected int m_ReferTargetID;
        [SerializeField] protected bool m_IsReferTarget;
        [SerializeField] protected GPoint m_IkPoint = new GPoint();
        [SerializeField] protected float m_EnterTime;
        [SerializeField] protected float m_ExitTime;

        #region property

        [EditorProperty("IK末端", EditorPropertyType.EEPT_CharacteLimbType)]
        public int IkTargetID
        {
            get { return m_IkTargetID; }
            set { m_IkTargetID = value; }
        }
        
        [EditorProperty("修改IK末端位置参考", EditorPropertyType.EEPT_Bool)]
        public bool IsReferTarget
        {
            get { return m_IsReferTarget; }
            set { m_IsReferTarget = value; }
        }

        [EditorProperty("IK末端位置参考对象", EditorPropertyType.EEPT_CharacteLimbType)]
        public int ReferTargetID
        {
            get { return m_ReferTargetID; }
            set { m_ReferTargetID = value; }
        }
        
        [EditorProperty("IK目标点位", EditorPropertyType.EEPT_GPoint)]
        public GPoint IkPoint
        {
            get { return m_IkPoint; }
            set { m_IkPoint = value; }
        }
        
        [EditorProperty("进入时混合时间", EditorPropertyType.EEPT_Float)]
        public float EnterTime
        {
            get { return m_EnterTime; }
            set { m_EnterTime = value; }
        }

        [EditorProperty("退出时混合时间", EditorPropertyType.EEPT_Float)]
        public float ExitTime
        {
            get { return m_ExitTime; }
            set { m_ExitTime = value; }
        }

        #endregion

        public int GetEvenType() => (int)EEvenType.EET_TowBoneIK;
        public IActionEventData Creact() => new Event_TowBoneIK();

        [NonSerialized] public Transform m_IKEnder;
        [NonSerialized] public Transform m_IKRefer;
        [NonSerialized] public bool m_IsValid = false;

        //IK临时储存数据
        [NonSerialized]public Vector2 m_BoneLength = Vector2.up;
        [NonSerialized]public float m_BlnedTime;
        [NonSerialized]public float[] AngleOffset = new float[6];

        [NonSerialized]public Vector3 forward = Vector3.forward;
        [NonSerialized]public Vector3 up = Vector3.right;
        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            ActionStateMachine _actionStateMachine = _actionState.ActionStateMachine;

            m_IsValid = false;
            if (_actionStateMachine.TryGetComponent(out CharacterConfig _config))
            {
                if (m_IsReferTarget)
                {
                    if (_config.HelpPointDic.TryGetValue((ECharacteLimbType)m_ReferTargetID, out Transform _transform2))
                    {
                        m_IKRefer = _transform2;
                    }
                    else
                    {
                        m_IsReferTarget = false;
                    }
                }

                if (_config.HelpPointDic.TryGetValue((ECharacteLimbType)m_IkTargetID, out Transform _transform))
                {
                    m_IkPoint.Init(_actionStateMachine);
                    m_IsValid = m_IkPoint.IsValid;
                    m_IKEnder = _transform;
                    m_BoneLength.y = m_IKEnder.parent.localPosition.magnitude;
                    m_BoneLength.x = m_IKEnder.localPosition.magnitude;

                    forward = FindAxis(Quaternion.Euler(0, 0, 0), _transform.localPosition);
                    up = FindAxis(_transform.parent.rotation,
                        Vector3.Cross(_transform.parent.position - _transform.position,
                            _transform.parent.position - _transform.parent.parent.position));
                }
            }

            if (m_IsValid)
            {
                if (_actionStateMachine.TryGetLogic(out EX_Update_AnimIK _animIK))
                {
                    _animIK.Enter(this);
                }
            }

        }

        public void Exit(ActionStatePart _actionState, bool _interruot)
        {
            ActionStateMachine _actionStateMachine = _actionState.ActionStateMachine;
            if (m_IsValid)
            {
                if (_actionStateMachine.TryGetLogic(out EX_Update_AnimIK _animIK))
                {
                    _animIK.Exit(this);
                }
            }
        }


        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_TowBoneIK _eventDataClone = _eventData as Event_TowBoneIK;

            _eventDataClone.IkTargetID = m_IkTargetID;
            _eventDataClone.IsReferTarget = m_IsReferTarget;
            _eventDataClone.ReferTargetID = m_ReferTargetID;
            _eventDataClone.IkPoint = (GPoint)m_IkPoint.Clone();
            _eventDataClone.EnterTime = m_EnterTime;
            _eventDataClone.ExitTime = m_ExitTime;
            
            return _eventDataClone;
        }
        
        private Vector3 FindAxis(Quaternion _rot, Vector3 _dir)
        {
            int _minID = 0;
            float _maxAngle = -1f;
            AngleOffset[0] = Vector3.Dot(_rot * Vector3.forward, _dir);
            AngleOffset[1] = Vector3.Dot(_rot * Vector3.back, _dir);
            AngleOffset[2] = Vector3.Dot(_rot * Vector3.left, _dir);
            AngleOffset[3] = Vector3.Dot(_rot * Vector3.right, _dir);
            AngleOffset[4] = Vector3.Dot(_rot * Vector3.up, _dir);
            AngleOffset[5] = Vector3.Dot(_rot * Vector3.down, _dir);
            for (int i = 0; i < AngleOffset.Length; i++)
            {
                if (AngleOffset[i] > _maxAngle)
                {
                    _maxAngle = AngleOffset[i];
                    _minID = i;
                }
            }

            if (_minID == 0) return Vector3.forward;
            if (_minID == 1) return Vector3.back;
            if (_minID == 2) return Vector3.left;
            if (_minID == 3) return Vector3.right;
            if (_minID == 4) return Vector3.up;
            if (_minID == 5) return Vector3.down;
            return Vector3.down;
        }
    }
}
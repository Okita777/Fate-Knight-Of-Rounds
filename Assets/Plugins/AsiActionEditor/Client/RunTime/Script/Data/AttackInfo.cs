using System;
using AsiActionEngine.RunTime;
// using AsiTimeLine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class AttackInfo : IAttackInfo
    {
        public enum EReferTarget
        {
            LookBeHit,
            Attaker,
            BeHit
        }
        
        // [SerializeField] protected string m_PartoclePath;

        //击退参考对象  击退方向向量  击退持续时间  击退后保持时间
        [SerializeField] private float mHitSpeed = 0f;
        [SerializeField] private float mHitduration = 0.05f;
        
        // //击退、击飞相关
        // [SerializeField] private EReferTarget mReferTarget = EReferTarget.LookBeHit;
        // [SerializeField] private EVector3 mHitVector = new EVector3(0,0,1.0f);
        // [SerializeField] private float mRepelledTime = 0.2f;
        // [SerializeField] private float mRepelledKeepTime = 0.0f;
        //
        // [NonSerialized] private ActionEditor_Effects mParticleSystem;
        // [NonSerialized] private bool mIsValid = false;
        // [NonSerialized] private float mLifeTime = 0.0f;
        // [NonSerialized] private bool mHitInit = false;
        #region Property
        // [EditorProperty("命中特效", EditorPropertyType.EEPT_GameObject)]
        // public string PartoclePath
        // {
        //     get { return m_PartoclePath; }
        //     set
        //     {
        //         #if UNITY_EDITOR
        //         if (!Application.isPlaying)
        //         {
        //             if (ActionEnginLoadData.Instance.LoadObject<ActionEditor_Effects>(value) == null)
        //             {
        //                 UnityEditor.EditorUtility.DisplayDialog("警告", "当前对象不包含 ActionEditor_Effects 组件", "OK");
        //                 return;
        //             }
        //         }
        //         #endif
        //         m_PartoclePath = value;
        //     }
        // }
        [EditorProperty("命中僵直: ", EditorPropertyType.EEPT_Float)]
        public float HitSpeed
        {
            get { return mHitSpeed; }
            set { mHitSpeed = value; }
        }
        
        [EditorProperty("僵直持续时间(s): ", EditorPropertyType.EEPT_Float)]
        public float Hitduration
        {
            get { return mHitduration; }
            set { mHitduration = value; }
        }

        // [EditorProperty("参考目标: ", EditorPropertyType.EEPT_Enum)]
        // public EReferTarget ReferTarget
        // {
        //     get { return mReferTarget; }
        //     set { mReferTarget = value; }
        // }
        //
        // [EditorProperty("击退方向： ", EditorPropertyType.EEPT_Vector3)]
        // public EVector3 HitVector
        // {
        //     get { return mHitVector; }
        //     set { mHitVector = value; }
        // }
        //
        // [EditorProperty("击退持续时间(s):: ", EditorPropertyType.EEPT_Float)]
        // public float RepelledTime
        // {
        //     get { return mRepelledTime; }
        //     set { mRepelledTime = value; }
        // }
        //
        // [EditorProperty("击退后保持时间(s): ", EditorPropertyType.EEPT_Float)]
        // public float RepelledKeepTime
        // {
        //     get { return mRepelledKeepTime; }
        //     set { mRepelledKeepTime = value; }
        // }
        
        //击退、击飞相关
        #endregion
        public IAttackInfo Clone()
        {
            AttackInfo _attackInfo = new AttackInfo();

            // _attackInfo.PartoclePath = PartoclePath;
            _attackInfo.HitSpeed = mHitSpeed;
            _attackInfo.Hitduration = mHitduration;
            // _attackInfo.ReferTarget = mReferTarget;
            // _attackInfo.HitVector = mHitVector;
            // _attackInfo.RepelledTime = mRepelledTime;
            // _attackInfo.RepelledKeepTime = mRepelledKeepTime;

            return _attackInfo;
        }

        //受击参数，受击者的行为状态机，攻击者，受击点
        public void BeHit(IAttackInfo _IattackInfo, ActionStateMachine _stateMachine, Unit _attacker, Vector3 _hitPoint)
        {
            AttackInfo _attackInfo = (AttackInfo)_IattackInfo;

            if (_stateMachine.TryGetLogic(out Ex_Update_CharacterControl _characterControl))
            {
                
            }
            // EngineDebug.Log
            // (
            //     $"受击对象: {_stateMachine.CurUnit.name}" + "\n" +
            //     $"攻击来源: {_attacker.name}" + "\n" +
            //     $"接受受击(HitSpeed): {_attackInfo.HitSpeed}" + "\n" +
            //     $"接受受击(mHitduration): {_attackInfo.mHitduration}" + "\n" +
            //     ""
            // );
            
            //受击顿帧
            _stateMachine.SetSpeed(_attackInfo.HitSpeed,_attackInfo.mHitduration);
        }
        
        //命中参数，攻击者的行为状态机，受击者，受击点
        public void OnHit(IAttackInfo _IattackInfo, ActionStateMachine _stateMachine, Unit _hiter, Vector3 _hitPoint)
        {
            AttackInfo _attackInfo = (AttackInfo)_IattackInfo;
            // EngineDebug.Log
            // (
            //     $"受击对象: {_stateMachine.CurUnit.name}" + "\n" +
            //     $"攻击来源: {_attacker.name}" + "\n" +
            //     $"接受受击(HitSpeed): {_attackInfo.HitSpeed}" + "\n" +
            //     $"接受受击(mHitduration): {_attackInfo.mHitduration}" + "\n" +
            //     ""
            // );

            //将数值用于攻击者顿帧
            _stateMachine.SetSpeed(_attackInfo.HitSpeed,_attackInfo.mHitduration);
        }

        #region 攻击盒逻辑
        public void OnHitStart(ActionStatePart _statePart)
        {

        }

        public void OnHitUpdate(ActionStatePart _statePart, Vector3 _point, Quaternion _rotate)
        {
            // if (!mHitInit)
            // {
            //     if (mIsValid)
            //     {
            //         ActionEngineResources.Instance.Remove(m_PartoclePath, mParticleSystem);
            //     }
            //     mLifeTime = 100;
            //     ActionEngineResources.Instance.LoadToObjectPool<ActionEditor_Effects>(m_PartoclePath, (Component _obj) =>
            //     {
            //         if (_obj is ActionEditor_Effects _particle)
            //         {
            //             mParticleSystem = _particle;
            //             mIsValid = true;
            //         }
            //     }, 5, 10);
            //     mHitInit = true;
            // }
            //
            // if (mIsValid)
            // {
            //     mLifeTime += deltaTime;
            //     ActionEngineResources.Instance.ResetLife(mParticleSystem, 5);//重置生命
            //     if (mLifeTime > 0.002f)
            //     {
            //         mParticleSystem.transform.SetPositionAndRotation(_point, _rotate);
            //         mParticleSystem.Play();
            //         mLifeTime = 0;
            //     }
            // }
        }

        public void OnHitEnd(ActionStatePart _statePart)
        {
            // mIsValid = false;
            // mHitInit = false;
        }
        #endregion

    }
}
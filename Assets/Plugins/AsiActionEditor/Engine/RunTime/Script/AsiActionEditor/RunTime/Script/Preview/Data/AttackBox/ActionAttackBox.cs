using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.RunTime
{
    
    [System.Serializable]
    public class AttackBoxInfo
    {
        [SerializeField] public AttackBoxPart[] Box = new AttackBoxPart[0];
        [SerializeField] public int AttackBoxType;

        [SerializeField] public ECharacteLimbType ReferPoint;
        [SerializeField] public EVector3 OffsetPos = new EVector3();
        [SerializeField] public EVector3 OffsetRot = new EVector3();
        [SerializeField] public EVector3 Scale = new EVector3(1, 0.2f, 0);//X长度,Y半径
        [SerializeField] public float HitInterval = 0.0f;

        public AttackBoxInfo Clone()
        {
            AttackBoxInfo _attackBoxInfo = new AttackBoxInfo();

            _attackBoxInfo.Box = Box;
            _attackBoxInfo.AttackBoxType = AttackBoxType;

            _attackBoxInfo.ReferPoint = ReferPoint;
            _attackBoxInfo.OffsetPos = OffsetPos;
            _attackBoxInfo.OffsetRot = OffsetRot;
            _attackBoxInfo.Scale = Scale;
            _attackBoxInfo.HitInterval = HitInterval;

            return _attackBoxInfo;
        }
    }
    
    [System.Serializable]
    public struct AttackBoxPart
    {
        [SerializeField] public EVector3 StartPos;
        [SerializeField] public EVector3 Dir;
        [SerializeField] public int TriggerTime;

        public AttackBoxPart(Vector3 _StartPos, Vector3 _dir, int _TriggerTime)
        {
            StartPos =new EVector3(_StartPos.x, _StartPos.y, _StartPos.z);
            Dir = new EVector3(_dir.x, _dir.y, _dir.z);
            TriggerTime = _TriggerTime;
        }
    }
}
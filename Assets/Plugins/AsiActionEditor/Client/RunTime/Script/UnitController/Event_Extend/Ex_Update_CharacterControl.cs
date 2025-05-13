using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    public class Ex_Update_CharacterControl : ActionLogics
    {
        private readonly int RotConst = 6;
        
        private bool mIsMove = false;
        private bool mChangeGround_last = false;
        private CharacterController mCharacter;

        public Vector3 CharacterVelocity;//玩家位移速度
        public Vector3 CharacterMove;//玩家位移


        public float CharacterGravity { get; set; }
        public float PosY { get; set; }

        public bool ChackGround { get; private set; } = false;
        public bool ChackGround_Character { get; private set; } = false;
        
        private Quaternion[] ChracterRots { get; set; }//玩家旋转，但是不同等级  序列号越大优先级越低
        private bool[] IsRots;

        #region Init
        public override void Start(ActionStateMachine _actionState)
        {
            // if(!_actionState.IsLocalClient)return;//非本地客户端  不执行

            if (mCharacter is null)
            {
                if (_actionState.TryGetComponent(out CharacterController _characterController,
                        _actionState.CurUnit.RootTarget))
                {
                    //数据初始化
                    mCharacter = _characterController;
                    mIsMove = true;
                    // mPosY = CurUnit.transform.position.y;
                    CharacterVelocity = Vector3.zero;
                    ChracterRots = new Quaternion[RotConst];
                    IsRots = new bool[RotConst];
                }
                else
                {
                    mIsMove = false;
                    EngineDebug.LogError($"角色无法位移 [<color=#FFCC00>CharacterController</color>] 获取失败！！");
                }
            }
        }
        #endregion

        #region Update
        public override void Update(ActionStateMachine _actionState, float _delaTime)
        {
            if(!_actionState.IsLocalClient)return;//非本地客户端  不执行
            if(RepelledUpdate(_delaTime))return;//无位移时不继续执行
            
            
            if (mIsMove)
            {
                //检查角色是否在地面
                ChackGround = Physics.Raycast(_actionState.CurUnit.transform.TransformPoint(0, 0.2f, 0), Vector3.down, 0.3f);

                //浮空时重置重力
                if (mChangeGround_last != ChackGround)
                {
                    if (!ChackGround && PosY < 0)
                    {
                        // Debug.Log("触发");
                        PosY = 0f;
                    }
                    mChangeGround_last = ChackGround;
                }
                
                //常规重力计算
                PosY -= CharacterGravity * _delaTime;
                PosY = Mathf.Max(PosY, -CharacterGravity * 3);//限制下落的最大加速度
                CharacterVelocity.y += PosY;

                CharacterVelocity *= _delaTime;
                mCharacter.Move(CharacterVelocity + CharacterMove);
                ChackGround_Character = mCharacter.isGrounded;
                CharacterVelocity = Vector3.zero;//位移后清空速度数据
                CharacterMove = Vector3.zero;//位移后清空位移数据

                bool _isRot = false;
                Quaternion _curRot = Quaternion.identity;
                for (int i = 0; i < ChracterRots.Length; i++)
                {
                    if (IsRots[i])
                    {
                        _curRot = ChracterRots[i];
                        _isRot = true;
                        IsRots[i] = false;
                    }
                }

                if (_isRot)
                {
                    _actionState.CurUnit.RootTarget.rotation = _curRot;
                }
            }
        }

        private bool RepelledUpdate(float _deltaTime)
        {
            if (mRepelled_E > 0)
            {
                mRepelled_E -= _deltaTime;
                float _property = Mathf.Max(0, mRepelled_E / mRepelled_S);
                Vector3 _nowPos = Vector3.Lerp(mRepelled_DeltaPos_Last, Vector3.zero, _property);
                Vector3 _deltaPos = _nowPos - mRepelled_DeltaPos_Last;
                mCharacter.Move(_deltaPos);
                mRepelled_DeltaPos_Last = _nowPos;
                if (mRepelled_E <= 0 && mKeepTime <= 0)
                {//继承最终的力度
                    
                }
                return true;
            }

            if (mKeepTime > 0)
            {
                mKeepTime -= _deltaTime;
                return true;
            }
            return false;
        }
        #endregion


        public void SetRot(Quaternion _rot, int _priority = 0)
        {
            // if(!_actionState.IsLocalClient)return;//非本地客户端  不执行
            int _nowPriority = Mathf.Clamp(_priority, 0, RotConst - 1);
            IsRots[_nowPriority] = true;
            ChracterRots[_nowPriority] = _rot;
        }
        
        public Vector3 GetV3ToInputMove(ActionStateMachine _actionState, Vector3 _mouveDir, Quaternion _referDir)
        {
            return Quaternion.LookRotation(_actionState.PlayerInputMoveDir) * _referDir * _mouveDir;
        }
        public Vector3 GetV3ToInputMove(ActionStateMachine _actionState, Vector3 _mouveDir)
        {
            if (_actionState.PlayerInputMoveDir == Vector3.zero)
            {
                return _actionState.CurUnit.transform.TransformDirection(_mouveDir);
            }
            Quaternion _rot = Quaternion.LookRotation(_actionState.PlayerInputMoveDir) *
                              Quaternion.Euler(0, _actionState.GetCharacterFor.eulerAngles.y, 0);
            _mouveDir = _rot * _mouveDir;
            return  _mouveDir;
        }

        //击退相关
        private Vector3 mRepelled_Pos_S, mRepelled_Pos_E;
        private Vector3 mRepelled_DeltaPos_Last;
        private float mRepelled_S, mRepelled_E;
        private float mKeepTime;
        public void BeRepelled(Vector3 _finishPos, float _repelledTime, float _keepTime)
        {
            mRepelled_Pos_S = Vector3.zero;
            mRepelled_Pos_E = _finishPos;
            mRepelled_S = _repelledTime;
            mRepelled_E = _repelledTime;
            mKeepTime = _keepTime;

            mRepelled_DeltaPos_Last = mRepelled_Pos_S;
        }
    }
}
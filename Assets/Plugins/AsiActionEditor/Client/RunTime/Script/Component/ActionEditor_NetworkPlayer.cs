using UnityEngine;
using AsiActionEngine.RunTime;

#if MIRROR_81_OR_NEWER
using Mirror;
#endif


namespace AsiTimeLine.RunTime
{
#if MIRROR_81_OR_NEWER
    public class ActionEditor_NetworkPlayer : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnChangePlayer))] private int mPlayerID;
        [SyncVar(hook = nameof(OnSyncActionState))] private ActionSyncInfo mActionInfo;
        
        private Unit mLocalUnit = null;

        #region 结构体
        public struct ActionSyncInfo
        {
            public int mActionID;
            public int mMixTime;
            public int mOffsetTime;

            public ActionSyncInfo(int _mActionID, int _mMixTime, int _mOffsetTime)
            {
                mActionID = _mActionID;
                mMixTime = _mMixTime;
                mOffsetTime = _mOffsetTime;
            }
        }
        #endregion
        public override void OnStartClient()
        {
            base.OnStartClient();
            //本地才会执行的函数
            if (isOwned)
            {
                mPlayerID = 1;
                CreactPlayer(mPlayerID, true);
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            RemovePlayer();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        private void RemovePlayer()
        {
            //删除玩家
            if (mLocalUnit != null)
            {
                //卸载回调
                if (isOwned)
                {
                    mLocalUnit.ActionStateMachine.OnChange -= OnChangeActionInfo;
                    ActionEngineManager_Input.Instance.ChangePlayer(null);
                }

                ActionEngineManager_Unit.Instance.DestoryUnit(mLocalUnit, b =>
                {
                    if (!b)
                    {
                        EngineDebug.LogError($"单位删除失败!!!!  [{mLocalUnit.name}]");
                    }
                });
            }
        }
        #region 同步的回调函数
        //切换动画
        private void OnSyncActionState(ActionSyncInfo _old, ActionSyncInfo _new)
        {
            if (!isOwned && mLocalUnit)
            {
                mLocalUnit.ActionStateMachine.ChangeAction(_new.mActionID, _new.mMixTime, _new.mOffsetTime);
            }
        }
        
        //切换玩家
        private void OnChangePlayer(int _old, int _new)
        {
            if (!isOwned)
            {
                CreactPlayer(_new, false);
            }
        }

        private void CreactPlayer(int _unitID, bool _isOwner)
        {
            if(mLocalUnit != null)
            {
                RemovePlayer();
            }
            ActionEngineManager_Input.Instance.CreactGameManager();

            ActionEngineManager_Unit.Instance.CreactUnit(_unitID, (Unit _unit) =>
            {
                _unit.transform.SetParent(transform);
                _unit.transform.SetPositionAndRotation(transform.position, transform.rotation);
                if (_unit.ActionStateMachine.TryGetComponent(out CharacterController _characterController,
                        _unit.transform, "mLocalCharacter"))
                {
                    _characterController.enabled = false;
                    CharacterController _mCharacter = transform.GetComponent<CharacterController>();
                    _mCharacter.slopeLimit = _characterController.slopeLimit;
                    _mCharacter.stepOffset = _characterController.stepOffset;
                    _mCharacter.skinWidth = _characterController.skinWidth;
                    _mCharacter.minMoveDistance = _characterController.minMoveDistance;
                    _mCharacter.center = _characterController.center;
                    _mCharacter.radius = _characterController.radius;
                    _mCharacter.height = _characterController.height;
                }

                mLocalUnit = _unit;
                
                //如果是本地发起的角色创建，则将此角色注册到输入系统
                if (_isOwner)
                {
                    ActionEngineManager_Input.Instance.ChangePlayer(_unit);
                    _unit.RootTarget = transform;
                    _unit.ActionStateMachine.IsLocalClient = true;
                    _unit.ActionStateMachine.OnChange += OnChangeActionInfo;
                }
                else
                {
                    _unit.RootTarget = _unit.transform;
                    _unit.ActionStateMachine.IsLocalClient = false;
                }
            });
        }
        
        //发送动画同步参数
        private void OnChangeActionInfo(int _mActionID, int _mMixTime, int _mOffsetTime)
        {
            mActionInfo = new ActionSyncInfo(_mActionID, _mMixTime, _mOffsetTime);
        }
        #endregion
    }
#else 
    public class ActionEditor_NetworkPlayer : MonoBehaviour
    {
        
    }
#endif


}
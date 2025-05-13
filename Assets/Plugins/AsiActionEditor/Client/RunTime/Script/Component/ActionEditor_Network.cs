using System.Collections;
using System.Collections.Generic;
using AsiActionEngine.RunTime;

#if Fishnet
using FishNet.Object;
using FishNet.Object.Synchronizing;
#endif
using UnityEngine;

#if Fishnet
public class ActionEditor_Network : NetworkBehaviour
{
    [HideInInspector] public Unit mUnit;
    public readonly SyncVar<int> mUnitID = new SyncVar<int>();//单位ID
    public readonly SyncVar<ActionSyncData> mChangeActionID = new SyncVar<ActionSyncData>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {//仅在本地注册事件
            //添加行为改变时的监听
            mUnit.ActionStateMachine.OnChange += (id, time, offsetTime) => SendActionID(id, time, offsetTime);
        }
        else
        {//非本地仅注册更新
            mChangeActionID.OnChange += ExtruetActionID;
            
            //设定为非本地  跳过部分不必要的逻辑
            mUnit.ActionStateMachine.SetToClient(false);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        
        if (IsOwner)
        {
            mUnit.ActionStateMachine.OnChange -= SendActionID;
        }
        else
        {
            mChangeActionID.OnChange -= ExtruetActionID;
        }
    }

    #region 回调
    private void SendActionID(int _actionID, int _mixTime, int _offsetTime)
    {
        //行为改变时向其它发送通知
        mChangeActionID.Value = new ActionSyncData(_actionID, _mixTime, _offsetTime);
    }

    private void ExtruetActionID(ActionSyncData prev, ActionSyncData next, bool asserver)
    {
        mUnit.ActionStateMachine.ChangeAction(prev.ActionID,prev.MixTime,prev.OffsetTime);
    }
    #endregion
}

public struct ActionSyncData
{
    public int ActionID;
    public int MixTime;
    public int OffsetTime;
    public ActionSyncData(int _actionID, int _mixTime, int _offsetTime)
    {
        ActionID = _actionID;
        MixTime = _mixTime;
        OffsetTime = _offsetTime;
    }
}
#else
public class ActionEditor_Network : MonoBehaviour
{
    
}
#endif


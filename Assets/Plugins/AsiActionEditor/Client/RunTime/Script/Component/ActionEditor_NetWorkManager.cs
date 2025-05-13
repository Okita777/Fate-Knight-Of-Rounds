using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if MIRROR_81_OR_NEWER
using Mirror;
#elif Fishnet
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
#endif
namespace AsiTimeLine.RunTime
{
#if MIRROR_81_OR_NEWER
    public class ActionEditor_NetWorkManager : NetworkManager
    {
        public override void Start()
        {
            base.Start();
        }
        public override void Update()
        {
            base.Update();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            //生成玩家时调用
            // base.OnServerAddPlayer(conn);
            Transform _startPos = GetStartPosition();
            GameObject _player = null;
            if(_startPos)_player = Instantiate(playerPrefab, _startPos.position, _startPos.rotation);
            else _player = Instantiate(playerPrefab);

            //为连接添加生成的玩家对象
            NetworkServer.AddPlayerForConnection(conn, _player);
        }
    }
#elif Fishnet
    public class ActionEditor_PlayerSpawner : MonoBehaviour
    {

        private NetworkManager _networkManager;

        private void Start()
        {
            // throw new NotImplementedException();
        }
    }
#else
    public class ActionEditor_PlayerSpawner : MonoBehaviour
    {
    }
#endif

}
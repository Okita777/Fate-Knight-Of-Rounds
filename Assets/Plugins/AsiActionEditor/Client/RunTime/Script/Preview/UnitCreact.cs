using System;
using AsiActionEngine.RunTime;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AsiTimeLine.RunTime
{
    public class UnitCreact :MonoBehaviour
    {
        public int UnitID;
        public bool IsPlayer = false;
        private void Start()
        {
            ActionEngineManager_Input.Instance.CreactGameManager();
            
            ActionEngineManager_Unit.Instance.CreactUnit(UnitID, (Unit _unit) =>
            {
                // EngineDebug.Log("创建角色了");
                _unit.transform.SetPositionAndRotation(transform.position, transform.rotation);
                if (IsPlayer)
                {
                    ActionEngineManager_Input.Instance.ChangePlayer(_unit);
                }
            });
        }
    }
}
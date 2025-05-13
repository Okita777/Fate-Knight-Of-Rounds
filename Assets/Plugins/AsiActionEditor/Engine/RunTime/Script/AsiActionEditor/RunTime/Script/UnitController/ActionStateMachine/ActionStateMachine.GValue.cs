using System.Collections.Generic;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public partial class ActionStateMachine
    {
        public EngineGValue GValue;
        
        public Dictionary<int, PointData> PointDic = new Dictionary<int, PointData>();
        public Dictionary<int, Transform> TransfromDic = new Dictionary<int, Transform>();
        public Dictionary<int, Unit> UnitDic = new Dictionary<int, Unit>();

        public int GetLayer(int _curLayer)
        {
            if (_curLayer < 0)
            {
                int selectID = _curLayer * -1 - 1;
                if (TryGetComponent(out ActionEditor_LayerMask layerMask))
                {
                    return layerMask.mLayers[selectID];
                }
                return _curLayer * -1 - 1;
            }
            return _curLayer;
        }
        public PointData GetPoint(int _id)
        {
            if (PointDic.TryGetValue(_id, out PointData pointData)) return pointData;
            return new PointData();
        }
        public void SetPoint(int _id, PointData pointData)
        {
            if(!PointDic.TryAdd(_id,pointData))
                PointDic[_id] = pointData;
        }
        public Transform GetTransform(int _id)
        {
            if (TransfromDic.TryGetValue(_id, out Transform pointData))
            {
                if (pointData is null)
                {
                    TransfromDic.Remove(_id);
                    return null;
                }
                return pointData;
            }
            return null;
        }
        public void SetTransform(int _id, Transform pointData)
        {
            if(!TransfromDic.TryAdd(_id,pointData))
                TransfromDic[_id] = pointData;
        }
        public Unit GetUnit(int _id)
        {
            if (UnitDic.TryGetValue(_id, out Unit pointData))
            {
                if (pointData is null)
                {
                    UnitDic.Remove(_id);
                    return null;
                }
                return pointData;
            }
            return null;
        }
        public void SetUnit(int _id, Unit pointData)
        {
            if(!UnitDic.TryAdd(_id,pointData))
                UnitDic[_id] = pointData;
        }

        public bool RemovePoint(int _id)
        {
            return PointDic.Remove(_id);
        }
        public bool RemoveTransform(int _id)
        {
            return TransfromDic.Remove(_id);
        }
        public bool RemoveUnit(int _id)
        {
            return UnitDic.Remove(_id);
        }
        private void GValueInit(EngineGValue _gValue)
        {
            GValue = _gValue;
        }
    }
}
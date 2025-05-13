using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    #if UNITY_EDITOR
    //Editor专用的序列化字典
    [Serializable]public class InterrupOffset: SerializedDictionary<string, int> { }
    #endif
    
    //Config用到的序列化字典
    [Serializable] public class CharacterLimbDic : SerializedDictionary<ECharacteLimbType,Transform> { }

    [Serializable] public class CinemachineDic : SerializedDictionary<string, Behaviour[]> { }

    [Serializable] public class AudioSourceDic : SerializedDictionary<string, AudioSource> { }
    
    [Serializable] public class SerAnimationCurve : SerializedAnimCurve { }
    
    [Serializable] public class GraphEdiDataDic : SerializedDictionary<string, AudioSource> { }
    
    [Serializable] public class NodeEdiDataDic : SerializedDictionary<string, NodeEdiData> { }

    [Serializable] public class NodeEdiData
    {
        public List<Vector2> nodePositions = new List<Vector2>();
        public string nodeTitle = "蓝图功能描述";
        public string nodeToolTip = "蓝图功能的详细描述";

        public NodeEdiData Clone()
        {
            NodeEdiData _node = new NodeEdiData();
            _node.nodeTitle = this.nodeTitle;
            _node.nodeToolTip = this.nodeToolTip;
            List<Vector2> positions = new List<Vector2>();
            foreach (Vector2 v in nodePositions) positions.Add(v);
            _node.nodePositions = positions;
            return _node;
        }

        public void SetNewPos(NodeEdiData newPos)
        {
            nodePositions.Clear();
            foreach (Vector2 v in newPos.nodePositions) nodePositions.Add(v);
        }
    }
}
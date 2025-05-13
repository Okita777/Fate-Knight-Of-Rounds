using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class ActionPreviewMark : MonoBehaviour
    {
        public bool IsPlayer;
        public string ActionName;
        public string DefaltWeapon;
        public string DefaltCamera;
        
        [Header("相机预览设置(预览用)")]
        public float CamRotSpeed;
        public Vector3 CamOffsetPos;

        //重新加载Action数据
        public virtual void ReLoadActionInfo(){}
    }
}
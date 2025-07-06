using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ResourceSystem.Core
{
    /// <summary>
    /// 缓存的资源数据
    /// </summary>
    [System.Serializable]
    public class CachedAsset
    {
        public UnityEngine.Object Asset;
        public AsyncOperationHandle Handle;
        public float LoadTime;
        public float LastAccessTime;
    }

    /// <summary>
    /// 预加载组
    /// </summary>
    [System.Serializable]
    public class PreloadGroup
    {
        public string Name;
        public List<string> Addresses;
        public PreloadPriority Priority;
        public PreloadStatus Status;
    }

    /// <summary>
    /// 资源包配置
    /// </summary>
    [System.Serializable]
    public class ResourcePackConfig
    {
        [Header("基础信息")]
        public string PackName;
        public string Description;
        public PreloadPriority Priority = PreloadPriority.Normal;

        [Header("资源列表")]
        public List<string> AssetAddresses = new List<string>();

        [Header("依赖关系")]
        public List<string> Dependencies = new List<string>();

        [Header("可选配置")]
        public bool AutoLoad = false;
        public bool PersistentLoad = false; // 是否持久化加载（不自动释放）

        public ResourcePackConfig()
        {
            AssetAddresses = new List<string>();
            Dependencies = new List<string>();
        }

        public ResourcePackConfig(string packName, List<string> addresses, PreloadPriority priority = PreloadPriority.Normal)
        {
            PackName = packName;
            AssetAddresses = new List<string>(addresses);
            Priority = priority;
            Dependencies = new List<string>();
        }
    }

    /// <summary>
    /// 预加载优先级
    /// </summary>
    public enum PreloadPriority
    {
        Low = 0,
        Normal = 1,
        High = 2
    }

    /// <summary>
    /// 预加载状态
    /// </summary>
    public enum PreloadStatus
    {
        Pending,
        Loading,
        Completed,
        Failed
    }

    /// <summary>
    /// 资源缓存信息
    /// </summary>
    [System.Serializable]
    public class ResourceCacheInfo
    {
        public int CachedAssetCount;
        public int TotalReferenceCount;
        public int PreloadGroupCount;
    }

    /// <summary>
    /// 资源包类型枚举
    /// </summary>
    public enum ResourcePackType
    {
        UI,           // UI资源包
        Audio,        // 音频资源包
        Scene,        // 场景资源包
        Character,    // 角色资源包
        Effect,       // 特效资源包
        Texture,      // 贴图资源包
        Model,        // 模型资源包
        Animation,    // 动画资源包
        Custom        // 自定义资源包
    }
}
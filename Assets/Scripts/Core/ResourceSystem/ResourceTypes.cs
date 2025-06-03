using System;
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
    /// 预加载组数据
    /// </summary>
    [System.Serializable]
    public class PreloadGroup
    {
        public string Name;
        public List<string> Addresses;
        public PreloadPriority Priority;
        public PreloadStatus Status;
        public int LoadedCount;
    }

    /// <summary>
    /// 预加载优先级
    /// </summary>
    public enum PreloadPriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Critical = 3
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
    /// 资源加载状态
    /// </summary>
    public enum ResourceLoadStatus
    {
        NotLoaded,
        Loading,
        Loaded,
        Failed
    }

    /// <summary>
    /// 资源缓存信息
    /// </summary>
    [System.Serializable]
    public class ResourceCacheInfo
    {
        public int CachedAssetCount;
        public int LoadingOperationCount;
        public int PreloadGroupCount;
        public int TotalReferenceCount;
    }

    /// <summary>
    /// 资源包配置
    /// </summary>
    [System.Serializable]
    public class ResourcePackConfig
    {
        public string PackName;
        public List<string> AssetAddresses;
        public string Description;
        public bool AutoLoad;
        public PreloadPriority Priority;
    }
}



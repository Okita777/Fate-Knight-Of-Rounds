using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ResourceSystem.Core
{
    /// <summary>
    /// 简化版资源管理器 - 核心功能保留
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        #region 单例模式
        private static ResourceManager _instance;
        private static readonly object _lock = new object();

        public static ResourceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            GameObject go = new GameObject("ResourceManager");
                            _instance = go.AddComponent<ResourceManager>();
                            DontDestroyOnLoad(go);
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region 私有字段
        [Header("基础配置")]
        [SerializeField] private bool enableLogging = true;
        [SerializeField] private int maxCacheSize = 100;
        [SerializeField] private float autoReleaseInterval = 300f; // 5分钟

        // 核心数据结构
        private Dictionary<string, CachedAsset> _assetCache = new Dictionary<string, CachedAsset>();
        private Dictionary<string, int> _referenceCount = new Dictionary<string, int>();
        private Dictionary<string, PreloadGroup> _preloadGroups = new Dictionary<string, PreloadGroup>();
        #endregion

        #region 生命周期
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private async void Initialize()
        {
            try
            {
                Log("🔄 初始化资源管理器");

                // 🚨 修复：简化的 Addressable 初始化
                var initOp = Addressables.InitializeAsync();
                await initOp.Task;

                Log("✅ 资源管理器初始化完成");

                // 启动自动清理
                if (autoReleaseInterval > 0)
                {
                    InvokeRepeating(nameof(AutoReleaseUnusedAssets), autoReleaseInterval, autoReleaseInterval);
                }
            }
            catch (Exception e)
            {
                LogError($"初始化失败: {e.Message}");
            }
        }
        #endregion

        #region 核心加载功能
        /// <summary>
        /// 异步加载资源
        /// </summary>
        public async Task<T> LoadAssetAsync<T>(string address, Action<T> callback = null, bool autoAddRef = true) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(address))
            {
                LogError("地址为空");
                callback?.Invoke(null);
                return null;
            }

            // 检查缓存
            if (_assetCache.TryGetValue(address, out CachedAsset cachedAsset))
            {
                if (cachedAsset.Asset is T asset)
                {
                    cachedAsset.LastAccessTime = Time.time;
                    if (autoAddRef)
                        AddReference(address);
                    callback?.Invoke(asset);
                    Log($"从缓存加载: {address}");
                    return asset;
                }
            }

            // 加载新资源
            try
            {
                var handle = Addressables.LoadAssetAsync<T>(address);
                T result = await handle.Task;

                if (result != null)
                {
                    // 添加到缓存
                    _assetCache[address] = new CachedAsset
                    {
                        Asset = result,
                        Handle = handle,
                        LoadTime = Time.time,
                        LastAccessTime = Time.time
                    };

                    if (autoAddRef)
                        AddReference(address);
                    Log($"加载成功: {address}");
                    callback?.Invoke(result);
                    return result;
                }
                else
                {
                    LogError($"加载失败: {address}");
                    callback?.Invoke(null);
                    return null;
                }
            }
            catch (Exception e)
            {
                LogError($"加载异常 {address}: {e.Message}");
                callback?.Invoke(null);
                return null;
            }
        }

        /// <summary>
        /// 同步获取已缓存的资源
        /// </summary>
        public T GetAsset<T>(string address) where T : UnityEngine.Object
        {
            if (_assetCache.TryGetValue(address, out CachedAsset cachedAsset))
            {
                cachedAsset.LastAccessTime = Time.time;
                return cachedAsset.Asset as T;
            }
            return null;
        }

        /// <summary>
        /// 批量加载资源
        /// </summary>
        public async Task<List<T>> LoadAssetsAsync<T>(List<string> addresses) where T : UnityEngine.Object
        {
            List<Task<T>> loadTasks = new List<Task<T>>();
            foreach (string address in addresses)
            {
                loadTasks.Add(LoadAssetAsync<T>(address));
            }

            T[] results = await Task.WhenAll(loadTasks);
            return new List<T>(results.Where(r => r != null));
        }
        #endregion

        #region 预加载功能
        /// <summary>
        /// 创建预加载组
        /// </summary>
        public void CreatePreloadGroup(string groupName, List<string> addresses, PreloadPriority priority = PreloadPriority.Normal)
        {
            _preloadGroups[groupName] = new PreloadGroup
            {
                Name = groupName,
                Addresses = new List<string>(addresses),
                Priority = priority,
                Status = PreloadStatus.Pending
            };

            Log($"创建预加载组: {groupName} ({addresses.Count} 个资源)");
        }

        /// <summary>
        /// 执行预加载
        /// </summary>
        public async Task<bool> PreloadGroupAsync(string groupName, Action<float> progressCallback = null)
        {
            if (!_preloadGroups.TryGetValue(groupName, out PreloadGroup group))
            {
                LogError($"预加载组不存在: {groupName}");
                return false;
            }

            if (group.Status == PreloadStatus.Completed)
            {
                Log($"预加载组已完成: {groupName}");
                return true;
            }

            group.Status = PreloadStatus.Loading;
            Log($"开始预加载: {groupName}");

            try
            {
                List<Task<UnityEngine.Object>> loadTasks = new List<Task<UnityEngine.Object>>();
                foreach (string address in group.Addresses)
                {
                    loadTasks.Add(LoadAssetAsync<UnityEngine.Object>(address, null, false));
                }

                int totalTasks = loadTasks.Count;

                // 监控进度
                while (loadTasks.Any(t => !t.IsCompleted))
                {
                    int completedTasks = loadTasks.Count(t => t.IsCompleted);
                    float progress = (float)completedTasks / totalTasks;
                    progressCallback?.Invoke(progress);
                    await Task.Delay(50);
                }

                await Task.WhenAll(loadTasks);

                group.Status = PreloadStatus.Completed;
                Log($"预加载完成: {groupName}");
                progressCallback?.Invoke(1.0f);
                return true;
            }
            catch (Exception e)
            {
                group.Status = PreloadStatus.Failed;
                LogError($"预加载失败 {groupName}: {e.Message}");
                return false;
            }
        }
        #endregion

        #region 引用计数和释放
        /// <summary>
        /// 增加引用计数
        /// </summary>
        public void AddReference(string address)
        {
            if (!_referenceCount.ContainsKey(address))
                _referenceCount[address] = 0;
            _referenceCount[address]++;
        }

        /// <summary>
        /// 减少引用计数
        /// </summary>
        public void RemoveReference(string address)
        {
            if (_referenceCount.ContainsKey(address))
            {
                _referenceCount[address]--;
                if (_referenceCount[address] <= 0)
                {
                    _referenceCount.Remove(address);
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void ReleaseAsset(string address)
        {
            if (_assetCache.TryGetValue(address, out CachedAsset cachedAsset))
            {
                if (cachedAsset.Handle.IsValid())
                {
                    Addressables.Release(cachedAsset.Handle);
                }
                _assetCache.Remove(address);
                Log($"释放资源: {address}");
            }

            _referenceCount.Remove(address);
        }

        /// <summary>
        /// 自动释放未使用的资源
        /// </summary>
        private void AutoReleaseUnusedAssets()
        {
            List<string> toRelease = new List<string>();
            float currentTime = Time.time;

            foreach (var kvp in _assetCache)
            {
                string address = kvp.Key;
                CachedAsset asset = kvp.Value;

                bool hasReference = _referenceCount.ContainsKey(address) && _referenceCount[address] > 0;
                bool isOld = (currentTime - asset.LastAccessTime) > autoReleaseInterval;

                if (!hasReference && isOld)
                {
                    toRelease.Add(address);
                }
            }

            foreach (string address in toRelease)
            {
                ReleaseAsset(address);
            }

            if (toRelease.Count > 0)
            {
                Log($"自动释放了 {toRelease.Count} 个资源");
            }
        }

        /// <summary>
        /// 释放所有资源
        /// </summary>
        public void ReleaseAllAssets()
        {
            foreach (var kvp in _assetCache)
            {
                if (kvp.Value.Handle.IsValid())
                {
                    Addressables.Release(kvp.Value.Handle);
                }
            }

            _assetCache.Clear();
            _referenceCount.Clear();
            Log("释放所有资源");
        }
        #endregion

        #region 实用方法
        /// <summary>
        /// 获取缓存信息
        /// </summary>
        public ResourceCacheInfo GetCacheInfo()
        {
            return new ResourceCacheInfo
            {
                CachedAssetCount = _assetCache.Count,
                TotalReferenceCount = _referenceCount.Count,
                PreloadGroupCount = _preloadGroups.Count
            };
        }

        /// <summary>
        /// 检查资源是否已缓存
        /// </summary>
        public bool IsAssetCached(string address)
        {
            return _assetCache.ContainsKey(address);
        }
        #endregion

        #region 日志方法
        private void Log(string message)
        {
            if (enableLogging)
                Debug.Log($"[ResourceManager] {message}");
        }

        private void LogError(string message)
        {
            Debug.LogError($"[ResourceManager] {message}");
        }
        #endregion

        #region 清理
        private void OnDestroy()
        {
            CancelInvoke();
            ReleaseAllAssets();
        }

        private void OnApplicationQuit()
        {
            ReleaseAllAssets();
        }
        #endregion
    }
}
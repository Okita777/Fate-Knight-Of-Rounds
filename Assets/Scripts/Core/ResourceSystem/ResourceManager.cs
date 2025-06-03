using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace ResourceSystem.Core
{
    /// <summary>
    /// 核心资源管理器 - 基于Addressable的统一资源管理系统
    /// 提供资源加载、预加载、缓存、释放等功能
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        #region 单例模式
        private static ResourceManager _instance;
        public static ResourceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("ResourceManager");
                    _instance = go.AddComponent<ResourceManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        #endregion

        #region 私有字段
        [Header("资源管理配置")]
        [SerializeField] private bool enableResourceLogging = true;
        [SerializeField] private int maxCacheSize = 100;
        [SerializeField] private float autoReleaseInterval = 300f; // 5分钟自动释放未使用资源

        // 资源缓存字典 - 存储已加载的资源
        private Dictionary<string, CachedAsset> _assetCache = new Dictionary<string, CachedAsset>();

        // 异步操作追踪 - 防止重复加载
        private Dictionary<string, AsyncOperationHandle> _loadingOperations = new Dictionary<string, AsyncOperationHandle>();

        // 预加载组管理
        private Dictionary<string, PreloadGroup> _preloadGroups = new Dictionary<string, PreloadGroup>();

        // 引用计数管理
        private Dictionary<string, int> _referenceCount = new Dictionary<string, int>();

        // 场景资源分组
        private Dictionary<string, List<string>> _sceneAssets = new Dictionary<string, List<string>>();
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

        private void Initialize()
        {
            // 使用协程进行初始化
            StartCoroutine(InitializeAddressableSystem());

            Log("ResourceManager initialized successfully.");
        }

        private System.Collections.IEnumerator InitializeAddressableSystem()
        {
            // 异步初始化Addressable系统
            var initHandle = Addressables.InitializeAsync();

            yield return initHandle;

            if (initHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Log("Addressable system initialized successfully.");

                // 启动自动清理协程
                if (autoReleaseInterval > 0)
                {
                    InvokeRepeating(nameof(AutoReleaseUnusedAssets), autoReleaseInterval, autoReleaseInterval);
                }
            }
            else
            {
                LogError($"Failed to initialize Addressable system: {initHandle.OperationException}");
            }

            // 释放初始化句柄
            Addressables.Release(initHandle);
        }
        #endregion

        #region 核心加载功能
        /// <summary>
        /// 异步加载单个资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="address">资源地址</param>
        /// <param name="callback">加载完成回调</param>
        /// <param name="autoAddRef">是否自动增加引用计数</param>
        public async Task<T> LoadAssetAsync<T>(string address, Action<T> callback = null, bool autoAddRef = true) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(address))
            {
                LogError("Address is null or empty.");
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
                    Log($"Asset loaded from cache: {address}");
                    return asset;
                }
            }

            // 检查是否正在加载
            if (_loadingOperations.ContainsKey(address))
            {
                var existingOp = _loadingOperations[address];
                await existingOp.Task;

                if (existingOp.Status == AsyncOperationStatus.Succeeded)
                {
                    T result = existingOp.Result as T;
                    if (autoAddRef)
                        AddReference(address);
                    callback?.Invoke(result);
                    return result;
                }
            }

            // 开始新的加载操作
            try
            {
                var handle = Addressables.LoadAssetAsync<T>(address);
                _loadingOperations[address] = handle;

                T loadedAsset = await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    // 添加到缓存
                    _assetCache[address] = new CachedAsset
                    {
                        Asset = loadedAsset,
                        Handle = handle,
                        LoadTime = Time.time,
                        LastAccessTime = Time.time
                    };

                    if (autoAddRef)
                        AddReference(address);

                    Log($"Asset loaded successfully: {address}");
                    callback?.Invoke(loadedAsset);
                    return loadedAsset;
                }
                else
                {
                    LogError($"Failed to load asset: {address}, Error: {handle.OperationException}");
                    callback?.Invoke(null);
                    return null;
                }
            }
            catch (Exception e)
            {
                LogError($"Exception while loading asset {address}: {e.Message}");
                callback?.Invoke(null);
                return null;
            }
            finally
            {
                _loadingOperations.Remove(address);
            }
        }

        /// <summary>
        /// 同步加载资源（仅用于已预加载的资源）
        /// </summary>
        public T GetAsset<T>(string address) where T : UnityEngine.Object
        {
            if (_assetCache.TryGetValue(address, out CachedAsset cachedAsset))
            {
                cachedAsset.LastAccessTime = Time.time;
                return cachedAsset.Asset as T;
            }

            LogWarning($"Asset not found in cache: {address}. Consider preloading this asset.");
            return null;
        }

        /// <summary>
        /// 批量加载资源
        /// </summary>
        public async Task<List<T>> LoadAssetsAsync<T>(List<string> addresses, Action<List<T>> callback = null) where T : UnityEngine.Object
        {
            List<T> results = new List<T>();
            List<Task<T>> loadTasks = new List<Task<T>>();

            foreach (string address in addresses)
            {
                loadTasks.Add(LoadAssetAsync<T>(address));
            }

            T[] loadedAssets = await Task.WhenAll(loadTasks);
            results.AddRange(loadedAssets);

            callback?.Invoke(results);
            Log($"Batch loaded {results.Count}/{addresses.Count} assets.");
            return results;
        }
        #endregion

        #region 预加载功能
        /// <summary>
        /// 创建预加载组
        /// </summary>
        public void CreatePreloadGroup(string groupName, List<string> addresses, PreloadPriority priority = PreloadPriority.Normal)
        {
            if (_preloadGroups.ContainsKey(groupName))
            {
                LogWarning($"Preload group already exists: {groupName}");
                return;
            }

            _preloadGroups[groupName] = new PreloadGroup
            {
                Name = groupName,
                Addresses = new List<string>(addresses),
                Priority = priority,
                Status = PreloadStatus.Pending
            };

            Log($"Created preload group: {groupName} with {addresses.Count} assets.");
        }

        /// <summary>
        /// 执行预加载组
        /// </summary>
        public async Task<bool> PreloadGroupAsync(string groupName, Action<float> progressCallback = null)
        {
            if (!_preloadGroups.TryGetValue(groupName, out PreloadGroup group))
            {
                LogError($"Preload group not found: {groupName}");
                return false;
            }

            if (group.Status == PreloadStatus.Loading)
            {
                LogWarning($"Preload group is already loading: {groupName}");
                return false;
            }

            group.Status = PreloadStatus.Loading;
            group.LoadedCount = 0;

            Log($"Starting preload for group: {groupName}");

            try
            {
                List<Task> loadTasks = new List<Task>();

                foreach (string address in group.Addresses)
                {
                    loadTasks.Add(LoadAssetAsync<UnityEngine.Object>(address, null, false));
                }

                // 监控进度
                while (loadTasks.Count > 0)
                {
                    int completedCount = 0;
                    for (int i = loadTasks.Count - 1; i >= 0; i--)
                    {
                        if (loadTasks[i].IsCompleted)
                        {
                            completedCount++;
                            loadTasks.RemoveAt(i);
                            group.LoadedCount++;
                        }
                    }

                    float progress = (float)group.LoadedCount / group.Addresses.Count;
                    progressCallback?.Invoke(progress);

                    if (loadTasks.Count > 0)
                    {
                        await Task.Delay(50); // 50ms检查间隔
                    }
                }

                group.Status = PreloadStatus.Completed;
                Log($"Preload group completed: {groupName}");
                progressCallback?.Invoke(1.0f);
                return true;
            }
            catch (Exception e)
            {
                group.Status = PreloadStatus.Failed;
                LogError($"Preload group failed: {groupName}, Error: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 按标签预加载资源
        /// </summary>
        public async Task<bool> PreloadByLabelAsync(string label, Action<float> progressCallback = null)
        {
            try
            {
                var locations = await Addressables.LoadResourceLocationsAsync(label).Task;

                if (locations.Count == 0)
                {
                    LogWarning($"No assets found with label: {label}");
                    return false;
                }

                List<string> addresses = new List<string>();
                foreach (var location in locations)
                {
                    addresses.Add(location.PrimaryKey);
                }

                string groupName = $"Label_{label}";
                CreatePreloadGroup(groupName, addresses);
                return await PreloadGroupAsync(groupName, progressCallback);
            }
            catch (Exception e)
            {
                LogError($"Failed to preload by label {label}: {e.Message}");
                return false;
            }
        }
        #endregion

        #region 场景资源管理
        /// <summary>
        /// 注册场景资源组
        /// </summary>
        public void RegisterSceneAssets(string sceneName, List<string> assetAddresses)
        {
            if (!_sceneAssets.ContainsKey(sceneName))
            {
                _sceneAssets[sceneName] = new List<string>();
            }

            _sceneAssets[sceneName].AddRange(assetAddresses);
            Log($"Registered {assetAddresses.Count} assets for scene: {sceneName}");
        }

        /// <summary>
        /// 预加载场景资源
        /// </summary>
        public async Task<bool> PreloadSceneAssetsAsync(string sceneName, Action<float> progressCallback = null)
        {
            if (!_sceneAssets.TryGetValue(sceneName, out List<string> addresses))
            {
                LogWarning($"No assets registered for scene: {sceneName}");
                return false;
            }

            string groupName = $"Scene_{sceneName}";
            CreatePreloadGroup(groupName, addresses, PreloadPriority.High);
            return await PreloadGroupAsync(groupName, progressCallback);
        }

        /// <summary>
        /// 释放场景资源
        /// </summary>
        public void ReleaseSceneAssets(string sceneName)
        {
            if (!_sceneAssets.TryGetValue(sceneName, out List<string> addresses))
            {
                return;
            }

            foreach (string address in addresses)
            {
                ReleaseAsset(address);
            }

            Log($"Released assets for scene: {sceneName}");
        }
        #endregion

        #region 引用计数和释放
        /// <summary>
        /// 增加资源引用计数
        /// </summary>
        public void AddReference(string address)
        {
            if (!_referenceCount.ContainsKey(address))
            {
                _referenceCount[address] = 0;
            }
            _referenceCount[address]++;

            Log($"Added reference for {address}, count: {_referenceCount[address]}");
        }

        /// <summary>
        /// 减少资源引用计数
        /// </summary>
        public void RemoveReference(string address)
        {
            if (_referenceCount.ContainsKey(address))
            {
                _referenceCount[address]--;
                Log($"Removed reference for {address}, count: {_referenceCount[address]}");

                if (_referenceCount[address] <= 0)
                {
                    _referenceCount.Remove(address);
                    // 可以选择立即释放或等待自动清理
                    // ReleaseAsset(address);
                }
            }
        }

        /// <summary>
        /// 释放单个资源
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
                _referenceCount.Remove(address);

                Log($"Released asset: {address}");
            }
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

                // 检查引用计数和最后访问时间
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
                Log($"Auto-released {toRelease.Count} unused assets.");
            }
        }

        /// <summary>
        /// 强制释放所有资源
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
            _loadingOperations.Clear();

            Log("Released all assets.");
        }
        #endregion

        #region 工具方法
        /// <summary>
        /// 获取资源加载状态
        /// </summary>
        public ResourceLoadStatus GetAssetStatus(string address)
        {
            if (_assetCache.ContainsKey(address))
            {
                return ResourceLoadStatus.Loaded;
            }
            if (_loadingOperations.ContainsKey(address))
            {
                return ResourceLoadStatus.Loading;
            }
            return ResourceLoadStatus.NotLoaded;
        }

        /// <summary>
        /// 获取缓存信息
        /// </summary>
        public ResourceCacheInfo GetCacheInfo()
        {
            return new ResourceCacheInfo
            {
                CachedAssetCount = _assetCache.Count,
                LoadingOperationCount = _loadingOperations.Count,
                PreloadGroupCount = _preloadGroups.Count,
                TotalReferenceCount = _referenceCount.Count
            };
        }

        /// <summary>
        /// 检查资源是否存在
        /// </summary>
        public async Task<bool> CheckAssetExistsAsync(string address)
        {
            try
            {
                var locations = await Addressables.LoadResourceLocationsAsync(address).Task;
                return locations.Count > 0;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 日志方法
        private void Log(string message)
        {
            if (enableResourceLogging)
            {
                Debug.Log($"[ResourceManager] {message}");
            }
        }

        private void LogWarning(string message)
        {
            if (enableResourceLogging)
            {
                Debug.LogWarning($"[ResourceManager] {message}");
            }
        }

        private void LogError(string message)
        {
            Debug.LogError($"[ResourceManager] {message}");
        }
        #endregion

        #region 清理
        private void OnDestroy()
        {
            ReleaseAllAssets();
        }

        private void OnApplicationQuit()
        {
            ReleaseAllAssets();
        }
        #endregion
    }
}



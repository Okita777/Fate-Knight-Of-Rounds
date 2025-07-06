using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ResourceSystem.Core
{
    /// <summary>
    /// 资源包管理器
    /// 管理不同类型的资源包，支持模块化资源管理
    /// </summary>
    public class ResourcePackManager : MonoBehaviour
    {
        private static ResourcePackManager _instance;
        private static readonly object _packLock = new object();

        public static ResourcePackManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_packLock)
                    {
                        if (_instance == null)
                        {
                            GameObject go = new GameObject("ResourcePackManager");
                            _instance = go.AddComponent<ResourcePackManager>();
                            DontDestroyOnLoad(go);
                        }
                    }
                }
                return _instance;
            }
        }

        [Header("资源包配置")]
        [SerializeField] private List<ResourcePackConfig> predefinedPacks = new List<ResourcePackConfig>();

        private ResourceManager resourceManager;
        private Dictionary<string, ResourcePackConfig> _registeredPacks = new Dictionary<string, ResourcePackConfig>();
        private Dictionary<string, bool> _loadedPacks = new Dictionary<string, bool>();

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
            resourceManager = ResourceManager.Instance;

            // 注册预定义的资源包
            foreach (var pack in predefinedPacks)
            {
                RegisterResourcePack(pack);
            }

            Debug.Log($"ResourcePackManager initialized with {predefinedPacks.Count} predefined packs.");
        }

        /// <summary>
        /// 注册资源包
        /// </summary>
        public void RegisterResourcePack(ResourcePackConfig packConfig)
        {
            if (_registeredPacks.ContainsKey(packConfig.PackName))
            {
                Debug.LogWarning($"Resource pack already registered: {packConfig.PackName}");
                return;
            }

            _registeredPacks[packConfig.PackName] = packConfig;
            _loadedPacks[packConfig.PackName] = false;

            Debug.Log($"Registered resource pack: {packConfig.PackName}");
        }

        /// <summary>
        /// 加载资源包
        /// </summary>
        public async Task<bool> LoadResourcePackAsync(string packName, Action<float> progressCallback = null)
        {
            if (!_registeredPacks.TryGetValue(packName, out ResourcePackConfig pack))
            {
                Debug.LogError($"Resource pack not found: {packName}");
                return false;
            }

            if (_loadedPacks[packName])
            {
                Debug.LogWarning($"Resource pack already loaded: {packName}");
                return true;
            }

            Debug.Log($"Loading resource pack: {packName}");

            // 创建预加载组
            resourceManager.CreatePreloadGroup(packName, pack.AssetAddresses, pack.Priority);

            // 执行预加载
            bool success = await resourceManager.PreloadGroupAsync(packName, progressCallback);

            if (success)
            {
                _loadedPacks[packName] = true;
                Debug.Log($"Resource pack loaded successfully: {packName}");
            }
            else
            {
                Debug.LogError($"Failed to load resource pack: {packName}");
            }

            return success;
        }

        /// <summary>
        /// 卸载资源包
        /// </summary>
        public void UnloadResourcePack(string packName)
        {
            if (!_registeredPacks.TryGetValue(packName, out ResourcePackConfig pack))
            {
                Debug.LogError($"Resource pack not found: {packName}");
                return;
            }

            if (!_loadedPacks[packName])
            {
                Debug.LogWarning($"Resource pack not loaded: {packName}");
                return;
            }

            // 释放包中的所有资源
            foreach (string address in pack.AssetAddresses)
            {
                resourceManager.ReleaseAsset(address);
            }

            _loadedPacks[packName] = false;
            Debug.Log($"Resource pack unloaded: {packName}");
        }

        /// <summary>
        /// 检查资源包是否已加载
        /// </summary>
        public bool IsPackLoaded(string packName)
        {
            return _loadedPacks.TryGetValue(packName, out bool loaded) && loaded;
        }

        /// <summary>
        /// 获取所有已注册的资源包
        /// </summary>
        public List<string> GetRegisteredPacks()
        {
            return new List<string>(_registeredPacks.Keys);
        }

        /// <summary>
        /// 获取已加载的资源包
        /// </summary>
        public List<string> GetLoadedPacks()
        {
            List<string> loadedPacks = new List<string>();
            foreach (var kvp in _loadedPacks)
            {
                if (kvp.Value)
                {
                    loadedPacks.Add(kvp.Key);
                }
            }
            return loadedPacks;
        }
    }
}
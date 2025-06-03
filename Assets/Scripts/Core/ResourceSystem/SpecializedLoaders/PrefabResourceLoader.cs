using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ResourceSystem.Core
{


    /// <summary>
    /// 预制体资源专用加载器
    /// 提供预制体实例化和池化管理
    /// </summary>
    public class PrefabResourceLoader
    {
        private ResourceManager resourceManager;
        private Dictionary<string, Queue<GameObject>> _prefabPools = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<GameObject, string> _instanceToAddress = new Dictionary<GameObject, string>();

        public PrefabResourceLoader()
        {
            resourceManager = ResourceManager.Instance;
        }

        /// <summary>
        /// 加载并实例化预制体
        /// </summary>
        public async Task<GameObject> InstantiatePrefabAsync(string address, Transform parent = null, bool usePool = false)
        {
            // 如果使用对象池
            if (usePool && _prefabPools.TryGetValue(address, out Queue<GameObject> pool) && pool.Count > 0)
            {
                GameObject pooledInstance = pool.Dequeue();
                pooledInstance.SetActive(true);
                if (parent != null)
                {
                    pooledInstance.transform.SetParent(parent);
                }
                return pooledInstance;
            }

            // 加载预制体
            GameObject prefab = await resourceManager.LoadAssetAsync<GameObject>(address);
            if (prefab == null)
                return null;

            // 实例化
            GameObject instance = UnityEngine.Object.Instantiate(prefab, parent);

            // 如果使用对象池，记录实例
            if (usePool)
            {
                _instanceToAddress[instance] = address;
            }

            return instance;
        }

        /// <summary>
        /// 回收预制体实例到对象池
        /// </summary>
        public void RecyclePrefabInstance(GameObject instance)
        {
            if (_instanceToAddress.TryGetValue(instance, out string address))
            {
                if (!_prefabPools.ContainsKey(address))
                {
                    _prefabPools[address] = new Queue<GameObject>();
                }

                instance.SetActive(false);
                instance.transform.SetParent(null);
                _prefabPools[address].Enqueue(instance);
            }
            else
            {
                // 非池化对象直接销毁
                UnityEngine.Object.Destroy(instance);
            }
        }

        /// <summary>
        /// 清理对象池
        /// </summary>
        public void ClearPool(string address)
        {
            if (_prefabPools.TryGetValue(address, out Queue<GameObject> pool))
            {
                while (pool.Count > 0)
                {
                    GameObject instance = pool.Dequeue();
                    if (instance != null)
                    {
                        _instanceToAddress.Remove(instance);
                        UnityEngine.Object.Destroy(instance);
                    }
                }
                _prefabPools.Remove(address);
            }
        }
    }
}
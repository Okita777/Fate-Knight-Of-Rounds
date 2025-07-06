# 资源管理系统 API 文档

## ResourceManager

#### LoadAssetAsync<T>() ⭐⭐⭐
异步加载指定地址的资源，支持自动缓存和引用计数管理
- `address`: 资源地址
- `callback`: 加载完成回调
- `autoAddRef`: 是否自动增加引用计数
```csharp
Task<T> LoadAssetAsync<T>(string address, Action<T> callback = null, bool autoAddRef = true)
```

#### GetAsset<T>() ⭐⭐⭐
同步获取已缓存的资源，不会触发新的加载操作
- `address`: 资源地址
- `返回值`: 已缓存的资源或null
```csharp
T GetAsset<T>(string address)
```

#### ReleaseAsset() ⭐⭐
立即释放指定资源，清除缓存并减少引用计数
- `address`: 要释放的资源地址
```csharp
void ReleaseAsset(string address)
```

#### PreloadGroupAsync() ⭐⭐
执行预加载组的加载操作，支持进度回调
- `groupName`: 预加载组名称
- `progressCallback`: 进度回调(0.0-1.0)
- `返回值`: 是否加载成功
```csharp
Task<bool> PreloadGroupAsync(string groupName, Action<float> progressCallback = null)
```

#### LoadAssetsAsync<T>()
批量加载多个资源，所有资源并行加载
- `addresses`: 资源地址列表
- `返回值`: 成功加载的资源列表
```csharp
Task<List<T>> LoadAssetsAsync<T>(List<string> addresses)
```

#### CreatePreloadGroup()
创建预加载组，用于批量预加载相关资源
- `groupName`: 组名称
- `addresses`: 资源地址列表
- `priority`: 预加载优先级
```csharp
void CreatePreloadGroup(string groupName, List<string> addresses, PreloadPriority priority = PreloadPriority.Normal)
```

#### AddReference()
手动增加资源引用计数
- `address`: 资源地址
```csharp
void AddReference(string address)
```

#### RemoveReference()
手动减少资源引用计数
- `address`: 资源地址
```csharp
void RemoveReference(string address)
```

#### GetCacheInfo()
获取当前缓存状态信息
- `返回值`: 包含缓存资源数量、引用计数等信息
```csharp
ResourceCacheInfo GetCacheInfo()
```

#### IsAssetCached()
检查指定资源是否已缓存
- `address`: 资源地址
- `返回值`: 是否已缓存
```csharp
bool IsAssetCached(string address)
```

#### ReleaseAllAssets()
释放所有缓存的资源，通常在场景切换时使用
```csharp
void ReleaseAllAssets()
```

## ResourcePackManager

#### LoadResourcePackAsync() ⭐⭐⭐
异步加载指定的资源包
- `packName`: 资源包名称
- `progressCallback`: 加载进度回调
- `返回值`: 是否加载成功
```csharp
Task<bool> LoadResourcePackAsync(string packName, Action<float> progressCallback = null)
```

#### RegisterResourcePack() ⭐⭐
注册资源包配置
- `packConfig`: 资源包配置对象
```csharp
void RegisterResourcePack(ResourcePackConfig packConfig)
```

#### UnloadResourcePack()
卸载指定资源包，释放包内所有资源
- `packName`: 资源包名称
```csharp
void UnloadResourcePack(string packName)
```

#### IsPackLoaded()
检查资源包是否已加载
- `packName`: 资源包名称
- `返回值`: 是否已加载
```csharp
bool IsPackLoaded(string packName)
```

#### GetRegisteredPacks()
获取已注册的资源包列表
- `返回值`: 资源包名称列表
```csharp
List<string> GetRegisteredPacks()
```

#### GetLoadedPacks()
获取已加载的资源包列表
- `返回值`: 资源包名称列表
```csharp
List<string> GetLoadedPacks()
```

## AudioResourceLoader

#### LoadAudioAsync() ⭐⭐⭐
加载音频资源，提供额外的音频缓存优化
- `address`: 音频资源地址
- `autoAddRef`: 是否自动增加引用计数
- `返回值`: 音频剪辑对象
```csharp
Task<AudioClip> LoadAudioAsync(string address, bool autoAddRef = true)
```

#### PreloadAudioGroupAsync()
预加载音频组
- `audioAddresses`: 音频地址列表
- `progressCallback`: 进度回调
- `返回值`: 是否预加载成功
```csharp
Task<bool> PreloadAudioGroupAsync(List<string> audioAddresses, Action<float> progressCallback = null)
```

#### ReleaseAudio()
释放音频资源
- `address`: 音频资源地址
```csharp
void ReleaseAudio(string address)
```

## PrefabResourceLoader

#### InstantiatePrefabAsync() ⭐⭐⭐
加载并实例化预制体，支持对象池优化
- `address`: 预制体地址
- `parent`: 父级Transform
- `usePool`: 是否使用对象池
- `返回值`: 实例化的GameObject
```csharp
Task<GameObject> InstantiatePrefabAsync(string address, Transform parent = null, bool usePool = false)
```

#### RecyclePrefabInstance() ⭐⭐
回收预制体实例到对象池或销毁
- `instance`: 要回收的GameObject实例
```csharp
void RecyclePrefabInstance(GameObject instance)
```

#### ClearAllPools()
清理所有对象池，销毁池中的所有对象
```csharp
void ClearAllPools()
```

#### ClearPool()
清理指定地址的对象池
- `address`: 预制体地址
```csharp
void ClearPool(string address)
```

#### GetPoolStatus()
获取所有对象池的状态
- `返回值`: 地址到池中对象数量的映射
```csharp
Dictionary<string, int> GetPoolStatus()
```
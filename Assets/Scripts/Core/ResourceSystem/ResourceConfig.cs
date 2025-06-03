using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem.Core
{
    /// <summary>
    /// 资源系统配置文件
    /// </summary>
    [CreateAssetMenu(fileName = "ResourceConfig", menuName = "Game/Resource Config")]
    public class ResourceConfig : ScriptableObject
    {
        [Header("常用资源地址")]
        public string playerPrefabAddress = "Characters/Player/PlayerPrefab";
        public string uiCanvasAddress = "UI/MainCanvas";
        public string gameManagerAddress = "Core/GameManager";

        [Header("预加载配置")]
        public List<ResourcePackConfig> preloadPacks = new List<ResourcePackConfig>();

        [Header("场景资源映射")]
        public List<SceneResourceMapping> sceneResourceMappings = new List<SceneResourceMapping>();

        [Header("音频资源")]
        public List<string> backgroundMusicAddresses = new List<string>();
        public List<string> soundEffectAddresses = new List<string>();
    }

    [System.Serializable]
    public class SceneResourceMapping
    {
        public string sceneName;
        public List<string> requiredAssets = new List<string>();
        public bool autoPreload = true;
    }
}


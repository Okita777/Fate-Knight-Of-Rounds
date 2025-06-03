using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ResourceSystem.Core
{
    /// <summary>
    /// 音频资源专用加载器
    /// 提供音频资源的特殊管理功能
    /// </summary>
    public class AudioResourceLoader
    {
        private ResourceManager resourceManager;
        private Dictionary<string, AudioClip> _audioCache = new Dictionary<string, AudioClip>();

        public AudioResourceLoader()
        {
            resourceManager = ResourceManager.Instance;
        }

        /// <summary>
        /// 加载音频资源
        /// </summary>
        public async Task<AudioClip> LoadAudioAsync(string address, bool autoAddRef = true)
        {
            if (_audioCache.TryGetValue(address, out AudioClip cachedClip))
            {
                return cachedClip;
            }

            AudioClip clip = await resourceManager.LoadAssetAsync<AudioClip>(address, autoAddRef: autoAddRef);
            if (clip != null)
            {
                _audioCache[address] = clip;
            }

            return clip;
        }

        /// <summary>
        /// 预加载音频组
        /// </summary>
        public async Task<bool> PreloadAudioGroupAsync(List<string> audioAddresses, Action<float> progressCallback = null)
        {
            resourceManager.CreatePreloadGroup("AudioGroup", audioAddresses, PreloadPriority.Normal);
            return await resourceManager.PreloadGroupAsync("AudioGroup", progressCallback);
        }

        /// <summary>
        /// 释放音频资源
        /// </summary>
        public void ReleaseAudio(string address)
        {
            if (_audioCache.ContainsKey(address))
            {
                _audioCache.Remove(address);
            }
            resourceManager.ReleaseAsset(address);
        }
    }
}
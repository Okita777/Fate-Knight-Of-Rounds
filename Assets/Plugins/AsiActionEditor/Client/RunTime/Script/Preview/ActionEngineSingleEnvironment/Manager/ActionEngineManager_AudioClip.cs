using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AsiTimeLine.RunTime
{
    public class ActionEngineManager_AudioClip
    {
        #region Instance
        private static ActionEngineManager_AudioClip _instance = null;
        public static ActionEngineManager_AudioClip Instance
        {
            get
            {
                if (_instance is null) _instance = new ActionEngineManager_AudioClip();
                return _instance;
            }
        }
        #endregion

        public const string _assetsFolder = "Audio";
        public bool loaded = false;
        public AudioClipDicList _audioClipDicList = null;
        
        private bool initialized = false;

        public void Init()
        {
            if (!initialized)
            {
                initialized = true;
                LoadAudioClipDicList(false);
            }
        }

        private void LoadAudioClipDicList(bool _loadAllAudioClipDic)
        {
            ActionEnginLoadData.Instance.LoadAudioClip(_assetsFolder, (value) =>
            {
                _audioClipDicList = value;
                _audioClipDicList.Load(_loadAllAudioClipDic);
                loaded = true;
            });
        }
    }
}
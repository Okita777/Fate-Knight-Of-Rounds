using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    public class ActionEngineManager_GValue
    {
        #region Instance
        private static ActionEngineManager_GValue _instance;
        public static ActionEngineManager_GValue Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new ActionEngineManager_GValue();
                }

                return _instance;
            }
        }
        #endregion

        private EngineGValue _engineGValue;
        private bool isLoaded = false;

        public void Init()
        {
            OnLoad();
        }
        private EngineGValue OnLoad()
        {
            if (isLoaded) return _engineGValue;
            ActionEnginLoadData.Instance.LoadGValue((value) =>
            {
                _engineGValue = value;
            });
            isLoaded = true;
            return _engineGValue;
        }

        public EngineGValue GValue
        {
            get { return OnLoad().Clone(); }
        }
    }
}
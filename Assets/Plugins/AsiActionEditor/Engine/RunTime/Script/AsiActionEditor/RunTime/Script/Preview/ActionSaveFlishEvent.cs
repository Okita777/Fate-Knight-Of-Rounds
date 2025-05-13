using UnityEngine.Events;

namespace AsiActionEngine.RunTime
{
    public class ActionSaveFlishEvent
    {
        // private static ActionSaveFlishEvent _instance;
        // public static ActionSaveFlishEvent Instance;
        public static UnityEvent ActionEvent = new UnityEvent();

        public static void Run()
        {
            ActionEvent?.Invoke();
            ActionEvent?.RemoveAllListeners();
        }
    }
}
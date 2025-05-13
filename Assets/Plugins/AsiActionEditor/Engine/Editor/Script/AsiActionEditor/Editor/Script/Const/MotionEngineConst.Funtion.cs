using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class MotionEngineConst
    {
        public static bool FindObjects<T>(out T _FindValue) where T: Behaviour
        {
            var _mainUI1 = Resources.FindObjectsOfTypeAll<T>();
            foreach (var item in _mainUI1)
            {
                if (!EditorUtility.IsPersistent(item))
                {
                    _FindValue = item;
                    return true;
                }
            }

            _FindValue = null;
            return false;
        }
    }
}
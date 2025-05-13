using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public class MiscFunctions
    {
        //查找场景中所有的对象
        public static List<T> FindObjects<T>() where T: MonoBehaviour
        {
            List<T> _retrunValue = new List<T>();
            var _mainUI1 = Resources.FindObjectsOfTypeAll<T>();
            foreach (var item in _mainUI1)
            {
                if (!EditorUtility.IsPersistent(item))
                    _retrunValue.Add(item);
            }
            return _retrunValue;
        }
    }
}
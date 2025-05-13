using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public partial class EngineDebug
    {
        public static void Log(string _string)
        {
#if UNITY_EDITOR
            Debug.Log(_string);
#endif
        }
        public static void LogWarning(string _string)
        {
#if UNITY_EDITOR
            Debug.LogWarning(_string);
#endif
        }
        public static void LogError(string _string)
        {
#if UNITY_EDITOR
            Debug.LogError(_string);
#endif
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color, float life = 0)
        {
#if UNITY_EDITOR
            EngineScenceDraw.Line(start, end, color, life);
#endif
        }
        
        public static void DrawBox(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
#if UNITY_EDITOR
            EngineScenceDraw.Box(position, rotation, scale, color);
#endif
        }
        
        public static void DrawSphere(Vector3 pos, float radius, Color color, float life = 0)
        {
#if UNITY_EDITOR
            EngineScenceDraw.Sphere(pos, Quaternion.identity, radius, color);
#endif
// #if UNITY_EDITOR
//             Vector3 startpos = Vector3.forward * radius;
//             for (int i = 1; i <= 375; i += 15)
//             {
//                 Vector3 lastpos = Quaternion.Euler(0,i,0) * Vector3.forward * radius;
//                 Debug.DrawLine(startpos + pos, lastpos + pos, color, life);
//                 startpos = lastpos;
//             }
//             startpos = Vector3.forward * radius;
//             for (int i = 1; i <= 375; i += 15)
//             {
//                 Vector3 lastpos = Quaternion.Euler(i,0,0) * Vector3.forward * radius;
//                 Debug.DrawLine(startpos + pos, lastpos + pos, color, life);
//                 startpos = lastpos;
//             }
//             startpos = Vector3.right * radius;
//             for (int i = 1; i <= 375; i += 15)
//             {
//                 Vector3 lastpos = Quaternion.Euler(0,0,i) * Vector3.right * radius;
//                 Debug.DrawLine(startpos + pos, lastpos + pos, color, life);
//                 startpos = lastpos;
//             }
// #endif
        }
        public static void DrawCapsule(Vector3 startPos, Vector3 endPos, float radius, Color color, float life = 0)
        {
#if UNITY_EDITOR
            EngineScenceDraw.Capsule(startPos, endPos, radius, color, life);
#endif
        }

        public static void DisplayDialog(string title,string message,string ok,string cancel = "")
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog(title, message, ok, cancel);
#endif

        }
    }
}
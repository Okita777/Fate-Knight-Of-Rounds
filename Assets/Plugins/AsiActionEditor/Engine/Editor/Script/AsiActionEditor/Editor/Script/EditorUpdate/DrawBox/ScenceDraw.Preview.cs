using System;
using System.Diagnostics;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;
// using Debug = UnityEngine.Debug;

namespace AsiActionEngine.Editor
{
    public partial class ScenceDraw : UnityEditor.Editor
    {
        private void OnSceneGUI(SceneView _sceneView)
        {

            // Stopwatch _stopwatch = new Stopwatch();
            // _stopwatch.Start();

            DrawBox();
            mIsChange = false;
            // _stopwatch.Stop();
            // EngineDebug.Log("运行耗时(ms)" + _stopwatch.ElapsedMilliseconds);
        }


    }
}
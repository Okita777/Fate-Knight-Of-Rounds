using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class ScenceDraw
    {
        private static ScenceDraw mInstance = null;

        public static ScenceDraw Instance
        {
            get
            {
                if (mInstance is null)
                {
                    // Debug.LogWarning("Instance Null");
                    mInstance = new ScenceDraw();
                    SceneView.duringSceneGui += mInstance.OnSceneGUI;
                }

                return mInstance;
            }
        }

        private bool mIsChange = false;
        public void OnUpdateDraw()
        {
            if (!mIsChange)
            {
                TimeLineWindow.Instance.UpdateTimeToNow();
                SceneView.RepaintAll();
                mIsChange = true;
            }
        }
    }
}
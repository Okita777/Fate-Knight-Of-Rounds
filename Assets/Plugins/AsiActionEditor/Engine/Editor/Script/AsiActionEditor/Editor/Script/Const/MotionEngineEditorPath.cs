using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public class MotionEngineEditorPath
    {
        private static MotionEngineEditorPath mInstance;
        public static MotionEngineEditorPath Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new MotionEngineEditorPath();
                return mInstance;
            }
        }

        private string mEditorLocalPath = null;
        public string EditorLocalPath
        {
            get
            {
                if (string.IsNullOrEmpty(mEditorLocalPath))
                {
                    string[] _guid = AssetDatabase.FindAssets(nameof(MotionEngineEditorPath));
                    string[] _nowPath = AssetDatabase.GUIDToAssetPath(_guid[0]).Split('/');
                    for (int i = 0; i < _nowPath.Length - 3; i++)
                        mEditorLocalPath += (_nowPath[i] + "/");
                }
                return mEditorLocalPath;
            }
        }
    }
}
using System;
using AsiActionEngine.Editor;
using AsiTimeLine.RunTime;
using UnityEditor;

namespace AsiTimeLine.Editor
{
    [CustomEditor(typeof(ActionEditor_LockUnit))]
    public class ActionEditor_LockUnit_Editor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            using (var _chack = new EditorGUI.ChangeCheckScope())
            {
                DrawEditorAttribute.Draw(target);
                if (_chack.changed)
                {
                    EditorUtility.SetDirty(target);
                }
            }
            // base.OnInspectorGUI();
        }
    }
}
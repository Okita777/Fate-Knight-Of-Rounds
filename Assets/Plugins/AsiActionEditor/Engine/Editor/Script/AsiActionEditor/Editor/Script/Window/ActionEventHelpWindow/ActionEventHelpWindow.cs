using System;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class ActionEventHelpWindow : EditorWindow
    {
        private static Action m_DrawFuntion = null;
        private static EditorActionEvent m_ActionEvent = null;

        public static void Show(Action funtion, EditorActionEvent _actionEvent)
        {
            string title = _actionEvent.EventData.GetType().ToString();
            ActionEventHelpWindow window =
                (ActionEventHelpWindow)GetWindow(typeof(ActionEventHelpWindow), true, title, true);
            m_DrawFuntion = funtion;
            m_ActionEvent = _actionEvent;
            window.Show();
        }
        private void OnGUI()
        {
            if (m_DrawFuntion == null)
            {
                if(m_ActionEvent == null)this.Close();
                GUILayout.Label("当前事件未编写任何说明");
                GUILayout.Space(10);

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("当前事件类型：");
                    using (new GUIColorScope(Color.cyan))
                    {
                        GUILayout.Label(m_ActionEvent.EventData.GetType().ToString());
                    }
                }
                if (GUILayout.Button("跳转至编写说明的函数"))
                {
                    string _path = "";
                    string[] _guid = AssetDatabase.FindAssets("DrawInspector.Event.HelpWindow");
                    string[] _nowPath = AssetDatabase.GUIDToAssetPath(_guid[0]).Split('/');
                    for (int i = 0; i < _nowPath.Length-1; i++)
                        _path += (_nowPath[i] + "/");
                    _path += _nowPath[^1];
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(_path, 20);
                }
            }
            else
            {
                m_DrawFuntion();
            }
        }
    }
}
using AsiActionEngine.Editor;
using AsiTimeLine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiTimeLine.Editor
{
    [CustomEditor(typeof(ActionEditor_Audio))]
    public class ActionEditor_Audio_Editor : UnityEditor.Editor
    {
        private ActionEditor_Audio main => target as ActionEditor_Audio;
        public override void OnInspectorGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("音频组件设置");
            GUILayout.Space(10);

            //绘制添加按钮
            using (new GUIColorScope(Color.green))
            {
                if (GUILayout.Button("添加音频播放组件"))
                {
                    main.m_AudioSourceNames.Add("音频组件_" + main.m_AudioSourceNames.Count);
                    main.m_AudioSources.Add(null);
                }
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                for (int i = 0; i < main.m_AudioSourceNames.Count; i++)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        main.m_AudioSourceNames[i] = EditorGUILayout.TextField(main.m_AudioSourceNames[i]);
                        main.m_AudioSources[i] =
                            EditorGUILayout.ObjectField(main.m_AudioSources[i], typeof(AudioSource), true) as
                                AudioSource;
                        using (new GUIColorScope(Color.red))
                        {
                            if (GUILayout.Button("-", GUILayout.Width(25)))
                            {
                                main.m_AudioSourceNames.RemoveAt(i);
                                main.m_AudioSources.RemoveAt(i);
                            }
                        }
                    }
                }

                if (check.changed)
                {
                    EditorUtility.SetDirty(main);
                }
            }

            GUILayout.Space(10);
            if (GUILayout.Button("打开音频配置窗口"))
            {
                AudioClipWindows.Instance.Open();
            }
        }
    }
}
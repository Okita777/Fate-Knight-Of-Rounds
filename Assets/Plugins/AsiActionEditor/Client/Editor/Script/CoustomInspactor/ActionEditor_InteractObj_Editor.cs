using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using AsiTimeLine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiTimeLine.Editor
{
    [CustomEditor(typeof(ActionEditor_InteractObj))]
    public class ActionEditor_InteractObj_Editor : UnityEditor.Editor
    {
        ActionEditor_InteractObj main => target as ActionEditor_InteractObj;
        public override void OnInspectorGUI()
        {
            GUILayout.Label("ActionEngine交互对象");
            GUILayout.Space(10);

            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                main.InteractName = EditorGUILayout.DelayedTextField("交互时的显示名称：", main.InteractName);
                main.AutoTrigger = EditorGUILayout.Toggle("进入时自动触发", main.AutoTrigger);
                GUILayout.Space(10);

                GUILayout.Label("触发时执行的事件：");
                main.m_TriggerActionName_Self =
                    EditorGUILayout.DelayedTextField("自身播放动画：", main.m_TriggerActionName_Self);
                main.m_TriggerActionName = EditorGUILayout.DelayedTextField("触发者播放动画：", main.m_TriggerActionName);
                main.m_mixTime = EditorGUILayout.DelayedIntField("融合时间(ms)", main.m_mixTime);
                main.m_offsetTime = EditorGUILayout.DelayedIntField("剪切时间(ms)", main.m_offsetTime);
                GUILayout.Space(10);

                GUILayout.Label("写入挂点：");
                using (new GUIColorScope(Color.green))
                {
                    if (GUILayout.Button("添加挂点"))
                    {
                        GValue_SetTransform _value = new GValue_SetTransform();
                        _value.m_IsSet = true;
                        main.m_GValueSets.Add(_value);
                        main.m_Transforms.Add(null);
                    }
                }

                for (int i = 0; i < main.m_Transforms.Count; i++)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        DrawEditorAttribute.OnDrawSetGTransform(main.m_GValueSets[i]);
                        main.m_Transforms[i] = (Transform)EditorGUILayout.ObjectField(main.m_Transforms[i], typeof(Transform));
                        using (new GUIColorScope(Color.red))
                        {
                            if (GUILayout.Button("-", GUILayout.Width(20)))
                            {
                                main.m_GValueSets.RemoveAt(i);
                                main.m_Transforms.RemoveAt(i);
                            }
                        }
                    }
                }

                if (_check.changed)
                {
                    EditorUtility.SetDirty(main);
                }
            }
        }
    }
}
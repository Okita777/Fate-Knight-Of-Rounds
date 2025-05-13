using System;
using System.Collections.Generic;
using System.Linq;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    [CustomEditor(typeof(CharacterConfig))]
    public class CharacterConfig_Editor : UnityEditor.Editor
    {
        private ECharacteLimbType _characteLimb;
        private Transform _transform;

        private CharacterConfig main => target as CharacterConfig;
        public override void OnInspectorGUI()
        {
            GUIStyle _style = new GUIStyle();
            _style.normal.textColor = Color.white;
            _style.alignment = TextAnchor.MiddleCenter;
            _style.fontSize = 24;
            GUILayout.Label("挂点配置", _style);

            //挂点配置
            GUILayout.Space(10);
            using (new GUILayout.HorizontalScope())
            {
                _characteLimb = (ECharacteLimbType)EditorGUILayout.EnumPopup(_characteLimb);
                _transform = EditorGUILayout.ObjectField(_transform,typeof(Transform),true) as Transform;
                if (GUILayout.Button("添加类型"))
                {
                    if (main.HelpPointDic.ContainsKey(_characteLimb))
                    {
                        EngineDebug.LogError("无法添加，字典里已存在同值");
                    }
                    else
                    {
                        main.HelpPointDic.Add(_characteLimb, _transform);
                        EditorUtility.SetDirty(main);
                    }
                }
            }
            
            GUILayout.Space(10);
            List<ECharacteLimbType> _dicEnum = main.HelpPointDic.Keys.ToList();
            for (int i = 0; i < _dicEnum.Count; i++)
            {
                using (new GUILayout.HorizontalScope())
                {
                    using (new GUIColorScope(Color.red))
                    {
                        if (GUILayout.Button("-",GUILayout.Width(25)))
                        {
                            main.HelpPointDic.Remove(_dicEnum[i]);
                            EditorUtility.SetDirty(main);
                            return;
                        }
                    }

                    Enum _targetType = EditorGUILayout.EnumPopup(_dicEnum[i]);

                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        main.HelpPointDic[_dicEnum[i]] =
                            EditorGUILayout.ObjectField(main.HelpPointDic[_dicEnum[i]], typeof(Transform), true) as Transform;
                        if (_check.changed)
                        {
                            EditorUtility.SetDirty(main);
                        }
                    }
                }
            }
        }
    }
}
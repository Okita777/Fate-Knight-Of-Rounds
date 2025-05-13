using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    [CustomEditor(typeof(ActionEditor_LayerMask))]
    public class ActionEditor_LayerMask_Editor : UnityEditor.Editor
    {
        private ActionEditor_LayerMask main => target as ActionEditor_LayerMask;
        private Vector2 scrollPos;
        private List<string> layerNames = new List<string>();
        private List<int> layerInts = new List<int>();
        public override void OnInspectorGUI()
        {
            using (new GUIColorScope(Color.green))
            {
                if (GUILayout.Button("添加层级"))
                {
                    layerNames.Clear();
                    layerInts.Clear();
                    layerNames.AddRange(main.mLayerName);
                    layerInts.AddRange(main.mLayers);
                    layerNames.Add($"NewLayer {layerNames.Count}");
                    layerInts.Add(0);
                    main.mLayerName = layerNames.ToArray();
                    main.mLayers = layerInts.ToArray();
                    EditorUtility.SetDirty(main);
                    AssetDatabase.SaveAssetIfDirty(main);
                }
            }

            using (var _scroll = new EditorGUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = _scroll.scrollPosition;
                for (int i = 0; i < main.mLayerName.Length; i++)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        using (var _check = new EditorGUI.ChangeCheckScope())
                        {
                            main.mLayerName[i] = EditorGUILayout.DelayedTextField(main.mLayerName[i],GUILayout.Width(120));
                            LayerMask _mask = EditorGUILayout.MaskField(
                                InternalEditorUtility.LayerMaskToConcatenatedLayersMask(main.mLayers[i]),
                                InternalEditorUtility.layers);
                            main.mLayers[i] = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(_mask);
                            if (_check.changed)
                            {
                                EditorUtility.SetDirty(main);
                            }
                        }

                        using (new GUIColorScope(Color.red))
                        {
                            if (GUILayout.Button("-", GUILayout.Width(20)))
                            {
                                if (layerNames.Count > 1)
                                {
                                    layerNames.Clear();
                                    layerInts.Clear();
                                    layerNames.AddRange(main.mLayerName);
                                    layerInts.AddRange(main.mLayers);
                                    layerNames.RemoveAt(i);
                                    layerInts.RemoveAt(i);
                                    main.mLayerName = layerNames.ToArray();
                                    main.mLayers = layerInts.ToArray();
                                    EditorUtility.SetDirty(main);
                                    AssetDatabase.SaveAssetIfDirty(main);
                                }
                            }
                        }
                    }
                }
            }

            // base.OnInspectorGUI();
        }
    }
}
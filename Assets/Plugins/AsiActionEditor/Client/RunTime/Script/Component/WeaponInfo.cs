using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
// using Ara;
#endif

namespace ActionEngine.Runtime
{
    [System.Serializable]
    public class WeaponInfo : MonoBehaviour
    {
        public SkinnedMeshRenderer mesh;
        public Transform trailRoot;
        public Material trailMaterial;
        // public OverrideParameter<AnimationCurve> thicknessOverLength = new OverrideParameter<AnimationCurve>(AnimationCurve.Linear(0, 1, 0, 1));
        // public OverrideParameter<Gradient> colorOverLength = new OverrideParameter<Gradient>(new Gradient());
        // public OverrideParameter<AnimationCurve> thicknessOverTime = new OverrideParameter<AnimationCurve>(AnimationCurve.Linear(0, 1, 0, 1));
        // public OverrideParameter<Gradient> colorOverTime = new OverrideParameter<Gradient>(new Gradient());
        // public OverrideParameter<float> initialThickness = new OverrideParameter<float>(1.0f);
        // public OverrideParameter<Color> initialColor = new OverrideParameter<Color>(Color.white);
        // public OverrideParameter<float> timeInterval = new OverrideParameter<float>(0);
        // public OverrideParameter<float> minDistance = new OverrideParameter<float>(0);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(WeaponInfo))]
    [CanEditMultipleObjects]
    public class WeaponInfoEditor : Editor
    {
        private WeaponInfo m_Info;

        private void Awake()
        {
            m_Info = target as WeaponInfo;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("自动配置"))
            {
                foreach (var gameObject in Selection.gameObjects)
                {
                    if (gameObject.TryGetComponent(out WeaponInfo weapon))
                    {
                        var skinnedMR = weapon.GetComponentInChildren<SkinnedMeshRenderer>();
                        if (skinnedMR)
                        {
                            weapon.mesh = skinnedMR;
                        }

                        if (skinnedMR.transform.childCount > 0)
                        {
                            Transform trailRoot = skinnedMR.transform.GetChild(0);
                            // if(trailRoot && trailRoot.TryGetComponent(out AraTrail record))
                            // {
                            //     weapon.thicknessOverLength.SetParameter(new AnimationCurve(record.thicknessOverLength.keys));
                            //     weapon.colorOverLength.SetParameter(CopyGradient(record.colorOverLength));
                            //     weapon.thicknessOverTime.SetParameter(new AnimationCurve(record.thicknessOverTime.keys));
                            //     weapon.colorOverTime.SetParameter(CopyGradient(record.colorOverTime));
                            //     weapon.initialThickness.SetParameter(record.initialThickness);
                            //     weapon.initialColor.SetParameter(record.initialColor);
                            //     weapon.timeInterval.SetParameter(record.timeInterval);
                            //     weapon.minDistance.SetParameter(record.minDistance);
                            //     // weapon.time.SetParameter(record.time);
                            //
                            //     weapon.trailMaterial = record.materials[0];
                            // }

                            weapon.trailRoot = trailRoot;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Error", "未检测到刀光根挂点", "ok");
                        }
                        
                        EditorUtility.SetDirty(weapon);
                    }
                }
            }

            base.OnInspectorGUI();
        }

        private Gradient CopyGradient(Gradient source)
        {
            Gradient copy = new Gradient();
            copy.colorKeys = source.colorKeys;
            copy.alphaKeys = source.alphaKeys;
            return copy;
        }
    }
#endif
}
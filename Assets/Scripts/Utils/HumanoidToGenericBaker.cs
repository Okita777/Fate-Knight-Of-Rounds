using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimpleHumanoidToGenericConverter : EditorWindow
{
    [Header("必要设置")]
    public GameObject humanoidModel; // 带Humanoid Avatar的模型
    public GameObject genericModel;  // 目标Generic模型
    public AnimationClip humanoidClip; // 要转换的Humanoid动画

    [Header("转换设置")]
    public float sampleRate = 60f; // 采样率
    public bool includeFingers = true; // 是否包含手指动画
    public bool includeRootMotion = true; // 是否包含根运动
    public bool optimizeKeyframes = true; // 是否优化关键帧
    public float optimizeThreshold = 0.001f; // 优化阈值

    private Dictionary<HumanBodyBones, string> boneMap = new Dictionary<HumanBodyBones, string>();
    private Vector2 scrollPosition;

    [MenuItem("Tools/Simple Humanoid to Generic Converter")]
    static void Init()
    {
        var window = GetWindow<SimpleHumanoidToGenericConverter>("Humanoid→Generic");
        window.minSize = new Vector2(350, 500);
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("Humanoid to Generic Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 基本设置
        humanoidModel = (GameObject)EditorGUILayout.ObjectField("Humanoid模型", humanoidModel, typeof(GameObject), true);
        genericModel = (GameObject)EditorGUILayout.ObjectField("Generic模型", genericModel, typeof(GameObject), true);
        humanoidClip = (AnimationClip)EditorGUILayout.ObjectField("Humanoid动画", humanoidClip, typeof(AnimationClip), false);

        EditorGUILayout.Space();

        // 转换设置
        EditorGUILayout.LabelField("转换设置", EditorStyles.boldLabel);
        sampleRate = EditorGUILayout.Slider("采样率", sampleRate, 30f, 120f);
        includeFingers = EditorGUILayout.Toggle("包含手指动画", includeFingers);
        includeRootMotion = EditorGUILayout.Toggle("包含根运动", includeRootMotion);
        optimizeKeyframes = EditorGUILayout.Toggle("优化关键帧", optimizeKeyframes);

        if (optimizeKeyframes)
        {
            optimizeThreshold = EditorGUILayout.Slider("优化阈值", optimizeThreshold, 0.0001f, 0.01f);
        }

        EditorGUILayout.Space();

        // 骨骼映射预览
        if (humanoidModel != null && genericModel != null)
        {
            if (GUILayout.Button("刷新骨骼映射"))
            {
                BuildBoneMapping();
            }

            if (boneMap.Count > 0)
            {
                EditorGUILayout.LabelField($"映射骨骼数量: {boneMap.Count}", EditorStyles.helpBox);
            }
        }

        EditorGUILayout.Space();

        // 转换按钮
        bool canConvert = CanConvert();

        if (!canConvert)
        {
            EditorGUILayout.HelpBox("请设置所有必要字段", MessageType.Warning);
        }

        GUI.enabled = canConvert;

        if (GUILayout.Button("开始转换", GUILayout.Height(40)))
        {
            ConvertAnimation();
        }

        GUI.enabled = true;

        EditorGUILayout.EndScrollView();
    }

    bool CanConvert()
    {
        if (humanoidModel == null || genericModel == null || humanoidClip == null)
            return false;

        var animator = humanoidModel.GetComponent<Animator>();
        if (animator == null || animator.avatar == null || !animator.avatar.isHuman)
            return false;

        return true;
    }

    void BuildBoneMapping()
    {
        boneMap.Clear();

        var humanoidAnimator = humanoidModel.GetComponent<Animator>();
        if (humanoidAnimator == null)
            return;

        var genericBones = genericModel.GetComponentsInChildren<Transform>();
        var genericBoneDict = new Dictionary<string, Transform>();

        // 建立Generic模型的骨骼字典
        foreach (var bone in genericBones)
        {
            if (!genericBoneDict.ContainsKey(bone.name))
                genericBoneDict[bone.name] = bone;
        }

        // 映射主要骨骼
        var mainBones = new HumanBodyBones[]
        {
            HumanBodyBones.Hips, HumanBodyBones.Spine, HumanBodyBones.Chest, HumanBodyBones.UpperChest,
            HumanBodyBones.Neck, HumanBodyBones.Head,
            HumanBodyBones.LeftShoulder, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand,
            HumanBodyBones.RightShoulder, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand,
            HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot, HumanBodyBones.LeftToes,
            HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot, HumanBodyBones.RightToes
        };

        foreach (var humanBone in mainBones)
        {
            var humanTransform = humanoidAnimator.GetBoneTransform(humanBone);
            if (humanTransform == null)
                continue;

            if (genericBoneDict.ContainsKey(humanTransform.name))
            {
                string genericPath = GetRelativePath(genericModel.transform, genericBoneDict[humanTransform.name]);
                boneMap[humanBone] = genericPath;
            }
        }

        // 映射手指骨骼
        if (includeFingers)
        {
            var fingerBones = new HumanBodyBones[]
            {
                // 左手手指
                HumanBodyBones.LeftThumbProximal, HumanBodyBones.LeftThumbIntermediate, HumanBodyBones.LeftThumbDistal,
                HumanBodyBones.LeftIndexProximal, HumanBodyBones.LeftIndexIntermediate, HumanBodyBones.LeftIndexDistal,
                HumanBodyBones.LeftMiddleProximal, HumanBodyBones.LeftMiddleIntermediate, HumanBodyBones.LeftMiddleDistal,
                HumanBodyBones.LeftRingProximal, HumanBodyBones.LeftRingIntermediate, HumanBodyBones.LeftRingDistal,
                HumanBodyBones.LeftLittleProximal, HumanBodyBones.LeftLittleIntermediate, HumanBodyBones.LeftLittleDistal,
                // 右手手指
                HumanBodyBones.RightThumbProximal, HumanBodyBones.RightThumbIntermediate, HumanBodyBones.RightThumbDistal,
                HumanBodyBones.RightIndexProximal, HumanBodyBones.RightIndexIntermediate, HumanBodyBones.RightIndexDistal,
                HumanBodyBones.RightMiddleProximal, HumanBodyBones.RightMiddleIntermediate, HumanBodyBones.RightMiddleDistal,
                HumanBodyBones.RightRingProximal, HumanBodyBones.RightRingIntermediate, HumanBodyBones.RightRingDistal,
                HumanBodyBones.RightLittleProximal, HumanBodyBones.RightLittleIntermediate, HumanBodyBones.RightLittleDistal
            };

            foreach (var fingerBone in fingerBones)
            {
                var humanTransform = humanoidAnimator.GetBoneTransform(fingerBone);
                if (humanTransform == null)
                    continue;

                if (genericBoneDict.ContainsKey(humanTransform.name))
                {
                    string genericPath = GetRelativePath(genericModel.transform, genericBoneDict[humanTransform.name]);
                    boneMap[fingerBone] = genericPath;
                }
            }
        }
    }

    void ConvertAnimation()
    {
        string savePath = EditorUtility.SaveFilePanelInProject(
            "保存Generic动画",
            humanoidClip.name + "_Generic.anim",
            "anim",
            "选择保存位置");

        if (string.IsNullOrEmpty(savePath))
            return;

        try
        {
            EditorUtility.DisplayProgressBar("转换动画", "初始化...", 0f);

            // 构建骨骼映射
            BuildBoneMapping();

            // 创建新动画剪辑
            var newClip = new AnimationClip();
            newClip.name = humanoidClip.name + "_Generic";
            newClip.frameRate = humanoidClip.frameRate;

            var humanoidAnimator = humanoidModel.GetComponent<Animator>();

            // 计算采样数据
            float duration = humanoidClip.length;
            int totalFrames = Mathf.CeilToInt(duration * sampleRate);

            // 存储曲线数据
            var curveData = new Dictionary<string, AnimationCurve[]>();

            // 为每个映射的骨骼创建曲线
            foreach (var boneMapping in boneMap)
            {
                string path = boneMapping.Value;
                curveData[path] = new AnimationCurve[10]; // pos(3) + rot(4) + scale(3)

                for (int i = 0; i < 10; i++)
                {
                    curveData[path][i] = new AnimationCurve();
                }
            }

            // 根运动曲线
            AnimationCurve[] rootCurves = null;
            if (includeRootMotion)
            {
                rootCurves = new AnimationCurve[10];
                for (int i = 0; i < 10; i++)
                {
                    rootCurves[i] = new AnimationCurve();
                }
            }

            // 采样动画数据
            for (int frame = 0; frame <= totalFrames; frame++)
            {
                float time = (float)frame / sampleRate;
                float progress = (float)frame / totalFrames;

                EditorUtility.DisplayProgressBar("转换动画", $"采样帧 {frame}/{totalFrames}", progress * 0.8f);

                // 应用动画到Humanoid模型
                humanoidClip.SampleAnimation(humanoidModel, time);

                // 采样每个映射的骨骼
                foreach (var boneMapping in boneMap)
                {
                    var humanBone = boneMapping.Key;
                    string path = boneMapping.Value;

                    var transform = humanoidAnimator.GetBoneTransform(humanBone);
                    if (transform == null)
                        continue;

                    var curves = curveData[path];

                    // 位置
                    curves[0].AddKey(time, transform.localPosition.x);
                    curves[1].AddKey(time, transform.localPosition.y);
                    curves[2].AddKey(time, transform.localPosition.z);

                    // 旋转
                    curves[3].AddKey(time, transform.localRotation.x);
                    curves[4].AddKey(time, transform.localRotation.y);
                    curves[5].AddKey(time, transform.localRotation.z);
                    curves[6].AddKey(time, transform.localRotation.w);

                    // 缩放
                    curves[7].AddKey(time, transform.localScale.x);
                    curves[8].AddKey(time, transform.localScale.y);
                    curves[9].AddKey(time, transform.localScale.z);
                }

                // 根运动
                if (includeRootMotion && rootCurves != null)
                {
                    var rootTransform = humanoidModel.transform;

                    rootCurves[0].AddKey(time, rootTransform.localPosition.x);
                    rootCurves[1].AddKey(time, rootTransform.localPosition.y);
                    rootCurves[2].AddKey(time, rootTransform.localPosition.z);

                    rootCurves[3].AddKey(time, rootTransform.localRotation.x);
                    rootCurves[4].AddKey(time, rootTransform.localRotation.y);
                    rootCurves[5].AddKey(time, rootTransform.localRotation.z);
                    rootCurves[6].AddKey(time, rootTransform.localRotation.w);

                    rootCurves[7].AddKey(time, rootTransform.localScale.x);
                    rootCurves[8].AddKey(time, rootTransform.localScale.y);
                    rootCurves[9].AddKey(time, rootTransform.localScale.z);
                }
            }

            EditorUtility.DisplayProgressBar("转换动画", "生成曲线...", 0.9f);

            // 设置曲线到动画剪辑
            string[] propertyNames = {
                "localPosition.x", "localPosition.y", "localPosition.z",
                "localRotation.x", "localRotation.y", "localRotation.z", "localRotation.w",
                "localScale.x", "localScale.y", "localScale.z"
            };

            foreach (var pathCurves in curveData)
            {
                string path = pathCurves.Key;
                var curves = pathCurves.Value;

                for (int i = 0; i < curves.Length; i++)
                {
                    if (optimizeKeyframes)
                    {
                        OptimizeCurve(curves[i]);
                    }

                    newClip.SetCurve(path, typeof(Transform), propertyNames[i], curves[i]);
                }
            }

            // 设置根运动曲线
            if (includeRootMotion && rootCurves != null)
            {
                for (int i = 0; i < rootCurves.Length; i++)
                {
                    if (optimizeKeyframes)
                    {
                        OptimizeCurve(rootCurves[i]);
                    }

                    newClip.SetCurve("", typeof(Transform), propertyNames[i], rootCurves[i]);
                }
            }

            // 复制动画设置
            var originalSettings = AnimationUtility.GetAnimationClipSettings(humanoidClip);
            AnimationUtility.SetAnimationClipSettings(newClip, originalSettings);

            // 保存动画
            AssetDatabase.CreateAsset(newClip, savePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();

            Debug.Log($"转换完成！\n保存路径: {savePath}\n映射骨骼: {boneMap.Count} 个");

            // 选中生成的动画
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<AnimationClip>(savePath);
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError($"转换失败: {e.Message}");
            EditorUtility.DisplayDialog("转换失败", e.Message, "确定");
        }
    }

    void OptimizeCurve(AnimationCurve curve)
    {
        if (curve.keys.Length <= 2)
            return;

        var keys = new List<Keyframe>(curve.keys);
        var optimized = new List<Keyframe> { keys[0] }; // 保留第一个

        for (int i = 1; i < keys.Count - 1; i++)
        {
            var prev = keys[i - 1];
            var current = keys[i];
            var next = keys[i + 1];

            // 线性插值检查
            float t = (current.time - prev.time) / (next.time - prev.time);
            float interpolated = Mathf.Lerp(prev.value, next.value, t);

            if (Mathf.Abs(current.value - interpolated) > optimizeThreshold)
            {
                optimized.Add(current);
            }
        }

        optimized.Add(keys[keys.Count - 1]); // 保留最后一个
        curve.keys = optimized.ToArray();
    }

    string GetRelativePath(Transform root, Transform target)
    {
        if (target == root)
            return "";

        var path = new List<string>();
        var current = target;

        while (current != null && current != root)
        {
            path.Insert(0, current.name);
            current = current.parent;
        }

        return string.Join("/", path);
    }
}
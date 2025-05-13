using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
using UnityEditor.Animations;
using System.IO;

namespace FbxAnimationEditor
{
    public class FbxEditorWindow : EditorWindow
    {
        #region enum
        enum TransformType
        {
            Position,
            Rotation,
            Scale
        }

        enum VectorAxis 
        { 
            x,
            y,
            z
        }
        #endregion

        #region File
        const string ArtPath = "Assets/EditorResources/Character/";//动作美术资源路径  
        const string BundlePath = "Assets/Resources/Entity/Character/";//动作资源路径
        static EditorWindow m_Window;
        //static Object selectObj;//上一次选中的对象
        ModelImporter modelImporter;
        ModelImporterClipAnimation[] allAnima;

        [SerializeField]
        ClipAnimationInfoCurve[] my_Copy;
        [SerializeField]
        List<string> my_Copy2 = new List<string>();
        string clipName = "";
        bool unfold = false;

        string assetpath;
        AvatarMask avatarMask;

        int FrameSpeed = 30;//帧率
        float FrameTime;//帧时
        float Proportion;//Humanoid比例

        [SerializeField]
        int startFrame;
        [SerializeField]
        Vector3 offsetPos;
        [SerializeField]
        bool IsRota = false, IsHumanoid = false,PosX = true,PosY = true,PosZ = true;
        bool ishuona = false;
        RuntimeAnimatorController animator;

        SerializedObject _mobj;
        SerializedProperty _startFrame, _offsetPos, _IsRota, _IsHumanoid, _my_Copy;
        float animaEndTime = 0;
        bool animaIsLoop = false;
        bool isSetFootEvents = false;
        bool isRightFoot = false;
        #endregion

        #region MenuItem
        [MenuItem("美术工具/动画导出工具/FbxCurEditor &`")]
        static void StartWindow()
        {
            if (m_Window == null)
            {
                m_Window = GetWindow(typeof(FbxEditorWindow), true);
                m_Window.titleContent = new GUIContent("RootMotionEditor");
            }
            else
            {
                m_Window.Close();
                m_Window = null;
            }
        }
        #endregion

        #region Behaviour
        private void OnEnable()
        {
            Init();
        }

        void OnInspectorUpdate()
        {
            Repaint();//每帧执行OnGUI函数
        }

        private void OnGUI()
        {
            UpdateGui();
        }
        #endregion

        #region Method
        void Init()
        {
            FrameTime = 1.0f / FrameSpeed;
            // selectObj = null;
            modelImporter = null;

            _mobj = new SerializedObject(this);
            _startFrame = _mobj.FindProperty("startFrame");
            _offsetPos = _mobj.FindProperty("offsetPos");
            _IsRota = _mobj.FindProperty("IsRota");
            _IsHumanoid = _mobj.FindProperty("IsHumanoid");
            _my_Copy = _mobj.FindProperty("my_Copy2");
        }

        void UpdateGui()
        {
            Object selectObject = Selection.activeObject;
            assetpath = AssetDatabase.GetAssetPath(selectObject);
            _mobj.Update();

            //if (selectObj != selectObject)
            modelImporter = null;
            animator = null;
            string[] formats = new string[0];
            bool IsAnimationClip = false;
            //assetpath = AssetDatabase.GetAssetPath(selectObject);
            //Debug.Log(assetpath);

            if (assetpath != "")
            {
                formats = assetpath.Split('.');
            }
            if (formats.Length > 1)
            {
                string format = formats[^1].ToLower();
                if (format == "fbx")
                {
                    // Debug.Log("选中FBX了");
                    ModelImporter importer = (ModelImporter)ModelImporter.GetAtPath(assetpath);
                    try
                    {
                        allAnima = importer.clipAnimations;
                        //allAnima = importer.defaultClipAnimations;
                        if (allAnima.Length > 0)
                        {
                            // Debug.Log("选中FBX了");
                            modelImporter = importer;//如果选中的对象存在动画文件的话
                        }
                    }
                    catch (System.Exception)
                    {
                        modelImporter = null;
                        allAnima = null;
                    }
                }else if(format == "controller")
                {
                    animator = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(assetpath);
                }
                else if (format == "anim")
                {
                    IsAnimationClip = true;
                }

            }

            //selectObj = selectObject;

            if (IsAnimationClip)
            {
                GUILayout.Label(selectObject.name);
                if (GUILayout.Button("跳转至FBX"))
                {
                    try
                    {
                        string FbxPath = ArtPath;//"Assets/Apps/MMO/ArtResource/Animation/"
                        int BundlePathLenth = BundlePath.Split('/').Length - 1;

                        string _assetpath = AssetDatabase.GetAssetPath(selectObject);
                        string[] ptch_Arr = _assetpath.Split('/');
                        string ptch_Creacte = FbxPath;
                        for (int i = BundlePathLenth ; i < ptch_Arr.Length - 1; i++)
                        {
                            //ptch_Creacte += ptch_Arr[i].Split('_')[1] + "/";
                            /*
                            string[] a = ptch_Arr[i].Split('_');
                            ptch_Creacte += a[1];
                            for (int t = 2; t < a.Length; t++)
                                ptch_Creacte += "_" + a[t];
                            */
                            ptch_Creacte += ptch_Arr[i] + "/";
                        }
                        //Debug.Log(ptch_Creacte);

                        string animaName = ptch_Arr[ptch_Arr.Length - 1].Split('.')[0];

                        DirectoryInfo qwe = new DirectoryInfo(ptch_Creacte);
                        var asd = qwe.GetFiles("*", SearchOption.AllDirectories);
                        for (int i = 0; i < asd.Length; i++)
                        {
                            string m_name = asd[i].Name;
                            if (m_name.ToLower().EndsWith(".fbx"))
                            {
                                if (m_name.Contains(animaName))
                                {
                                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(ptch_Creacte + m_name);
                                    EditorGUIUtility.PingObject(obj);
                                    Selection.activeObject = obj;
                                    return;
                                }
                            }
                        }
                        if (EditorUtility.DisplayDialog("注意", "未找到此文件，请先检查对应的FBX命名或者当前AnimationClip命名", "我知道了", "关闭")) { }
                    }
                    catch (System.Exception)
                    {
                        if (EditorUtility.DisplayDialog("注意", "未找到此文件，请先检查对应的FBX命名或者当前AnimationClip命名", "我知道了", "关闭")) { }
                    }

                    //string createAnimPath = ptch_Creacte + "/" + animaName + ".anim";

                }
            }

            if (modelImporter)
            {

                GUILayout.Label(selectObject.name);
                if (GUILayout.Button("跳转至AnimationClip"))
                {
                    try
                    {
                        string AnimPath = BundlePath;//"Assets/Apps/MMO/Bundles/Arts/Anim/" 
                        int ArtPathLenth = ArtPath.Split('/').Length - 1;

                        string _assetpath = AssetDatabase.GetAssetPath(selectObject);
                        string[] ptch_Arr = _assetpath.Split('/');
                        string ptch_Creacte = AnimPath;
                        for (int i = ArtPathLenth; i < ptch_Arr.Length - 1; i++)
                        {
                            //ptch_Creacte += "B_" + ptch_Arr[i] + "/";
                            ptch_Creacte += ptch_Arr[i] + "/";
                        }
                        //Debug.Log(ptch_Creacte);
                        string animaName = ptch_Arr[ptch_Arr.Length - 1].Split('.')[0];
                        animaName = animaName.Split('@')[0];

                        DirectoryInfo qwe = new DirectoryInfo(ptch_Creacte);

                        var asd = qwe.GetFiles("*", SearchOption.AllDirectories);
                        for (int i = 0; i < asd.Length; i++)
                        {
                            string m_name = asd[i].Name;
                            if (m_name.ToLower().EndsWith(".anim"))
                            {
                                if (m_name.Contains(animaName))
                                {
                                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(ptch_Creacte + m_name);
                                    EditorGUIUtility.PingObject(obj);
                                    Selection.activeObject = obj;
                                    return;
                                }
                            }
                        }
                        if (EditorUtility.DisplayDialog("注意", "未找到此文件，请先创建此FBX文件的动画", "我知道了", "关闭")) { }
                    }
                    catch (System.Exception)
                    {
                        if (EditorUtility.DisplayDialog("注意", "未找到此文件，请先创建此FBX文件的动画", "我知道了", "关闭")) { }
                        //throw;
                    }
                }

                EditorGUI.BeginChangeCheck();


                // //GUILayout.Space(20);
                // //GUILayout.Label("参考初始帧:");
                // //EditorGUILayout.PropertyField(_startFrame);
                //
                // GUILayout.Space(20);
                // GUILayout.Label("参考帧位置:");
                // EditorGUILayout.PropertyField(_offsetPos);
                //
                // GUILayout.Space(20);
                // EditorGUILayout.BeginHorizontal();
                // animaIsLoop = EditorGUILayout.ToggleLeft("动画是否循环", animaIsLoop);
                // EditorGUILayout.EndHorizontal();
                //
                // //GUILayout.Space(20);
                // //EditorGUILayout.BeginHorizontal();
                // //GUILayout.Label("是否在动画结束时改变了角色朝向(不用勾选，自动判断):");
                // //EditorGUILayout.Toggle(IsRota);
                // ////EditorGUILayout.PropertyField(_IsRota);
                // //EditorGUILayout.EndHorizontal();
                //
                // GUILayout.Space(20);
                // EditorGUILayout.BeginHorizontal();
                // isSetFootEvents = EditorGUILayout.ToggleLeft("动画是否存在踩踏事件", isSetFootEvents);
                // EditorGUILayout.EndHorizontal();
                // if (isSetFootEvents && animaIsLoop)
                // {
                //     EditorGUILayout.BeginHorizontal();
                //     GUILayout.Space(20);
                //     isRightFoot = EditorGUILayout.ToggleLeft("是否为右脚起步", isRightFoot);
                //     EditorGUILayout.EndHorizontal();
                // }




                GUILayout.Space(20);
                /*
                GUILayout.Label("烘焙轴:");
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("X");
                PosX = EditorGUILayout.Toggle(PosX);
                GUILayout.Label("Y");
                PosY = EditorGUILayout.Toggle(PosY);
                GUILayout.Label("Z");
                PosZ = EditorGUILayout.Toggle(PosZ);
                EditorGUILayout.EndHorizontal();
                */

                if (GUILayout.Button("设置FBX配置文件"))
                    SetFbxCur_Human();

                if (GUILayout.Button("生成动画"))
                    CreateAnimaClip_Human();
                if (EditorGUI.EndChangeCheck()) _mobj.ApplyModifiedProperties();

                // GUILayout.Space(20);
                // EditorGUILayout.BeginHorizontal();
                // if (GUILayout.Button("复制非烘焙曲线(不可多选)"))
                // {
                //     string _assetpath = AssetDatabase.GetAssetPath(selectObject);
                //     ModelImporter _modelImporter = ((ModelImporter)ModelImporter.GetAtPath(_assetpath));
                //     ModelImporterClipAnimation[] _allanim = _modelImporter.clipAnimations;
                //     my_Copy = GetCurveCopy(_allanim[0].curves);
                //
                //     string[] ptch_Arr = _assetpath.Split('/');
                //     string animaName = ptch_Arr[ptch_Arr.Length - 1].Split('.')[0];
                //
                //     clipName = animaName;
                // }
                // if (GUILayout.Button("粘贴曲线（可多选，覆盖同名曲线）"))
                // {
                //     //Debug.LogError("粘贴曲线");
                //     Object[] selectObject2 = Selection.objects;
                //
                //     for (int i = 0; i < selectObject2.Length; i++)
                //     {
                //         string _assetpath = AssetDatabase.GetAssetPath(selectObject2[i]);
                //         ModelImporter _modelImporter = ((ModelImporter)ModelImporter.GetAtPath(_assetpath));
                //         ModelImporterClipAnimation[] _allanim = _modelImporter.clipAnimations;
                //         _allanim[0].curves = GetCurvePaste(_allanim[0].curves, my_Copy);
                //         _modelImporter.clipAnimations = _allanim;
                //         _modelImporter.SaveAndReimport();
                //     }
                // }
                //
                // if (unfold)
                // {
                //     if (GUILayout.Button("详细(展开)"))
                //     {
                //         unfold = false;
                //     }
                // }
                // else
                // {
                //     if (GUILayout.Button("详细(收起)"))
                //     {
                //         unfold = true;
                //     }
                // }
                // EditorGUILayout.EndHorizontal();
                // GUILayout.Label(clipName);
                // if (unfold)
                // {
                //     EditorGUI.BeginChangeCheck();
                //     EditorGUILayout.PropertyField(_my_Copy);//my_Copy2
                //     if (EditorGUI.EndChangeCheck()) {
                //         _mobj.ApplyModifiedProperties();
                //         //ClipAnimationInfoCurve[] my_Copy3 = new ClipAnimationInfoCurve[my_Copy2.Count];
                //         List<ClipAnimationInfoCurve> my_Copy3 = new List<ClipAnimationInfoCurve>();
                //         foreach (string a in my_Copy2) {
                //             foreach (ClipAnimationInfoCurve b in my_Copy) {
                //                 if (a == b.name) my_Copy3.Add(b);
                //             }
                //         }
                //         my_Copy = my_Copy3.ToArray();
                //     }
                //     //my_Copy = GUILayout.
                // }

            }

            if (animator)
            {
                GUILayout.Label(selectObject.name);
                if (GUILayout.Button("给当前Animator配置动画曲线")) 
                {
                    if (EditorUtility.DisplayDialog("注意", "      即将对该Animator添加相关曲线参数，请不要修改它们的默认值，\n \n       你确定要创建吗？", "确定", "关闭"))
                    {
                        SetRunTimeAnimatorController(animator);
                    }
                }
                if (GUILayout.Button("在文件夹中选中此Animator")) 
                {
                    EditorGUIUtility.PingObject(animator);
                    Selection.activeObject = animator;

                }
            }

            if (animator == null && modelImporter == null && IsAnimationClip == false)
            {
                GUILayout.Label("请先选择带动画的FBX文件");
            }
        }
        #endregion

        #region Event
        void SaveClipFile(AnimationClip clip, string path)
        {
            AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(clip, true);
            clip.ClearCurves();
            foreach (AnimationClipCurveData dt in curveDatas)
            {
                clip.SetCurve(dt.path, dt.type, dt.propertyName, dt.curve);
            }
            
            AnimationClip loadedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

            if (loadedClip)
            {
                //检查是否已经存在此文件
                EditorUtility.CopySerialized(clip, loadedClip);
                AssetDatabase.SaveAssets();
            }
            else
            {
                AssetDatabase.CreateAsset(clip, path);
            }

            // 保存成功
        }

        /// <summary>
        /// 创建动画
        /// </summary>
        /// <param name="fbxanim">Fbx文件下的动画</param>
        void CreateAnimaClip_Human()
        {
            if ((startFrame != 0 || offsetPos != Vector3.zero) && animaIsLoop) 
            {
                if (EditorUtility.DisplayDialog("警告", "交互动画不能循环，请保证参考位置和参考帧为零或者去掉循环动画的勾选", "我知道了", "关闭")) { }
                return;
            }

            Object[] selectObject = Selection.objects;
            if (selectObject.Length > 0)
            {
                string animaNames = "";
                for (int v = 0; v < selectObject.Length; v++)
                {
                    string _assetpath = AssetDatabase.GetAssetPath(selectObject[v]);
                    AnimationClip fbxanim = AssetDatabase.LoadAssetAtPath<AnimationClip>(_assetpath);
                    ModelImporter _modelImporter = ((ModelImporter)ModelImporter.GetAtPath(_assetpath));
                    ModelImporterClipAnimation[] _allanim = _modelImporter.clipAnimations;

                    {
                        AnimationClip a = new AnimationClip();

                        EditorUtility.CopySerialized(fbxanim, a);//复制fbx动画

                        EditorCurveBinding[] editorBinding = GetBindingToBindingPathChildes(a, "ChatRoot", 1, TransformType.Position);//得到ChatRoot下所有子级的曲线绑定
                        EditorCurveBinding[] editorBinding2 = GetBindingToBindingPathChildes(a, "Root", 0, TransformType.Position);//得到ChatRoot下所有子级的曲线绑定
                        //IsRota = GetIsRota(a);
                        ReSetBindingsPos(editorBinding, a);
                        ReSetBindingsPos2(editorBinding2, a);
                        //if(IsHumanoid) ReSetBindingsPos_Human(a);
                        try
                        {


                            //创建动画
                            string[] ptch_Arr = _assetpath.Split('/');
                            string ptch_Creacte = BundlePath;
                            //Debug.Log(ptch_Creacte);
                            int ArtPathLenth = ArtPath.Split('/').Length - 1;
                            for (int i = ArtPathLenth; i < ptch_Arr.Length - 1; i++)
                            {
                                //ptch_Creacte += "B_" + ptch_Arr[i] ;
                                ptch_Creacte += ptch_Arr[i] ;
                                if (i < ptch_Arr.Length - 2)
                                    ptch_Creacte += "/";
                            }
                            //ptch_Creacte += "AnimaClips";

                            //return;
                            if (!UnityEngine.Windows.Directory.Exists(ptch_Creacte))
                            {
                                //Debug.Log("动画文件路径不存在，已重新创建");
                                UnityEngine.Windows.Directory.CreateDirectory(ptch_Creacte);
                            }

                            string animaName = ptch_Arr[ptch_Arr.Length - 1].Split('.')[0];
                            animaName = animaName.Split('@')[0];
                            string createAnimPath = ptch_Creacte + "/" + animaName + ".anim";
                            string createAnimPathMeta = createAnimPath + ".meta";

                            SaveClipFile(a, createAnimPath);
                            animaNames += "     " + animaName + "\n";

                        }
                        catch (System.Exception)
                        {
                            if (EditorUtility.DisplayDialog("注意", "动画文件的关键帧设置失败！！ \n    请确认你fbx文件是否存在“Cur_PosX，Cur_PosY，Cur_PosZ”曲线，或者是否存在“ChatRoot”挂点", "我知道了", "关闭")) { }
                            //throw;
                        }
                    }
                }
                if (EditorUtility.DisplayDialog("注意", "动画文件: \n" + animaNames + "\n创建成功!!", "我知道了", "关闭")) { }
            }

        }
        void SetRunTimeAnimatorController(RuntimeAnimatorController _animator) {
            AnimatorController animatorController = _animator as AnimatorController;
            AnimatorControllerParameter parameter = new AnimatorControllerParameter();
            parameter.defaultFloat = 1.0f;
            parameter.type = AnimatorControllerParameterType.Float;

            //来自于动画数据烘焙
            AnimatorControllerParameter[] allParameter = animatorController.parameters;
            List<string> ParameterNames = new List<string> {
                //烘焙的曲线
                "Cur_PosX",
                "Cur_PosY",
                "Cur_PosZ",
                "Cur_AnimMix",
                "Cur_RotY",
                "Cur_ChatHeight",
                "Cur_ChatRadius",

                //非曲线，Animator用
                "Locked",//过渡到锁定对象
                "Move_Right",//锁定后的左右值
                "Move_Forward",//锁定后的前后值
                "Move_Speed",//角色速度
                "Move_TrunR",//角色左右倾斜
                "TurnAngle",//角色转向角度
                "Look_UP",//垂直方向注视
                "Look_Right",//水平方向注视
                "Weapon_State",//武器状态
                "Hit_State",

                //功能曲线
                "FootRight",//左右脚判断
                //"Cur_Hitrecover",//硬直
                "Cur_FaceWeight",//脸部表情权重  是否代替程序向脸部权重

                //功能
                "MoveSpeed",
                ////功能曲线
                //"Cur_Hitrecover",
                //"MoveRight",
                //"MoveUp",
                //"MoveUp_End",
                //"FootRight",
                //"FootRight_Forward",
                //"FootRightNormal",
                ////"FootRight_End",

                //权重曲线
                "Cur_RootWeight",
                "Cur_RotaWeight",
                //"Cur_LookWeight",
                //"Cur_FootIkWeight",
                //"Cur_GroundWeight",
                //"Cur_FaceWeight",

                ////非曲线，Animator用
                //"Look_UP",
                //"Look_Right",
                //"IsMove",
                //"IsGround",
                //"Right",
                //"Forward",
                //"Looked",
                //"GetUp",

                //"Cur_AnimSpeed",
                //"MoveSpeed",
                //"ClambSpeed",
                //"SwimSpeed"
            };

            for (int i = 0; i < ParameterNames.Count; i++)
            {
                string nameA = ParameterNames[i];
                bool isfind = false;
                for (int t = 0; t < allParameter.Length; t++)
                {
                    if (nameA == allParameter[t].name)
                    {
                        isfind = true;
                        ParameterNames.RemoveAt(i);
                        i--;
                        break;
                    }
                }
                if (!isfind)
                {
                    switch (nameA)
                    {
                        case "MoveSpeed":
                        case "ClambSpeed":
                        case "SwimSpeed":
                        case "Cur_FootIkWeight":
                        case "Cur_GroundWeight":
                        //case "Cur_FaceWeight":
                        case "Cur_RotaWeight"://默认值为1的float
                            parameter.name = nameA;
                            animatorController.AddParameter(parameter);
                            break;
                        case "IsMove":
                        case "IsGround":
                        case "GetUp"://默认值为false的bool
                            animatorController.AddParameter(nameA, AnimatorControllerParameterType.Bool);
                            break;
                        case "FootRight_Forward":
                            AnimatorControllerParameter parameter2 = new AnimatorControllerParameter();
                            parameter2.type = AnimatorControllerParameterType.Float;
                            parameter2.defaultFloat = -0.1f;
                            parameter2.name = nameA;
                            animatorController.AddParameter(parameter2);
                            break;
                        default://默认参数为0 并且参数类型为float的值
                            animatorController.AddParameter(nameA, AnimatorControllerParameterType.Float);
                            break;
                    }
                }
            }
            if (ParameterNames.Count == 0)
            {
                if (EditorUtility.DisplayDialog("注意", "      当前Animator已具备所有参数，无需配置", "确定", "关闭")) { }
            }
            else
            {
                string debugName = "已添加以下对象: " + "\n";
                for (int i = 0; i < ParameterNames.Count; i++)
                {
                    debugName += "   " + ParameterNames[i] + "\n";
                }
                Debug.Log(debugName);
            }
        }
        void SetFbxCur_Human()
        {
            if ((offsetPos != Vector3.zero) && animaIsLoop)
            {
                if (EditorUtility.DisplayDialog("警告", "交互动画不能循环，请保证参考位置和参考帧为零或者去掉循环动画的勾选" , "我知道了", "关闭")) { }
                return;
            }


            if (allAnima.Length < 1)
            {
                if (EditorUtility.DisplayDialog("警告", "未能获取动画，请手动初始化一下文件", "我知道了", "关闭")) { }
                return;
            }

            Object[] selectObject = Selection.objects;
            if (selectObject.Length > 0)
            {
                for (int v = 0; v < selectObject.Length; v++)
                {
                    startFrame = 0;
                    string _assetpath = AssetDatabase.GetAssetPath(selectObject[v]);
                    ModelImporter _modelImporter = ((ModelImporter)ModelImporter.GetAtPath(_assetpath));
                    ModelImporterClipAnimation[] _allanim = _modelImporter.clipAnimations;
                    string[] path_Arr = _assetpath.Split('/');
                    string path_Creacte = null;
                    for (int i = 0; i < path_Arr.Length - 1; i++)
                        path_Creacte += path_Arr[i] + "/";

                    if (IsHumanoid)
                    {
                        avatarMask = (AvatarMask)AssetDatabase.LoadAssetAtPath(path_Creacte + "RootMask.mask", typeof(AvatarMask));
                        if (!avatarMask)
                        {
                            if (EditorUtility.DisplayDialog("警告", "未能在当前文件夹下找到 ‘RootMask.mask’ ，请手动创建并设定一下(设置avatar为skin，transform全勾上)", "我知道了", "关闭")) { }
                            return;
                            //AnimationUtility.SetKeyLeftTangentMode
                        }
                    }
                    HumanoidSet(_modelImporter, _allanim);

                    AnimationClip anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(_assetpath);
                    AnimationCurve testCur = AnimationUtility.GetEditorCurve(anim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.x"));
                    if (testCur == null)
                    {
                        if (EditorUtility.DisplayDialog("警告", "获取曲线失败！！未找到 ‘RootPoint’ ，请检查当前骨骼", "我知道了", "关闭")) { }
                        return;
                    }
                    animaEndTime = testCur.keys[testCur.keys.Length - 1].time;

                    if (IsHumanoid)
                    {
                        Avatar mainAvatar = null;
                        ModelImporter mImporter = (ModelImporter)ModelImporter.GetAtPath(path_Creacte + "Main_Tpose.fbx");
                        if (mImporter == null)
                        {
                            if (EditorUtility.DisplayDialog("警告", "未能在当前文件夹下找到 “Main_Tpose.fbx” ，请确认命名" + "\n" + path_Creacte + "Main_Tpose.fbx", "我知道了", "关闭")) { }
                            return;
                        }

                        mainAvatar = mImporter.sourceAvatar;
                        if (!mainAvatar)
                        {
                            //mImporter = (ModelImporter)ModelImporter.GetAtPath(path_Creacte + "Main_Tpose.fbx");
                            mImporter.animationType = ModelImporterAnimationType.Human;
                            mImporter.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                            mImporter.SaveAndReimport();
                            mainAvatar = mImporter.sourceAvatar;
                        }

                    }
                    else
                    {
                        //_modelImporter.animationType = ModelImporterAnimationType.Generic;
                        //_modelImporter.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
                        //_modelImporter.SaveAndReimport();
                    }

                    IsRota = GetIsRota(anim);

                    AnimationCurve animacur = new AnimationCurve();
                    animacur.AddKey(0, 1);
                    animacur.AddKey(1, 1);
                    AnimationCurve animacur2 = new AnimationCurve();
                    animacur2.AddKey(0, 0);
                    animacur2.AddKey(1, 0);

                    int curLenth = 2;
                    if (IsRota) curLenth++;
                    if (isSetFootEvents) curLenth++;

                    string animaName = path_Arr[path_Arr.Length - 1].Split('.')[0];
                    string[] frames = animaName.Split('@');
                    for (int i = 1; i < frames.Length; i++)
                    {
                        string[] frames2 = frames[i].Split('-');
                        if (frames2.Length == 1)
                        {
                            try
                            {
                                startFrame = int.Parse(frames2[0]);
                            }
                            catch (System.Exception)
                            {
                                if (EditorUtility.DisplayDialog("警告", "检测到无效帧数，请检查FBX命名", "确定", "关闭")) { }
                                return;
                            }
                            break;
                        }
                    }

                    if (startFrame != 0 || offsetPos != Vector3.zero)
                    {
                        if (animaIsLoop)
                        {
                            if (EditorUtility.DisplayDialog("警告", "交互动画不能循环，请保证参考位置和参考帧为零或者去掉循环动画的勾选" + "\n" + "文件：" + selectObject[v].name, "我知道了", "关闭")) { }
                            return;
                        }

                        curLenth += 4;
                    }
                    ClipAnimationInfoCurve[] newCur = new ClipAnimationInfoCurve[curLenth];


                    if (startFrame != 0 || offsetPos != Vector3.zero)
                    {
                        newCur[0].name = "Cur_PosX";
                        newCur[0].curve = SetCurveTangentMode(GetCurvePos(AnimationUtility.GetEditorCurve(anim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.x")), VectorAxis.x), AnimationUtility.TangentMode.Linear);
                        newCur[1].name = "Cur_PosY";
                        newCur[1].curve = SetCurveTangentMode(GetCurvePos(AnimationUtility.GetEditorCurve(anim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.y")), VectorAxis.y), AnimationUtility.TangentMode.Linear);
                        newCur[2].name = "Cur_PosZ";
                        newCur[2].curve = SetCurveTangentMode(GetCurvePos(AnimationUtility.GetEditorCurve(anim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.z")), VectorAxis.z), AnimationUtility.TangentMode.Linear);

                        newCur[3].name = "Cur_AnimMix";
                        newCur[3].curve = animacur;
                    }
                    if (IsRota)
                        newCur = GetCurveRot(anim, newCur);

                    int animaCur = 0;

                    if (isSetFootEvents)
                    {
                        newCur[curLenth - 3 - animaCur].name = "FootRight";
                        if (animaIsLoop)
                        {
                            AnimationCurve newanimacur = new AnimationCurve();
                            if (isRightFoot)
                            {
                                newanimacur.AddKey(0, 1);
                                newanimacur.AddKey(0.5f, -1);
                                newanimacur.AddKey(1, 1);
                            }
                            else
                            {
                                newanimacur.AddKey(0, -1);
                                newanimacur.AddKey(0.5f, 1);
                                newanimacur.AddKey(1, -1);
                            }

                            SetCurveTangentMode(newanimacur, AnimationUtility.TangentMode.Linear);
                            newCur[curLenth - 3 - animaCur].curve = newanimacur;
                        }
                        else
                        {
                            newCur[curLenth - 3 - animaCur].curve = GetFootEvents();
                        }
                        animaCur++;
                    }//FootRight 容器


                    newCur[curLenth - 2].name = "Cur_ChatHeight";
                    bool curveIsValue = false;
                    newCur[curLenth - 2].curve = SetCurveTangentMode(GetCurvePos(AnimationUtility.GetEditorCurve(anim, EditorCurveBinding.FloatCurve("Root/RootPoint/ControllerState", typeof(Transform), "m_LocalPosition.y")),out curveIsValue), AnimationUtility.TangentMode.Linear);

                    if (!curveIsValue)
                    {
                        //Selection.activeObject = _modelImporter;
                        if (EditorUtility.DisplayDialog("警告", "找不到路径‘Root/RootPoint/ControllerState’对象" + "\n" +
                            "\n" +
                            "请按以下方法检测当前fbx：" + "\n" +
                            "   1、请检查‘Max文件’下的ControllerState是否有k帧或者绑定是否有误" + "\n" +
                            "   2、如果方法1没问题，那就可能是meta文件错误，请删除当前fbx的meta文件重新配置" + "\n" +
                            "", "确定", "关闭")) { }
                        EditorGUIUtility.PingObject(_modelImporter);
                        //Debug.LogWarning("发生错误的对象：" + _modelImporter.name);
                        return;
                    }
                    newCur[curLenth - 1].name = "Cur_ChatRadius";
                    newCur[curLenth - 1].curve = SetCurveTangentMode(GetCurvePos(AnimationUtility.GetEditorCurve(anim, EditorCurveBinding.FloatCurve("Root/RootPoint/ControllerState", typeof(Transform), "m_LocalScale.z"))), AnimationUtility.TangentMode.Linear);

                    _allanim[0].curves = GetCurveContrast(_allanim[0].curves, newCur);
                    _modelImporter.clipAnimations = _allanim;
                    _modelImporter.SaveAndReimport();
                    //Debug.Log("设置成功");
                }
            }
        }
        void MaskUpdate()
        {
            Object[] selectObject = Selection.objects;
            if (selectObject.Length > 0)
            {
                for (int v = 0; v < selectObject.Length; v++)
                {
                    string _assetpath = AssetDatabase.GetAssetPath(selectObject[v]);
                    ModelImporter _modelImporter = ((ModelImporter)ModelImporter.GetAtPath(_assetpath));
                    ModelImporterClipAnimation[] _allanim = _modelImporter.clipAnimations;

                    string[] ptch_Arr = _assetpath.Split('/');
                    string animaName = ptch_Arr[ptch_Arr.Length - 1].Split('.')[0];
                    animaName = animaName.Split('@')[0];
                    _allanim[0].name = animaName;

                    string[] path_Arr = _assetpath.Split('/');
                    string path_Creacte = null;
                    for (int i = 0; i < path_Arr.Length - 1; i++)
                        path_Creacte += path_Arr[i] + "/";

                    avatarMask = (AvatarMask)AssetDatabase.LoadAssetAtPath(path_Creacte + "RootMask.mask", typeof(AvatarMask));
                    if (!avatarMask)
                    {
                        if (EditorUtility.DisplayDialog("警告", "未能在当前文件夹下找到 ‘RootMask.mask’ ，请手动创建并设定一下(设置avatar为skin，transform全勾上)", "我知道了", "关闭")) { }
                        return;
                        //AnimationUtility.SetKeyLeftTangentMode
                    }
                    HumanoidSet(_modelImporter, _allanim);

                }
            }
        }

        /// <summary>
        /// 从binding属性名返回binding组
        /// </summary>
        /// <param name="fbxanim">输入动画</param>
        /// <param name="bindingName">输入绑定点的名字</param>
        /// <returns></returns>
        EditorCurveBinding[] GetBindingToBindingName(AnimationClip fbxanim, string[] bindingName,out bool isValid)
        {
            EditorCurveBinding[] editorbinding = new EditorCurveBinding[bindingName.Length];
            int number = 0;
            foreach (EditorCurveBinding m_CurveBinding in AnimationUtility.GetCurveBindings(fbxanim))
            {
                string name = m_CurveBinding.propertyName;

                for (int i = 0; i < bindingName.Length; i++)
                {
                    if (name.Contains(bindingName[i]))
                    {
                        editorbinding[i] = m_CurveBinding;
                        number++;
                        break;
                    }
                }

                if (number == editorbinding.Length)
                {
                    isValid = true;
                    return editorbinding;
                }
            }
            if (EditorUtility.DisplayDialog("注意", "未找齐所有CurveBindings，请检查fbx文件Rig模式或者插件是否有勾 ‘Is Humanoid’ ", "我知道了", "关闭")) { }
            //editorbinding = new EditorCurveBinding[0];
            isValid = false;
            return editorbinding;
        }

        /// <summary>
        /// 从binding路径名返回binding组
        /// </summary>
        /// <param name="fbxanim">输入动画</param>
        /// <param name="bindingName"输入绑定点的名字></param>
        /// <param name="childerCount">索引层级</param>
        /// <param name="IsPosition">是否返回位置曲线的绑定</param>
        /// <returns></returns>
        EditorCurveBinding[] GetBindingToBindingPath(AnimationClip fbxanim, string bindingName,int childerCount,TransformType PlayerTransformType)
        {
            EditorCurveBinding[] editorbinding = new EditorCurveBinding[0];

            if (PlayerTransformType == TransformType.Rotation)
            {
                editorbinding = new EditorCurveBinding[4];
            }
            else
            {
                editorbinding = new EditorCurveBinding[3];
            }

            int number = 0;
            foreach (EditorCurveBinding m_CurveBinding in AnimationUtility.GetCurveBindings(fbxanim))
            {
                string[] name = m_CurveBinding.path.Split('/');

                if (name.Length-1 == childerCount)
                {
                    for (int i = 0; i < editorbinding.Length; i++)
                    {
                        if (name[childerCount].Contains(bindingName))
                        {//得到绑定路径名后  判断是否为位移的曲线
                            if (PlayerTransformType == TransformType.Position)
                            {
                                if (m_CurveBinding.propertyName.Contains("m_LocalPosition"))
                                {
                                    editorbinding[number] = m_CurveBinding;
                                    number++;
                                    break;
                                }
                            }
                            else if(PlayerTransformType == TransformType.Rotation)
                            {
                                if (m_CurveBinding.propertyName.Contains("m_LocalRotation")) 
                                {
                                    editorbinding[number] = m_CurveBinding;
                                    number++;
                                    break;
                                }
                            }
                            else //m_LocalScale
                            {
                                editorbinding[number] = m_CurveBinding;
                                number++;
                                break;
                            }
                        }
                    }
                }

                if (number == editorbinding.Length)
                {
                    return editorbinding;
                }
            }
            if (EditorUtility.DisplayDialog("注意", "未找到第" + childerCount + "层下的" + bindingName + ",  请检查你的fbx文件   " + PlayerTransformType + ": " + number, "我知道了", "关闭")) { }
            //editorbinding = new EditorCurveBinding[0];
            return editorbinding;
        }
        EditorCurveBinding[] GetBindingToBindingPathChildes(AnimationClip fbxanim, string bindingName, int childerCount, TransformType PlayerTransformType)
        {
            EditorCurveBinding[] editorbinding = new EditorCurveBinding[AnimationUtility.GetCurveBindings(fbxanim).Length];
            EditorCurveBinding[] editorbinding_Out = new EditorCurveBinding[0];

            int number = 0;
            foreach (EditorCurveBinding m_CurveBinding in AnimationUtility.GetCurveBindings(fbxanim))
            {
                string[] name = m_CurveBinding.path.Split('/');

                if (name.Length == childerCount + 2)
                {
                    //Debug.Log(m_CurveBinding.path);
                    if (name[childerCount].Contains(bindingName))
                    {//得到绑定路径名后  判断是否为位移的曲线
                        if (PlayerTransformType == TransformType.Position)
                        {
                            if (m_CurveBinding.propertyName.Contains("m_LocalPosition"))
                            {
                                editorbinding[number] = m_CurveBinding;
                                number++;
                            }
                        }
                        else if (PlayerTransformType == TransformType.Rotation)
                        {
                            if (m_CurveBinding.propertyName.Contains("m_LocalRotation"))
                            {
                                editorbinding[number] = m_CurveBinding;
                                number++;
                            }
                        }
                        else //m_LocalScale
                        {
                            editorbinding[number] = m_CurveBinding;
                            number++;
                        }
                    }
                }
            }

            if (number > 1)
            {
                editorbinding_Out = new EditorCurveBinding[number];
                for(int i = 0;i< number; i++)
                {
                    editorbinding_Out[i] = editorbinding[i];
                }
                //if (EditorUtility.DisplayDialog("注意", "收集到了 " + number + "个绑定", "我知道了", "关闭")) { }

            }
            else
            {

                //if (EditorUtility.DisplayDialog("注意", "未找到第" + childerCount + "层下" + bindingName + "的子级,  请检查你的fbx文件   " + PlayerTransformType + ": " + number, "我知道了", "关闭")) { }
            }
            //editorbinding = new EditorCurveBinding[0];
            return editorbinding_Out;
        }
        /// <summary>
        /// 从binding路径名返回binding组
        /// </summary>
        /// <param name="fbxanim">输入动画</param>
        /// <param name="bindingName">输入绑定点的名字</param>
        /// <param name="editorbinding">返回的曲线绑定</param>
        /// <returns></returns>
        EditorCurveBinding[] GetBindingToBindingPath(AnimationClip fbxanim, string[] bindingName)
        {
            EditorCurveBinding[] editorbinding = new EditorCurveBinding[bindingName.Length];
            int number = 0;
            foreach (EditorCurveBinding m_CurveBinding in AnimationUtility.GetCurveBindings(fbxanim))
            {
                string name = m_CurveBinding.path;

                for (int i = 0; i < bindingName.Length; i++)
                {
                    if (name.Contains(bindingName[i]))
                    {
                        editorbinding[i] = m_CurveBinding;
                        number++;
                    }
                }

                if (number == editorbinding.Length)
                {
                    return editorbinding;
                }
            }
            if (EditorUtility.DisplayDialog("注意", "未找齐所有CurveBindings，请检查文件或者命名组", "我知道了", "关闭")) { }
            //editorbinding = new EditorCurveBinding[0];
            return editorbinding;
        }
        bool GetIsRota(AnimationClip fbxAnim) {
            //if (IsHumanoid)
            //    return false;
            try
            {
                AnimationCurve rotaX = AnimationUtility.GetEditorCurve(fbxAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.x"));
                AnimationCurve rotaY = AnimationUtility.GetEditorCurve(fbxAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.y"));
                AnimationCurve rotaZ = AnimationUtility.GetEditorCurve(fbxAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.z"));
                AnimationCurve rotaW = AnimationUtility.GetEditorCurve(fbxAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.w"));

                Quaternion rotaA = new Quaternion(rotaX.keys[0].value, rotaY.keys[0].value, rotaZ.keys[0].value, rotaW.keys[0].value).normalized;
                Quaternion rotaB = new Quaternion(rotaX.keys[rotaX.keys.Length - 1].value, rotaY.keys[rotaY.keys.Length - 1].value, rotaZ.keys[rotaZ.keys.Length - 1].value, rotaW.keys[rotaW.keys.Length - 1].value).normalized;
                if (Vector3.Dot(rotaA * Vector3.forward, rotaB * Vector3.forward) < 0.95f) return true;
            }
            catch (System.Exception)
            {
            }

            return false;
        }
        /// <summary>
        /// 初始化位置
        /// </summary>
        /// <param name="Bindings"></param>
        /// <param name="editorAnim"></param>
        void ReSetBindingsPos(EditorCurveBinding[] Bindings, AnimationClip editorAnim) {
            AnimationCurve[] RootPoint = new AnimationCurve[3];
            RootPoint[0] = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.x"));
            if(RootPoint[0] == null)
            {
                Debug.LogWarning("生成动画时，未找到 ‘Root/RootPoint’ 路径，请检查文件");
                //if (EditorUtility.DisplayDialog("注意", "未找到 ‘Root/RootPoint’ 路径，请检查文件", "我知道了", "关闭")) { }
                return;
            }
            RootPoint[1] = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.y"));
            RootPoint[2] = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.z"));

            AnimationCurve _newCUR = new AnimationCurve();
            _newCUR.AddKey(0, 0);
            _newCUR.AddKey(1, 1);
            
            // for (int i = 0; i < RootPoint.Length; i++)
            // {
            //     AnimationCurve _cur = RootPoint[i];
            //     for (int j = 1; j < _cur.keys.Length; j++)
            //     {
            //         if (_cur.keys[j].value != _cur.keys[0].value)
            //         {
            //             Debug.Log($"有值！！{i}\n{_cur.keys[j].value}");
            //             // break;
            //         }
            //     }
            // }
            
            //return;
            for (int i = 0; i < Bindings.Length; i++)
            {
                // Debug.Log(Bindings[i].path);
                if(Bindings[i].propertyName == "m_LocalPosition.x")
                {
                    AnimationCurve curA = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve(Bindings[i].path, typeof(Transform), Bindings[i].propertyName));
                    AnimationUtility.SetEditorCurve(editorAnim, Bindings[i], GetCurSub(curA, RootPoint[0]));
                }
                else if(Bindings[i].propertyName == "m_LocalPosition.y")
                {
                    AnimationCurve curA = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve(Bindings[i].path, typeof(Transform), Bindings[i].propertyName));
                    AnimationUtility.SetEditorCurve(editorAnim, Bindings[i], GetCurAdd(curA, RootPoint[2]));
                    // float _maxValue = int.MinValue;
                    // float _minValue = int.MaxValue;
                    // Keyframe[] _keys = RootPoint[1].keys;
                    // for (int j = 0; j < _keys.Length; j++)
                    // {
                    //     if (_keys[j].value > _maxValue)
                    //     {
                    //         _maxValue = _keys[j].value;
                    //     }else if (_keys[j].value < _minValue)
                    //     {
                    //         _minValue = _keys[j].value;
                    //     }
                    // }
                    // Debug.Log($"最大差值：{_maxValue - _minValue}");
                }
                else if(Bindings[i].propertyName == "m_LocalPosition.z")
                {
                    AnimationCurve curA = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve(Bindings[i].path, typeof(Transform), Bindings[i].propertyName));
                    AnimationUtility.SetEditorCurve(editorAnim, Bindings[i], GetCurSub(curA, RootPoint[1]));
                }
            }

            if (GetIsRota(editorAnim) && !IsHumanoid) 
            {
#if UNITY_2020_1_OR_NEWER
                string BipName = "Bip001";
                if (AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/ChatRoot/" + BipName, typeof(Transform), "m_LocalRotation.x")) == null)
                {
                    BipName = "Master";//老头环
                    if (AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/ChatRoot/" + BipName, typeof(Transform), "m_LocalRotation.x")) == null)
                    {
                        Debug.LogError("没有找到质心");
                        return;
                    }
                }
#else
                try
                {
                    var ac = EditorCurveBinding.FloatCurve("Root/ChatRoot/" + BipName, typeof(Transform), "m_LocalRotation.x");
                    if(ac == null)
                    {
                        Debug.Log("没有找到质心");
                    }
                }
                catch (System.Exception)
                {
                    Debug.Log("没有找到质心");
                    BipName = "Master";//老头环
                    try
                    {
                        EditorCurveBinding.FloatCurve("Root/ChatRoot/" + BipName, typeof(Transform), "m_LocalRotation.x");
                    }
                    catch (System.Exception)
                    {
                        if (EditorUtility.DisplayDialog("注意", "请确认BIP质心命名是否未‘Bip001’或者‘Bip01’", "我知道了", "关闭")) { }
                        return;
                        //throw;
                    }
                    //throw;
                }
#endif
                // Debug.Log("打个log啦：ReSetBindingsPos");

                AnimationCurve curXb = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/ChatRoot/" + BipName, typeof(Transform), "m_LocalRotation.x"));
                AnimationCurve curYb = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/ChatRoot/" + BipName, typeof(Transform), "m_LocalRotation.y"));
                AnimationCurve curZb = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/ChatRoot/" + BipName, typeof(Transform), "m_LocalRotation.z"));
                AnimationCurve curWb = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/ChatRoot/" + BipName, typeof(Transform), "m_LocalRotation.w"));

                AnimationCurve curX = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.x"));
                AnimationCurve curY = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.y"));
                AnimationCurve curZ = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.z"));
                AnimationCurve curW = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.w"));

                AnimationCurve cuX = new AnimationCurve();
                AnimationCurve cuY = new AnimationCurve();
                AnimationCurve cuZ = new AnimationCurve();
                AnimationCurve cuW = new AnimationCurve();
                Keyframe[] keys = curXb.keys;
                float aa = 0;
                float _rotaB_Late = 0;
                for (int i = 0; i < keys.Length; i++)
                {
                    float nowTime = keys[i].time;
                    Quaternion _rotaA = new Quaternion(curXb.Evaluate(nowTime), curYb.Evaluate(nowTime), curZb.Evaluate(nowTime), curWb.Evaluate(nowTime)).normalized;
                    Quaternion _rotaB = new Quaternion(curX.Evaluate(nowTime), curY.Evaluate(nowTime), curZ.Evaluate(nowTime), curW.Evaluate(nowTime)).normalized;
                    Debug.Log("关键帧值: " + (nowTime * 30) + "  :" + (_rotaA.eulerAngles.y - _rotaB.eulerAngles.y));
                    //Quaternion _rota = Quaternion.Euler(_rotaA.eulerAngles.x, (_rotaA.eulerAngles.y - _rotaB.eulerAngles.y), _rotaA.eulerAngles.z);
                    Quaternion _rota = Quaternion.identity;

                    float deltaRotaY = _rotaB.eulerAngles.y - _rotaB_Late;
                    _rotaB_Late = _rotaB.eulerAngles.y;
                    if (i > 0)
                    {
                        if(deltaRotaY > 180)
                            deltaRotaY -= 360;
                        else if(deltaRotaY < -180)
                            deltaRotaY += 360;
                        
                        aa += deltaRotaY;
                    }

                    //if (aa > 180) aa -= 360;
                    _rota = Quaternion.AngleAxis(-aa, Vector3.forward) * _rotaA;
                    //if (_rotaB.eulerAngles.y > 180)
                    //    _rota = Quaternion.Euler(0, -_rotaB.eulerAngles.y + 360, 0) * _rotaA;
                    //else
                    //    _rota = Quaternion.Euler(0, -_rotaB.eulerAngles.y, 0) * _rotaA;

                    //Debug.Log("关键帧值: " + (nowTime * 30) + "  :" + (aa));
                    //_rota = _rotaA;
                    cuX.AddKey(nowTime, _rota.x);
                    cuY.AddKey(nowTime, _rota.y);
                    cuZ.AddKey(nowTime, _rota.z);
                    cuW.AddKey(nowTime, _rota.w);
                }

                AnimationUtility.SetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/ChatRoot/" + BipName, typeof(Transform), "m_LocalRotation.x"), cuX);
                AnimationUtility.SetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/ChatRoot/" + BipName, typeof(Transform), "m_LocalRotation.y"), cuY);
                AnimationUtility.SetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/ChatRoot/" + BipName, typeof(Transform), "m_LocalRotation.z"), cuZ);
                AnimationUtility.SetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/ChatRoot/" + BipName, typeof(Transform), "m_LocalRotation.w"), cuW);

            }
        }
        void ReSetBindingsPos2(EditorCurveBinding[] Bindings, AnimationClip editorAnim)
        {
            // Debug.Log("打个log啦：ReSetBindingsPos");

            AnimationCurve[] RootPoint = new AnimationCurve[3];
            RootPoint[0] = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.x"));
            if (RootPoint[0] == null)
            {
                //if (EditorUtility.DisplayDialog("注意", "未找到 ‘Root/RootPoint’ 路径，请检查文件", "我知道了", "关闭")) { }
                Debug.LogWarning("生成动画时，未找到 ‘Root/RootPoint’ 路径，请检查文件");
                return;
            }
            RootPoint[1] = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.y"));
            RootPoint[2] = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.z"));

            //Debug.Log(Bindings.Length);
            for (int i = 0; i < Bindings.Length; i++)
            {
                // Debug.Log($"打个log啦：{Bindings[i].path}");

                if (Bindings[i].path != "Root/ChatRoot" && Bindings[i].path != "Root/RootPoint")
                {
                    Debug.Log(Bindings[i].path);
                    if (Bindings[i].propertyName == "m_LocalPosition.x")
                    {
                        AnimationCurve curA = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve(Bindings[i].path, typeof(Transform), Bindings[i].propertyName));
                        AnimationUtility.SetEditorCurve(editorAnim, Bindings[i], GetCurSub(curA, RootPoint[0]));
                    }
                    else if (Bindings[i].propertyName == "m_LocalPosition.y")
                    {
                        AnimationCurve curA = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve(Bindings[i].path, typeof(Transform), Bindings[i].propertyName));
                        AnimationUtility.SetEditorCurve(editorAnim, Bindings[i], GetCurSub(curA, RootPoint[1]));
                    }
                    else if (Bindings[i].propertyName == "m_LocalPosition.z")
                    {
                        AnimationCurve curA = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve(Bindings[i].path, typeof(Transform), Bindings[i].propertyName));
                        AnimationUtility.SetEditorCurve(editorAnim, Bindings[i], GetCurSub(curA, RootPoint[2]));
                    }
                }
            }
        }

        /// <summary>
        /// 初始化Humanoid位置
        /// </summary>
        /// <param name="Bindings"></param>
        /// <param name="editorAnim"></param>
        void ReSetBindingsPos_Human(AnimationClip editorAnim)
        {
            AnimationCurve[] RootPoint = new AnimationCurve[3];
            RootPoint[0] = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.x"));
            if (RootPoint[0] == null)
            {
                if (EditorUtility.DisplayDialog("注意", "未找到 ‘Root/RootPoint’ 路径，请检查文件", "我知道了", "关闭")) { }
                return;
            }
            RootPoint[1] = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.y"));
            RootPoint[2] = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalPosition.z"));

            string[] namea = new string[7] {
                    "RootT.x",
                    "RootT.y",
                    "RootT.z",
                    "RootQ.x",
                    "RootQ.y",
                    "RootQ.z",
                    "RootQ.w"
            };
            bool isValid = false;
            EditorCurveBinding[] editorBinding = GetBindingToBindingName(editorAnim, namea, out isValid);
            if (!isValid) return;

            AnimationCurve[] RootPoint2 = new AnimationCurve[3];
            RootPoint2[0] = AnimationUtility.GetEditorCurve(editorAnim, editorBinding[0]);
            RootPoint2[1] = AnimationUtility.GetEditorCurve(editorAnim, editorBinding[1]);
            RootPoint2[2] = AnimationUtility.GetEditorCurve(editorAnim, editorBinding[2]);

            if (!ishuona)
            {
                if (PosZ)
                    AnimationUtility.SetEditorCurve(editorAnim, editorBinding[2], GetCurAdd(RootPoint2[2], RootPoint[1], true));//Z = Z+Y
                if (PosX)
                    AnimationUtility.SetEditorCurve(editorAnim, editorBinding[0], GetCurSub(RootPoint2[0], RootPoint[0], true));//X = X-X
                if (PosY)
                    AnimationUtility.SetEditorCurve(editorAnim, editorBinding[1], GetCurSub(RootPoint2[1], RootPoint[2], true));//Y = Y-Z
            }
            else
            {
                if (PosX)
                    AnimationUtility.SetEditorCurve(editorAnim, editorBinding[0], GetCurSub(RootPoint2[0], RootPoint[0], true));//X = X-X
                if (PosY)
                    AnimationUtility.SetEditorCurve(editorAnim, editorBinding[1], GetCurSub(RootPoint2[1], RootPoint[1], true));//Y = Y-Y
                if (PosZ)
                    AnimationUtility.SetEditorCurve(editorAnim, editorBinding[2], GetCurSub(RootPoint2[2], RootPoint[2], true));//Z = Z-Z
            }


            if (IsRota)
            {
                AnimationCurve curXb = AnimationUtility.GetEditorCurve(editorAnim, editorBinding[3]);
                AnimationCurve curYb = AnimationUtility.GetEditorCurve(editorAnim, editorBinding[4]);
                AnimationCurve curZb = AnimationUtility.GetEditorCurve(editorAnim, editorBinding[5]);
                AnimationCurve curWb = AnimationUtility.GetEditorCurve(editorAnim, editorBinding[6]);

                AnimationCurve curX = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.x"));
                AnimationCurve curY = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.y"));
                AnimationCurve curZ = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.z"));
                AnimationCurve curW = AnimationUtility.GetEditorCurve(editorAnim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.w"));

                AnimationCurve cuX = new AnimationCurve();
                AnimationCurve cuY = new AnimationCurve();
                AnimationCurve cuZ = new AnimationCurve();
                AnimationCurve cuW = new AnimationCurve();
                Keyframe[] keys = curXb.keys;
                for (int i = 0; i < keys.Length; i++)
                {
                    float nowTime = keys[i].time;
                    Quaternion _rotaA = new Quaternion(curXb.Evaluate(nowTime), curYb.Evaluate(nowTime), curZb.Evaluate(nowTime), curWb.Evaluate(nowTime)).normalized;
                    Quaternion _rotaB = new Quaternion(curX.Evaluate(nowTime), curY.Evaluate(nowTime), curZ.Evaluate(nowTime), curW.Evaluate(nowTime)).normalized;
                    //Debug.Log("关键帧值: " + (nowTime * 30) + "  :" + (_rotaA.eulerAngles.y - _rotaB.eulerAngles.y));
                    //Quaternion _rota = Quaternion.Euler(_rotaA.eulerAngles.x, (_rotaA.eulerAngles.y - _rotaB.eulerAngles.y), _rotaA.eulerAngles.z);
                    Quaternion _rota = Quaternion.identity;
                    if (_rotaB.eulerAngles.y > 180)
                        _rota = Quaternion.Euler(0, -_rotaB.eulerAngles.y + 360, 0) * _rotaA;
                    else
                        _rota = Quaternion.Euler(0, -_rotaB.eulerAngles.y, 0) * _rotaA;

                    Debug.Log("关键帧值: " + (nowTime * 30) + "  :" + (-_rotaB.eulerAngles.y));
                    cuX.AddKey(nowTime, _rota.x);
                    cuY.AddKey(nowTime, _rota.y);
                    cuZ.AddKey(nowTime, _rota.z);
                    cuW.AddKey(nowTime, _rota.w);
                }

                AnimationUtility.SetEditorCurve(editorAnim, editorBinding[3], cuX);
                AnimationUtility.SetEditorCurve(editorAnim, editorBinding[4], cuY);
                AnimationUtility.SetEditorCurve(editorAnim, editorBinding[5], cuZ);
                AnimationUtility.SetEditorCurve(editorAnim, editorBinding[6], cuW);
            }
        }
        AnimationCurve GetCurSub(AnimationCurve curA,AnimationCurve curB,bool _isHumanoid = false) {
            AnimationCurve cur = new AnimationCurve();
            if (curA != null && curB != null)
            {
                curA = SetCurveTangentMode(curA, AnimationUtility.TangentMode.Linear);
                curB = SetCurveTangentMode(curB, AnimationUtility.TangentMode.Linear);
                Keyframe[] keys = curA.keys;
                for (int i = 0; i < keys.Length; i++)
                {
                    float KeyTime = keys[i].time;
                    if (_isHumanoid)
                        cur.AddKey(KeyTime, curA.Evaluate(KeyTime) - (curB.Evaluate(KeyTime)) * Proportion);//
                    else
                        cur.AddKey(KeyTime, keys[i].value - curB.Evaluate(KeyTime));
                }
                SetCurveTangentMode(cur, AnimationUtility.TangentMode.Linear);
            }
            else
            {
                if (EditorUtility.DisplayDialog("注意", "曲线相减中缺少了减数或者被减数" + "\n curA: " + (curA != null) + "\n curA: " + (curB != null), "我知道了", "关闭")) { }
            }

            return cur;
        }
        AnimationCurve GetCurAdd(AnimationCurve curA, AnimationCurve curB,bool _IsHumanoid = false)
        {
            AnimationCurve cur = new AnimationCurve();
            if (curA != null && curB != null)
            {
                Keyframe[] keys = curA.keys;
                for (int i = 0; i < keys.Length; i++)
                {
                    float KeyTime = keys[i].time;
                    if (_IsHumanoid)
                        cur.AddKey(KeyTime, keys[i].value + (curB.Evaluate(KeyTime) * Proportion));
                    else
                        cur.AddKey(KeyTime, keys[i].value + curB.Evaluate(KeyTime));

                }
            }
            else
            {
                if (EditorUtility.DisplayDialog("注意", "曲线相加中缺少了加数" + "\n curA: " + (curA != null) + "\n curA: " + (curB != null), "我知道了", "关闭")) { }
            }

            return cur;
        }
        ClipAnimationInfoCurve[] GetCurAdd(ClipAnimationInfoCurve[] curA, ClipAnimationInfoCurve[] curB)
        {
            ClipAnimationInfoCurve[] cur = new ClipAnimationInfoCurve[curA.Length + curB.Length];
            for(int i = 0; i < cur.Length; i++)
            {
                if(i< curA.Length)
                {
                    cur[i] = curA[i];
                }
                else
                {
                    cur[i] = curB[i - curA.Length];
                }
            }
            return cur;
        }

        /// <summary>
        /// 反转曲线参数
        /// </summary>
        /// <param name="cur"></param>
        /// <returns></returns>
        AnimationCurve[] ReverseCur(AnimationCurve[] cur) {
            AnimationCurve[] m_cur = new AnimationCurve[cur.Length];

            for(int i=0;i< cur.Length; i++)
            {
                var keys = cur[i].keys;
                m_cur[i].keys = cur[i].keys;
                for (int k = 0;k< keys.Length; k++)
                {
                    m_cur[i].keys[k].value = keys[k].value * -1;
                }
                //m_cur[i]
            }

            return m_cur;
        }
        AnimationCurve ReverseCur(AnimationCurve cur)
        {
            AnimationCurve m_cur = new AnimationCurve();
            if (cur != null)
            {
                Keyframe[] keys = cur.keys;

                //m_cur.keys = new Keyframe[keys.Length];
                //m_cur.keys = keys;
                //Debug.Log("关键帧数量：" + m_cur.keys.Length);
                for (int k = 0; k < keys.Length; k++)
                {
                    //m_cur.keys[k].value = keys[k].value * -1;
                    //m_cur.keys[k].value = 10f;
                    m_cur.AddKey(keys[k].time, keys[k].value * -1);
                }
            }
            else
            {
                Debug.Log("该曲线值为空: ");

            }
            //m_cur[i]

            return m_cur;
        }
        AnimationCurve GetFootEvents() {
            AnimationCurve a = new AnimationCurve();
            a.AddKey(0, -1);
            a.AddKey(1, -1);
            return a;
        }
        AnimationCurve GetCurvePos(AnimationCurve fbxCur,VectorAxis m_axis) {
            AnimationCurve animCur = new AnimationCurve();
            if(fbxCur != null)
            {
                //Debug.Log("初始位置：" + offsetPos);

                float startPos = 0;
                if (m_axis == VectorAxis.x)
                    startPos = offsetPos.x;
                else if (m_axis == VectorAxis.y)
                    startPos = offsetPos.y;
                else
                    startPos = ishuona ? offsetPos.z : -offsetPos.z;

                float startValue = fbxCur.Evaluate(FrameTime * startFrame) + startPos;//起始数值

                Keyframe[] keys = fbxCur.keys;
                float proportionValue = keys[keys.Length - 1].time;//得到最后一帧的时间

                for (int i = 0; i < keys.Length; i++)
                {
                    float m_frametime = keys[i].time;
                    animCur.AddKey(m_frametime / proportionValue, keys[i].value - startValue);//得到偏移后的曲线值
                }
            }
            else
            {
                Debug.Log("输入的曲线值为空");
            }

            return animCur;
        }
        AnimationCurve GetCurvePos(AnimationCurve fbxCur)
        {
            AnimationCurve animCur = new AnimationCurve();
            if (fbxCur != null)
            {
                //Debug.Log("初始位置：" + offsetPos);
                Keyframe[] keys = fbxCur.keys;
                float proportionValue = keys[keys.Length - 1].time;//得到最后一帧的时间
                for (int i = 0; i < keys.Length; i++)
                {
                    float m_frametime = keys[i].time;
                    animCur.AddKey(m_frametime / proportionValue, keys[i].value);//得到偏移后的曲线值
                }
            }
            else
            {
                Debug.Log("输入的曲线值为空");
            }

            return animCur;
        }
        AnimationCurve GetCurvePos(AnimationCurve fbxCur,out bool isValue)
        {
            isValue = false;
            AnimationCurve animCur = new AnimationCurve();
            if (fbxCur != null)
            {
                //Debug.Log("初始位置：" + offsetPos);
                Keyframe[] keys = fbxCur.keys;
                float proportionValue = keys[keys.Length - 1].time;//得到最后一帧的时间
                for (int i = 0; i < keys.Length; i++)
                {
                    float m_frametime = keys[i].time;
                    animCur.AddKey(m_frametime / proportionValue, keys[i].value);//得到偏移后的曲线值
                }
                isValue = true;
            }
            else
            {
                //Debug.Log("输入的曲线值为空");
            }
            return animCur;
        }
        AnimationCurve GetCurveVelocity(AnimationCurve fbxCur)
        {
            AnimationCurve animCur = new AnimationCurve();
            AnimationCurve animCur2 = new AnimationCurve();
            if (fbxCur != null)
            {
                Keyframe[] keys = fbxCur.keys;
                animaEndTime = keys[keys.Length - 1].time;
                animCur.AddKey(0, 0);
                for (int i = 1; i < keys.Length; i++)
                {
                    float time = 1/(keys[i].time - keys[i - 1].time);
                    float velocityPos = (keys[i].value - keys[i - 1].value) * time;
                    animCur.AddKey(keys[i].time / animaEndTime, velocityPos);//每秒移动速度
                }

                animCur2.AddKey(0, animCur.keys[1].value);
                for (int i = 0; i < animCur.keys.Length; i++)
                {
                    animCur2.AddKey(animCur.keys[i].time, animCur.keys[i].value);//每秒移动速度
                }

            }
            else
            {
                Debug.Log("输入的曲线值为空");
            }

            return animCur2;
        }
        AnimationCurve SetCurveTangentMode(AnimationCurve fbxCur, AnimationUtility.TangentMode tangentMode, bool turnCur = false) {
            if (fbxCur == null) return fbxCur;

            AnimationCurve newCur = new AnimationCurve();
            int keys = fbxCur.keys.Length;

            if (turnCur)
            {
                for (int i = 0; i < keys; i++)
                {
                    Keyframe test = fbxCur.keys[i];
                    newCur.AddKey(test.time, -test.value);
                }
            }
            else
            {
                newCur = fbxCur;
            }

            for (int i = 0; i < keys; i++)
            {
                AnimationUtility.SetKeyRightTangentMode(newCur, i, tangentMode);
                AnimationUtility.SetKeyLeftTangentMode(newCur, i, tangentMode);
            }

            return newCur;
        }

        /// <summary>
        /// 对比曲线值
        /// </summary>
        /// <param name="CurA"></param>
        /// <param name="CurB"></param>
        /// <returns></returns>
        ClipAnimationInfoCurve[] GetCurveContrast(ClipAnimationInfoCurve[] CurA, ClipAnimationInfoCurve[] CurB)
        {
            ClipAnimationInfoCurve[] cur_a = new ClipAnimationInfoCurve[CurA.Length];
            int ID = 0;
            bool Cur_RootWeight = false;//是否包含“Cur_RootWeight”曲线
            bool Cur_RotaWeight = false;//是否包含“Cur_RotaWeight”曲线
            bool Cur_GroundWeight = true;//是否包含“Cur_GroundWeight”曲线
            bool Cur_Hitrecover = true;//是否包含“Cur_Hitrecover”曲线
            bool Cur_FaceWeight = false;//是否包含“Cur_FaceWeight”曲线
            for (int i = 0; i < CurA.Length; i++)
            {
                switch (CurA[i].name)
                {
                    case "FootRight":
                        break;
                    case "Cur_AnimLoop":
                        break;
                    case "Cur_VelocityX":
                        break;
                    case "Cur_VelocityY":
                        break;
                    case "Cur_VelocityZ":
                        break;
                    case "Cur_PosX":
                        break;
                    case "Cur_PosY":
                        break;
                    case "Cur_PosZ":
                        break;
                    case "Cur_ChatHeight":
                        break;
                    case "Cur_ChatRadius":
                        break;
                    case "Cur_RotY":
                        break;
                    case "Cur_AnimMix":
                        break;
                    case "Cur_LHand":
                        break;
                    case "Cur_RHand":
                        break;
                    case "Cur_RootWeight": //收集非烘焙曲线
                        Cur_RootWeight = true;
                        cur_a[ID] = CurA[i];
                        ID++;
                        break;
                    case "Cur_RotaWeight": //收集非烘焙曲线
                        Cur_RotaWeight = true;
                        cur_a[ID] = CurA[i];
                        ID++;
                        break;
                    //case "Cur_GroundWeight": //收集非烘焙曲线
                    //    Cur_GroundWeight = true;
                    //    cur_a[ID] = CurA[i];
                    //    ID++;
                    //    break;
                    //case "Cur_Hitrecover": //收集非烘焙曲线
                    //    Cur_Hitrecover = true;
                    //    cur_a[ID] = CurA[i];
                    //    ID++;
                    //    break;
                    case "Cur_FaceWeight": //收集非烘焙曲线
                        Cur_FaceWeight = true;
                        cur_a[ID] = CurA[i];
                        ID++;
                        break;

                    default: //收集非烘焙曲线
                        cur_a[ID] = CurA[i];
                        ID++;
                        break;
                }
            }
            int IdAdd = 0;
            if (!Cur_RootWeight) IdAdd++;//需要添加曲线
            if (!Cur_Hitrecover) IdAdd++;//需要添加曲线
            if (!Cur_GroundWeight) IdAdd++;//需要添加曲线
            if (!Cur_FaceWeight) IdAdd++;//需要添加曲线
            if (!Cur_RotaWeight && (startFrame != 0 || offsetPos != Vector3.zero)) IdAdd++;//需要添加曲线
            ClipAnimationInfoCurve[] cur_a1 = new ClipAnimationInfoCurve[ID + IdAdd];
            for (int i = 0; i < ID; i++)
                cur_a1[i] = cur_a[i];

            int number = 0;
            if (!Cur_Hitrecover) {
                number++;
                AnimationCurve b = new AnimationCurve();
                if (startFrame != 0 || offsetPos != Vector3.zero)
                {
                    b.AddKey(0, 1);
                    b.AddKey(1, 1);
                }
                else {
                    b.AddKey(0, 0);
                    b.AddKey(1, 0);
                }
                b = SetCurveTangentMode(b, AnimationUtility.TangentMode.Constant);
                //Debug.LogError("cur_a1: " + cur_a1.Length);
                cur_a1[cur_a1.Length - number].name = "Cur_Hitrecover";
                cur_a1[cur_a1.Length - number].curve = b;
            }

            if (!Cur_GroundWeight) {
                number++;
                AnimationCurve b = new AnimationCurve();
                if (startFrame != 0 || offsetPos != Vector3.zero)
                {
                    b.AddKey(0, 0);
                    b.AddKey(0.3f, 0);
                    b.AddKey(1, 1);
                }
                else {
                    b.AddKey(0, 1);
                    b.AddKey(1, 1);
                }
                b = SetCurveTangentMode(b, AnimationUtility.TangentMode.Linear);
                cur_a1[cur_a1.Length - number].name = "Cur_GroundWeight";
                cur_a1[cur_a1.Length - number].curve = b;
            }

            if (!Cur_FaceWeight) {
                number++;
                AnimationCurve b = new AnimationCurve();
                b.AddKey(0, 0);
                b.AddKey(1, 0);
                cur_a1[cur_a1.Length - number].name = "Cur_FaceWeight";
                cur_a1[cur_a1.Length - number].curve = b;
            }

            if (!Cur_RotaWeight && (startFrame != 0 || offsetPos != Vector3.zero))
            {
                number++;
                AnimationCurve b = new AnimationCurve();
                b.AddKey(0, 0);
                b.AddKey(1, 0);
                cur_a1[cur_a1.Length - number].name = "Cur_RotaWeight";
                cur_a1[cur_a1.Length - number].curve = b;
            }

            if (!Cur_RootWeight)
            {
                number++;
                AnimationCurve a = new AnimationCurve();
                if (startFrame != 0 || offsetPos != Vector3.zero)
                {
                    a.AddKey(0, 1.01f);
                    a.AddKey(0.6f, 2);
                    a.AddKey(1, 2);
                    a = SetCurveTangentMode(a, AnimationUtility.TangentMode.Linear);
                }
                else
                {
                    a.AddKey(0, 1);
                    a.AddKey(1, 1);
                }

                cur_a1[cur_a1.Length - number].name = "Cur_RootWeight";
                cur_a1[cur_a1.Length - number].curve = a;
            }

            bool nullValue = false;
            for (int i = 0; i < cur_a1.Length; i++)
            {
                if (cur_a1[i].curve == null)
                {
                    if (EditorUtility.DisplayDialog("警告", i + "曲线: " + cur_a1[i].name + " 为空\n", "确定", "关闭")) { }
                    nullValue = true;
                }
            }
            if (nullValue) return CurB;

            return GetCurAdd(CurB, cur_a1);
        }
        ClipAnimationInfoCurve[] GetCurveCopy(ClipAnimationInfoCurve[] CurA)
        {
            List<ClipAnimationInfoCurve> cur_a2 = new List<ClipAnimationInfoCurve>();
            int ID = 0;
            for (int i = 0; i < CurA.Length; i++)
            {
                switch (CurA[i].name)
                {
                    case "FootRight":
                        break;
                    case "Cur_AnimLoop":
                        break;
                    case "Cur_VelocityX":
                        break;
                    case "Cur_VelocityY":
                        break;
                    case "Cur_VelocityZ":
                        break;
                    case "Cur_PosX":
                        break;
                    case "Cur_PosY":
                        break;
                    case "Cur_PosZ":
                        break;
                    case "Cur_ChatHeight":
                        break;
                    case "Cur_ChatRadius":
                        break;
                    case "Cur_RotY":
                        break;
                    case "Cur_AnimMix":
                        break;
                    case "Cur_LHand":
                        break;
                    case "Cur_RHand":
                        break;
                    default: //收集非烘焙曲线
                        cur_a2.Add(CurA[i]);
                        ID++;
                        break;
                }
            }
            my_Copy2.Clear();
            foreach (ClipAnimationInfoCurve a in cur_a2) {
                my_Copy2.Add(a.name);
            }
            return cur_a2.ToArray();
        }
        ClipAnimationInfoCurve[] GetCurvePaste(ClipAnimationInfoCurve[] CurA, ClipAnimationInfoCurve[] CurB)
        {
            List<ClipAnimationInfoCurve> cur_a = new List<ClipAnimationInfoCurve>();
            int ID = 0;
            for (int i = 0; i < CurA.Length; i++)
            {
                switch (CurA[i].name)
                {
                    case "FootRight":
                    case "Cur_AnimLoop":
                    case "Cur_VelocityX":
                    case "Cur_VelocityY":
                    case "Cur_VelocityZ":
                    case "Cur_PosX":
                    case "Cur_PosY":
                    case "Cur_PosZ":
                    case "Cur_ChatHeight":
                    case "Cur_ChatRadius":
                    case "Cur_RotY":
                    case "Cur_AnimMix":
                    case "Cur_LHand":
                    case "Cur_RHand":
                    //收集烘焙曲线
                        cur_a.Add(CurA[i]);
                        ID++;
                        break;
                    default:
                        bool repeat = false;
                        foreach (ClipAnimationInfoCurve a in CurB) {
                            if (CurA[i].name == a.name) { repeat = true;break; }
                        }
                        if (!repeat) cur_a.Add(CurA[i]);
                        break;
                }
            }

            foreach (ClipAnimationInfoCurve a in CurB) 
                cur_a.Add(a);

            return cur_a.ToArray();
        }
        ClipAnimationInfoCurve[] GetCurveRot(AnimationClip anim, ClipAnimationInfoCurve[] ClipCur)
        {
            ClipAnimationInfoCurve[] animCur = ClipCur;
            AnimationCurve _CurY = new AnimationCurve();
            //得到旋转的四元数
            AnimationCurve curX = AnimationUtility.GetEditorCurve(anim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.x"));
            AnimationCurve curY = AnimationUtility.GetEditorCurve(anim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.y"));
            AnimationCurve curZ = AnimationUtility.GetEditorCurve(anim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.z"));
            AnimationCurve curW = AnimationUtility.GetEditorCurve(anim, EditorCurveBinding.FloatCurve("Root/RootPoint", typeof(Transform), "m_LocalRotation.w"));

            if (curX.keys.Length > 3)
            {
                float endTime = curX.keys[curX.keys.Length - 1].time;

                int StartValue = 0;
                int EndValue = 0;

                float startTime = 0;
                float EndTime = 0;
                bool findEndTime = false;
                for (int i = 0; i < curX.keys.Length; i++)
                {
                    Quaternion NowRota = new Quaternion(curX.keys[i].value, curY.keys[i].value, curZ.keys[i].value, curW.keys[i].value).normalized;
                    int m_Angle = (int)(NowRota.eulerAngles.y * 100);
                    if (i == 0)
                    {
                        StartValue = m_Angle;
                        int eneId = curX.keys.Length - 1;
                        EndValue = (int)(((new Quaternion(curX.keys[eneId].value, curY.keys[eneId].value, curZ.keys[eneId].value, curW.keys[eneId].value).normalized).eulerAngles.y) * 100);
                    }
                    if (m_Angle == StartValue) {
                        startTime = curX.keys[i].time / endTime;
                    }

                    if (m_Angle == EndValue && !findEndTime)
                    {
                        EndTime = curX.keys[i].time / endTime;
                        findEndTime = true;
                    }
                }
                _CurY.AddKey(0, 1);
                _CurY.AddKey(startTime, 1);
                _CurY.AddKey(EndTime, 2);
                _CurY.AddKey(1, 2);


                _CurY = SetCurveTangentMode(_CurY, AnimationUtility.TangentMode.Linear);
            }
            else
            {
                if (EditorUtility.DisplayDialog("警告", "请不要在首尾帧去旋转此角色，在max文件里面确认后修改并重新上传fbx到unity", "确定", "关闭")) { }
                _CurY.AddKey(0, 0);
                _CurY.AddKey(1, 1);
            }

            int id = 0;
            if(startFrame != 0 || offsetPos != Vector3.zero) id = 4;
            animCur[id].name = "Cur_RotY";
            animCur[id].curve = _CurY;

            return animCur;
        }

        void HumanoidSet(ModelImporter import, ModelImporterClipAnimation[] ImporterClip,bool Bake = true)
        {
            ModelImporterClipAnimation[] newClip = ImporterClip;
            for (int i = 0; i < newClip.Length; i++)
            {
                newClip[i].loopTime = animaIsLoop;
                if (IsHumanoid)
                {
                    //Debug.Log("设置Mask");
                    newClip[i].maskType = ClipAnimationMaskType.CopyFromOther;
                    newClip[i].maskSource = avatarMask;
                }
                else
                {
                    newClip[i].maskType = ClipAnimationMaskType.None;
                }
                newClip[i].keepOriginalOrientation = Bake;//设置原点
                newClip[i].keepOriginalPositionXZ = Bake;
                newClip[i].keepOriginalPositionY = Bake;
                newClip[i].lockRootHeightY = Bake;//不烘焙
                newClip[i].lockRootPositionXZ = Bake;
                newClip[i].lockRootRotation = Bake;
            }

            import.motionNodeName = "Root/RootPoint";
            import.clipAnimations = newClip;
            import.SaveAndReimport();
        }
        float GetProportion() {
            float proportion = 0;
            string[] path_Arr = assetpath.Split('/');
            string path_Creacte = null;
            for (int i = 0; i < path_Arr.Length - 1; i++)
                path_Creacte += path_Arr[i] + "/";
            AnimationClip referAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(path_Creacte + "RootRefer.fbx");
            if (referAnim == null)
            {
                if (EditorUtility.DisplayDialog("警告", "未能在当前文件夹下找到 ‘RootRefer.fbx’ ，或者  ‘RootRefer.fbx’ 没有动画，请在Max按规则导出一下", "我知道了", "关闭")) { }
                return 0;
            }
            ModelImporter referImport = (ModelImporter)ModelImporter.GetAtPath(path_Creacte + "RootRefer.fbx");
            if (referImport.clipAnimations.Length > 0)
            {
                if (referImport.clipAnimations[0].maskType == ClipAnimationMaskType.None)
                    HumanoidSet(referImport, referImport.clipAnimations);
            }
            else
            {
                if (EditorUtility.DisplayDialog("警告", "请手动初始化 ‘RootRefer.fbx’ ", "我知道了", "关闭")) { }
                return 0;
            }

            string[] namea = new string[3] {
            "RootT.x",
            "RootT.y",
            "RootT.z"
            };
            bool isValid = false;
            EditorCurveBinding[] editorBinding = GetBindingToBindingName(referAnim, namea, out isValid);
            if (!isValid) return 0;

            AnimationCurve a = AnimationUtility.GetEditorCurve(referAnim, editorBinding[2]);//Z轴  前后
            proportion = Mathf.Abs(a.keys[0].value - a.keys[a.keys.Length - 1].value);
            proportion /= 10;

            return proportion;
        }
#endregion
    }
}

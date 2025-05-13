using System.Collections.Generic;
using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    // public struct MotionInfo
    // {
    //     public Motion mMotion;
    //     public float mMotionSpeed;
    //
    //     public MotionInfo(Motion _mMotion,float _mMotionSpeed)
    //     {
    //         mMotion = _mMotion;
    //         mMotionSpeed = _mMotionSpeed;
    //     }
    // }
    public partial class ResourcesWindow
    {
        #region struct
        private struct AnimaLayerAllMotion
        {
            public List<string> mAnimFullName;
            public Dictionary<string, AnimatorState> mAnimPathDic;

            public AnimaLayerAllMotion(List<string> _mAnimFullName,Dictionary<string, AnimatorState> _mAnimPathDic)
            {
                mAnimFullName = _mAnimFullName;
                mAnimPathDic  =  _mAnimPathDic;
            }
        }


        #endregion
        
        private GameObject mRole;
        private CharacterConfig mCharacterConfig;
        private Unit mUnit;
        private Animator mRoleAnim;
        private bool mCharacterConfigIsValid = false;

        // public GameObject Role => mRole;//实例到场景的预览角色
        public Animator RoleAnim => mRoleAnim;//预览角色的状态机
        // public List<string> AnimFullName => mAnimFullName;//Animator下的所有动画
        
        //创建预览单位
        public void PreviewModel(EditorUnitWarp _unitWarp) => OnPreviewModel(_unitWarp);
        
        //从动画名称获取到序号
        public int GetSelectIDToAnimName(string _name, int layer) => OnGetSelectIDToAnimName(_name, layer);
        public List<string> GetAnimFullName(int _layer) => OnGetAnimFullName(_layer);
        //从动画名称获取Motion
        public AnimatorState GetMotionToAnimName(string _name, int layer) => OnGetMotionToAnimName(_name, layer);

        #region Function
        private void OnPreviewModel(EditorUnitWarp _unitWarp)
        {
            if (!string.IsNullOrEmpty(_unitWarp.ModelPath))
            {
                Vector3 _InstancePos = Vector3.zero;
                Quaternion _InstanceRot = Quaternion.identity;
                Transform _InstanceParent = null;
                //todo:角色加载
                GameObject _model = AssetDatabase.LoadAssetAtPath<GameObject>(_unitWarp.ModelPath);
                if (_model != null)
                {
                    //初始化  清空旧资源
                    mRoleAnim = null;
                    List<ActionPreviewMark> _model_olds = MiscFunctions.FindObjects<ActionPreviewMark>();
                    float _camRotSpeed = 1f;
                    Vector3 _camOffsetPos = Vector3.zero;

                    //仅删除所有玩家对象
                    //if (_unitWarp.IsPlayer)
                    {
                        foreach (var _model_old in _model_olds)
                        {
                            //if (_model_old.IsPlayer)
                            {
                                _InstancePos = _model_old.transform.position;
                                _InstanceRot = _model_old.transform.rotation;
                                _InstanceParent = _model_old.transform.parent;
                                _camRotSpeed = _model_old.CamRotSpeed;
                                _camOffsetPos = _model_old.CamOffsetPos;
                            }
                            DestroyImmediate(_model_old.gameObject);
                        }
                    }

                    
                    //创建新的角色并给予标记
                    SetRole(Object.Instantiate(_model, _InstancePos, _InstanceRot, _InstanceParent)); 
                    ActionPreviewMark actionPreviewMark = mRole.AddComponent<ActionPreviewMark>();
                    actionPreviewMark.IsPlayer = _unitWarp.IsPlayer;
                    actionPreviewMark.CamRotSpeed = _camRotSpeed;
                    actionPreviewMark.CamOffsetPos = _camOffsetPos;
                    actionPreviewMark.ActionName = _unitWarp.Action;
                    actionPreviewMark.DefaltCamera = _unitWarp.CameID;
                    Unit _unit= mRole.GetComponent<Unit>();
                    _unit.UnitID = _unitWarp.ID;
                    // _unit.ActionGroupName = _unitWarp.Action;
                    EditorGUIUtility.PingObject(mRole);
                    if (mRole.TryGetComponent(out Animator _animator))
                    {
                        mRoleAnim = _animator;
                    }
                    mUnit = _model.GetComponent<Unit>();

                    ActionWindowMain.ActionEditorFuntion.ActionUnitPreview(actionPreviewMark, mRole);

                    InitAnimatorData(mRoleAnim);
                }
            }
            if (!string.IsNullOrEmpty(_unitWarp.SettingPath))
            {
                ActionWindowMain.EngineSetting = 
                    AssetDatabase.LoadAssetAtPath<ActionEngineSetting>(_unitWarp.SettingPath);
            }
            else
            {
                ActionWindowMain.EngineSetting = null;
            }
            TimeLineWindow.needInit = true;
            TimeLineWindow.Instance.Repaint();
        }
        
        // private List<string> mAnimFullName = new List<string>();
        // private Dictionary<string, Motion> mAnimPathDic = new Dictionary<string, Motion>();


        private AnimaLayerAllMotion[] m_AnimaAllMotions = new AnimaLayerAllMotion[4];
        public void InitAnimatorData(Animator _animator)
        {
            // m_AnimaAllMotions = new AnimaLayerAllMotion[4];
            AnimatorController _animatorController = _animator.runtimeAnimatorController as AnimatorController;
            if (_animatorController == null)
            {
                EditorUtility.DisplayDialog("警告", "初始化错误,未配置RuntimeAnimator", "我知道了");
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                if (i >= _animatorController.layers.Length)
                {
                    _animatorController.AddLayer(SetAnimaLayer(i, _animatorController));
                }

                m_AnimaAllMotions[i] = InitAnimatorLayerData(_animatorController.layers[i]);
            }
        }

        private AnimatorControllerLayer SetAnimaLayer(int _serial, AnimatorController _animatorController)
        {
            string _savePath = AssetDatabase.GetAssetPath(_animatorController);
            AnimatorStateMachine _stateMachine = new AnimatorStateMachine()
            {
                name = _animatorController.MakeUniqueLayerName("New Layer"),
                hideFlags = HideFlags.HideInHierarchy
            };
            
            AnimatorControllerLayer _controllerLayer = new AnimatorControllerLayer()
            {
                stateMachine = _stateMachine,
            };

            AnimatorState _animatorState = new AnimatorState();
            _animatorState.name = "Default";
            _controllerLayer.stateMachine.AddState(_animatorState, new Vector3(300, 100, 0));
            
            switch (_serial)
            {
                case 0:
                    _controllerLayer.name = "Base Layer";
                    _controllerLayer.avatarMask = null;
                    _controllerLayer.blendingMode = AnimatorLayerBlendingMode.Override;
                    _controllerLayer.defaultWeight = 1;
                    
                    break;
                case 1:
                    _controllerLayer.name = "Limb Layer";
                    _controllerLayer.avatarMask = null;
                    _controllerLayer.blendingMode = AnimatorLayerBlendingMode.Override;
                    _controllerLayer.defaultWeight = 1;

                    break;
                case 2:
                    _controllerLayer.name = "Upper Layer";
                    _controllerLayer.avatarMask = null;
                    _controllerLayer.blendingMode = AnimatorLayerBlendingMode.Additive;
                    _controllerLayer.defaultWeight = 1;

                    break;
                case 3:
                    _controllerLayer.name = "Noise Layer";
                    _controllerLayer.avatarMask = null;
                    _controllerLayer.blendingMode = AnimatorLayerBlendingMode.Additive;
                    _controllerLayer.defaultWeight = 1;

                    break;
            }

            AssetDatabase.AddObjectToAsset(_stateMachine, _savePath);
            AssetDatabase.AddObjectToAsset(_animatorState, _savePath);

            return _controllerLayer;
            // Debug.Log($"设置名称为: {_controllerLayer.name}");
        }

        private AnimaLayerAllMotion InitAnimatorLayerData(AnimatorControllerLayer _animaLayer)
        {
            List<string> mAnimFullName = new List<string>();
            mAnimFullName.Add("Null");
            Dictionary<string, AnimatorState> mAnimPathDic = new Dictionary<string, AnimatorState>();

            AnimatorStateMachine _stateMachine = _animaLayer.stateMachine;
            if (_stateMachine == null)
            {
                return new AnimaLayerAllMotion(mAnimFullName, mAnimPathDic);
            }
            
            foreach (var VARIABLE in _stateMachine.states)
            {
                mAnimFullName.Add(VARIABLE.state.name);
                // Debug.Log($"添加的对象: {VARIABLE.state.name}");
                mAnimPathDic.Add(VARIABLE.state.name, VARIABLE.state);
            }

            foreach (var VARIABLE in _stateMachine.stateMachines)
            {
                InitAnimatorDataDeep(mAnimFullName, mAnimPathDic, VARIABLE.stateMachine, VARIABLE.stateMachine.name);
            }

            return new AnimaLayerAllMotion(mAnimFullName, mAnimPathDic);
        }

        private void InitAnimatorDataDeep(
            List<string> _animFullName, 
            Dictionary<string, AnimatorState> _animPathDic,
            AnimatorStateMachine _state, 
            string _path)
        {
            _path += "/";
            foreach (var VARIABLE in _state.states)
            {
                _animPathDic.TryAdd(VARIABLE.state.name, VARIABLE.state);
                _animFullName.Add(_path + VARIABLE.state.name);
            }

            foreach (var VARIABLE in _state.stateMachines)
            {
                InitAnimatorDataDeep(_animFullName, _animPathDic, VARIABLE.stateMachine, _path + VARIABLE.stateMachine.name);
            }
        }

        public int OnGetSelectIDToAnimName(string _name, int _layer)
        {
            List<string> _AnimFuulName = m_AnimaAllMotions[_layer].mAnimFullName;
            if (!string.IsNullOrEmpty(_name))
            {
                if (_AnimFuulName is not null)
                {
                    for (int i = 0; i < _AnimFuulName.Count; i++)
                    {
                        if (_AnimFuulName[i].Split('/')[^1] == _name)
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        private List<string> OnGetAnimFullName(int _layer)
        {
            return m_AnimaAllMotions[_layer].mAnimFullName;
        }

        private AnimatorState OnGetMotionToAnimName(string _name, int _layer)
        {
            if (string.IsNullOrEmpty(_name))
            {
                return null;
            }
            if (m_AnimaAllMotions[_layer].mAnimPathDic.TryGetValue(_name,out var _motion))
            {
                return _motion;
            }
            else
            {
                Debug.Log($"不存在此键值: {_name}");
            }
            return null;
        }
        #endregion
    }
}
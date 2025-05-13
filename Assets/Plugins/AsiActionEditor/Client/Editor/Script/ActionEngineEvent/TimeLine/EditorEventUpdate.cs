using System;
using System.Collections.Generic;
using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using AsiTimeLine.RunTime;
using AsiActionEditor_Ex.RunTime;
using UnityEditor;
using UnityEngine;
using MotionEngineConst = AsiActionEngine.RunTime.MotionEngineConst;
using Object = UnityEngine.Object;

namespace AsiTimeLine.Editor
{
    public class EditorEventUpdate
    {
        public static Dictionary<object, Object> m_ObjectPool = new Dictionary<object, Object>();
        private static GameObject m_ActionPool = null;
        private static int m_LastTime => TimeLineWindow.Instance.mLastTimeLineTime;
        public static void OnInit()
        {
            mEventWeaponPointTime = int.MinValue;
            mEventCameraChangeTime = int.MinValue;
        }//并不是真的初始化  会和事件一起每帧执行  只是在每个事件执行前执行

        public static void OnChangeAction()
        {
            ClearPool();
            // Debug.Log("ChangeActionAA");
        }//切换Action时触发一次
        
        //_time是毫秒为单位
        public static bool OnUpdate(int _time, EditorActionEvent _actionEvent)
        {
            IActionEventData _eventData = _actionEvent.EventData;
            bool returnValue = true;
            if(_eventData is Event_Attach _eventWeaponPoint)
            {
                Update_EventWeaponPoint(_time, _eventWeaponPoint, _actionEvent);
            }//武器挂点切换
            else if (_eventData is Event_CameraChange _eventCameraChange)
            {
                Update_CameraChange(_time, _eventCameraChange, _actionEvent);
            }
            else if (_eventData is Event_PlayParticle _epp)
            {
                Update_PlayParticle(_time, _actionEvent, _epp);
                // Update_CameraChange(_time, _eventCameraChange, _actionEvent);
            }//特效播放
            else if (_eventData is Event_TimeScale _ets)
            {
                Update_TimeScale(_time, _actionEvent, _ets);
            }
            // else if (_eventData is Event_RaycastHit _erh)
            // {
            //     Update_TimeScale(_time, _actionEvent, _erh);
            // }
            else if (_eventData is Event_PlayAudio _epa)
            {
                Update_PlayAudio(_time, _actionEvent, _epa);
            }
            else
            {
                returnValue = false;
            }
            return returnValue;
        }

        #region InitEditorFuntion
        private static Transform CreateActionPool(string _poolName)
        {
            if (m_ActionPool == null)
            {
                m_ActionPool = GameObject.Find("ActionEnginePool");
                if (m_ActionPool is not null)
                {
                    Object.DestroyImmediate(m_ActionPool);
                }
                
                m_ActionPool = new GameObject();
                if (!Application.isPlaying)
                    m_ActionPool.AddComponent<GameObjectDestory>();
                m_ActionPool.name = "ActionEnginePool";
                // Debug.Log("更新");
            }

            Transform _PPtransform = m_ActionPool.transform.Find(_poolName);
            if (_PPtransform is null)
            {
                _PPtransform = new GameObject().transform;
                _PPtransform.name = _poolName;
                _PPtransform.SetParent(m_ActionPool.transform);
            }
            return _PPtransform;
        }
        
        private static void ClearPool()
        {
            if (m_ActionPool is not null)
            {
                Object.DestroyImmediate(m_ActionPool);
            }
            m_ObjectPool.Clear();
        }
        #endregion
        
        #region UpdateEditorFuntion

        private static void Update_PlayAudio(int _time, EditorActionEvent _actionEvent, Event_PlayAudio _epp)
        {
            if (_actionEvent.TriggerTime >= m_LastTime && _actionEvent.TriggerTime < _time)
            {
                if (ResourcesWindow.Instance.GetRole().TryGetComponent(out ActionEditor_Audio audio))
                {
                    if (_epp.m_CoustomAudio)
                    {
                        audio.PlayAudio(_epp.m_AudioVolume, _epp.m_AudioSourceIndex, _epp.m_AudioDicID,
                            _epp.m_AudioDicChailID);
                    }
                    else
                    {
                        audio.PlayAudio(_epp.m_AudioVolume, _epp.m_AudioSourceIndex, _epp.m_AudioDicID);
                        EngineDebug.LogWarning("无法在Editor预览时正确播放字典中的音效");
                    }
                }
            }
        }
        private static void Update_TimeScale(int _time, EditorActionEvent _actionEvent, Event_TimeScale _epp)
        {
            if (_time > _actionEvent.TriggerTime)
            {
                if (_time < _actionEvent.TriggerTime + _actionEvent.Duration)
                {
                    TimeLineWindow.Instance.TimeScale = _epp.TimeScale;
                }
                else
                {
                    TimeLineWindow.Instance.TimeScale = 1;
                }
            }

        }

        private static void Update_PlayParticle(int _time, EditorActionEvent _actionEvent, Event_PlayParticle _epp)
        {
            if (m_ObjectPool.TryGetValue(_epp, out Object _obj))
            {
                if (_obj is ActionEditor_Effects _particleSystem)
                {
                    if (ResourcesWindow.Instance.GetRole().TryGetComponent(out  CharacterConfig _config))
                    {
                        if (_config.HelpPointDic.TryGetValue((ECharacteLimbType)_epp.PartPointType, out Transform _point))
                        {
                            Vector3 _pos = _point.TransformPoint(_epp.OffsetPos.GetValue());
                            Quaternion _rot = _point.rotation * Quaternion.Euler(_epp.OffsetRot.GetValue());
                            _particleSystem.transform.SetPositionAndRotation(_pos, _rot);
                            _particleSystem.transform.localScale = _epp.LocalScale.GetValue();
                        }
                    }
                    float _nowTime = (_time - _actionEvent.TriggerTime) / (float)MotionEngineConst.TimeDoubling;
                    foreach (var VARIABLE in _particleSystem.particleSystems)
                    {
                        VARIABLE.Simulate(Mathf.Max(0, _nowTime),true,true,true); //更新粒子效果
                    }
                    //_particleSystem.Simulate(Mathf.Max(0, _nowTime),true,true,true); //更新粒子效果
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_epp.PartoclePath))
                {
                    Transform _partPool = CreateActionPool("ActionEditor_Effects");
                    ActionEditor_Effects _loadObject = ActionEnginLoadData.Instance.LoadObject<ActionEditor_Effects>(_epp.PartoclePath);
                    if (_loadObject is not null)
                    {
                        _loadObject = Object.Instantiate(_loadObject, _partPool);
                        // ActionEditor_Effects _particleSystem = _loadObject.GetComponent<ActionEditor_Effects>();
                        if (_loadObject == null)
                        {
                            _epp.PartoclePath = String.Empty;
                            EditorUtility.DisplayDialog("警告", "当前对象最父级未挂载 ActionEditor_Effects,已经取消绑定", "我知道了");
                            return;
                        }
                        // _particleSystem.useAutoRandomSeed = false;
                        if (!Application.isPlaying)
                        {
                            _loadObject.useAutoRandomSeed = false;
                            // foreach (var VARIABLE in _loadObject.GetComponentsInChildren<ActionEditor_Effects>())
                            // {
                            //     foreach (var _particleSystem2 in VARIABLE.particleSystems)
                            //     {
                            //         _particleSystem2.useAutoRandomSeed = false;
                            //     }
                            // }
                        }
                        
                        foreach (var _particleSystem2 in _loadObject.particleSystems)
                        {
                            _particleSystem2.Stop();
                        }
                        m_ObjectPool.Add(_epp, _loadObject);
                    }
                    else
                    {
                        _epp.PartoclePath = string.Empty;
                    }
                }
            }
        }

        private static int mEventCameraChangeTime;
        private static CharacterConfig _characterConfig;
        private static void Update_CameraChange(int _time, 
            Event_CameraChange _eventPlayAnim, EditorActionEvent _actionEvent)
        {
            if (_time >= _actionEvent.TriggerTime)
            {
                if (_actionEvent.TriggerTime > mEventCameraChangeTime)
                {
                    mEventCameraChangeTime = _actionEvent.TriggerTime;
                }
                else
                {
                    return;
                }

                if (ResourcesWindow.Instance.PreCamera is not null)
                {
                    CameraControl _cameraControl = ResourcesWindow.Instance.PreCamControl;
                    ResourcesWindow.Instance.GetRole().TryGetComponent(out CharacterConfig _characterConfig);

                    if (_characterConfig.HelpPointDic.TryGetValue((ECharacteLimbType)_eventPlayAnim.EnterPoint,
                            out Transform _camPoint))
                    {
                        _cameraControl.ChangeCam(_eventPlayAnim.EnterCam, _camPoint);
                    }
                    else
                    {
                        EngineDebug.LogWarning($"相机切换失败, 未配置 [{(ECharacteLimbType)_eventPlayAnim.EnterPoint}]");
                    }
                }
                else
                {
                    EngineDebug.LogError(
                        "Event_CameraChange 事件未执行!!\n" +
                        "<color=#FF0000>未创建相机预览</color> !!"
                    );
                }
            }
        }
        
        private static int mEventWeaponPointTime;
        private static void Update_EventWeaponPoint(int _time, 
            Event_Attach _eventPlayAnim, EditorActionEvent _actionEvent)
        {
            if (_time > _actionEvent.TriggerTime)
            {
                if (_actionEvent.TriggerTime > mEventWeaponPointTime)
                {
                    mEventWeaponPointTime = _actionEvent.TriggerTime;
                }
                else
                {
                    return;
                }
                if (ResourcesWindow.Instance.GetRole().TryGetComponent(out CharacterConfig _config))
                {
                    // Transform _refer = _eventPlayAnim.ReferTransform.Get();
                    // Transform _target = _eventPlayAnim.TargetTransform.Get();
                    Transform _refer = null;
                    Transform _target = null;
                    if (!_config.HelpPointDic.TryGetValue((ECharacteLimbType)_eventPlayAnim.ReferTransform.m_Value, out _refer))
                    {
                        EngineDebug.LogError("附加对象时，缺失目标");
                        return;
                    }

                    if (!_config.HelpPointDic.TryGetValue((ECharacteLimbType)_eventPlayAnim.TargetTransform.m_Value, out _target))
                    {
                        EngineDebug.LogError("附加对象时，缺失附加目标");
                        return;
                    }
                    _refer.SetParent(_target);
                    if (_eventPlayAnim.alignToTarget)
                    {
                        _refer.position = _target.position;
                        _refer.rotation = _target.rotation;
                    }

                    // Transform _weapon = _eventPlayAnim.IsRightWeapon ? _config.WeaponR : _config.WeaponL;
                    // if (_weapon is null)
                    // {
                    //     EngineDebug.LogError(
                    //         "Event_WeaponPointChange 事件未执行!!\n" +
                    //         $"<color=#FF0000>CharacterConfig</color> 的" +
                    //         $"{(_eventPlayAnim.IsRightWeapon ? "右": "左")}手武器配置为空!!"
                    //     );
                    //     return;
                    // }
                    //
                    // ECharacteLimbType _characteLimb = (ECharacteLimbType)_eventPlayAnim.LimbPointType;
                    // if (_config.HelpPointDic.TryGetValue(_characteLimb, out var _target))
                    // {
                    //     _weapon.parent = _target;
                    //     if (_eventPlayAnim.AlignToPoint)
                    //     {
                    //         _weapon.localPosition = Vector3.zero;
                    //         _weapon.rotation = _target.rotation;
                    //     }
                    // }
                }
                else
                {
                    EngineDebug.LogError(
                        "Event_WeaponPointChange 事件未执行!!\n" +
                        "未挂载 <color=#FF0000>CharacterConfig</color> 组件!!"
                    );
                }
            }
        }
        

        #endregion

        private static List<float> mEventTime = new List<float>();
        private static List<string> mEventAction = new List<string>();
        public static bool ReLoadActionData(string _groupName)
        {
            // EngineDebug.Log($"加载Action组： [{_groupName}]");
            List<Unit> _units = ActionEngineManager_Unit.Instance.Units;
            foreach (Unit _unit in _units)
            {
                if (_unit.ActionStateMachine.ActionGroupName == _groupName)
                {
                    //当前动画机
                    ActionStateMachine _stateMachine = _unit.ActionStateMachine;
                    
                    //收集每个Action层级状态渡过的时间
                    mEventTime.Clear();
                    foreach (ActionStatePart _part in _stateMachine.AllActionStatePart)
                    {
                        mEventTime.Add(_part.ElapsedTime);
                        mEventAction.Add(_part.ActionEnble ? _part.CurrentActionState.Name : String.Empty);
                    }
                    
                    //动画倍率
                    float TimeScale = _stateMachine.TimeScale;
                    
                    //重加载
                    ActionEngineManager_Unit.Instance.GetActionList_Editor(_groupName, list =>
                    {
                        ActionStateMachine statePart = new ActionStateMachine(_unit, _unit.GetComponent<Animator>(), 
                            list, ActionEngineManager_GValue.Instance.GValue, _stateMachine.InitGValue_Setting);
                        _unit.SetActionStateMachine(statePart);

                        if (ActionEngineManager_Input.Instance.IsPlayer(_unit))
                        {
                            CameraControl _cameraControl= ActionEngineManager_Input.Instance.CurCamera;
                            if (statePart.TryGetComponent(out CharacterConfig _config))
                            {
                                if (_config.HelpPointDic.TryGetValue(ECharacteLimbType.Cam_Main, out Transform _trans))
                                {
                                    _cameraControl.OnInit(_trans, statePart, 0);
                                }
                            }
                        }
                    });
                    // EngineDebug.Log($"触发加载Action组的对象： [{_unit.transform.GetHashCode()}]");
                    
                    _stateMachine = _unit.ActionStateMachine;
                    _stateMachine.TimeScale = TimeScale;
                    for (int i = 0; i < mEventTime.Count; i++)
                    {
                        string _actionName = mEventAction[i];
                        if (!string.IsNullOrEmpty(_actionName))
                        {
                            _stateMachine.ChangeAction(_actionName,0,0);
                            _stateMachine.AllActionStatePart[i].ElapsedTime = mEventTime[i];
                        }
                    }
                }
            }
            
            TimeLineWindow.Instance.UpdateTimeToNow();
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using AsiActionEditor_Ex.RunTime;
using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.DrawData;
using AsiTimeLine.RunTime;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AsiTimeLine.Editor
{
    public partial class DrawInspector
    {
        private static List<string> _eventNames = new List<string>();

        public static void DrawEvent(EditorActionEvent _actionEvent, bool _isInit)
        {
            IActionEventData _eventData = _actionEvent.EventData;

            EEvenType _evenType = (EEvenType)_eventData.GetEvenType();

            _eventNames.Clear();
            
            using (new GUIColorScope(Color.gray))
            {
                GUILayout.Label(_evenType.ToString());
            }
            GUILayout.Space(10);
            
            //属性面板绘制
            switch (_evenType)
            {
                // case EEvenType.EET_CharacterMove:
                //     DrawCharacterMove((Event_CharacterMove)_eventData);
                //     break;
                
                case EEvenType.EET_CameraChange:
                    DrawCameraChange(_actionEvent);
                    break;
                
                case EEvenType.EET_Partocle:
                    DrawPartocleData((Event_PlayParticle)_eventData, _isInit);
                    break;
                case EEvenType.EET_Audio:
                    DrawAudio((Event_PlayAudio)_eventData, _actionEvent);
                    break;
                
                case EEvenType.EET_Event_FindTargetToSphere:
                    DrawFindTargetToSphere((Event_FindTargetToSphere)_eventData, _actionEvent);
                    break;
                case EEvenType.EET_SetAnimFloat:
                    DrawSetAnimatorFloat((Event_SetAnimFloat)_eventData);
                    break;
                // case EEvenType.EET_UnitRot:
                //     DrawUnitRot((Event_UnitRot)_eventData, _actionEvent);
                //     break;
                case EEvenType.EET_SetGValue:
                    DrawSetGValue((Event_SetGValue)_eventData, _actionEvent);
                    break;
                case EEvenType.EET_RaycastHit:
                    DrawRaycastHit((Event_RaycastHit)_eventData, _actionEvent);
                    break;
                case EEvenType.EET_Attach:
                    DrawWeaponPointChange((Event_Attach)_eventData, _actionEvent);
                    break;
                case EEvenType.EET_TowBoneIK:
                    DrawEditorAttribute.Draw((Event_TowBoneIK)_eventData);
                    break;
                default:
                    //绘制 EditorProperty 
                    DrawEditorAttribute.Draw(_eventData);
                    break;
            }
        }

        #region DrawFuntion
        private static List<string> DrawDicNames = new List<string>();
        private static void DrawAudio(Event_PlayAudio _eventData, EditorActionEvent _actionEvent)
        {
            if (ResourcesWindow.Instance.GetRole().TryGetComponent(out ActionEditor_Audio audio))
            {
                DrawDicNames.Clear();
                AudioClipDicList _list = ActionEngineManager_AudioClip.Instance._audioClipDicList;
                foreach (AudioClipDic VARIABLE in _list.clips)
                {
                    DrawDicNames.Add(VARIABLE.name);
                }
                if (GUILayout.Button("打开音效设定窗口"))
                {
                    AudioClipWindows.Instance.Open();
                }
                GUILayout.Space(10);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("音量大小", GUILayout.Width(60));
                    _eventData.m_AudioVolume = EditorGUILayout.Slider(_eventData.m_AudioVolume, 0.0f, 1.0f);
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("选择音源组件", GUILayout.Width(80));
                    _eventData.m_AudioSourceIndex = (byte)EditorGUILayout.Popup(_eventData.m_AudioSourceIndex, audio.m_AudioSourceNames.ToArray());
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("选择音效字典", GUILayout.Width(80));
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        _eventData.m_AudioDicID = (byte)EditorGUILayout.Popup(_eventData.m_AudioDicID, DrawDicNames.ToArray());
                        if (_check.changed)
                        {
                            _eventData.m_AudioDicChailID = 0;
                        }
                    }
                }
                _eventData.m_CoustomAudio = GUILayout.Toggle(_eventData.m_CoustomAudio, "自定义具体播放音效");
                if (_eventData.m_CoustomAudio)
                {
                    DrawDicNames.Clear();
                    foreach (AudioClipGroup_Part VARIABLE in _list.clips[_eventData.m_AudioDicID].parts)
                    {
                        DrawDicNames.Add(VARIABLE.ClipName);
                    }
                    _eventData.m_AudioDicChailID = (byte)EditorGUILayout.Popup(_eventData.m_AudioDicChailID, DrawDicNames.ToArray());
                    
                    GUILayout.Space(10);

                    AudioClipGroup_Part _part = _list.clips[_eventData.m_AudioDicID]
                        .parts[_eventData.m_AudioDicChailID];

                    string _type = _part.isRandom ? "随机循环" : "上至下顺序循环";
                    GUILayout.Label($" [{_part.ClipName}]   下的音频列表， 该列表以   [{_type}]   的方式播放");
                    for (int i = 0; i < _part.AudioClips.Count; i++)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            using (var _check = new EditorGUI.ChangeCheckScope())
                            {
                                AudioClip _audioClip =
                                    EditorGUILayout.ObjectField(_part.AudioClips[i], typeof(AudioClip), false) as
                                        AudioClip;
                                if (_check.changed)
                                {
                                    //设置音频资产
                                    _part.SetAudioClip(i, _audioClip);
                                }
                            }
                            if (GUILayout.Button("打开配置窗口", GUILayout.Width(80)))
                            {
                                foreach (AudioClipGroup_Part VARIABLE2 in _list.clips[_eventData.m_AudioDicID].parts)
                                {
                                    VARIABLE2.open = false;
                                }

                                _part.open = true;
                                AudioClipWindows.Instance.selectedTool = _eventData.m_AudioDicID;
                                AudioClipWindows.Instance.Open();
                            }
                        }
                    }
                    if (GUILayout.Button("保存音频列表配置"))
                    {
                        AudioClipWindows.Instance.SaveAudioConfig();
                    }
                }
                else
                {
                    GUILayout.Space(10);
                    GUILayout.Label($" [{DrawDicNames[_eventData.m_AudioDicID]}]  字典的成员");
                    for (int i = 0; i < _list.clips[_eventData.m_AudioDicID].parts.Count; i++)
                    {
                        AudioClipGroup_Part VARIABLE = _list.clips[_eventData.m_AudioDicID].parts[i];
                        if (GUILayout.Button("字典成员" + VARIABLE.ClipName))
                        {
                            AudioClipWindows.Instance.selectedTool = _eventData.m_AudioDicID;
                            foreach (AudioClipGroup_Part VARIABLE2 in _list.clips[_eventData.m_AudioDicID].parts)
                            {
                                VARIABLE2.open = false;
                            }
                            VARIABLE.open = true;
                            AudioClipWindows.Instance.Open();
                        }
                    }

                }


            }
            else
            {
                GUILayout.Label("未在角色身上找到 [ActionEditor_Audio] 组件，无法执行");
            }
            // ResourcesWindow.Instance.TryGetCharacterConfig()
        }

        private static void DrawFindTargetToSphere(Event_FindTargetToSphere _eventData, EditorActionEvent _actionEvent)
        {
            _eventNames.Add("IsFindUnit");
            // if (!_eventData.IsFindUnit)
            // {
                _eventNames.Add("LayerMask");
            // }

            _eventNames.Add("RadiuCenterID");
            _eventNames.Add("Radius");
            _eventNames.Add("ReferDir");
            _eventNames.Add("FindType");
            if (_eventData.FindType == Event_FindTargetToSphere.EFindType.在范围内按分值查找单位)
            {
                _eventNames.Add("PosInttegral");
                _eventNames.Add("RotInttegral");
            }

            _eventNames.Add("IsSetNull");
            GUILayout.Space(5);
            if (_eventData.IsFindUnit) _eventNames.Add("SetUnit");
            else _eventNames.Add("SetTransform");
            DrawEditorAttribute.Draw(_eventData, _eventNames);

            GUILayout.Space(10);
            GUILayout.Label("仅Editor下预览用");
            using (new GUILayout.HorizontalScope())
            {
                // if (GUILayout.Button("在目标位置中心随机撒点"))
                // {
                //     // EngineDrawListData.Instance.Draw_Point_Group.Add();
                // }
                GUILayout.Label("在目标位置中心随机撒点, 数量：");
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    int number =
                        EditorGUILayout.DelayedIntField(EngineDrawListData.Instance.Draw_Point_Group.Count);
                    if (check.changed)
                    {
                        if (ResourcesWindow.Instance.TryGetCharacterConfig(out CharacterConfig config))
                        {
                            if (config.HelpPointDic.TryGetValue((ECharacteLimbType)_eventData.RadiuCenterID,
                                    out Transform _transform))
                            {
                                EngineDrawListData.Instance.Draw_Point_Group.Clear();
                                for (int i = 0; i < number; i++)
                                {
                                    Quaternion rot = Quaternion.Euler(0, 0, 0);
                                    Vector3 pos = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f),
                                        Random.Range(-1.0f, 1.0f));
                                    pos =pos.normalized * _eventData.Radius;
                                    pos += _transform.position;
                                    Draw_PointTransData _point =
                                        new Draw_PointTransData(pos, rot, "point_" + i, Color.red);
                                    EngineDrawListData.Instance.Draw_Point_Group.Add(_point);
                                }
                            }
                        }
                    }
                }
            }

            if (GUILayout.Button("清空点数据"))
            {
                EngineDrawListData.Instance.Draw_Point_Group.Clear();
            }
        }

        private static void DrawWeaponPointChange(Event_Attach _eventData, EditorActionEvent _actionEvent)
        {
            _eventNames.Add("AlignToPoint");
            _eventNames.Add("ReferTransform");

            if (_eventData.AlignToPoint)
            {//对齐至点数据
                _eventNames.Add("AlignPoint");
            }
            else
            {
                _eventNames.Add("TargetTransform");
                _eventNames.Add("SetPrente");
                _eventNames.Add("alignToTarget");
            }
            _eventNames.Add("LerpTime");
            _eventNames.Add("OffsetPos");
            _eventNames.Add("OffsetRot");
            DrawEditorAttribute.Draw(_eventData, _eventNames);
        }
        private static void DrawRaycastHit(Event_RaycastHit _event, EditorActionEvent _actionEvent)
        {
            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                DrawEditorAttribute.Draw(_event);
                if (_check.changed)
                {
                    ScenceDraw.Instance.OnUpdateDraw();
                }
            }
        }

        private static void DrawSetGValue(Event_SetGValue _event, EditorActionEvent _actionEvent)
        {
            if (_actionEvent.Duration == 0)
            {
                _eventNames.Add("GValueSet");
            }
            else
            {
                _eventNames.Add("GValueSet");
                _eventNames.Add("ExitGValueSet");
            }
            DrawEditorAttribute.Draw(_event, _eventNames);
        }
        private static void DrawUnitRot(Event_UnitRot _event, EditorActionEvent _actionEvent)
        {
            _eventNames.Add("IsMove");
            if (_event.IsMove)
            {
                _eventNames.Add("IsMovePre");
            }
            if (_actionEvent.Duration == 0)
            {
                _eventNames.Add("RotaType");
                _eventNames.Add("RotLerp");
                _eventNames.Add("OffsetRotY");
                _eventNames.Add("RotPriority");
                _eventNames.Add("TotalTime");
            }
            else
            {
                _eventNames.Add("RotaType");
                _eventNames.Add("RotLerp");
                _eventNames.Add("OffsetRotY");
                _eventNames.Add("RotPriority");
            }
            DrawEditorAttribute.Draw(_event, _eventNames);
        }
        
        private static void DrawSetAnimatorFloat(Event_SetAnimFloat _event)
        {
            _eventNames.Add("ValueType");
            if (_event.ValueType == Event_SetAnimFloat.EValueType._1D)
            {
                //1d参数绘制
                _eventNames.Add("AnimaFloatName");
                _eventNames.Add("AnimFloatFor");
                _eventNames.Add("AnimaFloatSpeed");
            }
            else
            {
                //2d参数绘制
                _eventNames.Add("AnimaFloatName");
                _eventNames.Add("AnimaFloatName2");
                _eventNames.Add("AnimFloatFor");
                _eventNames.Add("AnimaFloatSpeed");
            }
            DrawEditorAttribute.Draw(_event, _eventNames);

        }
        private static void DrawCameraChange(EditorActionEvent _actionEvent)
        {
            Event_CameraChange _eventCameraChange = (Event_CameraChange)_actionEvent.EventData;
            if (_actionEvent.Duration > 0)
            {
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    _eventNames.Add("EnterCam");
                    _eventNames.Add("EnterPoint");
                    _eventNames.Add("EnterTime");
                    _eventNames.Add("ExitCam");
                    _eventNames.Add("ExitCamPoint");
                    _eventNames.Add("CheckLable");
                    if (_check.changed)
                    {
                        TimeLineWindow.Instance.UpdateTimeToNow();
                        // SceneView.RepaintAll();
                    }
                }
                if (_eventCameraChange.CheckLable)
                {
                    _eventNames.Add("ActionLable");
                    _eventNames.Add("IsContain");
                }
            }
            else
            {
                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    _eventNames.Add("EnterCam");
                    _eventNames.Add("EnterPoint");
                    _eventNames.Add("EnterTime");
                    _eventNames.Add("CheckLable");
                    if (_check.changed)
                    {
                        TimeLineWindow.Instance.UpdateTimeToNow();
                    }
                }

                if (_eventCameraChange.CheckLable)
                {
                    _eventNames.Add("ActionLable");
                    _eventNames.Add("IsContain");
                }
            }
            _eventNames.Add("IsLockCam");
            DrawEditorAttribute.Draw(_eventCameraChange, _eventNames);

        }
        private static bool m_ReferPoint;
        private static void DrawPartocleData(Event_PlayParticle _event, bool _init)
        {
            if (_init)
            {
                m_ReferPoint = false;
            }

            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                _eventNames.Add("PartoclePath");
                if (_check.changed)
                {
                    GameObject _loadObject = ActionEnginLoadData.Instance.LoadObject<GameObject>(_event.PartoclePath);
                    if (!_loadObject.TryGetComponent(out ActionEditor_Effects _particleSystem))
                    {
                        EditorUtility.DisplayDialog("警告", "当前对象最父级未挂载 ActionEditor_Effects,已经取消绑定", "我知道了");
                        _event.PartoclePath = String.Empty;
                    }
                }
            }
            _eventNames.Add("PartPointType");
            _eventNames.Add("AlwaysFollow");
            _eventNames.Add("Life");
            DrawEditorAttribute.Draw(_event, _eventNames);

            GUILayout.Space(10);
            Action _setTrans = () =>
            {
                m_ReferPoint = false;
                if (EditorEventUpdate.m_ObjectPool.TryGetValue(_event, out Object _value))
                {
                    Transform _transform = (_value as ActionEditor_Effects).transform;
                    if (ResourcesWindow.Instance.GetRole().TryGetComponent(out CharacterConfig _config))
                    {
                        if (_config.HelpPointDic.TryGetValue((ECharacteLimbType)_event.PartPointType, out Transform _point))
                        {
                            _event.OffsetPos.SetValue( _point.InverseTransformPoint(_transform.position));
                            Quaternion _rot = _point.rotation * Quaternion.Inverse(_transform.rotation);
                            _event.OffsetRot.SetValue(_rot.eulerAngles);
                        }
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("警告", "未配置粒子", "我知道了");
                }
            };
            if (!m_ReferPoint)
            {
                
                if (GUILayout.Button("拾取坐标变换"))
                {
                    if (EditorEventUpdate.m_ObjectPool.TryGetValue(_event, out Object _value))
                    {
                        m_ReferPoint = true;
                        Selection.activeObject = _value;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("警告", "未配置粒子", "我知道了");
                    }
                }
                // DrawEditorAttribute.Draw(_event, new []{"OffsetPos","OffsetRot"});

                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("位置偏移: ", GUILayout.Width(60));
                        _event.OffsetPos = new EVector3(EditorGUILayout.Vector3Field("", _event.OffsetPos.GetValue()));
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("角度偏移: ", GUILayout.Width(60));
                        _event.OffsetRot = new EVector3(EditorGUILayout.Vector3Field("", _event.OffsetRot.GetValue()));
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("缩放: ", GUILayout.Width(60));
                        _event.LocalScale = new EVector3(EditorGUILayout.Vector3Field("", _event.LocalScale.GetValue()));
                    }
                    if (_check.changed)
                    {
                        TimeLineWindow.Instance.UpdateTimeToNow();
                    }
                }

            }
            else
            {
                Color _guiColor = GUI.color;
                GUI.color = Color.red;
                if (GUILayout.Button("应用坐标变换"))
                {
                    _setTrans();
                }
                GUI.color = _guiColor;

                
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("位置偏移: 设定中");
                    // _event.OffsetPos = EditorGUILayout.Vector3Field("", _event.OffsetPos);
                    // GUILayout.Label(_event.OffsetPos.ToString());
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("角度偏移: 设定中");
                    // _event.OffsetRot = EditorGUILayout.Vector3Field("", _event.OffsetRot);
                    // GUILayout.Label(_event.OffsetRot.ToString());
                }
                
            }

        }
        #endregion
    }
}
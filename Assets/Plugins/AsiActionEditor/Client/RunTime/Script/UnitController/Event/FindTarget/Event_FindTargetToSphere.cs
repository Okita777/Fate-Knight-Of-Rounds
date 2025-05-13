using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.DrawData;
using AsiTimeLine.RunTime;
using UnityEngine;

namespace AsiActionEditor_Ex.RunTime
{
    [System.Serializable]
    public class Event_FindTargetToSphere : IActionEventData
    {
        public enum EReferDir
        {
            CharacterConfig,
            SelfForward,
            CamForward,
            MoveInputDir,
        }

        public enum EFindType
        {
            在范围内按分值查找单位,
            找范围内最小角度单位,
            找范围内最小距离单位,
        }

        [SerializeField] protected bool m_IsFindUnit = true;
        [SerializeField] protected int m_LayerMask;
        [SerializeField] protected float m_Radius = 5;
        [SerializeField] protected int m_RadiuCenterID;
        [SerializeField] protected bool m_IsSetNull = true;
        [SerializeField] protected EReferDir m_ReferDir = EReferDir.CharacterConfig;
        [SerializeField] protected EFindType m_FindType = EFindType.在范围内按分值查找单位;
        [SerializeField] protected float m_PosInttegral = 2;
        [SerializeField] protected float m_RotInttegral = 5;
        [SerializeField] protected GValue_SetTransform m_SetTransform = new GValue_SetTransform();
        [SerializeField] protected GValue_SetUnit m_SetUnit = new GValue_SetUnit();
        
        #region MyRegion

        [EditorProperty("仅查找Unit单位", EditorPropertyType.EEPT_Bool)]
        public bool IsFindUnit
        {
            get { return m_IsFindUnit; }
            set { m_IsFindUnit = value; }
        }

        [EditorProperty("   LayerMask", EditorPropertyType.EEPT_LayerMask)]
        public int LayerMask
        {
            get { return m_LayerMask; }
            set { m_LayerMask = value; }
        }

        [EditorProperty("中心位置挂点", EditorPropertyType.EEPT_CharacteLimbType)]
        public int RadiuCenterID
        {
            get { return m_RadiuCenterID; }
            set { m_RadiuCenterID = value; }
        }

        [EditorProperty("半径", EditorPropertyType.EEPT_Float)]
        public float Radius
        {
            get { return m_Radius; }
            set { m_Radius = value; }
        }

        [EditorProperty("参考朝向", EditorPropertyType.EEPT_Enum)]
        public EReferDir ReferDir
        {
            get { return m_ReferDir; }
            set { m_ReferDir = value; }
        }
        [EditorProperty("查找对象方案", EditorPropertyType.EEPT_Enum)]
        public EFindType FindType
        {
            get { return m_FindType; }
            set { m_FindType = value; }
        }
        [EditorProperty("   位置分值", EditorPropertyType.EEPT_Float)]
        public float PosInttegral
        {
            get { return m_PosInttegral; }
            set { m_PosInttegral = value; }
        }
        [EditorProperty("   角度分值", EditorPropertyType.EEPT_Float)]
        public float RotInttegral
        {
            get { return m_RotInttegral; }
            set { m_RotInttegral = value; }
        }
        [EditorProperty("设置到指定目标", EditorPropertyType.EEPT_SetGTransform)]
        public GValue_SetTransform SetTransform
        {
            get { return m_SetTransform; }
            set { m_SetTransform = value; }
        }
        [EditorProperty("设置到指定目标", EditorPropertyType.EEPT_SetGUnit)]
        public GValue_SetUnit SetUnit
        {
            get { return m_SetUnit; }
            set { m_SetUnit = value; }
        }
        [EditorProperty("如果未检测到对象返回空值", EditorPropertyType.EEPT_Bool, LabelWidth = 150)]
        public bool IsSetNull
        {
            get { return m_IsSetNull; }
            set { m_IsSetNull = value; }
        }

        #endregion

        public int GetEvenType() => (int)EEvenType.EET_Event_FindTargetToSphere;

        public IActionEventData Creact() => new Event_FindTargetToSphere();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            ActionStateMachine _actionEngine = _actionState.ActionStateMachine;
            if (m_IsFindUnit) m_SetUnit.Init(_actionEngine);
            else m_SetTransform.Init(_actionEngine);
            if (_isSingle)
            {
                if (_actionEngine.TryGetComponent(out CharacterConfig _characterConfig))
                {
                    if (_characterConfig.HelpPointDic.TryGetValue((ECharacteLimbType)m_RadiuCenterID,
                            out Transform transform))
                    {
                        OnFind(transform, _actionEngine);
                    }
                }
            }
        }

        public void Update(ActionStatePart _actionState, ActionMachineTime _actionTime)
        {
            ActionStateMachine _actionEngine = _actionState.ActionStateMachine;
            if (_actionEngine.TryGetComponent(out CharacterConfig _characterConfig))
            {
                if (_characterConfig.HelpPointDic.TryGetValue((ECharacteLimbType)m_RadiuCenterID,
                        out Transform transform))
                {
                    OnFind(transform, _actionEngine);
                }
            }
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_FindTargetToSphere _event = _eventData as Event_FindTargetToSphere;
            _event.IsFindUnit = m_IsFindUnit;
            _event.LayerMask = m_LayerMask;
            _event.Radius = m_Radius;
            _event.RadiuCenterID = m_RadiuCenterID;
            _event.FindType = m_FindType;
            _event.PosInttegral = m_PosInttegral;
            _event.RotInttegral = m_RotInttegral;
            _event.SetTransform = m_SetTransform;
            _event.SetUnit = m_SetUnit;
            _event.IsSetNull = m_IsSetNull;
            _event.ReferDir = m_ReferDir;
            return _event;
        }

        public void EditorDraw(CharacterConfig characterConfig, ActionStatePart _actionState,
            ActionMachineTime _actionTime)
        {
            if (_actionTime.IsInRange)
            {
                if (characterConfig.HelpPointDic.TryGetValue((ECharacteLimbType)m_RadiuCenterID,
                        out Transform _transform))
                {
                    EngineScenceDraw.Sphere(_transform.position, _transform.rotation, m_Radius, Color.red);

                    EngineDrawListData.Instance.Draw_Point_Group_Sort.Clear();
                    EngineDrawListData.Instance.Draw_Point_Group_Sort.AddRange(EngineDrawListData.Instance
                        .Draw_Point_Group);
                    int _pointLenght = EngineDrawListData.Instance.Draw_Point_Group_Sort.Count;

                    if (_pointLenght > 0)
                    {

                        float _PosInttegral = m_PosInttegral;
                        float _RotInttegral = m_RotInttegral;
                        if (m_FindType == EFindType.找范围内最小角度单位)
                        {
                            _PosInttegral = 0;
                            _RotInttegral = 10;
                        }
                        else if (m_FindType == EFindType.找范围内最小距离单位)
                        {
                            _PosInttegral = 10;
                            _RotInttegral = 0;
                        }


                        float _radius = m_Radius * m_Radius;
                        foreach (Draw_PointTransData _point in EngineDrawListData.Instance.Draw_Point_Group_Sort)
                        {
                            Vector3 _position = _transform.position - _point.pos;
                            if (_position.sqrMagnitude < _radius)
                            {
                                float _posI = _position.sqrMagnitude * _PosInttegral; //距离越大，分值越大
                                float _angleI = Vector3.Dot(_transform.forward, _position.normalized) * _RotInttegral;
                                _point.value = _posI + _angleI;
                            }
                            else
                            {
                                //超出范围
                                _point.value = 999999999.0f;
                            }
                        }

                        EngineDrawListData.Instance.Draw_Point_Group_Sort.Sort((x, y) =>
                        {
                            return x.value.CompareTo(y.value);
                        });

                        //最终找到的点
                        Draw_PointTransData _Nowpoint = EngineDrawListData.Instance.Draw_Point_Group_Sort[0];
                        if (_Nowpoint.value > 999999998.0f)
                        {
                            EngineScenceDraw.Line(_transform.position, _Nowpoint.pos, Color.red);
                            _Nowpoint.name = "point (超出范围不计分)";
                        }
                        else
                        {
                            EngineScenceDraw.Line(_transform.position, _Nowpoint.pos, Color.blue);
                            _Nowpoint.name = "point (最终输出的点)";
                        }


                        //最大最小分值差
                        float _propertyV = EngineDrawListData.Instance.Draw_Point_Group_Sort[_pointLenght - 1].value -
                                           _Nowpoint.value;

                        for (int i = 1; i < EngineDrawListData.Instance.Draw_Point_Group_Sort.Count; i++)
                        {
                            Draw_PointTransData _point = EngineDrawListData.Instance.Draw_Point_Group_Sort[i];
                            if (_point.value > 999999998.0f)
                            {
                                EngineScenceDraw.Line(_transform.position, _point.pos, Color.red);
                                _point.name = "point (超出范围不计分)";
                            }
                            else
                            {
                                if (_propertyV != 0)
                                {
                                    float _mValue = _point.value - _Nowpoint.value;
                                    float _mV = _mValue / _propertyV;
                                    _mV = 1 - _mV;
                                    Color color = Color.Lerp(Color.red, Color.green, _mV);
                                    EngineScenceDraw.Line(_transform.position, _point.pos, color);
                                    _point.name = $"point 分值({_mV})";
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnFind(Transform _transform, ActionStateMachine _actionStateMachine)
        {
            float _PosInttegral = m_PosInttegral;
            float _RotInttegral = m_RotInttegral;
            if (m_FindType == EFindType.找范围内最小角度单位)
            {
                _PosInttegral = 0;
                _RotInttegral = 10;
            }else if (m_FindType == EFindType.找范围内最小距离单位)
            {
                _PosInttegral = 10;
                _RotInttegral = 0;
            }
            float _radius = m_Radius * m_Radius;
            if (m_IsFindUnit)
            {
                if (_actionStateMachine.TryGetStaticLogic(out Ex_FindTarget _findTarget))
                {
                    foreach (Unit VARIABLE in ActionEngineManager_Unit.Instance.Units)
                    {
                        if ((_transform.position - VARIABLE.transform.position).sqrMagnitude < _radius)
                        {
                            if ((m_LayerMask & (1 << VARIABLE.gameObject.layer)) != 0)
                            {
                                _findTarget.AddChild(VARIABLE, _transform.position,
                                    GetForward(_transform, _actionStateMachine), _PosInttegral, _RotInttegral);
                            }
                        }
                    } //将安全范围的对象添加进来

                    if (_findTarget.TryGetTarget(out Unit _unit))
                    {
                        m_SetUnit.Set(_unit);
                    }
                    else if (m_IsSetNull)
                    {
                        m_SetUnit.SetNull();
                    }
                }
            }
            else
            {
                if (_actionStateMachine.TryGetStaticLogic(out Ex_FindTarget _findTarget))
                {
                    Collider[] colliders = Physics.OverlapSphere(_transform.position, _radius, m_LayerMask);
                    foreach (Collider VARIABLE in colliders)
                    {
                        _findTarget.AddChild(VARIABLE, _transform.position, GetForward(_transform, _actionStateMachine),
                            _PosInttegral, _RotInttegral);
                    }

                    if (_findTarget.TryGetTarget(out Collider _collider))
                    {
                        m_SetTransform.Set(_collider.transform);
                    }
                    else if (m_IsSetNull)
                    {
                        m_SetTransform.SetNull();
                    }
                }
            }
        }

        private Vector3 GetForward(Transform _transform, ActionStateMachine _machine)
        {
            if (m_ReferDir == EReferDir.CharacterConfig) return _transform.forward;
            if (_machine.TryGetStaticLogic(out Ex_GetAngleToType _getAngle))
            {
                if (m_ReferDir == EReferDir.MoveInputDir) return _getAngle.Get(_machine, EAngleType.InputDir, _transform);
                if (m_ReferDir == EReferDir.CamForward) return _machine.GetCamRot() * Vector3.forward;
                if (m_ReferDir == EReferDir.SelfForward) return _machine.CurUnit.transform.forward;
            }
            return Vector3.forward;
        }
    }
}
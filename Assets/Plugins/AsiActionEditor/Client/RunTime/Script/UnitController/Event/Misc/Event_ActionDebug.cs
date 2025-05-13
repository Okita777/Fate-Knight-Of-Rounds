using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.GraphVal;
using UnityEngine;
using UnityEngine.Serialization;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_ActionDebug : IActionEventData
    {
        [SerializeField] protected string mDebugName = "";
        [SerializeField] protected GInt mGIntTest = new GInt(12);
        [SerializeField] protected GFloat mGFloatTest2 = new GFloat(23);
        [SerializeField] protected GEnum mEnumname = new GEnum(0);
        [SerializeField] protected GTransform mGTransform = new GTransform();
        [SerializeField] protected GValue_Setting mGintTest3 = new GValue_Setting();
        [SerializeField] protected GValue_Ratio mGintTest4 = new GValue_Ratio();
        [SerializeField] protected EVector3 mEVector3 = new EVector3();
        [SerializeField] protected GValue_SetUnit mUnitPos = new GValue_SetUnit();
        [SerializeField] protected GValue_SetTransform mTransPos = new GValue_SetTransform();
        [SerializeField] protected GValue_SetPoint mPointPos = new GValue_SetPoint();
        [FormerlySerializedAs("mEvent_Vector3")] [SerializeField] protected GraphEvent_NoValue_Vector3 mEventNoValueVector3 = new GraphEvent_NoValue_Vector3();

        #region property
        [EditorProperty("Unit位置绘制", EditorPropertyType.EEPT_SetGUnit)]
        public GValue_SetUnit UnitPos
        {
            get { return mUnitPos; }
            set { mUnitPos = value; }
        }
        [EditorProperty("Trans位置绘制", EditorPropertyType.EEPT_SetGTransform)]
        public GValue_SetTransform TransPos
        {
            get { return mTransPos; }
            set { mTransPos = value; }
        }
        [EditorProperty("Point位置绘制", EditorPropertyType.EEPT_SetGPoint)]
        public GValue_SetPoint PointPos
        {
            get { return mPointPos; }
            set { mPointPos = value; }
        }
        
        [EditorProperty("Debug标题", EditorPropertyType.EEPT_String)]
        public string DebugName
        {
            get { return mDebugName; }
            set { mDebugName = value; }
        }
        
        [EditorProperty("参数测试1", EditorPropertyType.EEPT_Int)]
        public GInt GIntTest
        {
            get { return mGIntTest; }
            set { mGIntTest = value; }
        }
        
        [EditorProperty("参数测试2", EditorPropertyType.EEPT_Float)]
        public GFloat GFloatTest2
        {
            get { return mGFloatTest2; }
            set { mGFloatTest2 = value; }
        }
        
        [EditorProperty("参数测试3", EditorPropertyType.EEPT_GValueSetting)]
        public GValue_Setting GintTest3
        {
            get { return mGintTest3; }
            set { mGintTest3 = value; }
        }
        [EditorProperty("参数测试4", EditorPropertyType.EEPT_GValueSRatio)]
        public GValue_Ratio GintTest4
        {
            get { return mGintTest4; }
            set { mGintTest4 = value; }
        }
        [EditorProperty("参数测试5", EditorPropertyType.EEPT_Enum, EnumNames = new []{"选项1","选项2","选项3"})]
        public GEnum Enumname
        {
            get { return mEnumname; }
            set { mEnumname = value; }
        }
        [EditorProperty("获取Trans", EditorPropertyType.EEPT_GTransform)]
        public GTransform GTransform
        {
            get { return mGTransform; }
            set { mGTransform = value; }
        }
        [EditorProperty("获取Trans", EditorPropertyType.EEPT_GraphValue)]
        public GraphEvent_NoValue_Vector3 EventNoValueVector3
        {
            get { return mEventNoValueVector3; }
            set { mEventNoValueVector3 = value; }
        }
        #endregion
        
        public int GetEvenType() => (int)EEvenType.EET_ActionDebug;
        public IActionEventData Creact() => new Event_ActionDebug();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            #if UNITY_EDITOR 
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            if (_isSingle)
            {
                if (mUnitPos.m_IsSet)
                {
                    mUnitPos.Init(_stateMachine);
                    if (mUnitPos.m_Value.IsValid)
                    {
                        Vector3 _pos = mUnitPos.m_Value.value.transform.position;
                        EngineScenceDraw.Sphere(_pos, Quaternion.identity, 1, Color.red, 1);
                        EngineScenceDraw.Line(_stateMachine.CurUnit.transform.position, _pos, Color.red, 1);
                    }
                }

                if (mTransPos.m_IsSet)
                {
                    mTransPos.Init(_stateMachine);
                    if (mTransPos.m_Value.isValid)
                    {
                        Vector3 _pos = mTransPos.m_Value.value.position;
                        EngineScenceDraw.Sphere(mTransPos.m_Value.value.position,Quaternion.identity, 0.8f,Color.green,1);
                        EngineScenceDraw.Line(_stateMachine.CurUnit.transform.position,_pos, Color.green, 1);
                    }
                    else
                    {
                        EngineDebug.Log("未获取");
                    }
                }

                if (mPointPos.m_IsSet)
                {
                    mPointPos.Init(_stateMachine);
                    if (mPointPos.m_Value.IsValid)
                    {
                        Vector3 _pos = mPointPos.m_Value.value.pos;
                        EngineScenceDraw.Sphere(mPointPos.m_Value.value.pos, Quaternion.identity, 0.6f, Color.blue, 1);
                        EngineScenceDraw.Line(_stateMachine.CurUnit.transform.position, _pos, Color.blue, 1);
                    }
                }
            }
            if (mGintTest4.GValue_RatioPart.Count > 0)
            {
                if (mGintTest4.CheckValue(_stateMachine))
                {
                    EngineDebug.LogWarning("通过判断");
                }
                else
                {
                    EngineDebug.LogWarning("未通过判断");
                }
            }
            
            return;
            mGTransform.Init(_stateMachine);
            mGFloatTest2.Init(_stateMachine);

            // if(mGTransform.)
            // EngineDebug.Log(mGTransform.value.name);
            string _eventDebug = "";
            for (int i = 0; i < _actionState.CurrentActionEvents.Count; i++)
            {
                ActionEvent _actionEvent = _actionState.CurrentActionEvents[i];
                _eventDebug += $"  {i}、 {_actionEvent.EventData.GetType().Name}  D:{_actionEvent.Duration}\n";
            }

            string _interruptDebug = "";
            for (int i = 0; i < _actionState.CurActionInterrupt.Count; i++)
            {
                ActionInterrupt _actionInterrupt = _actionState.CurActionInterrupt[i];
                _interruptDebug += $"  {i}、 {_actionInterrupt.ActionName}\n";
            }

            string _actionLable = "";
            List<int> _ActionLableList = _actionState.ActionStateMachine.GetActionLableList;
            for (int i = 0; i < _ActionLableList.Count; i++)
            {
                _actionLable += $"{i}: {_actionState.ActionStateMachine.GetActionLable(_ActionLableList[i])}\n";
            }

            
            EngineDebug.Log("mGFloatTest2: " + mGFloatTest2.value);
            
            EngineDebug.Log(
                "ActionDebug" +
                $"<color=#FFCC00>{mDebugName}</color>\n" +
                $"Debug信息来自 {_actionState.mCurActionLayer} 层级\n" +
                $"Action状态层为 {_actionState.GetActionType()} \n" +
                $"当前层级时间 {_actionState.ElapsedTime} ms\n" +
                $"事件数量: {_actionState.CurrentActionEvents.Count} \n" + 
                _eventDebug + "\n" + 
                $"打断轨数量: {_actionState.CurActionInterrupt.Count}\n" + 
                _interruptDebug + "\n" + 
                $"当前状态标签数量：{_ActionLableList.Count}\n" + 
                _actionLable + "\n\n"
            );


            #endif
        }

        public void Update(ActionStatePart _actionState, ActionMachineTime _actionTime)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;

            if (mUnitPos.m_IsSet)
            {
                mUnitPos.Init(_stateMachine);
                if (mUnitPos.m_Value.IsValid)
                {
                    Vector3 _pos = mUnitPos.m_Value.value.transform.position;
                    EngineScenceDraw.Sphere(_pos, Quaternion.identity, 1, Color.red);
                    EngineScenceDraw.Line(_stateMachine.CurUnit.transform.position,_pos, Color.red);
                }
            }

            if (mTransPos.m_IsSet)
            {
                mTransPos.Init(_stateMachine);
                if (mTransPos.m_Value.isValid)
                {
                    Vector3 _pos = mTransPos.m_Value.value.position;
                    EngineScenceDraw.Sphere(mTransPos.m_Value.value.position,Quaternion.identity, 0.8f,Color.green);
                    EngineScenceDraw.Line(_stateMachine.CurUnit.transform.position,_pos, Color.green);
                }
                else
                {
                    EngineDebug.Log("未获取");
                }
            }

            if (mPointPos.m_IsSet)
            {
                mPointPos.Init(_stateMachine);
                if (mPointPos.m_Value.IsValid)
                {
                    Vector3 _pos = mPointPos.m_Value.value.pos;
                    EngineScenceDraw.Sphere(mPointPos.m_Value.value.pos, Quaternion.identity, 0.6f, Color.blue);
                    EngineScenceDraw.Line(_stateMachine.CurUnit.transform.position, _pos, Color.blue);
                }
            }
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_ActionDebug actionDebug = _eventData as Event_ActionDebug;
            actionDebug.DebugName = mDebugName;
            actionDebug.Enumname = mEnumname;
            actionDebug.GFloatTest2 = (GFloat)mGFloatTest2.Clone();
            actionDebug.mGintTest4 = mGintTest4.Clone();
            actionDebug.mGintTest3 = mGintTest3.Clone();
            actionDebug.mTransPos = mTransPos.Clone();
            actionDebug.mUnitPos = mUnitPos.Clone();
            actionDebug.mPointPos = mPointPos.Clone();
            actionDebug.EventNoValueVector3 = (GraphEvent_NoValue_Vector3)mEventNoValueVector3.Clone();
            return actionDebug;
        }
    }
}
using UnityEngine;

namespace AsiActionEngine.RunTime.Graph
{
    //蓝图类型，以输出的变量类型分类
    [System.Serializable] public abstract class BluePrint_Vector3 : BluePrint_Value
    {
        public abstract Vector3 value { get; }
    }
    // [System.Serializable] public abstract class BluePrint_Quaternion : BluePrint_Value
    // {
    //     public abstract Quaternion value { get; }
    // }
    [System.Serializable] public abstract class BluePrint_Int : BluePrint_Value
    {
        public abstract int value { get; }
    }
    [System.Serializable] public abstract class BluePrint_Float : BluePrint_Value
    {
        public abstract float value { get; }
    }
    [System.Serializable] public abstract class BluePrint_Bool : BluePrint_Value
    {
        public abstract bool value { get; }
    }
    [System.Serializable] public abstract class BluePrint_Unit : BluePrint_Value
    {
        public abstract Unit value { get; }
    }
    [System.Serializable] public abstract class BluePrint_PointData : BluePrint_Value
    {
        public abstract PointData value { get; }
    }
    [System.Serializable] public abstract class BluePrint_Transform : BluePrint_Value
    {
        public abstract Transform value { get; }
    }
    
    //GValue
    [System.Serializable] public abstract class BluePrint_GPoint : BluePrint_Value
    {
        public abstract GPoint value(ActionStatePart part, ActionMachineTime _time);
    }
    [System.Serializable] public abstract class BluePrint_GInt : BluePrint_Value
    {
        public abstract GInt value(ActionStatePart part, ActionMachineTime _time);
    }
    [System.Serializable] public abstract class BluePrint_GFloat : BluePrint_Value
    {
        public abstract GFloat value(ActionStatePart part, ActionMachineTime _time);
    }
    
    //蓝图类型，输出的值为常量的类型
    [System.Serializable] public abstract class BluePrint_Value : IProperty
    {
        public bool IsNode = false;

        /// <summary>
        /// 仅在当前帧第一次获取参数时执行一次, 避免因多次调用引起重复运算
        /// </summary>
        /// <param name="part"></param>
        /// <param name="_time"></param>
        public abstract void Init(ActionStatePart part, ActionMachineTime _time);
        public abstract BluePrint_Value Clone();
    }
}
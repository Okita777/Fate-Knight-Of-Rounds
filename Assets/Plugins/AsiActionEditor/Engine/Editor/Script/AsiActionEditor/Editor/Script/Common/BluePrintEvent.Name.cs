using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.Graph;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class BluePrintEvent
    {
        public static string DisName(string bluePrintName)
        {
            switch (bluePrintName)
            {
                //返回常数的蓝图
                case nameof(GraphEvent_Value_Vector3):
                    return "三维向量 (Vector3)";
                case nameof(GraphEvent_Value_Float):
                    return "浮点数 (Float)";
                case nameof(GraphEvent_Value_Int):
                    return "整数 (Int)";
                case nameof(GraphEvent_Value_Transform):
                    return "角色挂点 (Transform)";
                case nameof(GraphEvent_GValue_Bool):
                    return "布尔 (Bool)";
                case nameof(GraphEvent_TrackData_DeltaTime):
                    return "DeltaTime (秒)";
                case nameof(GraphEvent_TrackData_TrackEventTime):
                    return "当前轨道时间 (秒)";
                
                //GValue值
                case nameof(GraphEvent_GValue_GInt):
                    return "整数 (GInt)";
                case nameof(GraphEvent_GValue_GFloat):
                    return "浮点数 (GFloat)";
                case nameof(GraphEvent_GValue_GTransform):
                    return "变换 (GTransform)";
                case nameof(GraphEvent_GValue_GPoint):
                    return "点数据 (GPoint)";
                case nameof(GraphEvent_GValue_GUnit):
                    return "单位 (GUnit)";
                
                //计算用的蓝图
                case nameof(GraphEvent_Math_Add_Vector3):
                    return "数学运算/加 (Vector3)";
                case nameof(GraphEvent_Math_Sub_Vector3):
                    return "数学运算/减 (Vector3)";
                case nameof(GraphEvent_Math_Mul_Vector3):
                    return "数学运算/乘 (Vector3)";
                case nameof(GraphEvent_Math_Div_Vector3):
                    return "数学运算/除 (Vector3)";
                
                case nameof(GraphEvent_Math_Add_Int):
                    return "数学运算/加 (Int)";
                case nameof(GraphEvent_Math_Sub_Int):
                    return "数学运算/减 (Int)";
                case nameof(GraphEvent_Math_Mul_Int):
                    return "数学运算/乘 (Int)";
                case nameof(GraphEvent_Math_Div_Int):
                    return "数学运算/除 (Int)";
                
                case nameof(GraphEvent_Math_Add_Float):
                    return "数学运算/加 (Float)";
                case nameof(GraphEvent_Math_Sub_Float):
                    return "数学运算/减 (Float)";
                case nameof(GraphEvent_Math_Mul_Float):
                    return "数学运算/乘 (Float)";
                case nameof(GraphEvent_Math_Div_Float):
                    return "数学运算/除 (Float)";
                
                case nameof(GraphEvent_Math_Comp_Bool):
                    return "对比 (Bool)";
                case nameof(GraphEvent_Math_Comp_Float):
                    return "对比 (Float)";
                case nameof(GraphEvent_Math_Comp_Int):
                    return "对比 (Int)";
                
                case nameof(GraphEvent_Math_NotBool):
                    return "NotBool";
                case nameof(GraphEvent_Math_QuaLook):
                    return "注视 (Euler)";
                
                case nameof(GraphEvent_Math_Select_Float):
                    return "选择 (Float)";
                case nameof(GraphEvent_Math_Select_Int):
                    return "选择 (Int)";
                case nameof(GraphEvent_Math_Select_Vector3):
                    return  "选择 (Vector3)";
                case nameof(GraphEvent_Math_Select_Point):
                    return  "选择 (Point)";
                
                //其它
                case nameof(GraphEvent_Make_Vector3):
                    return "创建 Vector3";
                case nameof(GraphEvent_Make_Point):
                    return "创建 Point";
                case nameof(GraphEvent_Breack_Vector3):
                    return "拆分 Vector3";
                case nameof(GraphEvent_Breack_Transform):
                    return "拆分 Transform";
                case nameof(GraphEvent_Breack_Point):
                    return "拆分 Point";
            }
            string bluePrintName_ = ActionWindowMain.ActionEditorFuntion.BluePrint_Name(bluePrintName);
            if (!string.IsNullOrEmpty(bluePrintName_)) return bluePrintName_;
            // EngineDebug.LogError($"节点名: {bluePrintName}");
            string[] _bluePrintNameArray = bluePrintName.Split('_');
            string _returnName = _bluePrintNameArray[^1];
            for (int i = 3; i < _bluePrintNameArray.Length; i++)
            {
                _returnName += "_" + _bluePrintNameArray[i];
            }
            return _returnName;
        }
    }
}
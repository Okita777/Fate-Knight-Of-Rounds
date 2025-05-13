using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.Graph;

namespace AsiActionEngine.Editor
{
    public partial class BluePrintEvent
    {
        public static IProperty Creact(string bluePrintName)
        {
            switch (bluePrintName)
            {
                //返回常数的蓝图
                case nameof(GraphEvent_Value_Bool):
                    return new GraphEvent_Value_Bool();
                case nameof(GraphEvent_Value_Vector3):
                    return new GraphEvent_Value_Vector3();
                case nameof(GraphEvent_Value_Float):
                    return new GraphEvent_Value_Float();
                case nameof(GraphEvent_Value_Int):
                    return new GraphEvent_Value_Int();
                case nameof(GraphEvent_Value_Transform):
                    return new GraphEvent_Value_Transform();
                case nameof(GraphEvent_GValue_Bool):
                    return new GraphEvent_GValue_Bool();
                case nameof(GraphEvent_TrackData_DeltaTime):
                    return new GraphEvent_TrackData_DeltaTime();
                case nameof(GraphEvent_TrackData_TrackEventTime):
                    return new GraphEvent_TrackData_TrackEventTime();
                //返回常数的蓝图 G系列
                case nameof(GraphEvent_GValue_GInt):
                    return new GraphEvent_GValue_GInt();
                case nameof(GraphEvent_GValue_GFloat):
                    return new GraphEvent_GValue_GFloat();
                case nameof(GraphEvent_GValue_GTransform):
                    return new GraphEvent_GValue_GTransform();
                case nameof(GraphEvent_GValue_GPoint):
                    return new GraphEvent_GValue_GPoint();
                case nameof(GraphEvent_GValue_GUnit):
                    return new GraphEvent_GValue_GUnit();
                
                //计算用的蓝图
                case nameof(GraphEvent_Math_Add_Vector3):
                    return new GraphEvent_Math_Add_Vector3();
                case nameof(GraphEvent_Math_Sub_Vector3):
                    return new GraphEvent_Math_Sub_Vector3();
                case nameof(GraphEvent_Math_Mul_Vector3):
                    return new GraphEvent_Math_Mul_Vector3();
                case nameof(GraphEvent_Math_Div_Vector3):
                    return new GraphEvent_Math_Div_Vector3();
                
                case nameof(GraphEvent_Math_Add_Int):
                    return new GraphEvent_Math_Add_Int();
                case nameof(GraphEvent_Math_Sub_Int):
                    return new GraphEvent_Math_Sub_Int();
                case nameof(GraphEvent_Math_Mul_Int):
                    return new GraphEvent_Math_Mul_Int();
                case nameof(GraphEvent_Math_Div_Int):
                    return new GraphEvent_Math_Div_Int();
                
                case nameof(GraphEvent_Math_Add_Float):
                    return new GraphEvent_Math_Add_Float();
                case nameof(GraphEvent_Math_Sub_Float):
                    return new GraphEvent_Math_Sub_Float();
                case nameof(GraphEvent_Math_Mul_Float):
                    return new GraphEvent_Math_Mul_Float();
                case nameof(GraphEvent_Math_Div_Float):
                    return new GraphEvent_Math_Div_Float();
                
                case nameof(GraphEvent_Math_Comp_Bool):
                    return new GraphEvent_Math_Comp_Bool();
                case nameof(GraphEvent_Math_Comp_Float):
                    return new GraphEvent_Math_Comp_Float();
                case nameof(GraphEvent_Math_Comp_Int):
                    return new GraphEvent_Math_Comp_Int();
                
                case nameof(GraphEvent_Math_NotBool):
                    return new GraphEvent_Math_NotBool();
                case nameof(GraphEvent_Math_QuaLook):
                    return new GraphEvent_Math_QuaLook();
                
                case nameof(GraphEvent_Math_Select_Float):
                    return new GraphEvent_Math_Select_Float();
                case nameof(GraphEvent_Math_Select_Int):
                    return new GraphEvent_Math_Select_Int();
                case nameof(GraphEvent_Math_Select_Vector3):
                    return new GraphEvent_Math_Select_Vector3();
                case nameof(GraphEvent_Math_Select_Point):
                    return new GraphEvent_Math_Select_Point();
                
                //其它
                case nameof(GraphEvent_Make_Vector3):
                    return new GraphEvent_Make_Vector3();
                case nameof(GraphEvent_Make_Point):
                    return new GraphEvent_Make_Point();
                case nameof(GraphEvent_Breack_Vector3):
                    return new GraphEvent_Breack_Vector3();
                case nameof(GraphEvent_Breack_Transform):
                    return new GraphEvent_Breack_Transform();
                case nameof(GraphEvent_Breack_Point):
                    return new GraphEvent_Breack_Point();
                case nameof(GraphEvent_BValue_Point):
                    return new GraphEvent_BValue_Point();
                case nameof(GraphEvent_BValue_Transform):
                    return new GraphEvent_BValue_Transform();
            }
            return ActionWindowMain.ActionEditorFuntion.BluePrint_Create(bluePrintName);
        }

        public readonly string[] GetDefaultValueNode = new[]
        {
            "Value",
            "Math/Add",
            "Math/Sub",
            "Math/Mul",
            "Math/Div",
        };
    }
}
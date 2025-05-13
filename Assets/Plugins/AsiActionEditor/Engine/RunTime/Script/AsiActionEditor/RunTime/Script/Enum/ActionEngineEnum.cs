using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public enum EEvenTypeInternal 
    { 
        EET_InValid,
        
        //动画状态相关

        
        // 角色行为相关 
        [Description("环境检测/射线检测")] EET_RayCast,


        // 动画跳转事件
        [Description("动画跳转")] EET_Interrupt,
        [Description("动画跳转[仅结束时跳转]")] EET_Interrupt_E,
        
        //道具交互
        
        // 杂项
        [Description("SetGvalue(蓝图)/Int")] EET_SetGInt,
        [Description("SetGvalue(蓝图)/Float")] EET_SetGFloat,
        [Description("SetGvalue(蓝图)/Point")] EET_SetGPoint, 

        //单位相关
        [Description("攻击盒")] EET_AttackBox,

        //单位状态

        
        //不希望能在轨道配置的事件
        [Description("动画播放")] EET_DTD_PlayAnim,
    }

    public enum EInputKeyType
    {
        [Description("按下")] OnDown,
        [Description("松开")] OnUp,
        [Description("点击")] OnClick,
        [Description("长按")] OnHold,
        [Description("按键状态_按下")] Down_State,
        [Description("按键状态_抬起")] Up_State,
    }

    //Animator临时解决方案
    public enum EAnimLayerType
    {
        [Description("动画主要层级")] BaseLayer,
        [Description("单个肢体层级")] LimbLayer,
        [Description("瞄准偏移层级")] UpperLayer,
        [Description("抖动叠加层级")] NoiseLayer,
        [Description("程序逻辑层级")] ScriptLayer
    }

    public enum EInterruptTypeInternal 
    {
        EIT_InValid,
        [Description("玩家按键检测")] EIT_CheckInput,
        [Description("玩家移动输入检测")] EIT_CheckMove,
        [Description("被命中")] EIT_CheckBeHit,
        [Description("命中对象")] EIT_CheckOnHit,
        [Description("随机权重")] EIT_CheckWeightRange,

    }

    public enum ECharacteLimbType
    {
        //角色肢体
        Root,
        Head,
        Neck,
        Chest,
        Spine2,
        Spine,
        Hips,
        Left_Upper_Leg,
        Left_Lower_Leg,
        Left_Foot,
        Right_Upper_Leg,
        Right_Lower_Leg,
        Right_Foot,
        Left_Shoulder,
        Left_Upper_Arm,
        Left_Lower_Arm,
        Left_Hand,
        Right_Shoulder,
        Right_Upper_Arm,
        Right_Lower_Arm,
        Right_Hand,
        
        //常规道具挂点
        HelpPoint_HUD,
        HelpPoint_WeaponL,
        HelpPoint_WeaponR,
        HelpPoint_WorldL,
        helpPoint_World,
        HelpPoint_WorldR,
        HelpPoint_BehindL,
        HelpPoint_BehindR,
        HelpPoint_WaistL,
        HelpPoint_WaistR,
        
        //相机挂点
        Cam_Main,
        Cam_Look,
        Cam_Ani_A,
        Cam_Ani_B,
        
        //道具挂点
        Weapone_L,
        Weapone_R,
        HUD,
    }

    public enum EGValueType
    {
        // Null,
        GBool,
        GInt,
        GFloat,
        GString,
        GEnum,
        // GColor,
        // GVector2,
        // GVector3,
        // GQuaternion,
        // GUnit,
        GTransform,
        GPoint,
        GUnit
    }

    public enum Ecomp_Int
    {
        [Description("==")]Equal,
        [Description("!=")]NotEqual,
        [Description(">")]Greater,
        [Description("<")]Less,
        [Description(">=")]GreaterEqual,
        [Description("<=")]LessEqual,
    }
    public enum Ecomp_Float
    {
        [Description(">=")]GreaterEqual,
        [Description("<=")]LessEqual,
    }
    public enum Ecomp_Bool
    {
        [Description("==")]Equal,
        [Description("!=")]NotEqual,
    }

    public class EnumNames
    {
        public static string[] Ecomp_Int = new[] { "==", "!=", ">", "<", ">=", "<=" };
        public static string[] Ecomp_Float = new[] { ">=", "<=" };
        public static string[] Ecomp_Bool = new[] { "==", "!=" };
    }
}

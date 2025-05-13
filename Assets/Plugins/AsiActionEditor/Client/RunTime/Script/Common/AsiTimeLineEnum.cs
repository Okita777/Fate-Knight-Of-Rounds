using System.ComponentModel;

namespace AsiTimeLine.RunTime
{
    public enum EConditionType
    {
        [Description("检测玩家是否在地面")] EIT_CheckGround,
        [Description("检测玩家和地面距离")] EIT_CheckHeight,
        [Description("检测玩家前方障碍")] EIT_CheckBarrier,
        // [Description("检查当前找到的交互点")] EIT_CheckFindPoint,
        
        [Description("检查目标位置或者角度/Transform")] EIT_CheckTransfrom_RotAndPosOffset,
        
        [Description("检测玩家输入方向和角色方向差值")] EIT_CheckInputToTranDir,
        [Description("检测单位行为状态")] EIT_CheckActionState,
        [Description("检测单位行为标签")] EIT_CheckActionLable,
        [Description("检查输入方向")] EIT_CheckInputDir,
        [Description("检查方向差")] EIT_CheckDirOffset,
        
        [Description("检测GValue")] EIT_CheckGValue,
        [Description("检测半径内的对象")] EIT_CheckRadiusObj,
    }
    
    public enum EEvenType 
    { 
        //相机切换
        [Description("相机跳转")] EET_CameraChange,
        [Description("相机抖动")] EET_CameraShake,
        
        //在场景中寻找目标
        [Description("按范围检测对象/球形")] EET_Event_FindTargetToSphere,
        
        //动画状态相关
        [Description("设置Animator的Float参数")] EET_SetAnimFloat,
        [Description("设置角色对齐到障碍交互点位")] EET_InteractBarrier,
        [Description("移动角色到钩锁交互点")] EET_MoveToHookPoint,
        [Description("瞄准偏移")] EET_TargetingMove,
        [Description("双骨骼IK")] EET_TowBoneIK,
        [Description("设置点数据")] EET_SetPointData,

        // 角色行为相关 
        [Description("音效")] EET_Audio,
        [Description("特效")] EET_Partocle,
        
        //道具交互
        [Description("对象附加")] EET_Attach,
        [Description("交互点类型")] EET_FindPoint,
        
        // 杂项
        [Description("Debug")] EET_ActionDebug,
        [Description("SetGValue")] EET_SetGValue,
        [Description("SetGTransform")] EET_SetGvalue_Transform,
        [Description("射线检测")] EET_RaycastHit,

        //单位相关
        // [Description("角色位移")] EET_CharacterMove,
        [Description("角色重力")] EET_CharacterGravity,
        [Description("角色力度施加")] EET_CharacterAddForce,
        [Description("角色仅位移")] EET_CharacterOnMove,
        [Description("单位朝向")] EET_UnitRot,
        [Description("软锁定")] EET_SoftLock,
        [Description("Root权重")] EET_RootWeight,
        [Description("设置命中盒")] EET_SetHitBoxs,
        [Description("时间缩放")] EET_TimeScale,
        [Description("设置层级")] EET_ChangeUnitLayer,

        //ActionLable
        [Description("延迟标签")] EET_Lable_Delay,
        [Description("单位状态标签")] EET_Lable,
        
        //2D控制器
        [Description("2D角色位移")] EET_2D_Move,
        [Description("2D角色重力")] EET_2D_Gravity,
        

    }
}
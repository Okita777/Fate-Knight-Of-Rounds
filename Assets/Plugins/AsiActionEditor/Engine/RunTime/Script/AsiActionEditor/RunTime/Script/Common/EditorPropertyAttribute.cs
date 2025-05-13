using System;
using System.Diagnostics;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    public enum EditorPropertyType
    {
        EEPT_Bool = 1,
        EEPT_Int,
        EEPT_Float,
        EEPT_String,
        EEPT_Vector2,
        EEPT_Vector3,
        EEPT_Vector4,
        EEPT_Color,
        EEPT_Quaternion,
        EEPT_Enum,
        EEPT_Unit,
        EEPT_EnumToActionConfig,
        EEPT_LayerMask,
        EEPT_Object,
        EEPT_AnimationCurve,

        EEPT_GameObject,

        //下拉列表
        EEPT_AnimatorState,
        EEPT_AnimatorParam,
        EEPT_CustomProperty,
        EEPT_Action,
        EEPT_ActionLable,
        EEPT_CharacteLimbType,
        EEPT_Camera,
        EEPT_SetGBool,
        EEPT_SetGInt,
        EEPT_SetGFloat,
        EEPT_SetGString,
        EEPT_SetGPoint,
        EEPT_SetGTransform,
        EEPT_SetGUnit,
        EEPT_GTransform,
        EEPT_GPoint,
        EEPT_SelectTransform,
        
        //列表
        EEPT_List,
        EEPT_GameObjectList,
        EEPT_GValueSetting,
        EEPT_GValueSRatio,
        EEPT_GraphValue
    }

    public enum EditorGraphPropertyType
    {
        EEPT_Bool = 1,
        EEPT_Int,
        EEPT_Float,
        EEPT_String,
        EEPT_Vector3,
        EEPT_Transform,
        EEPT_CharacteLimbType,
        EEPT_PointData,
        EEPT_GBool,
        EEPT_GInt,
        EEPT_GFloat,
        EEPT_GString,
        EEPT_GVector3,
        EEPT_GUnit,
        EEPT_GTransform,
        EEPT_GPoint,
        EEPT_Enum,
        EEPT_EnumCustom,
    }
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class EditorGraphPropertyAttribute : Attribute
    {
        private string mPropertyName;
        private EditorGraphPropertyType mPropertyType;
        private bool mIsInput;
        private string mDescription;
        private string[] mEnumNames;
        private float mLabelWidth;

        /// <summary>
        /// 控制柄的绘制
        /// </summary>
        /// <param name="name">控制柄描述</param>
        /// <param name="isInput">是否为输入端</param>
        /// <param name="type">控制柄参数类型</param>
        public EditorGraphPropertyAttribute(string name, bool isInput, EditorGraphPropertyType type)
        {
            mPropertyName = name;
            mPropertyType = type;
            mIsInput = isInput;
            mLabelWidth = 50;
        }

        #region Property
        public string PropertyName
        {
            get { return mPropertyName; }
            set { mPropertyName = value; }
        }
        public EditorGraphPropertyType PropertyType
        {
            get { return mPropertyType; }
            set { mPropertyType = value; }
        }
        public bool IsInput
        {
            get { return mIsInput; }
            set { mIsInput = value; }
        }
        public string Tooltip
        {
            get { return mDescription; }
            set { mDescription = value; }
        }

        public string[] EnumNames
        {
            get { return mEnumNames; }
            set
            {
                // EngineDebug.LogWarning("设置枚举");
                mEnumNames = value;
            }
        }
        public float LabelWidth
        {
            get { return mLabelWidth; }
            set { mLabelWidth = value; }
        }
        #endregion
    }
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class EditorPropertyAttribute : Attribute
    {
        private string mPropertyName;
        private EditorPropertyType mPropertyType;
        private bool mEdit;
        private string mDescription;
        private string[] mEnumNames;
        private float mLabelWidth;

        public EditorPropertyAttribute(string name, EditorPropertyType type)
        {
            mPropertyName = name;
            mPropertyType = type;
            mEdit = true;
            mLabelWidth = 100;
        }
        
        #region Property
        public string PropertyName
        {
            get { return mPropertyName; }
            set { mPropertyName = value; }
        }
        public EditorPropertyType PropertyType
        {
            get { return mPropertyType; }
            set { mPropertyType = value; }
        }
        public bool Edit
        {
            get { return mEdit; }
            set { mEdit = value; }
        }
        public string Tooltip
        {
            get { return mDescription; }
            set { mDescription = value; }
        }

        public string[] EnumNames
        {
            get { return mEnumNames; }
            set
            {
                // EngineDebug.LogWarning("设置枚举");
                mEnumNames = value;
            }
        }
        public float LabelWidth
        {
            get { return mLabelWidth; }
            set { mLabelWidth = value; }
        }
        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.GValueEquation;
using NUnit.Framework;
// using NUnit.Framework;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public struct SEnumName
    {
        [SerializeField] public List<string> names;
    }
    
    [System.Serializable]
    public class EditorEngineGValue
    {
        public EditorEngineGValuePart[] part;
        public List<int> RemoveIDs = new List<int>();
        public List<SEnumName> EnumNames = new List<SEnumName>();
        public List<SEnumName> TransNames = new List<SEnumName>();
        public List<SEnumName> PointNames = new List<SEnumName>();
        public List<SEnumName> UnitNames = new List<SEnumName>();
        public List<EditorGValueEquation> Equations = new List<EditorGValueEquation>();
        public EngineGValue GetEngineGValue()
        {
            EngineGValue _value = new EngineGValue();

            List<bool> mEngineBool = new List<bool>();
            List<int> mEngineInt = new List<int>();
            List<float> mEngineFloat = new List<float>();
            List<string>  mEngineString = new List<string>();
            List<byte>  mEngineEnum = new List<byte>();
            List<byte>  mEnginePoint = new List<byte>();
            List<byte>  mEngineTransform = new List<byte>();
            List<byte>  mEngineUnit = new List<byte>();
            List<EditorGValueEquation> m_Editor_Parts = new List<EditorGValueEquation>();

            List<EditorEngineGValuePart> parta = part.ToList();
            parta.Sort((x, y) => { return x.IndexID.CompareTo(y.IndexID); });
            foreach (EditorEngineGValuePart part in parta)
            {
                switch (part.ValueType)
                {
                    case EGValueType.GBool:
                        mEngineBool.Add(((GVS_Bool)part.DefaultValue).value);
                        break;
                    case EGValueType.GInt:
                        mEngineInt.Add(((GVS_Int)part.DefaultValue).value);
                        break;
                    case EGValueType.GFloat:
                        mEngineFloat.Add(((GVS_Float)part.DefaultValue).value);
                        break;
                    case EGValueType.GString:
                        mEngineString.Add(((GVS_String)part.DefaultValue).value);
                        break;
                    case EGValueType.GEnum:
                        mEngineEnum.Add(((GVS_Enum)part.DefaultValue).value);
                        break;
                    case EGValueType.GPoint:
                        mEnginePoint.Add(((GVS_Enum)part.DefaultValue).value);
                        break;
                    case EGValueType.GTransform:
                        mEngineTransform.Add(((GVS_Enum)part.DefaultValue).value);
                        break;
                    case EGValueType.GUnit:
                        mEngineUnit.Add(((GVS_Enum)part.DefaultValue).value);
                        break;
                }
            }
            
            //保存RunTime公式
            m_Editor_Parts.AddRange(Equations);
            m_Editor_Parts.Sort((x,y)=>{return x.m_RealID.CompareTo(y.m_RealID);});
            
            List<GValueEquation_FloatPart> _RuntimeData = new List<GValueEquation_FloatPart>();
            foreach (var VARIABLE in m_Editor_Parts)
            {
                _RuntimeData.Add(VARIABLE.GetRunTimeData());
            }
            
            _value.mEngineBool = mEngineBool.ToArray();
            _value.mEngineInt = mEngineInt.ToArray();
            _value.mEngineFloat = mEngineFloat.ToArray();
            _value.mEngineString = mEngineString.ToArray();
            _value.mEngineEnum = mEngineEnum.ToArray();
            _value.mEnginePointData = mEnginePoint.ToArray();
            _value.mEngineTransform = mEngineTransform.ToArray();
            _value.mEngineUnit = mEngineUnit.ToArray();
            _value.mGValueEquation = _RuntimeData.ToArray();

            return _value;
        }
    }
    
    [System.Serializable]
    public class EditorEngineGValuePart : IProperty
    {
        public string Name;//编辑器显示的名字
        public int Index;//实际ID
        public EGValueType ValueType;
        public int IndexID;//编辑器排序的ID
        public int ValueID;//同类型变量里面的所处序号
        public bool IsDefaultValue;
        [SerializeReference] public GVS DefaultValue;
    }
}
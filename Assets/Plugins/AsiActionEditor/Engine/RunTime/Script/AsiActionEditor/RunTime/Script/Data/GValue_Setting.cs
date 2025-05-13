using System.Collections.Generic;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class GValue_Setting
    {
        [UnityEngine.SerializeField] public List<GValue_SettingPar> parameter = new List<GValue_SettingPar>();
        [UnityEngine.SerializeField] public bool isOpen = true;

         public GValue_Setting Clone()
         {
#if UNITY_EDITOR
             GValue_Setting setting = new GValue_Setting();
             setting.parameter = new List<GValue_SettingPar>();
             foreach (GValue_SettingPar p in parameter)setting.parameter.Add(p.Clone());
             setting.isOpen = false;
             return setting;
#endif
             return this;
         }

         public void OnSet(ActionStateMachine _actionStateMachine)
         {
             foreach (var GVS in parameter)
             {
                 int _index = GVS.GValueIndexID;
                 int _m_index = 0;
                 switch (GVS.gValueType)
                 {
                     case EGValueType.GBool:
                         _actionStateMachine.GValue.mEngineBool[_index] = ((GVS_Bool)GVS.GValueValue).value;
                         break;
                     case EGValueType.GInt:
                         _actionStateMachine.GValue.mEngineInt[_index] = ((GVS_Int)GVS.GValueValue).value;
                         break;
                     case EGValueType.GFloat:
                         _actionStateMachine.GValue.mEngineFloat[_index] = ((GVS_Float)GVS.GValueValue).value;
                         break;
                     case EGValueType.GString:
                         _actionStateMachine.GValue.mEngineString[_index] = ((GVS_String)GVS.GValueValue).value;
                         break;
                     case EGValueType.GEnum:
                         _actionStateMachine.GValue.mEngineEnum[_index] = ((GVS_Enum)GVS.GValueValue).value;
                         break;
                     case EGValueType.GTransform:
                         _m_index = _actionStateMachine.GValue.mEngineTransform[_index];
                         _m_index += ((GVS_Enum)GVS.GValueValue).value * 1000;
                         _actionStateMachine.RemoveTransform(_m_index);
                         break;
                     case EGValueType.GUnit:
                         _m_index = _actionStateMachine.GValue.mEngineTransform[_index];
                         _m_index += ((GVS_Enum)GVS.GValueValue).value * 1000;
                         _actionStateMachine.RemoveUnit(_m_index);
                         break;
                     case EGValueType.GPoint:
                         _m_index = _actionStateMachine.GValue.mEngineTransform[_index];
                         _m_index += ((GVS_Enum)GVS.GValueValue).value * 1000;
                         _actionStateMachine.RemovePoint(_m_index);
                         break;
                 }
             }
         }
    }

    [System.Serializable]
    public class GValue_SettingPar
    {
        public GValue_SettingPar(EGValueType _egValueType, GVS _gValue)
        {
            gValueType = _egValueType;
            GValueValue = _gValue;
        }
        [UnityEngine.SerializeField] public EGValueType gValueType;
        [UnityEngine.SerializeField] public ushort GValueIndexID;
        [SerializeReference] public GVS GValueValue;

        public GValue_SettingPar Clone()
        {
            GValue_SettingPar setting = new GValue_SettingPar(gValueType,GValueValue);
            setting.GValueIndexID = GValueIndexID;
            return setting;
        }
    }

    [System.Serializable]
    public class GVS_Bool : GVS
    {
        public GVS_Bool(bool _value = false)
        {
            value = _value;
        }
        [UnityEngine.SerializeField] public bool value;
        public override GVS Clone()
        {
            return new GVS_Bool(value);
        }
    }
    [System.Serializable]
    public class GVS_Int : GVS
    {
        public GVS_Int(int _value = 0)
        {
            value = _value;
        }
        [UnityEngine.SerializeField] public int value = 0;
        public override GVS Clone()
        {
            return new GVS_Int(value);
        }
    }

    [System.Serializable]
    public class GVS_Float : GVS
    {
        public GVS_Float(float _value = 0.0f)
        {
            value = _value;
        }
        [UnityEngine.SerializeField] public float value = 0.0f;
        public override GVS Clone()
        {
            return new GVS_Float(value);
        }
    }
    
    [System.Serializable]
    public class GVS_String : GVS
    {
        public GVS_String(string _value = "")
        {
            value = _value;
        }
        [UnityEngine.SerializeField] public string value;
        public override GVS Clone()
        {
            return new GVS_String(value);
        }
    }
    
    [System.Serializable]
    public class GVS_Enum : GVS
    {
        public GVS_Enum(byte _value = 0)
        {
            value = _value;
        }
        [UnityEngine.SerializeField] public byte value;
        public override GVS Clone()
        {
            return new GVS_Enum(value);
        }
    }

    [System.Serializable]
    public abstract class GVS
    {
        public abstract GVS Clone();
    }
}
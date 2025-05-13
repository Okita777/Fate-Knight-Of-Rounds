using System;
using UnityEngine;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class SelectTransform
    {
        [SerializeField] public bool m_IsCharacterLimb;
        [SerializeField] public ushort m_Index;
        [SerializeField] public byte m_Value;
        [NonSerialized] private ActionStateMachine m_StateMachine = null;
        
        public void Init(ActionStateMachine _actionStateMachine)
        {
            m_StateMachine = _actionStateMachine;
            // EngineDebug.Log("设置过了啊");
        }

        public bool IsValid()
        {
#if UNITY_EDITOR
            if (m_StateMachine is null)
            {
                EngineDebug.LogError("SelectTransform 未初始化");
                return false;
            }
#endif
            return Get() is not null;
        }
        public bool IsValid(CharacterConfig character)
        {
            return Get(character) is not null;
        }
        public Transform Get(CharacterConfig character)
        {
            if (m_IsCharacterLimb)
            {
                if (character.HelpPointDic.TryGetValue((ECharacteLimbType)m_Value, out Transform helpPoint))
                {
                    return helpPoint;
                }
                return null;
            }

            int _index = m_Value;
            _index += m_Index * 1000;
            if (m_StateMachine == null)
            {
                EngineDebug.LogError("Select Transform Error");
                return null;
            }

            return m_StateMachine.GetTransform(_index);
        }

        public Transform Get()
        {
            if (m_IsCharacterLimb)
            {
                if (m_StateMachine.TryGetComponent(out CharacterConfig characterConfig))
                {
                    if (characterConfig.HelpPointDic.TryGetValue((ECharacteLimbType)m_Value, out Transform helpPoint))
                    {
                        return helpPoint;
                    }
                }

                return null;
            }

            int _index = m_Value;
            _index += m_Index * 1000;
            if (m_StateMachine == null)
            {
                EngineDebug.LogError("Select Transform Error");
                return null;
            }
            return m_StateMachine.GetTransform(_index);
        }

        public SelectTransform Clone()
        {
            return this;
        }
    }
}
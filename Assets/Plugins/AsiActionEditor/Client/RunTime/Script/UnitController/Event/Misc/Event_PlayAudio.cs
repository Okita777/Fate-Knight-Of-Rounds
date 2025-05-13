using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    [System.Serializable]
    public class Event_PlayAudio : IActionEventData
    {
        [SerializeField] public byte m_AudioSourceIndex;
        [SerializeField] public float m_AudioVolume = 1.0f;
        [SerializeField] public bool m_CoustomAudio;
        [SerializeField] public byte m_AudioDicID;
        [SerializeField] public byte m_AudioDicChailID;

        public int GetEvenType() => (int)EEvenType.EET_Audio;

        public IActionEventData Creact() => new Event_PlayAudio();

        public void Enter(ActionStatePart _actionState, bool _isSingle)
        {
            ActionStateMachine _stateMachine = _actionState.ActionStateMachine;
            if (_stateMachine.TryGetComponent(out ActionEditor_Audio audio))
            {
                if (m_CoustomAudio)
                {
                    audio.PlayAudio(m_AudioVolume,m_AudioSourceIndex,m_AudioDicID,m_AudioDicChailID);
                }
                else
                {
                    audio.PlayAudio(_stateMachine, m_AudioVolume, m_AudioSourceIndex, m_AudioDicID);
                }
            }
        }

        public IActionEventData Clone(IActionEventData _eventData)
        {
            Event_PlayAudio _event = _eventData as Event_PlayAudio;
            _event.m_AudioSourceIndex = m_AudioSourceIndex;
            _event.m_AudioVolume = m_AudioVolume;
            _event.m_CoustomAudio = m_CoustomAudio;
            _event.m_AudioDicID = m_AudioDicID;
            _event.m_AudioDicChailID = m_AudioDicChailID;
            return _event;
        }
    }
}
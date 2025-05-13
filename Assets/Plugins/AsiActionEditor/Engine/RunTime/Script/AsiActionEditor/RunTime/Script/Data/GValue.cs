using System;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public abstract class GValue
    {
        [UnityEngine.SerializeField] public ushort mValueIndex;
        [UnityEngine.SerializeField] public bool mType;
        [NonSerialized] public ActionStateMachine mStateMachine;

        public void Init(ActionStateMachine _actionState)
        {
            mStateMachine = _actionState;
        }

        public abstract GValue Clone();
    }
}
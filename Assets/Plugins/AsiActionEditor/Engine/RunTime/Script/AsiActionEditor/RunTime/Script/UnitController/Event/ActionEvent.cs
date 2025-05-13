using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class ActionEvent : IProperty
    {
        [SerializeField] protected int mTriggerTime;
        [SerializeField] protected int mDuration;
        [SerializeField] protected bool mForceInit = true;
        [SerializeField] protected bool mInheritable = false;
        [SerializeField] protected bool mEditorInheritable = false;//上下轨是否可继承

        [SerializeReference] private IActionEventData mActionEventData;

        public ActionEvent Clone(IActionEventData _actionEventData = null)//上一帧的
        {
            ActionEvent _ActionEvent = new ActionEvent();
            // ActionEvent _ActionEvent = this;
            _ActionEvent.TriggerTime = mTriggerTime;
            _ActionEvent.Duration = mDuration;
            _ActionEvent.ForceInit = mForceInit;
            _ActionEvent.Inheritable = mInheritable;
            _ActionEvent.EditorInheritable = mEditorInheritable;

            _ActionEvent.mActionEventData = mActionEventData.Clone(_actionEventData);//复制_actionEventData下的参数

            return _ActionEvent;
        }
        
        public ActionEvent CloneTo(ActionEvent _action)//上一帧的
        {
            // ActionEvent _ActionEvent = new ActionEvent();
            // ActionEvent _ActionEvent = this;
            mTriggerTime = _action.TriggerTime;
            mDuration = _action.Duration;
            mForceInit = _action.ForceInit;
            mInheritable = _action.Inheritable;
            mEditorInheritable = _action.mEditorInheritable;

            //mActionEventData = mActionEventData.Clone(_actionEventData);//复制_actionEventData下的参数

            return this;
        }
        
        #region Properties
        public int TriggerTime
        {
            get { return mTriggerTime; }
            set { mTriggerTime = value; }
        }

        public int Duration
        {
            get { return mDuration; }
            set { mDuration = value; }
        }

        public bool Inheritable
        {
            get { return mInheritable; }
            set { mInheritable = value; }
        }

        public bool EditorInheritable
        {
            get { return mEditorInheritable; }
            set { mEditorInheritable = value; }
        }

        public bool ForceInit
        {
            get { return mForceInit; }
            set { mForceInit = value; }
        }

        public IActionEventData EventData
        {
            get { return mActionEventData; }
            set { mActionEventData = value; }
        }
        #endregion
    }
}

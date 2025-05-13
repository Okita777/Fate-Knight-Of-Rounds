using System;
using System.Collections;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.RunTime
{
    /// <summary>
    /// 单位行为
    /// </summary>
    [System.Serializable]
    public class ActionState : IProperty
    {
        [SerializeField] protected int mID = -1;
        [SerializeField] protected int mTotalTime;//总时长
        [SerializeField] protected int mAnimaLayer;
        [SerializeField] protected int mMixTime;
        [SerializeField] protected int mOffsetTime;
        [SerializeField] protected int mActionType;
        [SerializeField] protected string mDefaultAction = String.Empty;
        [SerializeField] protected string mName = string.Empty;
        [SerializeField] protected ActionEvent mAnimEvent;
        [SerializeField] protected List<ActionEvent> mEventList = new List<ActionEvent>();//常规Update下更新的事件
        [SerializeField] protected List<ActionEvent> mEventList_Anim = new List<ActionEvent>();//在动画更新之后更新的事件
        [SerializeField] protected List<ActionInterrupt> mInterruptList_BeHit = new List<ActionInterrupt>();//受击时触发的跳转
        [SerializeField] protected List<ActionInterrupt> mInterruptList_OnHit = new List<ActionInterrupt>();//命中时触发的跳转
        [SerializeField] protected List<ActionInterrupt> mInterruptList = new List<ActionInterrupt>();//常规跳转
        [SerializeField] protected List<ActionInterrupt> mInterruptList_E = new List<ActionInterrupt>();//结束时跳转
        [SerializeField] protected List<ActionInterruptGroup> mInterruptGroupList = new List<ActionInterruptGroup>();//跳转组

        #region Properties
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }

        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        public int TotalTime
        {
            get { return mTotalTime; }
            set { mTotalTime = value; }
        }
        public int AnimaLayer
        {
            get { return mAnimaLayer; }
            set { mAnimaLayer = value; }
        }
        public int MixTime
        {
            get { return mMixTime; }
            set { mMixTime = value; }
        }
        public int OffsetTime
        {
            get { return mOffsetTime; }
            set { mOffsetTime = value; }
        }
        public int ActionLable
        {
            get { return mActionType; }
            set { mActionType = value; }
        }

        public string DefaultAction
        {
            get { return mDefaultAction; }
            set { mDefaultAction = value; }
        }
        public List<ActionEvent> EventList
        {
            get { return mEventList; }
            set { mEventList = value; }
        }
        public List<ActionEvent> EventList_Anim
        {
            get { return mEventList_Anim; }
            set { mEventList_Anim = value; }
        }
        public List<ActionInterrupt> InterruptList_BeHit
        {
            get { return mInterruptList_BeHit; }
            set { mInterruptList_BeHit = value; }
        }
        public List<ActionInterrupt> InterruptList_OnHit
        {
            get { return mInterruptList_OnHit; }
            set { mInterruptList_OnHit = value; }
        }
        public ActionEvent AnimEvent
        {
            get { return mAnimEvent;  }
            set { mAnimEvent = value; }
        }
        public List<ActionInterrupt> InterruptList
        {
            get { return mInterruptList; }
            set { mInterruptList = value; }
        }
        
        public List<ActionInterruptGroup> InterruptGroupList
        {
            get { return mInterruptGroupList; }
            set { mInterruptGroupList = value; }
        }
        #endregion
    }
}

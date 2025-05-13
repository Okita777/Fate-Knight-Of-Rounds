using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace AsiActionEngine.RunTime
{
    public partial class ActionStateMachine
    {
        private readonly int mActionPoolMaxSize = 20;//可继承的Action最大数量
        private readonly int mEventPooklMaxSize = 5;//同一类型中可继承的最大事件数量
        
        private Dictionary<int, EventPool> mEventDic = new Dictionary<int, EventPool>();//Event对象池，不管理回收
        
        private List<ActionEvent> mActionPool = new List<ActionEvent>();//Action对象池，不用管理回收
        private int mActionPoolID = 0;

        private void Init_EventPool()
        {
            for (int i = 0; i < mActionPoolMaxSize; i++)
            {
                mActionPool.Add(new ActionEvent());
            }//新建action对象池
        }

        public ActionEvent CreactAction(ActionEvent _action)
        {
            ActionEvent _returnActionEvent = mActionPool[mActionPoolID].CloneTo(_action);
            
            _returnActionEvent.EventData = CreactEventDataTo(_action.EventData);

            //循环利用
            if (mActionPoolID < mActionPoolMaxSize - 1)
            {
                mActionPoolID++;
            }
            else
            {
                mActionPoolID = 0;
            }
            return _returnActionEvent;
        }

        public IActionEventData CreactEventDataTo(IActionEventData _actionEventData)
        {
            EventPool _eventPool = null;
            if (!mEventDic.TryGetValue(_actionEventData.GetEvenType(), out _eventPool))
            {
                _eventPool = new EventPool(mEventPooklMaxSize, _actionEventData);
                mEventDic.Add(_actionEventData.GetEvenType(), _eventPool);
            }
            return _eventPool.GetEvent(_actionEventData);
        }
    }

    public class EventPool
    {
        private int mPoolID;
        private int mMaxSize;
        private IActionEventData[] mActionEvent;
        
        public EventPool(int _maxSize, IActionEventData _actionEventData)
        {
            mPoolID = 0;
            mMaxSize = _maxSize;
            mActionEvent = new IActionEventData[_maxSize];
            for (int i = 0; i < mActionEvent.Length; i++)
            {
                mActionEvent[i] = _actionEventData.Clone(_actionEventData.Creact());
            }
        }

        public IActionEventData GetEvent(IActionEventData _actionEventData)
        {
            IActionEventData _retrunValue = null;

            mActionEvent[mPoolID] = _actionEventData.Clone(mActionEvent[mPoolID]);
            _retrunValue = mActionEvent[mPoolID];
            

            mPoolID++;
            if (mPoolID < mMaxSize - 1)
            {
                mPoolID++;
            }
            else
            {
                mPoolID = 0;
            }

            return _retrunValue;
        }
    }
}
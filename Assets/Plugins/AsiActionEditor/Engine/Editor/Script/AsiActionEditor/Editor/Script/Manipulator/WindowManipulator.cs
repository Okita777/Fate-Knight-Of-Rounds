using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public delegate bool mouseActionEven(Event _event);

    public delegate bool onDrawEventTrack(Event _event, out EventTrack _eventTrack);

    public class MouseActionInfo
    {
        public mouseActionEven mEvent;
        public int mPriority;
        public MouseActionInfo(mouseActionEven _event, int _priority)
        {
            mEvent = _event;
            mPriority = _priority;
        }
    }

    public class WindowManipulator
    {
        private Dictionary<EMouseEvent, List<MouseActionInfo>> mMouseEvents = new Dictionary<EMouseEvent, List<MouseActionInfo>>();
        private List<onDrawEventTrack> mAllDrawEventTrack = new List<onDrawEventTrack>();
        
        public void Update(Rect _rect, Event _event)
        {
            CheckDragEvent(_event);

            if (ExecuteCallback(_rect, _event))
            {
                _event.Use();
            }

        }

        public void OnLoadMouseEvent(EMouseEvent _eventType, MouseActionInfo _even)
        {
            if(!mMouseEvents.ContainsKey(_eventType))
                mMouseEvents.Add(_eventType,new List<MouseActionInfo>());
            if (!mMouseEvents[_eventType].Contains(_even))
            {
                mMouseEvents[_eventType].Add(_even);
            }
        }
        public void UnLoadMouseEvent(EMouseEvent _eventType, MouseActionInfo _even)
        {
            if (mMouseEvents.TryGetValue(_eventType, out var _eventList))
            {
                _eventList.Remove(_even);
            }
            else
            {
                EngineDebug.LogWarning($"警告 从未申请过 {_eventType.ToString()} 类型");
            }
        }

        public void OnLoadEventTrack(onDrawEventTrack _eventTrack)
        {
            if (!mAllDrawEventTrack.Contains(_eventTrack))
            {
                mAllDrawEventTrack.Add(_eventTrack);
            }
        }
        public void UnLoadEventTrack(onDrawEventTrack _eventTrack)
        {
            if (mAllDrawEventTrack.Contains(_eventTrack))
            {
                mAllDrawEventTrack.Remove(_eventTrack);
            }
            else
            {
                EngineDebug.LogWarning($"警告 尝试卸载不存在的轨道绘制");
            }
        }
        public void Clear()
        {
            mMouseEvents.Clear();
            mAllDrawEventTrack.Clear();
        }

        private bool ExecuteCallback(Rect _rect, Event _event)
        {
            EMouseEvent _mouseEventType = EMouseEvent.Empty;
            EventType _EventType = _event.type;
            if (_EventType == EventType.MouseDown)
            {
                if (_event.button == 0)
                {
                    _mouseEventType = EMouseEvent.MouseDwonLeft;
                }
                else if (_event.button == 1)
                {
                    _mouseEventType = EMouseEvent.MouseDwonRight;
                }
            }
            else if (_EventType == EventType.MouseDrag)
            {
                _mouseEventType = EMouseEvent.MouseDrag;
            }
            else if (_EventType == EventType.MouseUp)
            {
                _mouseEventType = EMouseEvent.MouseUp;
            }

            if (_mouseEventType == EMouseEvent.Empty) return false;
            if (mMouseEvents.TryGetValue(_mouseEventType, out var _list))
            {
                switch (_event.type)
                {
                    case EventType.MouseDown:
                        if (!_rect.Contains(_event.mousePosition))
                            return false;
                        _list.Sort((x, y) => { return -x.mPriority.CompareTo(y.mPriority);});
                        break;
                    case EventType.ContextClick:
                        //Debug.LogWarning("熟读");
                        return true;

                }
                
                for (int i = 0; i < _list.Count; i++)
                {
                    if (_list[i].mEvent.Invoke(_event))
                    {
                        return true;
                    }
                }

                return false;
            }
            return false;
        }

        private void CheckDragEvent(Event _event)
        {
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                string _path = DragAndDrop.paths[0];
                if (!string.IsNullOrEmpty(_path))
                {
                    string _loadName = _path.Split('/')[^1].Split('.')[0];
                    string[] _getName = _loadName.Split('_');
                    if (_getName[0] == "Event")
                    {
                        foreach (var VARIABLE in mAllDrawEventTrack)
                        {
                            if (VARIABLE.Invoke(_event, out EventTrack track))
                            {
                                if (_event.type == EventType.DragExited)
                                {
                                    string _eventName = _getName[1];
                                    for (int i = 2; i < _getName.Length; i++)
                                    {
                                        _eventName += ("_" + _getName[i]);
                                    }
                                    if (ActionWindowMain.GetActionEventID("EET_" + _eventName, out int actionID))
                                    {
                                        track.CreactTrackEvent(actionID, _event.mousePosition.x);
                                    }
                                    else
                                    {
                                        EngineDebug.LogError($"导入事件失败 [{_loadName}]");
                                    }
                                }
                                else
                                {
                                    track.OnDrawSelectRect(_event, 5);
                                    DrawFuntion = () => { };
                                    mDraw = true;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (_event.type == EventType.DragExited)
            {
                //创建事件
            }
        }

        private bool mDraw = false;
        private Action DrawFuntion;
        public void OnDraw()
        {
            if (mDraw)
            {
                DrawFuntion();

                GUI.changed = true;
                mDraw = false;
            }
        }
    }
}
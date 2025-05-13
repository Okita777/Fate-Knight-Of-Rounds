using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class TimeLineWindow
    {
        private const float mTimeLineAnimSpeed = 12f;//TimeLine UI动画过渡速度
        
        private float mGUIAnimLastTime;
        private float mGUIAnimTime;

        private float mRangeMiniLerp;
        private float mRangeMaxLerp;

        private void Update_ShortcutKey(Rect _rect)
        {
            CheckInputKey(Event.current,_rect);
            GUIAnim();
        }

        private bool m_KeyLeftShift = false;
        private bool m_KeyLeftControl = false;

        private void CheckInputKey(Event _event,Rect _rect)
        {
            // if (_event.keyCode == KeyCode.LeftShift)
            // {
            //     GUI.changed = true;
            //     Debug.Log("AA");
            // }
            if (_event.type == EventType.KeyDown)
            {
                if (_event.keyCode == KeyCode.LeftShift)
                {
                    m_KeyLeftShift = true;
                }
                else if (_event.keyCode == KeyCode.LeftControl)
                {
                    m_KeyLeftControl = true;
                }
            }
            else if (_event.type == EventType.KeyUp)
            {
                if (_event.keyCode == KeyCode.LeftShift)
                {
                    m_KeyLeftShift = false;
                }
                else if (_event.keyCode == KeyCode.LeftControl)
                {
                    m_KeyLeftControl = false;
                }
            }
            
            if (!_rect.Contains(_event.mousePosition))
            {
                return;
            }
            
            if (_event.button == 2)
            {
                _event.Use();
            }
            else if (m_KeyLeftShift)
            {
                if (_event.isScrollWheel)
                {
                    SetTimeLineViewX_Pos(HandleUtility.niceMouseDeltaZoom * -0.005f * GetTimeViewScale());
                    _event.Use();
                }
            }
            else if (m_KeyLeftControl)
            {
                if (_event.isScrollWheel)
                {
                    float _zoomSpeed = HandleUtility.niceMouseDeltaZoom * -0.1f;// * GetTimeViewScale()
                    SetTimeLineViewX_Scale(_zoomSpeed, _rect, _event);
                    _event.Use();
                }
            }
        }

        private void SetTimeLineViewX_Pos(float _deltaPos)
        {
            if (_deltaPos < 0 && mRangeMini <= 0 || _deltaPos > 0 && mRangeMax >= 1)
            {
                return;
            }

            mRangeMiniLerp = mRangeMini;
            mRangeMaxLerp = mRangeMax;

            if (mRangeMiniLerp + _deltaPos < 0)
            {
                _deltaPos = -mRangeMiniLerp;
            }else if (mRangeMaxLerp + _deltaPos > 1)
            {
                _deltaPos = 1 - mRangeMaxLerp;
            }
            mRangeMiniLerp += _deltaPos;
            mRangeMaxLerp += _deltaPos;
            SetGUIAnim(0.5f);
            GUI.changed = true;
        }

        private void SetTimeLineViewX_Scale(float _deltaPos, Rect _rect, Event _event)
        {
            mRangeMiniLerp = mRangeMini;
            mRangeMaxLerp = mRangeMax;
            if (_deltaPos < 0)
            {
                if (mRangeMaxLerp - mRangeMiniLerp < 0.02f)
                {
                    return;
                }
            }
            else
            {
                if (mRangeMiniLerp == 0 && mRangeMaxLerp == 0)
                {
                    return;
                }
            }
            
            float _mousePropertion = (_event.mousePosition.x - _rect.x) / _rect.width;//处于时间轴的位置比例
            mRangeMiniLerp -= _deltaPos * _mousePropertion;
            mRangeMaxLerp += _deltaPos * (1 - _mousePropertion);
            mRangeMiniLerp = Mathf.Max(mRangeMiniLerp, 0);
            mRangeMaxLerp = Mathf.Min(1, mRangeMaxLerp);
            
            if (mRangeMaxLerp - mRangeMiniLerp < 0.01f)
            {
                if (mRangeMaxLerp > 0.5f)
                {
                    mRangeMiniLerp = mRangeMaxLerp - 0.01f;
                }
                else
                {
                    mRangeMaxLerp = mRangeMiniLerp + 0.01f;
                }
            }
            SetGUIAnim(0.5f);
            GUI.changed = true;
        }

        private void SetGUIAnim(float _duration)
        {
            mGUIAnimTime = _duration;
            mGUIAnimLastTime = Time.realtimeSinceStartup;
            GUI.changed = true;
        }
        private void GUIAnim()
        {
            if (mGUIAnimTime > 0)
            {
                float _editorDeltaTime = Time.realtimeSinceStartup - mGUIAnimLastTime;
                mGUIAnimTime -= _editorDeltaTime;

                if (mGUIAnimTime <= 0)
                {
                    mRangeMini = mRangeMiniLerp;
                    mRangeMax = mRangeMaxLerp;
                }
                else
                {
                    mRangeMini = Mathf.Lerp(mRangeMini, mRangeMiniLerp, _editorDeltaTime * mTimeLineAnimSpeed);
                    mRangeMax = Mathf.Lerp(mRangeMax, mRangeMaxLerp, _editorDeltaTime * mTimeLineAnimSpeed);
                }
                
                GUI.changed = true;
                mGUIAnimLastTime = Time.realtimeSinceStartup;
            }
        }

        private float GetTimeViewScale()
        {
            return Mathf.Max((mRangeMax - mRangeMini) * 5, 1);
        }
    }
}
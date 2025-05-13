using System.Collections.Generic;
using System.IO;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class TimeLineWindow
    {
        private TimeViewer mTimeViewer = new TimeViewer(60);

        private Rect mTimelineRect;
        private Rect mHeadrInteractRect;
        private Rect mBodyInteractRect;
        private string mAUSEID;

        public Rect TimelineRect => mTimelineRect;

        public void InitTimeLine()
        {
            mAUSEID = LoadUseID();
            if (string.IsNullOrEmpty(mAUSEID))
            {
                EngineDebug.LogError("Load Error... ...");
                return;
            }
            if (SelectActionState != null && SelectActionState.EditorAnimEvent != null)
            {
                mAnimEvent = new EventDisplay(SelectActionState.EditorAnimEvent);
            }
            else
            {
                mAnimEvent = null;
            }
            
            InitTimeLineControl();
            ActionWindowMain.ActionEditorFuntion.ChangeAction();
            mRangeMini = 0f;
            mRangeMax = 1f;
        }
        
        private void DrawTimeline()
        {
            Rect _rect = new Rect(0, EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            Rect _headRect = new Rect(_rect.x, _rect.y + 5f, MotionEngineConst.GUI_TimeHeadWindth - MotionEngineConst.mLinePadding, _rect.height);
            Rect _lon = new Rect(_headRect);
            _lon.x = 0;
            EditorStyles.toolbar.alignment = TextAnchor.MiddleRight;
            GUI.Label(_headRect, mTimeViewer.TimeHint, EditorStyles.toolbar);
            using (new GUIColorScope(mNtb)) GUI.Label(_lon, mAUSEID);

            float _offset = MotionEngineConst.GUI_TimeHeadWindth + MotionEngineConst.mLinePadding;
            Rect _timelineRect = new Rect(_rect.x + _offset, _rect.y, _rect.width - _offset - MotionEngineConst.GUI_SrocllviewWidth,
                position.height - _rect.y - 20);

            DrawTimeInterval(_timelineRect);
        }

        private float mRangeMini = 0;
        private float mRangeMax = 1;

        private float mTimeStart = 0f;
        private float mTimeRange = 10000;
        private float mTimeScale = 1f;
        private int mActionTotalTime = 10000;
        private readonly Color mNtb = new Color(0.31f, 0.31f, 0.31f, 1);
        private void DrawTimeInterval(Rect _timelineRect)
        {
            if (_timelineRect.width > 0)
            {
                mTimelineRect = _timelineRect;
                mHeadrInteractRect = new Rect(_timelineRect.x, _timelineRect.y, _timelineRect.width, 27f);
                mBodyInteractRect = new Rect(_timelineRect.x, _timelineRect.y + 29f, _timelineRect.width,
                    _timelineRect.height - 29f);
            }

            //时间轴缩放倍率
            float _timeRenge = (mRangeMax - mRangeMini);
            if (_timeRenge > 0)
            {
                mTimeScale = 1 / _timeRenge;

                //显示的Rect范围
                mTimeRange = mBodyInteractRect.width * mTimeScale;
                mTimeStart = mTimeRange * mRangeMini;

                //当前提供的动画长度
                mActionTotalTime = SelectActionState.TotalTime;

                //帧绘制参考  初始化时执行
                float _referInterval = 100f; //理想状态是每个大帧最大间隔是100像素
                int _referFrameCoun = GetReferFrameCount(_referInterval, mTimeRange); //从参考间隔得到理想的帧绘制数量

                //帧显示数据
                int _displayTotalFrame = GetDisplayTotalFrameCount(mActionTotalTime); //显示的总帧数
                int _frameInterval =
                    GetReferFrameDisCount(_displayTotalFrame, 5, _referFrameCoun, out float _drawFrameCount); //间隔帧X5
                float _frameIntervalPx = GetFrameIntervalPx(_drawFrameCount, mTimeRange);

                int _startFrame = 0;
                if (mTimeStart > 0)
                    _startFrame = (int)(mTimeStart / _frameIntervalPx) + 1;
                for (int i = _startFrame; i < (int)_drawFrameCount + 1; i++)
                {
                    float _posX = mHeadrInteractRect.x + i * _frameIntervalPx - mTimeStart;

                    if (_posX > mHeadrInteractRect.x + mHeadrInteractRect.width + 2) break; //加2是为了防止浮点抖动

                    Vector2 _startPos = new Vector2(_posX, mHeadrInteractRect.y + mHeadrInteractRect.height);
                    Vector2 _endPos = new Vector2(_posX, _startPos.y - 12f);
                    Vector2 _endPos2 = new Vector2(_posX, _startPos.y + mBodyInteractRect.height);

                    Handles.color = EngineSetting.colorWhite;
                    Handles.DrawLine(_startPos, _endPos);
                    Handles.color *= 0.4f;
                    Handles.DrawLine(_startPos, _endPos2);

                    Rect _frameRect = new Rect(_startPos.x + 2f, _startPos.y - 24f, 50f, 20f);
                    GUI.Label(_frameRect, (i * 5 * Mathf.Pow(2, _frameInterval)).ToString(), EngineSetting.fontDefault);
                }

                DrawTimeLineControl(mHeadrInteractRect);
            }

            Rect _timeRangeRect = new Rect(mBodyInteractRect);
            _timeRangeRect.y += _timeRangeRect.height;
            _timeRangeRect.height = 20f;
            EditorGUI.MinMaxSlider(_timeRangeRect, ref mRangeMini, ref mRangeMax, 0f, 1f);
            if (_timeRangeRect.Contains(Event.current.mousePosition) && Event.current.clickCount == 2)
            {
                mRangeMini = 0f;
                mRangeMax = 1f;
            }
        }
        public Rect GetEvenTrackRect(float _y, float _height)
        {
            return new Rect(mBodyInteractRect.x, _y, mBodyInteractRect.width, _height);
        }


        //获得参考帧间隔来返回当前参考的绘制帧数量
        private int GetReferFrameCount(float _referInterval, float _rectWidth)
        {
            float _referCount = _rectWidth / _referInterval;
            return (int)_referCount;
        }

        //显示的总帧数
        private int GetDisplayTotalFrameCount(int _actionTotalTime)
        {
            float _frameTime = 1f / mTimeViewer.FrameRate;
            return Mathf.RoundToInt((float)_actionTotalTime / RunTime.MotionEngineConst.TimeDoubling / _frameTime);
        }

        //获得最接近参考的间隔帧数  out渲染的帧数量
        private int GetReferFrameDisCount(int _TotalFrame, int _interval, int _referFrameCount, out float _drawFrameCount)
        {
            for (int i = 0; i < 1000; i++)
            {
                float _frameCount = (float)_TotalFrame / (_interval * Mathf.Pow(2,i));
                if (_frameCount < _referFrameCount + 0.5f)
                {
                    _drawFrameCount = _frameCount;
                    return i;
                }
            }
            _drawFrameCount = 8f;
            return 2;
        }

        //获得帧之间间隔的像素数量
        private float GetFrameIntervalPx(float _drawFrameCount,float _rectWidth)
        {
            return _rectWidth / _drawFrameCount;
        }
        
        //根据位置返回时间  单位秒
        public int GetTimeToPos(float _mousePosX)
        {
            float _nowTimePos = mTimeStart + (_mousePosX - TimelineRect.x);
            float _percentageTime = _nowTimePos / mTimeRange;
            float _outTime = _percentageTime * mActionTotalTime;
            return GetTimeToAbsorpyion(_outTime);
            // return (int)_outTime;
        }

        public int GetTimeToAbsorpyion(float _time, bool TimeAbsorpyion = false)
        {
            if (mTimeViewer.TimeAbsorpyion || TimeAbsorpyion)
            {
                float _SnapInterval = mTimeViewer.SnapInterval * RunTime.MotionEngineConst.TimeDoubling;
                float _outTime = _time / _SnapInterval;
                return (int)(Mathf.RoundToInt(_outTime) * _SnapInterval);
            }
            return (int)_time;
        }

        private string LoadUseID()
        {
            string _id = string.Empty;
            string _path = MotionEngineConst.EditorResourcesPath + "MainID/MainID.json";
            if (!File.Exists(_path))
            {
                EngineDebug.LogWarning($"客户端加载 Action 数据失败，\n加载路径：{_path}");
                return string.Empty;
            }

            _id = File.ReadAllText(_path);
            //EditorActionStateInfo _LoadInfo = new EditorActionStateInfo(null);
            //JsonUtility.FromJsonOverwrite(_str, _LoadInfo);

            return _id;
        }

        //根据时间返回在时间轴上的具体位置
        public float GetPosToTime(int Time)
        {
            return (((float)Time / mActionTotalTime) * mTimeRange - mTimeStart) + mBodyInteractRect.x;
        }
    }
}
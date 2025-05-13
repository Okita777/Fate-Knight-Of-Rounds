using UnityEngine;

namespace AsiActionEngine.Editor
{
    public class TimeViewer
    {
        #region Properties
        private float mTimer;
        private int mFrameRate;
        public int FrameRate => mFrameRate;

        private float mSnapInterval;
        public float SnapInterval => mSnapInterval;

        public GUIContent TimeHint { get; private set; }
        public bool TimeAbsorpyion { get; private set; } = true;

        #endregion
        
        public TimeViewer(int _frameRate)
        {
            mFrameRate = _frameRate;
            mSnapInterval = 1f / _frameRate;
            TimeHint = new GUIContent("0 (0)");
        }

        public void ChangeRate(int _frameRate)
        {
            mFrameRate = _frameRate;
            mSnapInterval = 1f / _frameRate;
        }

        public float ToSecond(int _misTime)
        {
            return (float)_misTime / RunTime.MotionEngineConst.TimeDoubling;
        }

        public int GetFrameToTime(int _time)
        {
            return Mathf.RoundToInt(ToSecond(_time) * mFrameRate);
        }

        public void SetTimeAbsorpyion(bool _isAbs = true)
        {
            TimeAbsorpyion = _isAbs;
        }

        public void SetTime(int _time)
        {
            TimeHint = new GUIContent($"{ToSecond(_time)} ({GetFrameToTime(_time)})");
        }

        public int NextFramTime(int _nowTime)
        {
            int _timeDoubling = RunTime.MotionEngineConst.TimeDoubling;
            float _curTime = (float)_nowTime / _timeDoubling;
            int _nowFrame = Mathf.RoundToInt(_curTime / mSnapInterval);//总帧数
            return Mathf.RoundToInt((_nowFrame * mSnapInterval + mSnapInterval) * _timeDoubling);
        }
        public int UpFramTime(int _nowTime)
        {
            int _timeDoubling = RunTime.MotionEngineConst.TimeDoubling;
            float _curTime = (float)_nowTime / _timeDoubling;
            int _nowFrame = Mathf.RoundToInt(_curTime / mSnapInterval);//总帧数
            return Mathf.RoundToInt((_nowFrame * mSnapInterval - mSnapInterval) * _timeDoubling);
        }
    }
}
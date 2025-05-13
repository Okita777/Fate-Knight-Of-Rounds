using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class TimeLineWindow
    {
        public void BekaAttackBoxParts(EditorActionEvent _actionEvent, int _number)
        {
            int _bakeNumber = _number;
            if (_bakeNumber < 1)
            {
                float _timeDoubling = RunTime.MotionEngineConst.TimeDoubling;
                mTargetBakeNumber = Mathf.RoundToInt(_actionEvent.Duration / (mTimeViewer.SnapInterval * _timeDoubling));
                // mTargetBakeNumber++;
            }
            else
            {
                mTargetBakeNumber = _number;
            }

            if (mTargetBakeNumber > 0)
            {
                mTargetBakeInterval = (float)_actionEvent.Duration / mTargetBakeNumber;
                if (_bakeNumber < 1) mTargetBakeNumber++;
                mBekaNumber = 0;
                mLateTime = mNowTime;
                mAttackBoxTriggerTime = _actionEvent.TriggerTime;
                mNowTime = _actionEvent.TriggerTime;
                mActionEvent_Attack = _actionEvent;
                mAttackBoxEndTime = _actionEvent.TriggerTime + _actionEvent.Duration;
                mAttackBoxPart = new List<AttackBoxPart>();
                Repaint();
            }
        }

        private int mLateTime;
        private int mAttackBoxTriggerTime;
        private int mAttackBoxEndTime;
        private int mBekaNumber = 0;//烘焙进度
        private int mTargetBakeNumber = 0;//烘焙目标次数
        private float mTargetBakeInterval;//烘焙的每帧间隔
        private EditorActionEvent mActionEvent_Attack;
        private List<AttackBoxPart> mAttackBoxPart;

        private void UpdateBakeAttackBox()
        {
            if (mBekaNumber < mTargetBakeNumber)
            {
                if (ResourcesWindow.Instance.TryGetCharacterConfig(out CharacterConfig _config))
                {
                    Event_AttackBox _eventAttackBox = (Event_AttackBox)mActionEvent_Attack.EventData;
                    AttackBoxInfo attackBoxInfo = _eventAttackBox.AttackBoxInfo;
                    if (_config.HelpPointDic.TryGetValue(attackBoxInfo.ReferPoint, out Transform _transform))
                    {
                        // AttackBoxPart
                        int _curTime = Mathf.RoundToInt(mBekaNumber * mTargetBakeInterval);
                        mNowTime = mAttackBoxTriggerTime + _curTime;
                        if (mBekaNumber == mTargetBakeNumber - 1)
                        {
                            mNowTime = mAttackBoxEndTime;
                        }
                        UpdateTimeToNow();

                        Transform unitTrans = ResourcesWindow.Instance.GetRole().transform;
                        
                        Vector3 _startPos = _transform.TransformPoint(attackBoxInfo.OffsetPos.GetValue());
                        Quaternion _rot = (_transform.rotation * Quaternion.Euler(attackBoxInfo.OffsetRot.GetValue()));
                        Vector3 _dir = unitTrans.InverseTransformDirection(_rot * Vector3.forward);

                        _startPos = unitTrans.InverseTransformPoint(_startPos);
                        mAttackBoxPart.Add(new AttackBoxPart(_startPos, _dir, _curTime));
                        mBekaNumber++;
                        // Debug.Log("烘焙攻击盒");
                        if (mBekaNumber >= mTargetBakeNumber)
                        {
                            attackBoxInfo.Box = mAttackBoxPart.ToArray();
                        }
                        else
                        {
                            GUI.changed = true;
                        }
                    }
                }
            }
        }
    }
}
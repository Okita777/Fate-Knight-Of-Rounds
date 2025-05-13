using System;
using System.Collections.Generic;
using System.Linq;
using AsiActionEngine.Editor;
using AsiActionEngine.RunTime;
using AsiTimeLine.RunTime;
using AsiActionEditor_Ex.RunTime;

namespace AsiTimeLine.Editor
{
    public partial class DrawInspector
    {
        public static string GetTitle(EditorActionEvent _actionEvent, bool _isInit)
        {
            string _title = String.Empty;
            
            IActionEventData _eventData = _actionEvent.EventData;
            if (_eventData == null)
            {
                return "Error";
            }
            
            if (_eventData is Event_Lable _eusl)
            {
                if (_eusl.IsRemoveAll)
                {
                    _title = "状态标签: <color=#FF0000>删除所有标签</color>";
                }
                else
                {
                    _title = string.Format("状态标签：<color=#FFCC00>{0}</color>\n类型: {1}",
                        ResourcesWindow.Instance.ActionLable[_eusl.StateLable],
                        _eusl.IsRemove ? "<color=#FF0000>删除该标签</color>" : "<color=#00FF00>添加该标签</color>"
                    );
                }
            }
            else if (_eventData is Event_Attach _ewpc)
            {
                // _title = string.Format("武器挂点切换：{0}\n挂点: <color=#FFCC00>{1}</color>",
                //     _ewpc.IsRightWeapon ? "右" : "左",
                //     ((ECharacteLimbType)_ewpc.LimbPointType).ToString()
                // );
                
                _title = "附加对象";
                // if (_ewpc.ReferTransform.Get() is null)
                // {
                //     _title = "未设置要附加的对象";
                // }
                // else
                // {
                //     _title = string.Format("将：{0}\n附加至: <color=#FFCC00>{1}</color>",
                //         _ewpc.ReferTransform.Get().name,
                //         _ewpc.TargetTransform.Get() is null?"世界空间" : _ewpc.TargetTransform.Get().name
                //     );
                // }

            }else if (_eventData is Event_CameraChange _ecc)
            {
                if (ResourcesWindow.Instance.PreCamControl != null)
                {
                    string _pointName = "未找到挂点";
                    if (ResourcesWindow.Instance.GetRole().TryGetComponent(out CharacterConfig _config))
                    {
                        ECharacteLimbType[] _ECLT = _config.HelpPointDic.Keys.ToArray();
                        for (int i = 0; i < _ECLT.Length; i++)
                        {
                            if ((int)_ECLT[i] == _ecc.EnterPoint)
                            {
                                _pointName = _ECLT[i].ToString();
                                break;
                            }
                        }
                    }
                    
                    _title = string.Format(
                        "相机跳转: <color=#ffcc00><b>{0}</b></color>\nTime: <color=#ffcc00><b>{1}</b></color>   Point: <color=#ffcc00><b>{2}</b></color>", 
                        ResourcesWindow.Instance.PreCamControl.allCinemachine[_ecc.EnterCam].name, 
                        _ecc.EnterTime.ToString("F2"),
                        _pointName
                    );
                }
                else
                {
                    _title = "<color=#FF0000>未创建预览相机，无法显示</color>";
                }
            }else if (_eventData is Event_CameraShake _ecs)
            {
                if (ResourcesWindow.Instance.PreCamControl is not null)
                {
                    _title = string.Format(
                        "枢轴偏移: {0}\n振幅:{1}  频率:{2}", 
                        _ecs.PivotOffset,
                        _ecs.AmplitudeGain.ToString("F2"),
                        _ecs.FrequencyGain.ToString("F2")
                    );
                }
                else
                {
                    _title = "<color=#FF0000>未创建预览相机，无法显示</color>";
                }
            }
            else if (_eventData is Event_TimeScale _ets)
            {
                _title = "时间缩放：" + _ets.TimeScale;
            }
            
            return _title;
        }
    }
}
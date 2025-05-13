using System;
using AsiActionEngine.RunTime;

namespace AsiActionEngine.Editor
{
    public partial class ActionEventDescripted
    {
        private const string mTitleError = "数据丢失!!\n接口序列化失败";
        
        #region Instance
        private static ActionEventDescripted _instance = null;

        public static ActionEventDescripted Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ActionEventDescripted();
                }

                return _instance;
            }
        }
        #endregion
        
        public string GetTitle(EditorActionEvent _actionEvent)
        {
            // return "射线检测";
            string _title = String.Empty;
            
            IActionEventData _eventData = _actionEvent.EventData;
            if (_eventData == null)
            {
                return mTitleError;
            }
            
            if (_eventData is Event_PlayAnim _epa)
            {
                _title = string.Format("动画：{0}\n总长:{1}",
                    _epa.AnimName,
                    ((float)_actionEvent.Duration / RunTime.MotionEngineConst.TimeDoubling).ToString("0.001")
                );
            } 
            else if (_eventData is Event_AttackBox _eab)
            {
                int _bakerNuber = _eab.AttackBoxInfo.Box.Length;

                _title = string.Format("攻击盒：{0}\n类型: {1}",
                    _bakerNuber > 0 ? $"已烘焙 <color=#FFCC00>{_bakerNuber}</color>" : "<color=#FF0000>未烘焙</color>",
                    _boxType[_eab.AttackBoxInfo.AttackBoxType]
                );
            }
            else if (_eventData is Event_Cast_Ray _rdc)
            {
                _title = "射线检测";
            }
            else if (_eventData is Event_SetGFloat _esgf)
            {
                _title = "设置 GFloat";
            }
            else if (_eventData is Event_SetGInt _esgi)
            {
                _title = "设置 GInt";
            }
            else if (_eventData is Event_SetGPoint _esgp)
            {
                _title = "设置 GPoint";
            }
            else
            {
                _title = ActionWindowMain.ActionEditorFuntion.DrawEventTitle(_actionEvent, mIsInit);
                if (string.IsNullOrEmpty(_title))
                {
                    _title = ActionWindowMain.EventTypesDes[_eventData.GetEvenType() + ActionWindowMain.EventType_m.Length];
                }
            }
            
            return _title;
        }

        public string GetDetailed(EditorActionEvent _actionEvent)
        {
            string _title = String.Empty;
            
            IActionEventData _eventData = _actionEvent.EventData;
            if (_eventData == null)
            {
                return "";
            }
            
            if (_eventData is Event_PlayAnim _epa)
            {
                _title = string.Format("动画：{0}\n总长{1}",
                    _epa.AnimName,
                    (float)_actionEvent.Duration / RunTime.MotionEngineConst.TimeDoubling
                );
            }
            // else if (_eventData is RayCastData _rcd)
            // {
            //     _title = "射线检测";
            // }
            else
            {
                return ActionWindowMain.ActionEditorFuntion.DrawEventDetailed(_actionEvent, mIsInit);
            }
            
            return _title;
        }
    }
}
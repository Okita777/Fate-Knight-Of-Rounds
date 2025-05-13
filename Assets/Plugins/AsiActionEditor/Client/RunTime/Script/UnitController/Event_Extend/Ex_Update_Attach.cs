using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    public class Ex_Update_Attach : ActionLogics
    {
        private float m_UpdateTime = -10.0f;
        private Event_Attach m_Event_Attach;

        public void StartAttach(Event_Attach _Event_Attach)
        {
            if (m_UpdateTime > 0) Align();
            m_Event_Attach = _Event_Attach;
            m_UpdateTime = m_Event_Attach.LerpTime;
        }

        public override void Update(ActionStateMachine _actionState, float _deltaTime)
        {
            if (m_UpdateTime > 0)
            {
                m_UpdateTime -= _deltaTime;
                if (m_UpdateTime > 0)
                {
                    float t = m_UpdateTime / m_Event_Attach.LerpTime;
                    if (m_Event_Attach.alignToTarget)
                    {
                        m_Event_Attach.m_Refer.position = Vector3.Lerp(m_Event_Attach.AlignPoint.value.pos, m_Event_Attach.m_StartPos, t);
                        m_Event_Attach.m_Refer.rotation = Quaternion.Lerp(m_Event_Attach.AlignPoint.value.rot, m_Event_Attach.m_StartRot, t);
                    }
                    else
                    {
                        m_Event_Attach.m_Refer.position = m_Event_Attach.m_Target.position;
                        m_Event_Attach.m_Refer.rotation = m_Event_Attach.m_Target.rotation;
                    }
                }
                else
                {
                    Align();
                }
            }
        }

        private void Align()
        {
            if (m_Event_Attach.alignToTarget)
            {
                m_Event_Attach.m_Refer.position = m_Event_Attach.AlignPoint.value.pos;
                m_Event_Attach.m_Refer.rotation = m_Event_Attach.AlignPoint.value.rot;
            }
            else
            {
                m_Event_Attach.m_Refer.position = m_Event_Attach.m_Target.position;
                m_Event_Attach.m_Refer.rotation = m_Event_Attach.m_Target.rotation; 
            }
        }
    }
}
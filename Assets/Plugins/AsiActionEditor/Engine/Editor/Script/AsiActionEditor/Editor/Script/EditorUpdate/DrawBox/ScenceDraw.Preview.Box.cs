using System.Collections.Generic;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.DrawData;
// using AsiActionEditor.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class ScenceDraw
    {
        // private List<DrawPoly> mDrawBoxs = new List<DrawPoly>();
        // private bool mUpdateToEvent = true;

        public ScenceDraw()
        {
            DrawBoxInit();
        }
        private void DrawBox()
        {
            // EngineDebug.Log("绘制");

            if (Application.isPlaying)
            {
                return;
            }
            else
            {
                if (ActionWindowMain.ScenceDraw_Editor)
                {
                    //绘制场景图形
                    TimeLineWindow.Instance.OnScenseDraw(TimeLineWindow.Instance.NowTime);
                } 
            }
            EngineDrawListData.Instance.Draw(-2);

            
            // Handles.color = Color.green;
            // for (int i = 0; i < mDrawBoxs.Count; i++)
            // {
            //     mDrawBoxs[i].Draw();
            // }
        }

        public void DrawBoxInit()
        {
            // mDrawBoxs.Clear();
            EngineDrawListData.Instance.Init();
        }

        // public void CreactDrawBox(DrawPoly _drawPoly)
        // {
        //     mDrawBoxs.Add(_drawPoly);
        // }
    }

    public class DrawBox : DrawPoly
    {
        public Vector3 startPos;
        public Vector3 endPos;
        public Vector2 scale;

        public DrawBox(Vector3 _startPos,Vector3 _endPos,Vector2 _scale)
        {
            startPos = _startPos;
            endPos = _endPos;
            scale = _scale;
        }
        
        public override void Draw()
        {
            
        }
    }

    public class DrawSphere : DrawPoly
    {
        public Vector3 startPos;
        public float radius;
        public DrawSphere(Vector3 _startPos,float _radius)
        {
            startPos = _startPos;
            radius = _radius;
        }
        public override void Draw()
        {
            
        }
    }

    public class DrawCapsule : DrawPoly
    {
        public Vector3 startPos;
        public Vector3 endPos;
        public Vector3 AxisY;
        public float radius;

        public DrawCapsule(Vector3 _startPos, Vector3 _endPos,Vector3 _AxisY, float _radius)
        {
            startPos = _startPos;
            endPos = _endPos;
            AxisY = _AxisY;
            radius = _radius;
        }
        
        public override void Draw()
        {
            if (endPos == startPos)
            {
                Quaternion _rot = Quaternion.Euler(0, 0, 0);
                Handles.DrawWireDisc(startPos, _rot * Vector3.up, radius);
                Handles.DrawWireDisc(startPos, _rot * Vector3.forward, radius);
                Handles.DrawWireDisc(startPos, _rot * Vector3.right, radius);
            }//目标和起点重叠时
            else
            {
                Quaternion _rot = Quaternion.LookRotation(endPos - startPos, AxisY);
                Handles.DrawWireDisc(startPos,_rot * Vector3.forward,radius);
                Handles.DrawWireDisc(endPos,_rot * Vector3.forward,radius);

                Vector3 _up = _rot * Vector3.up * radius;
                Vector3 _dwon = _rot * Vector3.down * radius;
                Vector3 _left = _rot * Vector3.left * radius;
                Vector3 _right = _rot * Vector3.right * radius;
                Handles.DrawLine(startPos + _up, endPos + _up);
                Handles.DrawLine(startPos + _dwon, endPos + _dwon);
                Handles.DrawLine(startPos + _left, endPos + _left);
                Handles.DrawLine(startPos + _right, endPos + _right);

                Handles.DrawWireArc(startPos,_rot * Vector3.right,_rot * Vector3.down,180,radius);
                Handles.DrawWireArc(startPos,_rot * Vector3.down,_rot * Vector3.left,180,radius);
                Handles.DrawWireArc(endPos,_rot * Vector3.right,_rot * Vector3.down,-180,radius);
                Handles.DrawWireArc(endPos,_rot * Vector3.down,_rot * Vector3.left,-180,radius);
            }//绘制胶囊
        }
    }

    public class DrawLine : DrawPoly
    {
        public Vector3 startPos;
        public Vector3 endPos;

        public DrawLine(Vector3 _startPos, Vector3 _endPos)
        {
            startPos = _startPos;
            endPos = _endPos;
        }
        public override void Draw()
        {
            Handles.DrawLine(startPos, endPos);

        }
    }
    
    public class DrawPoly
    {
        public virtual void Draw(){}
    }
}
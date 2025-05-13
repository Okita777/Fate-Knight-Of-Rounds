using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class BluePrintWindows_ReturnValue
    {
       private struct BluePrintLine
        {
            public Vector2 startPos;
            public Vector2 endPos;
            public int inputTarget;
            public int index;
            // private Vector3[] points;

            public BluePrintLine(Vector2 startPos, Vector2 endPos, int inputTarget, int index)
            {
                this.startPos = startPos;
                this.endPos = endPos;
                this.inputTarget = inputTarget;
                this.index = index;
                // points = new Vector3[5];
            }

            public BluePrintLine SetStartPos(Vector2 _startPos)
            {
                return new BluePrintLine(_startPos, endPos, inputTarget, index);
            }
            public void Draw(Color _color)
            {
                Handles.color = _color;

                Vector2 n_startPos = startPos;
                Vector2 n_endPos = endPos;
                n_startPos.x += 10;
                n_endPos.x -= 10;
                Vector2 offsetPos = n_endPos - n_startPos;
                if (offsetPos.x > 0)
                {
                    Vector2 offsetPos2 = new Vector2(offsetPos.x, Math.Abs(offsetPos.y));
                    float dis = (offsetPos2.x - offsetPos2.y);
                    Vector2 startPos2 = n_startPos;
                    //长边优先使用直线
                    if (offsetPos2.x > offsetPos2.y)
                    {
                        startPos2.x += dis;
                    }
                    else
                    {
                        // if(offsetPos.y > 0)
                        startPos2.y += offsetPos.y > 0 ? Mathf.Abs(dis) : dis;
                    }

                    // points[0] = startPos;
                    // points[1] = n_startPos;
                    // points[2] = startPos2;
                    // points[3] = n_endPos;
                    // points[4] = endPos;

                    Handles.DrawLine(n_startPos, startPos2);
                    Handles.DrawLine(startPos2, n_endPos);
                }
                else
                {
                    // points[0] = startPos;
                    // points[1] = n_startPos;
                    // points[2] = n_startPos;
                    // points[3] = n_endPos;
                    // points[4] = endPos;
                    
                    Handles.DrawLine(n_startPos, n_endPos);
                }
                Handles.DrawLine(n_startPos, startPos);
                Handles.DrawLine(endPos, n_endPos);
                
                // Handles.DrawLines(points);
            }
        }
        private static List<BluePrintLine> bluePrintLines = new List<BluePrintLine>();
    }
}
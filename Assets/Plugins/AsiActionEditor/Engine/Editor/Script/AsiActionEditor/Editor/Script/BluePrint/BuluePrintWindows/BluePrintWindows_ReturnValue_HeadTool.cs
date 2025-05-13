using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class BluePrintWindows_ReturnValue
    {
        private void DrawHeadTool(Rect rect)
        {
            Rect headRect = new Rect(0, 0, rect.width, headHeight);
            EditorGUI.DrawRect(headRect, Color.black * 0.3f);
            Handles.color = Color.black * 0.2f;
            // Handles.color = new Color(0.1f, 0.1f, 0.1f, 1);
            Handles.DrawLine(new Vector2(0, headHeight), new Vector2(rect.width,headHeight));

            if (valueFlid)
            {
                headRect.width = valueWidth;
                GUI.Label(headRect, "可配置参数", centerGUI);
                Handles.DrawLine(new Vector2(valueWidth, 0), new Vector2(valueWidth,rect.height));

                headRect.x += headRect.width;
                headRect.width = 150;
                if (GUI.Button(headRect, "", EditorStyles.label))
                {
                    viewPos = Vector2.zero;
                }
                GUI.Label(headRect, viewPos.ToString(), centerGUI);
            }
        }
    }
}
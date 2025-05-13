using System;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple = true,Inherited = true)]
    public class BoxGroupAttribute : PropertyAttribute{
        public string title;
        public string color; //这里不能直接用Color，会报错，不知道为什么，所以只能传16进制字符串，然后自己解析

        public BoxGroupAttribute(string title, string color = "#FFFFFF")
        {
            this.title = title;
            this.color = color;
        }
    }

    [CustomPropertyDrawer(typeof(BoxGroupAttribute))]
    public class TitleAttributeDrawer : DecoratorDrawer
    {
        private GUIStyle style = new GUIStyle();

        public override void OnGUI(Rect position)
        {
            BoxGroupAttribute ta = attribute as BoxGroupAttribute;

            style.normal.textColor = GetColor(ta.color);
            style.fontSize = 30;
            position = EditorGUI.IndentedRect(position);

            GUI.Label(position, ta.title, style);
        }

        public override float GetHeight()
        {
            return base.GetHeight() + 15;
        }

        Color GetColor(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                return Color.black;

            hex = hex.ToLower();

            if (hex.IndexOf("#") == 0 && (hex.Length == 7 || hex.Length == 9))
            {
                //判断输入的是否是16进制颜色值，如果不是，返回黑色
                for (int i = 1; i < hex.Length; i++)
                {
                    if ((hex[i] >= '0' && hex[i] <= '9') == false && (hex[i] >= 'a' && hex[i] <= 'f') == false)
                    {
                        return Color.black;
                    }
                }

                int r = Convert.ToInt32(hex.Substring(1, 2), 16);
                int g = Convert.ToInt32(hex.Substring(3, 2), 16);
                int b = Convert.ToInt32(hex.Substring(5, 2), 16);

                return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
            }

            return Color.black;
        }
    }

}
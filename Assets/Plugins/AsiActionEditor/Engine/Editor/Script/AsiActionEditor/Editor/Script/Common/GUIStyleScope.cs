using System;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public class GUIStyleScope : IDisposable
    {
        private enum EGUIStyleType
        {
            TextAnchor,
            FontSize,
            FontColor
        }

        private EGUIStyleType GUIStyleType;
        private GUIStyle mGUIStyle;
        
        private TextAnchor mTextAnchor;
        public GUIStyleScope(GUIStyle _style, TextAnchor _anchor)
        {
            GUIStyleType = EGUIStyleType.TextAnchor;
            mGUIStyle = _style;
            mTextAnchor = _style.alignment;
            
            mGUIStyle.alignment = _anchor;
        }

        private int mFontSize;
        public GUIStyleScope(GUIStyle _style,int _fontSize)
        {
            GUIStyleType = EGUIStyleType.FontSize;
            mGUIStyle = _style;
            mFontSize = _style.fontSize;

            _style.fontSize = _fontSize;
        }

        private Color mFontColor;
        public GUIStyleScope(GUIStyle _style,Color _fontColor)
        {
            GUIStyleType = EGUIStyleType.FontColor;
            mGUIStyle = _style;
            mFontColor = _style.normal.textColor;

            _style.normal.textColor = _fontColor;
        }
        
        public void Dispose()
        {
            if (GUIStyleType == EGUIStyleType.TextAnchor)
                mGUIStyle.alignment = mTextAnchor;
            else if (GUIStyleType == EGUIStyleType.FontSize)
                mGUIStyle.fontSize = mFontSize;
            else if (GUIStyleType == EGUIStyleType.FontColor)
                mGUIStyle.normal.textColor = mFontColor;
        }
    }
}
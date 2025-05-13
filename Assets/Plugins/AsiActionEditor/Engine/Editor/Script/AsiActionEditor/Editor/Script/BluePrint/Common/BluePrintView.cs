using System;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public class BluePrintView
    {
        #region Instance
        private static BluePrintView _instance = null;
        public static BluePrintView Instance
        {
            get
            {
                if(_instance is null)_instance = new BluePrintView();
                return _instance;
            }
        }
        #endregion

        public void Background(Rect _rect, Vector2 _position, float _size = 1)
        {
            using (new GUI.GroupScope(_rect))
            {
                DrawGrid(_rect, _position, 20 * _size, 0.2f, Color.gray);
                DrawGrid(_rect, _position, 100 * _size, 0.4f, Color.gray);
            }
        }

        private void DrawGrid(Rect _rect, Vector2 _position, float _gridSpacing,float _gridOpacity,Color _gridColor)
        {
            int widthDivs = Mathf.CeilToInt(_rect.width / _gridSpacing) + 1;
            int heightDivs = Mathf.CeilToInt(_rect.height / _gridSpacing) + 1;

            Handles.BeginGUI();
            {
                Handles.color = new Color(_gridColor.r, _gridColor.g, _gridColor.b, _gridOpacity);
                Vector3 gridOffset = new Vector3(_position.x % _gridSpacing, _position.y % _gridSpacing, 0);

                for (int i = 0; i < widthDivs; i++)
                {
                    Handles.DrawLine(
                        new Vector3(_gridSpacing * i, -_gridSpacing, 0) + gridOffset,
                        new Vector3(_gridSpacing * i, _rect.height + _gridSpacing, 0) + gridOffset
                    );
                }

                for (int i = 0; i < heightDivs; i++)
                {
                    Handles.DrawLine(
                        new Vector3(-_gridSpacing, _gridSpacing * i, 0) + gridOffset,
                        new Vector3(_rect.width + _gridSpacing, _gridSpacing * i, 0) + gridOffset
                    );
                }
                Handles.color = Color.white;
            }
            Handles.EndGUI();
        }
        
        public Rect DrawNode(Vector2 _widthAndHeight, Vector2 _position, Vector2 _viewPos, float _size = 100)
        {
            Rect _nodeRect = new Rect(-_widthAndHeight.x*0.5f, -_widthAndHeight.y * 0.5f, _widthAndHeight.x, _widthAndHeight.y);
            _nodeRect.position += _viewPos + _position;
            return _nodeRect;
        } 
    }
}
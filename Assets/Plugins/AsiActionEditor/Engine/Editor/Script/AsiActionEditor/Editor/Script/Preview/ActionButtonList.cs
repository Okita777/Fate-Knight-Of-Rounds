using System;
using System.Collections.Generic;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public class ActionButtonList
    {
        private const float m_buttonHeight = 20f;
        private const float m_buttonInterval = 0f;


        static private Vector2 m_ActionScrow;
        public static void DrawAction(Rect _rect, List<EditorActionState> _actionStates, Action<EditorActionState> _select, int _selectID)
        {
            float _mainHeight = m_buttonHeight + m_buttonInterval;
            using (new GUI.GroupScope(_rect))
            {
                _rect.position = Vector2.zero;
                Rect _actionViewRect = new Rect(_rect);
                _actionViewRect.width -= 15f;
                _actionViewRect.height = _actionStates.Count * _mainHeight;
                using (var _scrow = new GUI.ScrollViewScope(_rect, m_ActionScrow, _actionViewRect,false,true))
                {
                    m_ActionScrow = _scrow.scrollPosition;

                    
                    float _startPos = 0;
                    for (int i = 0; i < _actionStates.Count; i++)
                    {
                        float _endPos = _startPos + _mainHeight;

                        float _checkStartPos = m_ActionScrow.y;
                        float _checkEndPos = m_ActionScrow.y + _rect.height;

                        // _checkStartPos += 60f;
                        // _checkEndPos -= 60f;
                        
                        bool _isDraw = _endPos > _checkStartPos;
                        if (_isDraw)
                        {
                            _isDraw = _startPos < _checkEndPos;
                            if (!_isDraw) break;
                        }

                        if (_isDraw)
                        {
                            Rect _buttonRect = new Rect(_actionViewRect);
                            _buttonRect.y = _startPos;
                            _buttonRect.height = m_buttonHeight;
                            EditorActionState _actionState = _actionStates[i];
                            bool _isSelect = _actionState == ResourcesWindow.Instance.GetEditorActionStateToSelect();
                            using (new GUIColorScope(Color.gray,_isSelect))
                            {
                                if (GUI.Button(_buttonRect, ""))
                                {
                                    _select(_actionState);
                                }
                            }

                            GUIStyle _style = new GUIStyle();
                            _style.alignment = TextAnchor.MiddleCenter;
                            _style.normal.textColor = Color.white;
                            GUI.Label(_buttonRect, _actionState.Name, _style);
                            _style.alignment = TextAnchor.MiddleLeft;
                            _style.normal.textColor *= 0.8f;
                            GUI.Label(_buttonRect, " " +GetLayerName(_actionState.AnimaLayer), _style);
                            _style.alignment = TextAnchor.MiddleRight;
                            _style.normal.textColor = Color.cyan;
                            GUI.Label(_buttonRect, _actionState.ID.ToString() + " ", _style);
                        }
                        _startPos += _mainHeight;
                    }
                }
            }
        }

        static private string GetLayerName(int _layerID)
        {
            if (_layerID == 0)
            {
                return "Base";
            }else if (_layerID == 1)
            {
                return "Limb";
            }else if (_layerID == 2)
            {
                return "Upper";
            }else if (_layerID == 3)
            {
                return "Noise";
            }

            return "<color=#FFCC00>Script</color>";
        }
    }
}
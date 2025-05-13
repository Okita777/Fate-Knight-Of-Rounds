using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AsiActionEngine.Editor
{
    [System.Serializable]
    public class TrackEvents
    {
        public TrackEvents(string _trackGroupName, string _trackName)
        {
            trackGroupName = _trackGroupName;
            trackName = _trackName;
        }
        private const float elementHeight = 30;
        private const float eventEnumHeight = 20;
        
        public string trackGroupName = "TrackGroup";
        public string trackName = "TrackName";
        public List<string> evenTypes = new List<string>();
        public Color trackColor = Color.cyan;//轨道颜色
        public GUIContent trackIcon = new GUIContent();//轨道图标
        
        private readonly Color backGround = new Color(0.6f,0.6f,0.6f,0.1f);
        
        private bool mOnfold = true;//折叠状态
        private bool needInit = true;
        private ReorderableList EventTypes;
        public bool Onfold
        {
            get
            { 
                return mOnfold;
            }
            set
            {
                mOnfold = value;
            }
        }

        public void Draw(Rect _rect, int _index)
        {
            if (!Init())
            {
                return;
            }
            
            float _color_w = 50;
            float _icon_w = 20;
            float _flod = 10;
            
            //绘制背景
            Rect _backGround = new Rect(_rect);
            _backGround.height = elementHeight;
            _backGround.y += 2f;
            _backGround.width -= 5;
            _backGround.x += 5;
            EditorGUI.DrawRect(_backGround, backGround);

            //绘制下拉按钮
            Rect _flodRect = new Rect(_backGround);
            _flodRect.width = _flod;
            _flodRect.height = _rect.height-2;
            using (new GUIColorScope(Color.gray,!mOnfold))
            {
                if (GUI.Button(_flodRect, ""))
                {
                    mOnfold = !mOnfold;
                }
            }

            
            _backGround.height -= 8;
            _backGround.y += 4;
            
            //绘制轨道图标
            Rect _groupIcon = new Rect(_backGround);
            _groupIcon.width = _icon_w;
            _groupIcon.x += 4 + _flod;
            if (GUI.Button(_groupIcon, trackIcon,EditorStyles.label))
            {
                
            }

            float _colorPosX = (_rect.width + _rect.x - _color_w - 5);
            
            //绘制轨道组名
            Rect _groupName = new Rect(_groupIcon);
            _groupName.x += 4 + _groupIcon.width;
            _groupName.width = _colorPosX -(_groupIcon.width + _groupIcon.x + 3);
            trackGroupName = EditorGUI.TextField(_groupName, trackGroupName);
            
            //轨道色
            Rect _trck_color = new Rect(_rect);
            _trck_color.height = _groupIcon.height;
            _trck_color.y = _groupIcon.y;
            _trck_color.width = _color_w;
            _trck_color.x = _colorPosX;
            trackColor = EditorGUI.ColorField(_trck_color,new GUIContent(""), trackColor,true,true,false); 
            
            //展开后的绘制
            if (!Onfold)
            {
                float _height = 20;
                
                //绘制轨道头部图标
                Rect _tracktextrue = new Rect(_backGround);
                _tracktextrue.width -= 15;
                _tracktextrue.x += 15;
                _tracktextrue.y += _tracktextrue.height + 12;
                _tracktextrue.height = 60;
                EditorGUI.DrawRect(_tracktextrue,Color.black * 0.2f);

                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    var trackTextrue = (Texture)EditorGUI.ObjectField(_tracktextrue,
                        "  轨道图标",trackIcon.image,typeof(Texture),true);
                    if (_check.changed)
                    {
                        if (trackTextrue is not null)
                        {
                            trackIcon.image = trackTextrue;
                        }
                    }
                }
                
                //绘制轨道名称
                Rect _trackName = new Rect(_tracktextrue);
                _trackName.y += _trackName.height + 2;
                _trackName.height = _height;
                EditorGUI.DrawRect(_trackName,Color.black * 0.2f);
                trackName = EditorGUI.TextField(_trackName,"  子轨名称", trackName);
                
                //绘制事件类型列表
                Rect _trackEventList = new Rect(_trackName);
                _trackEventList.y += _trackEventList.height + 5;
                _trackEventList.height = eventEnumHeight * evenTypes.Count + 20;

                EventTypes.DoList(_trackEventList);
            }
            //绘制不可视按钮触发回调 防止拖拽
            GUI.Button(_rect, "",EditorStyles.label);
        }
        
        public float GetHeight()
        {
            float _height = elementHeight;
            if (!Onfold)
            {
                _height = 180f + (eventEnumHeight + 2) * (Mathf.Max(evenTypes.Count, 1));
            }
            return _height;
        }

        private bool Init()
        {
            if (EventTypes is not null)
            {
                needInit = false;
                return true;
            }
            EventTypes = new ReorderableList(evenTypes, null, true, true, true, true);
            EventTypes.elementHeight = eventEnumHeight;
            GUIStyle _guiStyle = new GUIStyle();
            _guiStyle.normal.textColor = Color.white;
            EventTypes.drawHeaderCallback = rect => { GUI.Label(rect, 
                $"[<color=#FFCC00>{trackGroupName}</color>]  轨道事件",_guiStyle);};
            EventTypes.drawElementCallback = (rect, index, active, focused) =>
            {
                // // evenTypes[index] = edi
                // // int _selectID = ActionWindowMain.EventTypes.IndexOf(evenTypes[index]);
                // int _selectID = evenTypes[index];
                // using (var _check = new EditorGUI.ChangeCheckScope())
                // {
                //     _selectID = EditorGUI.Popup(rect, _selectID, ActionWindowMain.EventTypesDes.ToArray());
                //     if (_check.changed)
                //     {
                //         // evenTypes[index] = ActionWindowMain.EventTypes[_selectID];
                //         evenTypes[index] = _selectID;
                //     }
                // }

                if (ActionWindowMain.GetActionEventID(evenTypes[index], out int eventID))
                {
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        int selectedIndex = EditorGUI.Popup(rect, eventID, ActionWindowMain.EventTypesDes.ToArray());
                        if (_check.changed)
                        {
                            evenTypes[index] = ActionWindowMain.EventTypes[selectedIndex];
                        }
                    }
                }
                else
                {
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            using (new GUIColorScope(Color.red))
                            {
                                rect.width -= 60;
                                int selectedIndex = EditorGUI.Popup(rect, -1, ActionWindowMain.EventTypesDes.ToArray());
                                rect.x += rect.width;
                                rect.width = 60;

                                GUI.Label(rect, "数据丢失");
                                if (_check.changed)
                                {
                                    evenTypes[index] = ActionWindowMain.EventTypes[selectedIndex];
                                }
                            }

                        }
                    }
                }
            };

            needInit = false;
            return true;
        }
    }
}
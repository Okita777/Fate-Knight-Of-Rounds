using System;
using System.Collections.Generic;
using System.IO;
// using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;

namespace AsiActionEngine.Editor
{
    public class InputModuleWindow : EditorWindow
    {
        private readonly string[] mousbuttonType = new[]
        {
            "鼠标左键",
            "鼠标右键",
            "鼠标中键"
        };
        
        private readonly string[] vector2Type = new[]
        {
            "无输入",
            "WASD",
            "MouseDelta",
            "Dpad"
        };
        
        private InputModuleInfo InputModuleInfo;
        private ReorderableList keyCodeList;
        private ReorderableList mouseButtonList;
        private ReorderableList comboInputList;
        private ReorderableList keyCombinationsList;
        private ReorderableList inpuActionList;

        private int selectID;
        private int selectMainMenu;
        private string inputActionName;
        private Vector2 scroPos;
        private List<int> keyCodeListID = new List<int>();
        private List<int> mouseButtonListID = new List<int>();
        private List<int> comboInputListID = new List<int>();
        
        private static InputModuleWindow mWindow;
        public static void OpenWindow()
        {
            if (mWindow == null)
            {
                mWindow = GetWindow<InputModuleWindow>("输入系统设置");
                mWindow.Show();
            }
            else
            {
                mWindow.Close();
            }
        }
        private void OnEnable()
        {
            LoadJson();
            Init();
        }

        private void OnGUI()
        {
            GUIStyle _tilt = new GUIStyle();
            float _footHeight = 38f;

            //绘制头部
            Rect _head = new Rect(position);
            _head.y = 10f;
            _head.x = 0;
            _head.height = 40f;
            EditorGUI.DrawRect(_head, Color.black * 0.2f);
            _tilt.alignment = TextAnchor.MiddleCenter;
            _tilt.normal.textColor = Color.white;
            _tilt.fontSize = 20;
            GUI.Box(_head, "Action Input System", _tilt);

            //行为列表和按键配置
            Rect _ActionList = new Rect(_head);
            _ActionList.y += _ActionList.height + 10;
            _ActionList.height = 20f;

            selectMainMenu = GUI.Toolbar(_ActionList, selectMainMenu, new[] { "按键配置", "行为列表配置" });

            if (selectMainMenu == 0)
            {
                // //绘制添加按钮
                Rect _buttonList = new Rect(_ActionList);

                //绘制身体
                Rect _body = new Rect(_buttonList);
                _body.y += _body.height + 10;
                _body.height = position.height - _body.y;
                // EditorGUI.DrawRect(_body,Color.green);

                if (_body.height > 0)
                {
                    using (new GUI.GroupScope(_body))
                    {
                        float _height = position.height - _body.y - _footHeight;
                        using (var scro = new GUILayout.ScrollViewScope(scroPos, false, true,
                                   GUILayout.Height(_height)))
                        {
                            scroPos = scro.scrollPosition;
                            keyCodeList.DoLayoutList();
                            mouseButtonList.DoLayoutList();
                            comboInputList.DoLayoutList();
                        }
                    }
                }
            }
            else if (selectMainMenu == 1)
            {
                // List<string> _actionList = InputActionList.Instance.ActionList;
                
                //绘制身体
                Rect _body = new Rect(_ActionList);
                _body.y += _body.height + 10;
                _body.height = position.height - _body.y;

                if (_body.height > 0)
                {
                    using (new GUI.GroupScope(_body))
                    {
                        float _height = position.height - _body.y - _footHeight;
                        using (var scro = new GUILayout.ScrollViewScope(scroPos, false, true,
                                   GUILayout.Height(_height)))
                        {
                            scroPos = scro.scrollPosition;

                            using (new GUILayout.HorizontalScope())
                            {
                                if (GUILayout.Button("添加InputAction",GUILayout.Width(120)))
                                {
                                    if (!InputActionList.Instance.ActionList.Contains(inputActionName))
                                    {
                                        InputActionList.Instance.ActionList.Add(inputActionName);
                                    }
                                }
                                inputActionName = GUILayout.TextField(inputActionName);
                            }
                            inpuActionList.DoLayoutList();
                        }
                    }
                }
            }

            Rect _footRect = new Rect(position);
            _footRect.height = _footHeight;
            _footRect.y = position.height - _footHeight;
            _footRect.x = 0;
            EditorGUI.DrawRect(_footRect, Color.gray * 0.1f);

            Rect _saveButton = new Rect(_footRect);
            _saveButton.y += 5;
            _saveButton.height -= 15;
            // using (new GUIColorScope(Color.green))
            {
                if (GUI.Button(_saveButton,"保存所有配置"))
                {
                    SaveJson();
                } 
            }
            
            _footRect.height = 2;
            EditorGUI.DrawRect(_footRect, Color.gray * 0.2f);

        }

        private void Init()
        {
            if (InputModuleInfo.InputModulePart == null || InputModuleInfo.InputModulePart.Count < 1)
            {
                InputModuleInfo.InputModulePart.Add(new InputModulePart());
            }

            keyCodeListID.Clear();
            mouseButtonListID.Clear();
            comboInputListID.Clear();
            
            foreach (var _inputKeyAction in InputModuleInfo.InputModulePart[selectID].keyActions)
            {
                if (string.IsNullOrEmpty(_inputKeyAction.mAction))
                {
                    _inputKeyAction.mAction = InputActionList.Instance.ActionList[0];
                    keyCodeListID.Add(0);
                }
                else
                {
                    keyCodeListID.Add(FindSelectActionID(_inputKeyAction.mAction));
                }
            }
            
            foreach (var _mouseButtonAction in InputModuleInfo.InputModulePart[selectID].mouseActions)
            {
                if (string.IsNullOrEmpty(_mouseButtonAction.mAction))
                {
                    _mouseButtonAction.mAction = InputActionList.Instance.ActionList[0];
                    mouseButtonListID.Add(0);
                }
                else
                {
                    mouseButtonListID.Add(FindSelectActionID(_mouseButtonAction.mAction));
                }
            }
            
            foreach (var _mouseButtonAction in InputModuleInfo.InputModulePart[selectID].ComboInputActions)
            {
                if (string.IsNullOrEmpty(_mouseButtonAction.mAction))
                {
                    _mouseButtonAction.mAction = InputActionList.Instance.ActionList[0];
                    comboInputListID.Add(0);
                }
                else
                {
                    comboInputListID.Add(FindSelectActionID(_mouseButtonAction.mAction));
                }
            }
            
            KeyCodeListDraw();
            MouseButtonListDraw();
            ComboListDraw();
            ActionListDraw();
        }
        
        private void CreactKeyAction(string _action,KeyCode _key)
        {
            InputModuleInfo.InputModulePart[selectID].keyActions.Add(new InputKeyAction(_action,_key));
            keyCodeListID.Add(0);
        }
        
        private void CreactMouseAction(string _action,int _key)
        {
            InputModuleInfo.InputModulePart[selectID].mouseActions.Add(new MouseButtonAction(_action,_key));
            mouseButtonListID.Add(0);
        }
        
        private void CreactComboAction(string _action,int _key)
        {
            List<int> _comboList = new List<int>();
            _comboList.Add(119);
            _comboList.Add(119);
            InputModuleInfo.InputModulePart[selectID].ComboInputActions.Add(new KeyCombinations(_action, _comboList));
            comboInputListID.Add(0);
        }
        private int FindSelectActionID(string _name)
        {
            return InputActionList.Instance.ActionList.IndexOf(_name);
        }

        #region 初始化数据

        private void KeyCodeListDraw()
        {
            keyCodeList = new ReorderableList(InputModuleInfo.InputModulePart[selectID].keyActions,null,true,true,true,false);
            keyCodeList.drawElementCallback = (rect, index, active, focused) =>
            {
                rect.height -= 5f;
                int _lenth = InputModuleInfo.InputModulePart[selectID].keyActions.Count;
                if (index >= _lenth) { return; }
                
                InputKeyAction _action = InputModuleInfo.InputModulePart[selectID].keyActions[index];

                Rect _ElementRect = new Rect(rect);
                _ElementRect.width = 80;
                _action.KeyCode = (KeyCode)EditorGUI.EnumPopup(_ElementRect,_action.KeyCode);
                
                _ElementRect.x += _ElementRect.width + 5;
                _ElementRect.width = rect.width - _ElementRect.x-9f;
                
                if (keyCodeListID[index] < 0)
                {
                    _action.mAction = EditorGUI.TextField(_ElementRect,_action.mAction);
                }
                else
                {
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        keyCodeListID[index] = EditorGUI.Popup(_ElementRect, keyCodeListID[index],
                            InputActionList.Instance.ActionList.ToArray());
                        if (_check.changed)
                        {
                            _action.mAction = InputActionList.Instance.ActionList[keyCodeListID[index]];
                        }
                    }
                }

                _ElementRect.width = 30;
                _ElementRect.x = rect.x + rect.width - _ElementRect.width;
                using (new GUIColorScope(Color.red))
                {
                    if (GUI.Button(_ElementRect, "-"))
                    {
                        InputModuleInfo.InputModulePart[selectID].keyActions.RemoveAt(index);
                        return;
                    }
                }

            };
            keyCodeList.drawHeaderCallback = rect =>
            {
                GUI.Label(rect,"键盘按键配置");
            };
            keyCodeList.onAddCallback = list =>
            {
                CreactKeyAction(InputActionList.Instance.ActionList[0], KeyCode.A);
            };
        }
        private void MouseButtonListDraw()
        {
            mouseButtonList = new ReorderableList(InputModuleInfo.InputModulePart[selectID].mouseActions,null,true,true,true,false);
            mouseButtonList.drawElementCallback = (rect, index, active, focused) =>
            {
                rect.height -= 5f;
                int _lenth = InputModuleInfo.InputModulePart[selectID].mouseActions.Count;
                if (index >= _lenth) { return; }
                MouseButtonAction _action = InputModuleInfo.InputModulePart[selectID].mouseActions[index];

                Rect _ElementRect = new Rect(rect);
                _ElementRect.width = 80;
                _action.mouseButton = EditorGUI.Popup(_ElementRect, _action.mouseButton, mousbuttonType);
                
                _ElementRect.x += _ElementRect.width + 5;
                _ElementRect.width = rect.width - _ElementRect.x-9f;
                
                if (mouseButtonListID[index] < 0)
                {
                    _action.mAction = EditorGUI.TextField(_ElementRect, _action.mAction);
                }
                else
                {
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        mouseButtonListID[index] = EditorGUI.Popup(_ElementRect, mouseButtonListID[index],
                            InputActionList.Instance.ActionList.ToArray());
                        if (_check.changed)
                        {
                            _action.mAction = InputActionList.Instance.ActionList[mouseButtonListID[index]];
                        }
                    }
                }
                
                _ElementRect.width = 30;
                _ElementRect.x = rect.x + rect.width - _ElementRect.width;
                using (new GUIColorScope(Color.red))
                {
                    if (GUI.Button(_ElementRect, "-"))
                    {
                        InputModuleInfo.InputModulePart[selectID].mouseActions.RemoveAt(index);
                        return;
                    }
                }
            };
            mouseButtonList.drawHeaderCallback = rect =>
            {
                GUI.Label(rect,"鼠标按键配置");
            };
            mouseButtonList.onAddCallback = list =>
            {
                CreactMouseAction(InputActionList.Instance.ActionList[0], 0);
            };
        }
        private void ComboListDraw()
        {
            comboInputList = new ReorderableList(InputModuleInfo.InputModulePart[selectID].ComboInputActions,null,true,true,true,false);
            comboInputList.drawElementCallback = (rect, index, active, focused) =>
            {
                Vector2 _Pos_S = new Vector2(0, rect.y + rect.height);
                Vector2 _Pos_E = new Vector2(rect.width + 27, rect.y + rect.height);
                rect.height -= 5;
                rect.y += 2;
                
                int _lenth = InputModuleInfo.InputModulePart[selectID].ComboInputActions.Count;
                if (index >= _lenth) { return; }
                KeyCombinations _action = InputModuleInfo.InputModulePart[selectID].ComboInputActions[index];

                Rect _ElementRect = new Rect(rect);
                _ElementRect.height = 18;
                _ElementRect.width = 80;
                using (new GUIColorScope(Color.green))
                {
                    if (GUI.Button(_ElementRect, "添加元素"))
                    {
                        _action.keyGroup.Add(119);
                    }
                }
                
                _ElementRect.x += _ElementRect.width + 5;
                _ElementRect.width = rect.width - _ElementRect.x-9f;
                if (comboInputListID[index] < 0)
                {
                    _action.mAction = EditorGUI.TextField(_ElementRect, _action.mAction);
                }
                else
                {
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        // using (new GUIColorScope(Color.gray * 1.5f))
                        {
                            comboInputListID[index] = EditorGUI.Popup(_ElementRect, comboInputListID[index],
                                InputActionList.Instance.ActionList.ToArray(), EditorStyles.popup);
                        }

                        if (_check.changed)
                        {
                            _action.mAction = InputActionList.Instance.ActionList[comboInputListID[index]];
                        }
                    }
                }
                
                _ElementRect.width = 30;
                _ElementRect.x = rect.x + rect.width - _ElementRect.width;
                using (new GUIColorScope(Color.red))
                {
                    if (GUI.Button(_ElementRect, "-"))
                    {
                        InputModuleInfo.InputModulePart[selectID].ComboInputActions.RemoveAt(index);
                        return;
                    }
                }
                
                //绘制连击元素
                float _elementWidth = rect.width / _action.keyGroup.Count;
                Rect _elementRect = new Rect(rect);
                _elementRect.height = 18;
                _elementRect.width = _elementWidth;
                _elementRect.y += 20;
                for (int i = 0; i < _action.keyGroup.Count; i++)
                {
                    _elementRect.x = _elementRect.width * i + 20;
                    DrawComboElement(_elementRect, _action.keyGroup, i);
                }


                Handles.color = Color.black * 0.5f;
                Handles.DrawLine(_Pos_S, _Pos_E);
            };
            comboInputList.elementHeight = 40;
            comboInputList.drawHeaderCallback = rect =>
            {
                GUI.Label(rect,"连击按键配置");
            };
            comboInputList.onAddCallback = list =>
            {
                CreactComboAction(InputActionList.Instance.ActionList[0], 0);
            };
        }

        private void ActionListDraw()
        {
            inpuActionList = new ReorderableList(InputActionList.Instance.ActionList,null,true,true,false,false);
            inpuActionList.drawElementCallback = (rect, index, active, focused) =>
            {
                if (index >= InputActionList.Instance.ActionList.Count)
                {
                    return;
                }
                float _buttonWindth = 30;
                rect.width -= _buttonWindth;
                EditorGUI.TextField(rect, InputActionList.Instance.ActionList[index]);
                rect.x += rect.width;
                rect.width = _buttonWindth;
                using (new GUIColorScope(Color.red))
                {
                    if (GUI.Button(rect, "-"))
                    {
                        InputActionList.Instance.ActionList.RemoveAt(index);
                    }
                }
            };
            inpuActionList.drawHeaderCallback = rect =>
            {
                GUI.Label(rect,"输入行为列表");
            };
            // string _debug = "";
            
        }
        private void DrawComboElement(Rect _rect, List<int> _comboList, int _index)
        {
            _rect.width -= 5;
            float _handlsWidth = 10;
            // int _returnValue = _comboList[_index];

            using (new GUIColorScope(Color.gray*1.5f))
            {
                //绘制键鼠切换键
                Rect _handlsL = new Rect(_rect);
                _handlsL.width = _handlsWidth;
                if (GUI.Button(_handlsL, ""))
                {
                    if (_comboList[_index] > -1)
                    {
                        _comboList[_index] = -1;
                    }
                    else
                    {
                        _comboList[_index] = 119; //w
                    }
                }

                //绘制主要按键
                Rect _handlsC = new Rect(_rect);
                _handlsC.x += _handlsWidth;
                _handlsC.width -= _handlsWidth * 2;
                if (_comboList[_index] > -1)
                {
                    KeyCode _getValue = (KeyCode)EditorGUI.EnumPopup(_handlsC, (KeyCode)_comboList[_index]);
                    _comboList[_index] = (int)_getValue;
                }
                else
                {
                    int _selectID = _comboList[_index] * -1 - 1;
                    int _selectMoustButton = EditorGUI.Popup(_handlsC, _selectID, mousbuttonType);
                    _comboList[_index] = _selectMoustButton * -1 - 1;
                }


                //绘制删除按键
                Rect _handlsR = new Rect(_rect);
                _handlsR.width = _handlsWidth;
                _handlsR.x += _rect.width - _handlsWidth;

                if (GUI.Button(_handlsR, "-"))
                {
                    if (_comboList.Count > 2)
                    {
                        _comboList.RemoveAt(_index);
                    }
                }
            }
        }

        #endregion
        
        #region 数据存取

        private void LoadJson()
        {

            string _path = ActionWindowMain.ActionEditorFuntion.GetInputModule();

            string _str = AssetDatabase.LoadAssetAtPath<TextAsset>(_path).text;
            InputModuleInfo = new InputModuleInfo(null, 0);
            JsonUtility.FromJsonOverwrite(_str, InputModuleInfo);
            if (InputModuleInfo == null) InputModuleInfo = new InputModuleInfo(new List<InputModulePart>(), 0);
            selectID = InputModuleInfo.InputSystemID;

        }

        private void SaveJson()
        {
            InputActionList.Instance.SaveActionList();
            // string _path = RunTime.MotionEngineRuntimePath.Instance.InputModulePath();
            string _path = ActionWindowMain.ActionEditorFuntion.GetInputModule();
            // _path = ActionWindowMain.GetAssetPathToResources(_path);
            // Debug.Log("保存路径: " + _path);

            string _str = JsonUtility.ToJson(InputModuleInfo);
            File.WriteAllText(_path,_str);
            AssetDatabase.Refresh();
            EngineDebug.Log("保存Input配置列表成功");
        }
        #endregion
    }
}
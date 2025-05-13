using System;
using System.Collections.Generic;
using System.Reflection;
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.Graph;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class BluePrintWindows_ReturnValue
    {
#region Enum
        private enum EInteractType
        {
            Default,
            CreactNode,
            Handle_Head,
            Handle_Foot,
        }
        private enum EValType
        {
            NotValid,
            Point,
            Unit,
            String,
            Transform,
            Float,
            Int,
            Vector3,
            Bool,
        }
#endregion

        private EInteractType InteractType = EInteractType.Default;
        private EValType ValType = EValType.NotValid;
        private void DrawNodeValueGUI(Rect nodeHeadRect,NodeDisplay nodeDisplay)
        {
            Color inputFootBack = Color.white * 0.15f;
            inputFootBack.a = 1;
            Color inputFootBack2 = Color.white * 0.2f;
            inputFootBack2.a = 1;

            objectValIndex = 1;
            foreach (NodeDisplayValue VARIABLE in nodeDisplay.NodeValues)
            {
                objectValIndex++;
                //更新位置和高度
                nodeHeadRect.y += nodeHeadRect.height;
                nodeHeadRect.height = VARIABLE.height;
                
                if (VARIABLE.drawValData.IsInput)
                {
                    //绘制引脚
                    Rect inputRect = new Rect(nodeHeadRect);
                    inputRect.width = 20;
                    inputRect.height = 20;

                    if (InteractType == EInteractType.Default)
                    {
                        //引脚回调
                        Handl_Foot_CallBack(inputRect, VARIABLE, nodeDisplay.Main);
                        EditorGUIUtility.AddCursorRect(inputRect, MouseCursor.Link);
                    }
                    
                    //目标参数
                    object _val = GetProperty(nodeDisplay.Main, VARIABLE.valName);

                    if (InteractType == EInteractType.Handle_Head)
                    {
                        if (GetValIsType(_val))
                        {
                            EditorGUIUtility.AddCursorRect(inputRect, MouseCursor.Link);
                            if (inputRect.Contains(Event.current.mousePosition))
                            {
                                SetObject = nodeDisplay.Main;
                                SetObjectVal = VARIABLE.valName;
                                _SetObjectValue = VARIABLE;
                            }
                        }
                        else
                        {
                            EditorGUI.DrawRect(nodeHeadRect, Color.black * 0.3f);
                        }
                    }

                    //是否有赋值
                    bool isValid = _val is not null && VARIABLE._inputTarget is not null;

                    inputRect.height = 12;
                    inputRect.width = 12;
                    inputRect.x += 4;
                    inputRect.y += 4;
                    EditorGUI.DrawRect(inputRect, inputFootBack);
                    if (isValid)
                    {
                        inputRect.height = 8;
                        inputRect.width = 8;
                        inputRect.x += 2;
                        inputRect.y += 2;
                        EditorGUI.DrawRect(inputRect, inputFootBack2);
                        
                        //添加引线
                        Vector2 newPos = inputRect.position;
                        newPos.y += inputRect.height / 2;
                        bluePrintLines.Add(new BluePrintLine(Vector2.zero,newPos,GetProperty(nodeDisplay.Main, VARIABLE.valName).GetHashCode(),objectValIndex));
                        
                        // //绘制上一个节点
                        // DrawBluePrintNode(VARIABLE._inputTarget, nodeID + 1);
                    }

                    Rect bodyRect = new Rect(nodeHeadRect);
                    bodyRect.x += 20;
                    bodyRect.width = nodeHeadRect.width - 20;
                    
                    //绘制当前带引脚的变量
                    if (VARIABLE.drawFuntion is not null)
                    {
                        // EditorGUI.DrawRect(bodyRect, Color.red);
                        VARIABLE.drawFuntion(bodyRect, GetProperty(nodeDisplay.Main, VARIABLE.valName), VARIABLE);
                    }
                    else
                    {
                        bodyRect.width = VARIABLE.drawValData.LabelWidth;
                        GUI.Label(bodyRect,new GUIContent(VARIABLE.drawValData.PropertyName,VARIABLE.drawValData.Tooltip));
                    }
                }
                else
                {

                    if (VARIABLE.drawFuntion is not null)
                    {
                        VARIABLE.drawFuntion(nodeHeadRect, GetProperty(nodeDisplay.Main, VARIABLE.valName), VARIABLE);
                    }
                    else
                    {
                        EditorGUI.DrawRect(nodeHeadRect, Color.red * 0.7f);
                        Rect lableRect = new Rect(nodeHeadRect);
                        lableRect.width = VARIABLE.drawValData.LabelWidth;
                        GUI.Label(lableRect,new GUIContent(VARIABLE.drawValData.PropertyName,VARIABLE.drawValData.Tooltip));
                        
                        lableRect.x += lableRect.width;
                        lableRect.width = nodeHeadRect.width - lableRect.width;
                        GUI.Label(lableRect,$" 未实现绘制:[{VARIABLE.valName}]", centerGUI);
                    }
                }
            }
        }

        private void SetValType(object _value)
        {
            if (_value is BluePrint_Int)
            {
                ValType = EValType.Int;
            }else if (_value is BluePrint_Float)
            {
                ValType = EValType.Float;
            }else if (_value is BluePrint_Vector3)
            {
                ValType = EValType.Vector3;
            }else if (_value is BluePrint_Unit)
            {
                ValType = EValType.Unit;
            }else if (_value is BluePrint_PointData)
            {
                ValType = EValType.Point;
            }else if (_value is BluePrint_Transform)
            {
                ValType = EValType.Transform;
            }
            else if (_value is BluePrint_Bool)
            {
                ValType = EValType.Bool;
            }
            else
            {
                EngineDebug.LogError($"无效参数: {_value.GetType().Name}");
                ValType = EValType.NotValid;
            }
        }

        private bool GetValIsType(object _value)
        {
            if (ValType == EValType.Bool) return _value is BluePrint_Bool;
            if (ValType == EValType.Int)   return _value is BluePrint_Int;
            if (ValType == EValType.Unit)  return _value is BluePrint_Unit;
            if (ValType == EValType.Float) return _value is BluePrint_Float;
            if (ValType == EValType.Point) return _value is BluePrint_PointData;

            if (ValType == EValType.Vector3) return _value is BluePrint_Vector3;
            if (ValType == EValType.Transform) return _value is BluePrint_Transform;
            return false;
        }
    }
}
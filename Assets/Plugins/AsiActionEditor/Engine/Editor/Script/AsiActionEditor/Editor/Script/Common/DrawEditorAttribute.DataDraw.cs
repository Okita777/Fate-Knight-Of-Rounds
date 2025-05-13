using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class DrawEditorAttribute
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawRayHitData(RayCastData _castData, int _LayerConut = 0)
        {
            //头部高度
            int _RectHeight = 15 * 2;

            //底部高度
            int _endHeight = 10;

            //成员高度
            int _memberHeight = 20 * 5;

            if (_castData.m_Opne_SetGValue)
            {
                _memberHeight += 20 * 5;
            }

            //间隔距离
            int _interact = 5 * 3;

            using (var scroll = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scroll.scrollPosition;
                //预留空间
                Rect _rect =
                    EditorGUILayout.GetControlRect(
                        GUILayout.Height(_RectHeight + _interact + _memberHeight + _endHeight));

                //右移动次数
                _rect.x += _LayerConut * 20;
                if (_rect.x >= _rect.width) return;
                _rect.width -= _rect.x;

                //绘制背景
                EditorGUI.DrawRect(_rect, Color.black * 0.1f);

                //绘制头部背景
                _rect.height = 20;
                EditorGUI.DrawRect(_rect, Color.black * 0.1f);
                // using (new GUIColorScope(Color.blue))
                {
                    GUI.Label(_rect, $" {_LayerConut}级 射线 ----------------------------------------------------");
                }

                //添加间隔
                _rect.y += 5;

                //设置成员高度
                _rect.height = 20;

                //绘制成员
                _rect.y += _rect.height;
                using (new GUIColorScope(Color.cyan))
                {
                    GUI.Label(_rect, "常规参数设置");
                }

                // _rect.y += _rect.height;
                // DrawSelectTransform(_rect, "选择射线参考点", _castData.m_SelectTransform);
                // _rect.y += _rect.height;
                // DrawEVector3(_rect, "位置偏移", _castData.m_OffsetPosition);
                // _rect.y += _rect.height;
                // DrawEVector3(_rect, "旋转偏移", _castData.m_OffsetRotation);
                // _rect.y += _rect.height;
                // _castData.m_Distance = DrawFloat(_rect, "射线长度", _castData.m_Distance);
                _rect.y += _rect.height;
                DrawGraphEditorAttribute.Instance.DrawGraphButton(_rect, "m_point", _castData.m_point, "起始点", "射线的起始位置和角度");
                
                //添加间隔
                _rect.y += 5;
                
                _rect.y += _rect.height;
                DrawGraphEditorAttribute.Instance.DrawGraphButton(_rect, "m_length", _castData.m_length, "射线长度", "从起始点出发沿起始点方向前行距离");
                _rect.y += _rect.height;
                _castData.m_LayerMask = DrawLayerField(_rect, "检测层级", _castData.m_LayerMask);
                // DrawEVector3(_rect, "检测层级", _castData.m_OffsetRotation);
                _rect.y += _rect.height;
                DrawSetGBool(_rect, "射线是否成功检测到对象", _castData.m_SetGBool);

                //添加间隔
                _rect.y += 5;

                //Gvalue设置列表展开和收起
                Rect _rect2 = new Rect(_rect);
                _rect2.y += _rect2.height;
                _rect2.height = 15;
                if (Event.current.type == EventType.MouseDown)
                {
                    if (_rect2.Contains(Event.current.mousePosition))
                    {
                        _castData.m_Opne_SetGValue = !_castData.m_Opne_SetGValue;
                    }

                    InspectorWindow.Instance.Repaint();
                }

                if (_castData.m_Opne_SetGValue)
                {
                    EditorGUI.DrawRect(_rect2, Color.black * 0.1f);
                    Rect _rect3 = new Rect(_rect2);
                    _rect3.height += 20 * 5;
                    EditorGUI.DrawRect(_rect3, Color.black * 0.1f);
                    using (new GUIColorScope(Color.green))
                    {
                        GUI.Label(_rect2, " -  GValue设置列表");
                    }


                    //绘制成员
                    _rect.y = _rect2.height + _rect2.y;

                    DrawSetGTransform(_rect, "射线接触目标后写入的对象", _castData.m_SetGtransform);
                    _rect.y += _rect.height;
                    DrawSetGPoint(_rect, "射线接触目标后写入碰撞点位置", _castData.m_SetPoint);
                    _rect.y += _rect.height;
                    DrawSetGFloat(_rect, "射线接触目标后写入碰撞距离", _castData.m_SetFloat);
                    _rect.y += _rect.height;
                    DrawSetGString(_rect, "射线接触目标后写入碰撞对象名称", _castData.m_SetString);
                    _rect.y += _rect.height;
                    DrawSetGString(_rect, "射线接触目标后写入碰撞对象物理材质名称", _castData.m_SetString2);
                    _rect.y += _rect.height;
                }
                else
                {
                    EditorGUI.DrawRect(_rect2, Color.black * 0.2f);
                    using (new GUIColorScope(Color.green))
                    {
                        GUI.Label(_rect2, " + GValue设置列表");
                    }
                    _rect.y = _rect2.height + _rect2.y;
                }

                // //添加间隔
                // _rect.y += 5;
                //
                // //绘制进入列表
                // _rect2 = new Rect(_rect);
                // _rect2.height = 15;
                // if (Event.current.type == EventType.MouseDown)
                // {
                //     if (_rect2.Contains(Event.current.mousePosition))
                //     {
                //         _castData.m_Opne_CastData = !_castData.m_Opne_CastData;
                //     }
                //
                //     InspectorWindow.Instance.Repaint();
                // }
                // if (_castData.m_Opne_CastData)
                // {
                //     EditorGUI.DrawRect(_rect2, Color.black * 0.2f);
                //     // Rect _rect3 = new Rect(_rect2);
                //     // _rect3.height += 20 * 5;
                //     // EditorGUI.DrawRect(_rect3, Color.black * 0.1f);
                //     using (new GUIColorScope(Color.yellow))
                //     {
                //         GUI.Label(_rect2, " -  GValue成功检测后衍生列表");
                //     }
                //
                //     //绘制成员
                //     _rect.y = _rect2.height + _rect2.y;
                //
                // }
                // else
                // {
                //     EditorGUI.DrawRect(_rect2, Color.black * 0.2f);
                //     using (new GUIColorScope(Color.yellow))
                //     {
                //         GUI.Label(_rect2, " + GValue成功检测后衍生列表");
                //     }
                // }
                // // return;

                // //添加间隔
                // _rect.y = _rect2.y + _rect2.height + 5;
                //
                // //绘制出去列表
                // _rect2 = new Rect(_rect);
                // _rect2.height = 15;
                // if (Event.current.type == EventType.MouseDown)
                // {
                //     if (_rect2.Contains(Event.current.mousePosition))
                //     {
                //         _castData.m_Opne_CastData_NotHit = !_castData.m_Opne_CastData_NotHit;
                //     }
                //
                //     InspectorWindow.Instance.Repaint();
                // }
                // if (_castData.m_Opne_CastData_NotHit)
                // {
                //     EditorGUI.DrawRect(_rect2, Color.black * 0.2f);
                //     // Rect _rect3 = new Rect(_rect2);
                //     // _rect3.height += 20 * 5;
                //     // EditorGUI.DrawRect(_rect3, Color.black * 0.1f);
                //     using (new GUIColorScope(Color.yellow))
                //     {
                //         GUI.Label(_rect2, " -  GValue未成功检测后衍生列表");
                //     }
                //
                //     //绘制成员
                //     _rect.y = _rect2.height + _rect2.y;
                //
                // }
                // else
                // {
                //     EditorGUI.DrawRect(_rect2, Color.black * 0.2f);
                //     using (new GUIColorScope(Color.yellow))
                //     {
                //         GUI.Label(_rect2, " + GValue未成功检测后衍生列表");
                //     }
                // }
                
                // GUILayout.Label("Hit Data2");
            }
        }
    }
}
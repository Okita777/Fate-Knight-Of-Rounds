using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class ResourcesWindow
    {
        private readonly string[] mMenu = new string[]
        {
            "Unit",
            "Action",
            "GValue",
            "Bullet",
            "Item",
            "Camera"
        };
        private void DrawPreviewGUI()
        {
            OnDrawMenuHead();
            if (mSelectmenuID == 0)
            {
                DrawUnitGUI();
            }
            else if (mSelectmenuID == 1)
            {
                DrwaActionStateGUI();
            }
            else if (mSelectmenuID == 2)
            {
                DrwaGValueGUI();
            }
            else if (mSelectmenuID == 3)
            {
                // DrwaActionStateGUI();
            }
            else if (mSelectmenuID == 4)
            {
                DrawWeaponGUI();
            }
            else if (mSelectmenuID == 5)
            {
                DrawCameraGroupGUI();//整体
            }
        }

        private void OnDrawMenuHead()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUIStyle _guiStyle = new GUIStyle();
                _guiStyle.normal.textColor = Color.white;
                _guiStyle.alignment = TextAnchor.MiddleCenter;
                for (int i = 0; i < mMenu.Length; i++)
                {
                    bool _isSelect = i == mSelectmenuID;
                    string _buttonName = _isSelect ? $"--> {mMenu[i]} <--" : mMenu[i];
                    using (new GUIColorScope(Color.gray, _isSelect))
                    {
                        if (GUILayout.Button(_buttonName, EditorStyles.toolbarButton))
                        {
                            OnSelectMenu(i);

                        }
                    }
                }
            }
            Handles.color = Color.black;
            float _height = EditorGUIUtility.singleLineHeight + 2f;
            Handles.DrawLine(new Vector2(0,_height),new Vector2(position.width,_height));

        }
    }
}
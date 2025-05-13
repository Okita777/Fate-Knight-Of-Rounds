using UnityEditor;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public partial class ResourcesWindow
    {
        private void DrawWeaponGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("预览", EditorStyles.toolbarButton))
                {
                    
                }
                
                if (GUILayout.Button("复制", EditorStyles.toolbarButton))
                {
                    
                }

                if (GUILayout.Button("新建", EditorStyles.toolbarButton))
                {
                    
                }

                if (GUILayout.Button("删除", EditorStyles.toolbarButton))
                {
                    
                }

                if (GUILayout.Button("保存", EditorStyles.toolbarButton))
                {
                    
                }
                
            }
        }
    }
}
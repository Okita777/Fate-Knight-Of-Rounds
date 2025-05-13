using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace AsiActionEngine.Editor
{
    public partial class InspectorWindow : EditorWindow
    {
        #region Instance
        private static InspectorWindow _instance;
        public static InspectorWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetWindow<InspectorWindow>();
                    _instance.titleContent = new GUIContent("Inspector");
                }

                return _instance;
            }
        }

        #endregion

        private Dictionary<string, List<PropertyDrawer>>
            m_propertyDict = new Dictionary<string, List<PropertyDrawer>>();
        
        public bool needInit = true;

        private void OnEnable()
        {
            needInit = true;
        }

        private void OnGUI()
        {
            DrawPerviewGUI();
        }
    }
}
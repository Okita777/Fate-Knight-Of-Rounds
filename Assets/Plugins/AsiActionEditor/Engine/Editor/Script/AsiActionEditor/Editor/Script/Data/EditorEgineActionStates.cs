using UnityEngine;

namespace AsiActionEngine.Editor
{
    public class EditorEgineActionStates : ScriptableObject
    {
        public EditorActionStateInfo mEditorActionStateInfo;
        public AsiActionEventSelectID mAsiActionEventSelectID = new AsiActionEventSelectID(-1,0,0,0,0);
        // public int mSelectAction;
        public int mInputID;
        public bool mOnlySelected;//只是变换选择对象而已
    }
}
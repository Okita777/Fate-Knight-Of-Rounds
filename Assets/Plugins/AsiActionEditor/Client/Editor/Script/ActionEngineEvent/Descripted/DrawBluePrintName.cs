using AsiActionEngine.RunTime.Graph;

namespace AsiTimeLine.Editor
{
    public class DrawBluePrintName
    {
        public static string GetName(string name)
        {
            switch (name)
            {
                case nameof(GraphEvent_TrackData_Transform_CamMain):
                    return "相机 (Transform)";
                case nameof(GraphEvent_TrackData_Vector_InputDir):
                    return "输入方向 (Vector3)";
                case nameof(GraphEvent_TrackData_Unit_Player):
                    return "玩家单位 (Unit)";
                case nameof(GraphEvent_Other_Float_Distance):
                    return "距离 (float)";
                case nameof(GraphEvent_Other_Float_Dot):
                    return "点积 (float)";
                case nameof(GraphEvent_Other_Vector3_RotVector):
                    return "旋转Vector3 (Vector3)";
                case nameof(GraphEvent_Other_Vector3_Angle):
                    return "角度差 (float)";
            }
            return string.Empty;
        }
    }
}
using AsiActionEngine.RunTime;
using AsiActionEngine.RunTime.Graph;

namespace AsiTimeLine.Editor
{
    public class CreactBluePrint
    {
        public static IProperty CreateBluePrint(string bluePrintName)
        {
            switch (bluePrintName)
            {
                case nameof(GraphEvent_TrackData_Transform_CamMain):
                    return new GraphEvent_TrackData_Transform_CamMain();
                case nameof(GraphEvent_TrackData_Vector_InputDir):
                    return new GraphEvent_TrackData_Vector_InputDir();
                case nameof(GraphEvent_TrackData_Unit_Player):
                    return new GraphEvent_TrackData_Unit_Player();
                case nameof(GraphEvent_Other_Float_Distance):
                    return new GraphEvent_Other_Float_Distance();
                case nameof(GraphEvent_Other_Float_Dot):
                    return new GraphEvent_Other_Float_Dot();
                case nameof(GraphEvent_Other_Vector3_RotVector):
                    return new GraphEvent_Other_Vector3_RotVector();
                case nameof(GraphEvent_Other_Vector3_Angle):
                    return new GraphEvent_Other_Vector3_Angle();
            }

            return null;
        }
    }
}
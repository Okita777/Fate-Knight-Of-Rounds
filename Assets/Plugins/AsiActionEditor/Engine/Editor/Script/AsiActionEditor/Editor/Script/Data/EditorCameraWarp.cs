using AsiActionEngine.RunTime;

namespace AsiActionEngine.Editor
{
    public class EditorCameraWarp : CameraWarp
    {
        public EditorCameraWarp(int _id, string _name)
        {
            ID = _id;
            Name = _name;
        }

        public CameraWarp GetCameraWarp()
        {
            CameraWarp _cameraWarp = new CameraWarp();

            _cameraWarp.Name = Name;
            _cameraWarp.ID = ID;
            _cameraWarp.DefaultCamID = DefaultCamID;
            _cameraWarp.ModelPath = mModelPath;
        
            return _cameraWarp;
        }
    }
}
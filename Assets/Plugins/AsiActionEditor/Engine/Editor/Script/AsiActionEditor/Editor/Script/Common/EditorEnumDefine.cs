using System;
using System.ComponentModel;

namespace AsiActionEngine.Editor
{
    public enum EPreviweState
    {
        Play,
        Pause,
        Stop,
    }

    public enum EMouseEvent
    {
        Empty,
        MouseDwonLeft,
        MouseDwonRight,
        MouseUp,
        MouseClick,
        MouseDrag
    }

    
    public class EnumUtinity
    {
        public static string GetDescription2(Enum _enum)
        {
            var _field = _enum.GetType().GetField(_enum.ToString());
            var customAttribute = Attribute.GetCustomAttribute(_field, typeof(DescriptionAttribute));
            return customAttribute == null ? _enum.ToString() : ((DescriptionAttribute)customAttribute).Description;
            // return "";
        }
    }
}
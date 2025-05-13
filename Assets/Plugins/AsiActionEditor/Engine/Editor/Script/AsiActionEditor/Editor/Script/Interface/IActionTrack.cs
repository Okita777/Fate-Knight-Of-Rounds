using System;
using System.Collections.Generic;
using AsiActionEngine.RunTime;
using UnityEngine;

namespace AsiActionEngine.Editor
{
    public interface IActionTrack
    {
        //绘制开关
        public bool IsDraw { get; set; }

        //绘制
        public abstract void Draw(Rect _rect, float _DrawStart, float _DrawEnd);
        
        //装载回调
        public abstract void OnLoadEvent();
        
        //卸载回调
        public abstract void UnloadEvent();

        public abstract float GetHeight();

        public abstract IActionTrack Clone();
    }
}
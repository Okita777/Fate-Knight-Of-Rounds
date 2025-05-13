using System;
using System.Collections.Generic;
using UnityEngine;

namespace AsiTimeLine.RunTime
{
    public class ActionEditor_Effects : MonoBehaviour
    {
        public List<ParticleSystem> particleSystems = new List<ParticleSystem>();
        public float Life = 2;

        public void Play()
        {
            foreach (var VARIABLE in particleSystems)
            {
                VARIABLE.Play();
            }
        }

        public bool useAutoRandomSeed
        {
            get { return false; }
            set
            {
                foreach (var VARIABLE in transform.GetComponentsInChildren<ParticleSystem>())
                {
                    VARIABLE.useAutoRandomSeed = value;
                }
            }
        }
    }
}
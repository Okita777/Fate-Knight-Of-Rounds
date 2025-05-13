using System;
using UnityEngine;

namespace AsiTimeLine.Editor
{
    public class DestoryToRuntime : MonoBehaviour
    {
        private void Start()
        {
            Destroy(gameObject);
        }
    }
}
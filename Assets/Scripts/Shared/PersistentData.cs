using System;
using UnityEngine;

namespace Shared
{
    public class PersistentData : MonoBehaviour
    {
        public static PersistentData Instance;
        
        public string Username { get; set; }
        public string TargetIP { get; set; }

        public void Awake()
        {
            Instance = this;
        }
    }
}
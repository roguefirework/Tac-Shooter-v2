using Riptide.Utils;
using UnityEngine;

namespace Logging
{
    public class Logging
    {
        private static Logging _instance;
        private static readonly object InstanceLock = new object();
        public static Logging Instance
        {
            get
            {
                lock (InstanceLock)
                {
                    return _instance ??= new Logging();
                }
            }
        }
        private Logging()
        {
            
        }


        private bool _loggerInitialized = false;

        public void SetupRiptideLogger()
        {
            if (_loggerInitialized) return;
            
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            _loggerInitialized = true;

        }
        
    }
}
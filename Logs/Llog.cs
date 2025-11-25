using UnityEngine;

namespace SoMTools.Logs
{
    public static class Llog 
    {
        public static void Log(string message)
        {
            Debug.Log("[SoMTools] " + message);
        }
    }
}

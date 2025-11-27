using UnityEngine;

namespace SoMTools.Event
{
    // 범용 고정 구조체 (int, Vector3, string, GameObject)
    [System.Serializable]
    public struct SoMFixedEventData
    {
        public int intValue;
        public Vector3 vector3Value;
        public string stringValue;
        public GameObject gameObjectValue;
    }
}

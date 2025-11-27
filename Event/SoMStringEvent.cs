using UnityEngine;
using System.Collections.Generic;

namespace SoMTools.Event
{
    // 유동적 데이터용 문자열 이벤트
    [CreateAssetMenu(fileName = "New String Event", menuName = "SoMLib/SoMStringEvent (다이나믹 문자열)")]
    public class SoMStringEvent : ScriptableObject
    {
        private readonly List<ISoMStringEventListener> listeners = new List<ISoMStringEventListener>();

        public IReadOnlyList<ISoMStringEventListener> Listeners => listeners;

        public void Raise(string payload)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(payload);
            }
        }

        public void RegisterListener(ISoMStringEventListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        public void UnregisterListener(ISoMStringEventListener listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }
    }
}

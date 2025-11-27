using UnityEngine;
using System.Collections.Generic;

namespace SoMTools.Event
{
    // 고정 구조체 기반 이벤트
    [CreateAssetMenu(fileName = "New Fixed Event", menuName = "SoMTools/Event/SoMFixedEvent (확정된 구조체)")]
    public class SoMFixedEvent : ScriptableObject
    {
        private readonly List<ISoMFixedEventListener> listeners = new List<ISoMFixedEventListener>();

        public IReadOnlyList<ISoMFixedEventListener> Listeners => listeners;

        public void Raise(SoMFixedEventData data)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(data);
            }
        }

        public void RegisterListener(ISoMFixedEventListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        public void UnregisterListener(ISoMFixedEventListener listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

namespace SoMTools.Event
{
    // 고정 구조체 이벤트 리스너
    public class SoMFixedEventListener : MonoBehaviour, ISoMFixedEventListener
    {
        [Tooltip("수신할 GameEvent (고정 구조체)")]
        public SoMFixedEvent gameEvent;

        [Tooltip("이벤트 발생 시 실행될 응답")]
        public UnityEvent<SoMFixedEventData> response;

        private void OnEnable()
        {
            if (gameEvent != null)
            {
                gameEvent.RegisterListener(this);
            }
        }

        private void OnDisable()
        {
            if (gameEvent != null)
            {
                gameEvent.UnregisterListener(this);
            }
        }

        public void OnEventRaised(SoMFixedEventData data)
        {
            response?.Invoke(data);
        }
    }
}

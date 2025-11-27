using UnityEngine;
using UnityEngine.Events;

namespace SoMTools.Event
{
    // 문자열 이벤트 리스너
    public class SoMStringEventListener : MonoBehaviour, ISoMStringEventListener
    {
        [Tooltip("수신할 SoMStringEvent (유동적 데이터)")]
        public SoMStringEvent gameEvent;

        [Tooltip("이벤트 발생 시 실행될 응답")]
        public UnityEvent<string> response;

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

        public void OnEventRaised(string payload)
        {
            response?.Invoke(payload);
        }
    }
}

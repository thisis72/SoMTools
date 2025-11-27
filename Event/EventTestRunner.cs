using UnityEngine;
using UnityEngine.InputSystem;


namespace SoMTools.Event
{
    // 이벤트 테스트용 스크립트
    public class EventTestRunner : MonoBehaviour
    {
        [Header("고정 구조체 이벤트")]
        public SoMFixedEvent fixedEvent;
        public SoMFixedEvent playerDamageEvent;
        public SoMFixedEvent scoreEvent;

        [Header("문자열 이벤트")]
        public SoMStringEvent stringEvent;
        public SoMStringEvent complexEvent;
        public SoMStringEvent itemEvent;

        private int currentScore = 0;

        void Update()
        {
            // ===== 고정 구조체 이벤트 =====
            
            // Space: 범용 고정 이벤트
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (fixedEvent != null)
                {
                    var data = new SoMFixedEventData
                    {
                        intValue = Random.Range(1, 100),
                        vector3Value = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)),
                        stringValue = "테스트 메시지",
                        gameObjectValue = gameObject
                    };
                    fixedEvent.Raise(data);
                }
            }

            // 1: 플레이어 데미지 (고정 구조체)
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                if (playerDamageEvent != null)
                {
                    var data = new SoMFixedEventData
                    {
                        intValue = Random.Range(10, 50), // damage
                        vector3Value = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f)), // hitPosition
                        stringValue = "Orc", // attackerName
                        gameObjectValue = null
                    };
                    playerDamageEvent.Raise(data);
                }
            }

            // 2: 점수 변경 (고정 구조체)
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                if (scoreEvent != null)
                {
                    currentScore += Random.Range(10, 100);
                    var data = new SoMFixedEventData
                    {
                        intValue = currentScore,
                        vector3Value = Vector3.zero,
                        stringValue = "",
                        gameObjectValue = null
                    };
                    scoreEvent.Raise(data);
                }
            }

            // ===== 문자열 이벤트 =====

            // 3: 단순 문자열
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                if (stringEvent != null)
                {
                    stringEvent.Raise("Simple string message at " + Time.time.ToString("F2"));
                }
            }

        // 4: 복잡한 구조 (PayloadBuilder)
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            if (complexEvent != null)
            {
                string payload = SoMStringEventHelper.Create()
                    .Add("type", "Quest")
                    .Add("level", Random.Range(1, 50))
                    .Add("progress", Random.Range(0f, 1f))
                    .Add("complete", Random.value > 0.5f)
                    .Build();
                complexEvent.Raise(payload);
            }
        }            // 5: 아이템 획득 (문자열)
            if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                if (itemEvent != null)
                {
                    string[] items = { "Health Potion", "Mana Potion", "Gold Coin", "Diamond" };
                    string itemName = items[Random.Range(0, items.Length)];
                    
                    string payload = SoMStringEventHelper.Build(sb =>
                    {
                        sb.Append("item=").Append(itemName);
                        sb.Append("|id=").Append(Random.Range(1000, 9999));
                        sb.Append("|qty=").Append(Random.Range(1, 20));
                    });
                    itemEvent.Raise(payload);
                }
            }
        }
    }
}

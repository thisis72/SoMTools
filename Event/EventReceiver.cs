using UnityEngine;

namespace SoMTools.Event
{
    // 이벤트 수신 예제
    public class EventReceiver : MonoBehaviour
    {
        // ===== 고정 구조체 이벤트 핸들러 =====
        
        public void OnFixedEvent(SoMFixedEventData data)
        {
            Debug.Log($"[고정] int={data.intValue}, vec3={data.vector3Value}, string={data.stringValue}, obj={data.gameObjectValue?.name ?? "null"}");
        }
        
        public void OnPlayerDamage(SoMFixedEventData data)
        {
            // intValue = damage, vector3Value = hitPosition, gameObjectValue = attacker
            Debug.Log($"플레이어 피해: {data.intValue} 데미지, 위치: {data.vector3Value}, 공격자: {data.gameObjectValue?.name}");
        }
        
        public void OnScoreChanged(SoMFixedEventData data)
        {
            // intValue = newScore
            Debug.Log($"점수 변경: {data.intValue}점");
        }
        
        // ===== 문자열 이벤트 핸들러 =====
        
        public void OnStringEvent(string payload)
        {
            Debug.Log($"[문자열] 수신: {payload}");
        }
        
        public void OnComplexData(string payload)
        {
            var data = SoMStringEventHelper.Parse(payload);
            
            string type = SoMStringEventHelper.GetString(data, "type");
            int level = SoMStringEventHelper.GetInt(data, "level");
            float progress = SoMStringEventHelper.GetFloat(data, "progress");
            
            Debug.Log($"복잡한 데이터: type={type}, level={level}, progress={progress:F2}");
        }
        
        public void OnItemCollected(string payload)
        {
            var data = SoMStringEventHelper.Parse(payload);
            
            string itemName = SoMStringEventHelper.GetString(data, "item");
            int itemId = SoMStringEventHelper.GetInt(data, "id");
            int quantity = SoMStringEventHelper.GetInt(data, "qty");
            
            Debug.Log($"아이템 획득: {itemName} (ID: {itemId}) x{quantity}");
        }
    }
}

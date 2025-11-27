using SoMTools.Event;
using UnityEngine;

public class TestSoMEvent : MonoBehaviour
{
    [Header("Fixed Event Test")]
    public SoMFixedEvent testFixedEvent;
    public SoMFixedEventData testFixedEventData;

    [Header("String Event Test")]
    public SoMStringEvent testStringEvent;
    [TextArea(3, 10)]
    public string testStringPayload = "type=test|value=100|active=true";

    [ContextMenu("Raise Fixed Event")]
    public void RaiseFixedEvent()
    {
        if (testFixedEvent != null)
        {
            testFixedEvent.Raise(testFixedEventData);
            Debug.Log($"[TestSoMEvent] Fixed Event Raised: int={testFixedEventData.intValue}, " +
                      $"vector={testFixedEventData.vector3Value}, string={testFixedEventData.stringValue}");
        }
        else
        {
            Debug.LogWarning("[TestSoMEvent] testFixedEvent is null!");
        }
    }

    [ContextMenu("Raise String Event")]
    public void RaiseStringEvent()
    {
        if (testStringEvent != null)
        {
            testStringEvent.Raise(testStringPayload);
            Debug.Log($"[TestSoMEvent] String Event Raised: {testStringPayload}");
        }
        else
        {
            Debug.LogWarning("[TestSoMEvent] testStringEvent is null!");
        }
    }
}

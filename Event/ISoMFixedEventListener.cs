namespace SoMTools.Event
{
    // 리스너 인터페이스
    public interface ISoMFixedEventListener
    {
        // 이벤트 발생 시 호출되는 메서드
        void OnEventRaised(SoMFixedEventData data);
    }
}

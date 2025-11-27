#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace SoMTools.Event
{
    /// <summary>
    /// SoMFixedEvent의 Inspector 표시를 커스터마이징하여 등록된 리스너 목록을 표시합니다.
    /// </summary>
    [CustomEditor(typeof(SoMFixedEvent))]
    public class SoMFixedEventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SoMFixedEvent eventAsset = (SoMFixedEvent)target;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("등록된 리스너", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox($"현재 등록된 리스너 수: {eventAsset.Listeners.Count}", MessageType.Info);

            if (eventAsset.Listeners.Count == 0)
            {
                EditorGUILayout.HelpBox("등록된 리스너가 없습니다. (플레이 모드에서만 확인 가능)", MessageType.None);
                return;
            }

            EditorGUI.indentLevel++;
            foreach (var listener in eventAsset.Listeners)
            {
                if (listener == null) continue;

                // MonoBehaviour로 캐스팅 시도
                if (listener is MonoBehaviour mono)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    // GameObject 이름과 활성화 상태
                    string activeSymbol = mono.gameObject.activeInHierarchy ? "✓" : "✗";
                    string enabledSymbol = mono.enabled ? "◉" : "○";
                    
                    EditorGUILayout.LabelField($"{activeSymbol}{enabledSymbol}", GUILayout.Width(30));
                    EditorGUILayout.ObjectField(mono.gameObject, typeof(GameObject), true, GUILayout.Width(150));
                    EditorGUILayout.LabelField($"({mono.GetType().Name})", EditorStyles.miniLabel);
                    
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.LabelField($"- {listener.GetType().Name}");
                }
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("✓ = GameObject 활성, ◉ = Component 활성", MessageType.None);

            // 플레이 모드에서는 자동 갱신
            if (Application.isPlaying)
            {
                Repaint();
            }
        }
    }
}
#endif

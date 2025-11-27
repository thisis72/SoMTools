using UnityEngine;
using System.Collections.Generic;
using System.Text;

// =====================================================================================
// SoMStringEventHelper
// 문자열 기반 이벤트 페이로드를 효율적으로 생성/파싱/재사용하기 위한 통합 유틸리티.
// 주요 기능:
//   1) StringBuilder 풀링으로 GC 최소화
//   2) 람다 기반 단순 빌드(Build)와 체이닝 빌더(Builder) 제공
//   3) key=value|key=value 형태의 경량 커스텀 직렬화 포맷 파싱
//   4) 형 변환 및 안전한 접근 헬퍼(GetString, GetInt, GetFloat, GetBool, GetVector3)
// 사용 시나리오:
//   - 빈번한 이벤트 디스패치에서 JSON보다 가벼운 포맷 필요
//   - 구조가 자주 변하고 유연성이 중요할 때
//   - 고성능 요구: 최소 할당, 빠른 조립/파싱
// 포맷 규칙:
//   - 구분자: '|' 세그먼트 구분, '=' 키와 값 구분
//   - 값은 추가 인코딩 없이 그대로 삽입(특수문자 포함 시 자체 약속 필요)
//   - Vector3는 x,y,z 소수 고정(F{decimals})
// 주의 사항:
//   - 값 내부에 '|' 또는 '=' 사용하면 파싱이 깨짐 → 필요 시 escape 또는 다른 포맷 도입
//   - 멀티스레드 환경에서는 풀 사용 시 동기화 추가 필요(현재 단일 스레드 전제)
// =====================================================================================

namespace SoMTools.Event
{
    /// <summary>
    /// 문자열 이벤트용 통합 헬퍼 (풀링 + 빌더 + 파싱)
    /// </summary>
    public static class SoMStringEventHelper
    {
        // StringBuilder 재사용 풀. LIFO 스택 사용으로 캐시 친화성 유지.
        private static readonly Stack<StringBuilder> pool = new Stack<StringBuilder>();
        // 최대 풀 크기. 과도한 누적 방지. 필요 시 조정 가능.
        private const int MaxPoolSize = 10;

        /// <summary>
        /// 풀에서 StringBuilder 하나를 꺼내거나 새로 생성합니다. 초기 용량 256.
        /// </summary>
        private static StringBuilder Get()
        {
            if (pool.Count > 0)
            {
                var sb = pool.Pop();
                // 이전 내용 제거(용량 유지)
                sb.Clear();
                return sb;
            }
            return new StringBuilder(256); // 최초 생성 시 할당
        }

        /// <summary>
        /// 사용 완료된 StringBuilder를 풀에 반환. 풀 크기가 넘으면 GC 대상이 되도록 버림.
        /// </summary>
        private static void Release(StringBuilder sb)
        {
            if (pool.Count < MaxPoolSize)
            {
                sb.Clear(); // 내용 제거
                pool.Push(sb);
            }
            // 초과 시 반환하지 않고 폐기 → 메모리 제어 간단화
        }

        /// <summary>
        /// 람다(Action)로 즉석 페이로드를 구성하는 단순 인터페이스.
        /// 예: Build(sb => { sb.Append("type=Hit|dmg=25"); })
        /// </summary>
        public static string Build(System.Action<StringBuilder> builder)
        {
            var sb = Get();
            builder(sb);            // 호출자에서 조립
            string result = sb.ToString();
            Release(sb);            // 재사용 위해 반환
            return result;
        }

        /// <summary>
        /// 체이닝 방식으로 key=value 세그먼트를 누적하는 빌더.
        /// Add 호출 순서대로 '|' 삽입. AddRaw로 커스텀 세그먼트 가능.
        /// </summary>
        public sealed class Builder
        {
            private StringBuilder sb;
            private bool first = true;

            internal Builder(StringBuilder shared)
            {
                sb = shared;
            }

            /// <summary>
            /// 세그먼트 구분자 처리. 첫 세그먼트엔 구분자를 붙이지 않음.
            /// </summary>
            private void Sep()
            {
                if (!first) sb.Append('|');
                else first = false;
            }

            /// <summary>
            /// 문자열 값 추가.
            /// </summary>
            public Builder Add(string key, string value)
            {
                Sep();
                sb.Append(key).Append('=').Append(value);
                return this;
            }

            /// <summary>
            /// int 값 추가.
            /// </summary>
            public Builder Add(string key, int value)
            {
                Sep();
                sb.Append(key).Append('=').Append(value);
                return this;
            }

            /// <summary>
            /// float 값 추가. 소수 자릿수 지정.
            /// </summary>
            public Builder Add(string key, float value, int decimals = 2)
            {
                Sep();
                sb.Append(key).Append('=').Append(value.ToString("F" + decimals));
                return this;
            }

            /// <summary>
            /// bool 값 추가 (true/false 문자열).
            /// </summary>
            public Builder Add(string key, bool value)
            {
                Sep();
                sb.Append(key).Append('=').Append(value ? "true" : "false");
                return this;
            }

                        /// <summary>
                        /// Vector3 추가. x,y,z 순서로 콤마 구분.
                        /// </summary>
                        public Builder Add(string key, Vector3 v, int decimals = 2)
                        {
                                Sep();
                                sb.Append(key).Append('=')
                                    .Append(v.x.ToString("F" + decimals)).Append(',')
                                    .Append(v.y.ToString("F" + decimals)).Append(',')
                                    .Append(v.z.ToString("F" + decimals));
                                return this;
                        }

            /// <summary>
            /// 이미 key=value 형태로 준비된 세그먼트를 그대로 추가.
            /// </summary>
            public Builder AddRaw(string rawSegment)
            {
                Sep();
                sb.Append(rawSegment);
                return this;
            }

            /// <summary>
            /// 최종 문자열 반환 후 내부 StringBuilder를 풀에 반환.
            /// </summary>
            public string Build()
            {
                string result = sb.ToString();
                Release(sb);
                sb = null; // 재사용 방지
                return result;
            }
        }
        /// <summary>
        /// 체이닝 빌더 인스턴스 생성.
        /// </summary>
        public static Builder Create() => new Builder(Get());

        /// <summary>
        /// key=value|key=value 포맷을 Dictionary로 파싱. 잘못된 세그먼트는 무시.
        /// </summary>
        public static Dictionary<string, string> Parse(string payload)
        {
            var result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(payload)) return result;

            var pairs = payload.Split('|');
            foreach (var pair in pairs)
            {
                var kv = pair.Split('=');
                if (kv.Length == 2)
                    result[kv[0].Trim()] = kv[1].Trim();
            }
            return result;
        }

        // ===== 접근 헬퍼들: 존재 여부 + 형 변환 안전 처리 =====
        /// <summary>문자열 값 가져오기.</summary>
        public static string GetString(Dictionary<string, string> data, string key, string def = "")
            => data.TryGetValue(key, out var v) ? v : def;

        /// <summary>int 값 가져오기.</summary>
        public static int GetInt(Dictionary<string, string> data, string key, int def = 0)
            => data.TryGetValue(key, out var v) && int.TryParse(v, out var n) ? n : def;

        /// <summary>float 값 가져오기.</summary>
        public static float GetFloat(Dictionary<string, string> data, string key, float def = 0f)
            => data.TryGetValue(key, out var v) && float.TryParse(v, out var f) ? f : def;

        /// <summary>bool 값 가져오기(true/false 외 값은 기본값).</summary>
        public static bool GetBool(Dictionary<string, string> data, string key, bool def = false)
        {
            if (data.TryGetValue(key, out var v))
            {
                if (v == "true") return true;
                if (v == "false") return false;
            }
            return def;
        }

        /// <summary>Vector3 파싱(x,y,z). 형식 불일치 시 기본값 반환.</summary>
        public static Vector3 GetVector3(Dictionary<string, string> data, string key, Vector3 def = default)
        {
            if (!data.TryGetValue(key, out var v)) return def;
            var parts = v.Split(',');
            if (parts.Length != 3) return def;
            if (float.TryParse(parts[0], out var x) &&
                float.TryParse(parts[1], out var y) &&
                float.TryParse(parts[2], out var z))
                return new Vector3(x, y, z);
            return def;
        }
    }
}

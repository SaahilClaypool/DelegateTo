using System.Collections.Generic;

namespace DelegateTo.SourceGenerator
{
    static class KvpExtensions
    {
        public static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> kvp,
            out TKey key,
            out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }

        public static string Join<T>(this IEnumerable<T> self, string val) => string.Join(val, self);
    }
}
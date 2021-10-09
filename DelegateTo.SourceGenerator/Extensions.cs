global using System;
global using System.Text;
global using System.Linq;
global using System.Collections.Generic;
global using System.Diagnostics;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.Text;
global using DelegateTo.SourceGenerator;

namespace DelegateTo.SourceGenerator;

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
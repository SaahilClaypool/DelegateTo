using System;

namespace DelegateTo;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class GenerateDelegateAttribute : Attribute
{
    public string Prefix { get; set; } = string.Empty;
}

#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Diagnostics.CodeAnalysis;

namespace BrothTech;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(
        [NotNullWhen(false)]
        this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static bool IsNullOrEmpty(
        [NotNullWhen(false)]
        this string? value)
    {
        return string.IsNullOrEmpty(value); 
    }
}

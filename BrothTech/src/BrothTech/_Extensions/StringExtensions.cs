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

    public static string IfNullOrWhiteSpace(
        this string? value,
        string nullValue)
    {
        if (value.IsNullOrWhiteSpace())
            return nullValue;

        return value;
    }

    public static string IfNullOrEmpty(
        this string? value,
        string nullValue)
    {
        if (value.IsNullOrEmpty())
            return nullValue;

        return value;
    }
}

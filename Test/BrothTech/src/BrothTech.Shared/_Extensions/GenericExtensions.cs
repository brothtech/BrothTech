#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Runtime.CompilerServices;

namespace BrothTech;

public static class GenericExtensions
{
    public static T EnsureNotNull<T>(
        this T? value,
        [CallerArgumentExpression(nameof(value))]
        string? name = null)
        where T : class
    {
        return value ?? throw new ArgumentNullException(name ?? nameof(value));
    }

    public static TOut EnsureIs<TOut>(
        this object? value,
        [CallerArgumentExpression(nameof(value))]
        string? name = null)
    {
        return value.EnsureIs<object, TOut>(name);
    }

    public static TOut EnsureIs<TIn, TOut>(
        this TIn? value,
        [CallerArgumentExpression(nameof(value))]
        string? name = null)
    {
        if (value is TOut output)
            return output;

        throw new ArgumentOutOfRangeException(name ?? nameof(value));
    }
}
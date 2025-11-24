namespace BrothTech.Shared.Extensions;

/// <summary>
///     Extensions on generic types.
/// </summary>
public static class GenericExtensions
{
    /// <summary>
    ///     Determines if <see cref="left"/> is in <paramref name="leftValues"/>.
    /// </summary>
    public static bool In<T>(
        this T left, 
        params ReadOnlySpan<T> leftValues)
        where T : IEquatable<T>
    {
        foreach (var right in leftValues)
            if (left.Equals(right))
                return true;

        return false;
    }

    /// <summary>
    ///     Determines if <see cref="left"/> is not in <paramref name="leftValues"/>.
    /// </summary>
    public static bool NotIn<T>(
        this T left,
        params ReadOnlySpan<T> leftValues)
        where T : IEquatable<T>
    {
        return In(left, leftValues) is false;
    }
}
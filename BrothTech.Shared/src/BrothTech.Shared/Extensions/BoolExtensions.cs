namespace BrothTech.Shared.Extensions;

/// <summary>
///     Extends <see cref="bool"/>.
/// </summary>
public static class BoolExtensions
{
    /// <summary>
    ///     Returns <paramref name="valueIfTrue"/> if <paramref name="value"/> is
    ///     <see langword="true"/> or <paramref name="valueIfFalse"/> otherwise.
    /// </summary>
    public static T? If<T>(
        this bool value,
        T? valueIfTrue,
        T? valueIfFalse = default)
        where T : allows ref struct
    {
        if (value)
            return valueIfTrue;

        return valueIfFalse;
    }

    /// <summary>
    ///     Returns the return value or default if null of <paramref name="funcIfTrue"/> if <paramref name="value"/> is
    ///     <see langword="true"/> or <paramref name="funcIfFalse"/> otherwise.
    /// </summary>
    public static T? If<T>(
        this bool value, 
        Func<T?>? funcIfTrue,
        Func<T?>? funcIfFalse = null)
        where T : allows ref struct
    {
        if (value)
            return funcIfTrue is not null ? funcIfTrue() : default;

        return funcIfFalse is not null ? funcIfFalse() : default;
    }

    /// <summary>
    ///     Returns <paramref name="valueIfFalse"/> if <paramref name="value"/> is
    ///     <see langword="false"/> or <paramref name="valueIfTrue"/> otherwise.
    /// </summary>
    public static T? IfNot<T>(
        this bool value,
        T? valueIfFalse,
        T? valueIfTrue = default)
        where T : allows ref struct
    {
        return If(value, valueIfTrue, valueIfFalse);
    }

    /// <summary>
    ///     Returns the return value or default if null of <paramref name="funcIfFalse"/> if <paramref name="value"/> is
    ///     <see langword="false"/> or <paramref name="funcIfTrue"/> otherwise.
    /// </summary>
    public static T? IfNot<T>(
        this bool value,
        Func<T?>? funcIfFalse,
        Func<T?>? funcIfTrue = null)
        where T : allows ref struct
    {
        return If(value, funcIfTrue, funcIfFalse);
    }
}
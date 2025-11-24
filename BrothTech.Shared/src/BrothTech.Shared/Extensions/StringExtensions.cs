namespace BrothTech.Shared.Extensions;

/// <summary>
///     Extends <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Returns <paramref name="valueIfEmpty"/> if <paramref name="value"/> is null
    ///     or empty.
    /// </summary>
    public static string IfEmpty(
        this string? value,
        string valueIfEmpty)
    {
        if (string.IsNullOrEmpty(value))
            return valueIfEmpty;

        return value;
    }

    /// <summary>
    ///     Returns <paramref name="valueIfWhitespace"/> if <paramref name="value"/> is null
    ///     or white space.
    /// </summary>
    public static string IfWhiteSpace(
        this string? value,
        string valueIfWhitespace)
    {
        if (string.IsNullOrWhiteSpace(value))
            return valueIfWhitespace;

        return value;
    }

    /// <summary>
    ///     Returns the return value of <paramref name="funcIfNotNull"/> when passed <paramref name="value"/>
    ///     if <paramref name="value"/> is not null.
    /// </summary>
    public static string? IfNotNull(
        this string? value,
        Func<string, string> funcIfNotNull)
    {
        if (value is null)
            return value;

        return funcIfNotNull(value);
    }

    /// <summary>
    ///     Returns the return value of <paramref name="funcIfNotEmpty"/> when passed <paramref name="value"/>
    ///     if <paramref name="value"/> is not null or empty.
    /// </summary>
    public static string? IfNotEmpty(
        this string? value,
        Func<string, string> funcIfNotEmpty)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return funcIfNotEmpty(value);
    }

    /// <summary>
    ///     Returns the return value of <paramref name="funcIfNotWhitespace"/> when passed <paramref name="value"/>
    ///     if <paramref name="value"/> is not null or white space.
    /// </summary>
    public static string? IfNotWhiteSpace(
        this string? value,
        Func<string, string> funcIfNotWhitespace)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return funcIfNotWhitespace(value);
    }
}
#pragma warning disable IDE0130 // Namespace does not match folder structure
using BrothTech.Shared.Contracts.Results;
using Microsoft.Extensions.Logging;

namespace BrothTech;

public static class LoggerExtensions
{
    public static void Log(
        this ILogger logger,
        IEnumerable<ResultMessage> messages)
    {
        foreach (var message in messages)
            logger.Log(message.LogLevel, message.Message, message.Args);
    }

    public static void LogTraceIfEnabled(
        this ILogger logger,
        string? message,
        params object?[] args)
    {
        logger.LogIfEnabled(LogLevel.Trace, message, args);
    }

    public static void LogDebugIfEnabled(
        this ILogger logger,
        string? message,
        params object?[] args)
    {
        logger.LogIfEnabled(LogLevel.Debug, message, args);
    }

    public static void LogInformationIfEnabled(
        this ILogger logger,
        string? message,
        params object?[] args)
    {
        logger.LogIfEnabled(LogLevel.Trace, message, args);
    }

    public static void LogWarningIfEnabled(
        this ILogger logger,
        string? message,
        params object?[] args)
    {
        logger.LogIfEnabled(LogLevel.Warning, message, args);
    }

    public static void LogErrorIfEnabled(
        this ILogger logger,
        string? message,
        params object?[] args)
    {
        logger.LogIfEnabled(LogLevel.Error, message, args);
    }

    public static void LogCriticalIfEnabled(
        this ILogger logger,
        string? message,
        params object?[] args)
    {
        logger.LogIfEnabled(LogLevel.Critical, message, args);
    }

    public static void LogIfEnabled(
        this ILogger logger,
        LogLevel level,
        string? message,
        params object?[] args)
    {
        if (logger.IsEnabled(level) is false)
            return;

        logger.Log(level, message, args);
    }
}

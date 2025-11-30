#pragma warning disable IDE0130 // Namespace does not match folder structure
using BrothTech.Contracts.Results;
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
}

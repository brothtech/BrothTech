using Microsoft.Extensions.Logging;

namespace BrothTech.Contracts.Results;

public record ResultMessage(
    string Message, 
    LogLevel LogLevel = default, 
    params object?[] Args)
{
    public static implicit operator ResultMessage(
        (string message, object? arg1) result)
    {
        return new ResultMessage(
            Message: result.message, 
            LogLevel: LogLevel.Error, 
            result.arg1);
    }

    public static implicit operator ResultMessage(
        (string message, object? arg1, object? arg2) result)
    {
        return new ResultMessage(
            Message: result.message,
            LogLevel: LogLevel.Error,
            result.arg1, 
            result.arg2);
    }

    public static implicit operator ResultMessage(
        (string message, object? arg1, object? arg2, object? arg3) result)
    {
        return new ResultMessage(
            Message: result.message,
            LogLevel: LogLevel.Error,
            result.arg1,
            result.arg2,
            result.arg3);
    }
}

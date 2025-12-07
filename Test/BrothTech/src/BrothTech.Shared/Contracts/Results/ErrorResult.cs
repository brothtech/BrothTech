using Microsoft.Extensions.Logging;

namespace BrothTech.Shared.Contracts.Results;

public class ErrorResult :
    Result
{
    public new static readonly ErrorResult Failure = new() { IsSuccessful = false };

    private ErrorResult()
    {
    }

    public static ErrorResult FromMessages(
        IReadOnlyList<ResultMessage> messages)
    {
        return new ErrorResult
        {
            IsSuccessful = false,
            Messages = [.. messages]
        };
    }

    public static ErrorResult FromMessages(
        params ResultMessage[] messages)
    {
        return new ErrorResult
        {
            IsSuccessful = false,
            Messages = [.. messages]
        };
    }

    public static ErrorResult FromCriticalMessages(
        params string[] messages)
    {
        return FromMessages(LogLevel.Critical, messages);
    }

    public static ErrorResult FromErrorMessages(
        params string[] messages)
    {
        return FromMessages(LogLevel.Error, messages);
    }

    public static ErrorResult FromWarningMessages(
        params string[] messages)
    {
        return FromMessages(LogLevel.Warning, messages);
    }

    public static ErrorResult FromInformationMessages(
        params string[] messages)
    {
        return FromMessages(LogLevel.Information, messages);
    }

    public static ErrorResult FromDebugMessages(
        params string[] messages)
    {
        return FromMessages(LogLevel.Debug, messages);
    }

    public static ErrorResult FromTraceMessages(
        params string[] messages)
    {
        return FromMessages(LogLevel.Trace, messages);
    }

    public static ErrorResult FromMessages(
        LogLevel logLevel,
        params string[] messages)
    {
        return new ErrorResult
        {
            IsSuccessful = false,
            Messages = [.. messages.Select(x => new ResultMessage(x, logLevel))]
        };
    }
}

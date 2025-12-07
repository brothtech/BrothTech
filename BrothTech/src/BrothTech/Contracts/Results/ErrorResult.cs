using Microsoft.Extensions.Logging;

namespace BrothTech.Contracts.Results;

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
        params string[] errors)
    {
        return new ErrorResult
        {
            IsSuccessful = false,
            Messages = [.. errors.Select(x => new ResultMessage(x, LogLevel.Critical))]
        };
    }

    public static ErrorResult FromErrorMessages(
        params string[] errors)
    {
        return new ErrorResult
        {
            IsSuccessful = false,
            Messages = [.. errors.Select(x => new ResultMessage(x, LogLevel.Error))]
        };
    }

    public static ErrorResult FromWarningMessages(
        params string[] errors)
    {
        return new ErrorResult
        {
            IsSuccessful = false,
            Messages = [.. errors.Select(x => new ResultMessage(x, LogLevel.Warning))]
        };
    }

    public static ErrorResult FromInformationMessages(
        params string[] errors)
    {
        return new ErrorResult
        {
            IsSuccessful = false,
            Messages = [.. errors.Select(x => new ResultMessage(x, LogLevel.Information))]
        };
    }

    public static ErrorResult FromDebugMessages(
        params string[] errors)
    {
        return new ErrorResult
        {
            IsSuccessful = false,
            Messages = [.. errors.Select(x => new ResultMessage(x, LogLevel.Debug))]
        };
    }

    public static ErrorResult FromTraceMessages(
        params string[] errors)
    {
        return new ErrorResult
        {
            IsSuccessful = false,
            Messages = [.. errors.Select(x => new ResultMessage(x, LogLevel.Trace))]
        };
    }
}

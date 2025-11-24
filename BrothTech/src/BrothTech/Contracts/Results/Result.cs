using System.Diagnostics.CodeAnalysis;

namespace BrothTech.Contracts.Results;

public class Result<TItem> :
    Result
{
    public new TItem? Item
    {
        get => (TItem?)base.Item;
        set => base.Item = value;   
    }

    public new TItem? Resolve(
        string? message = null)
    {
        if (IsSuccessful is false)
            Throw(message);

        return Item;
    }

    public static implicit operator Result<TItem>(
        TItem item)
    {
        return new Result<TItem> 
        { 
            IsSuccessful = true,
            Item = item
        };
    }

    public static implicit operator Result<TItem>(
        List<ResultMessage> messages)
    {
        return new Result<TItem>
        {
            IsSuccessful = false,
            Messages = messages
        };
    }

    public static implicit operator Result<TItem>(
        bool isSuccessful)
    {
        return new Result<TItem>
        {
            IsSuccessful = isSuccessful
        };
    }

    public static implicit operator Result<TItem>(
        ErrorResult result)
    {
        return new Result<TItem> 
        {
            IsSuccessful = false,
            Messages = result.Messages
        };
    }

    public static implicit operator Result<TItem>(
        int exitCode)
    {
        return new Result<TItem>
        {
            IsSuccessful = exitCode == 0
        };
    }
}

public class Result
{
    public static readonly Result Success = new() { IsSuccessful = true };

    public static readonly ErrorResult Failure = ErrorResult.FromErrorMessages();

    public required bool IsSuccessful { get; set; }

    public object? Item { get; set; }

    public IReadOnlyList<ResultMessage> Messages { get; set; } = [];

    public object? Resolve(
        string? message = null)
    {
        if (IsSuccessful is false)
            Throw(message);

        return Item;
    }

    [DoesNotReturn]
    protected void Throw(
        string? message = null)
    {
        message ??= string.Join("\n", Messages.Select(x => x.Message));
        throw new Exception(message);
    }

    public static implicit operator Result(
        List<ResultMessage> messages)
    {
        return new Result
        {
            IsSuccessful = false,
            Messages = messages
        };
    }

    public static implicit operator Result(
        bool isSuccessful)
    {
        return new Result
        {
            IsSuccessful = isSuccessful
        };
    }

    public static implicit operator Result(
        int exitCode)
    {
        return new Result
        {
            IsSuccessful = exitCode == 0
        };
    }

    public static Result operator &(
        Result left,
        Result right)
    {
        return new Result
        {
            IsSuccessful = left.IsSuccessful && right.IsSuccessful,
            Item = (left.Item, right.Item),
            Messages = [.. left.Messages, .. right.Messages]
        };
    }

}
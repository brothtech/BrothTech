using System.Diagnostics.CodeAnalysis;

namespace BrothTech.Shared.Contracts.Results;

public class Result<TItem> :
    Result
{
    public new static readonly Result<TItem> Success = new() { IsSuccessful = true };

    public new static readonly Result<TItem> Failure = new() { IsSuccessful = false };

    public new TItem? Item
    {
        get => (TItem?)base.Item;
        init => base.Item = value;   
    }

    public Result<T> ToResult<T>(
        Func<TItem, T> func)
    {
        if (IsSuccessful && Item is not null)
            return func(Item);

        return ErrorResult.FromMessages([.. Messages]);
    }

    public bool HasSucceeded(
        out TItem? item)
    {
        item = Item;
        return IsSuccessful;
    }

    public bool HasSucceeded(
        out TItem? item,
        out IReadOnlyList<ResultMessage> messages)
    {
        item = Item;
        messages = Messages;
        return IsSuccessful;
    }

    public bool HasItem(
        [NotNullWhen(true)]
        out TItem? item)
    {
        return HasSucceeded(out item) && item is not null;
    }

    public bool HasItem(
        [NotNullWhen(true)]
        out TItem? item,
        out IReadOnlyList<ResultMessage> messages)
    {
        return HasSucceeded(out item, out messages) && item is not null;
    }

    public bool HasFailed(
        out TItem? item)
    {
        item = Item;
        return IsSuccessful is false;
    }

    public bool HasFailed(
        out TItem? item,
        out IReadOnlyList<ResultMessage> messages)
    {
        item = Item;
        messages = Messages;
        return IsSuccessful is false;
    }

    public bool HasNoItem(
        [NotNullWhen(false)]
        out TItem? item)
    {
        return HasFailed(out item) && item is null;
    }

    public bool HasNoItem(
        [NotNullWhen(false)]
        out TItem? item,
        out IReadOnlyList<ResultMessage> messages)
    {
        return HasFailed(out item, out messages) && item is null;
    }

    public Result<TItem> Out(
        out TItem? item)
    {
        item = Item;
        return this;
    }

    public Result<TItem> Out(
        out TItem? item,
        out IReadOnlyList<ResultMessage> messages)
    {
        item = Item;
        messages = Messages;
        return this;
    }

    public Result<TItem> OutWithItem(
        out TItem item)
    {
        item = Item!;
        return Item is null ? Failure : this;
    }

    public Result<TItem> OutWithItem(
        out TItem item,
        out IReadOnlyList<ResultMessage> messages)
    {
        item = Item!;
        messages = Messages;
        return Item is null ? Failure : this;
    }

    public Result<TItem> OutWithNoItem(
        out TItem item)
    {
        item = Item!;
        return Item is not null ? Failure : this;
    }

    public Result<TItem> OutWithNoItem(
        out TItem item,
        out IReadOnlyList<ResultMessage> messages)
    {
        item = Item!;
        messages = Messages;
        return Item is not null ? Failure : this;
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

    public static implicit operator Result<TItem>(
        Exception exception)
    {
        return ErrorResult.FromErrorMessages(exception.ToString());
    }

    public static bool operator true(
        Result<TItem> result)
    {
        return result.IsSuccessful;
    }

    public static bool operator false(
        Result<TItem> result)
    {
        return result.IsSuccessful is false;
    }
}

public class Result
{
    public static readonly Result Success = new() { IsSuccessful = true };

    public static readonly Result Failure = new() { IsSuccessful = false };

    public static Result Combine(
        Result left,
        Result right)
    {
        return new Result
        {
            IsSuccessful = left.IsSuccessful && right.IsSuccessful,
            Item = (left.Item, right.Item),
            Messages =
            [
                .. left.IsSuccessful is false || right.IsSuccessful ? left.Messages : Enumerable.Empty<ResultMessage>(),
                .. right.IsSuccessful is false || left.IsSuccessful ? right.Messages : Enumerable.Empty<ResultMessage>()
            ]
        };
    }

    public static Result Either(
        Result left,
        Result right)
    {
        if (left.IsSuccessful)
            return left;

        if (right.IsSuccessful)
            return right;

        return new Result
        {
            IsSuccessful = false,
            Messages = [.. left.Messages, .. right.Messages]
        };
    }

    public required bool IsSuccessful { get; set; }

    public object? Item { get; protected set; }

    public IReadOnlyList<ResultMessage> Messages { get; init; } = [];

    public bool HasSucceeded(
        out object? item)
    {
        item = Item;
        return IsSuccessful;
    }

    public bool HasSucceeded(
        out object? item,
        out IReadOnlyList<ResultMessage> messages)
    {
        item = Item;
        messages = Messages;
        return IsSuccessful;
    }

    public bool HasItem(
        [NotNullWhen(true)]
        out object? item)
    {
        return HasSucceeded(out item) && item is not null;
    }

    public bool HasItem(
        [NotNullWhen(true)]
        out object? item,
        out IReadOnlyList<ResultMessage> messages)
    {
        return HasSucceeded(out item, out messages) && item is not null;
    }

    public bool HasFailed(
        out object? item)
    {
        item = Item;
        return IsSuccessful is false;
    }

    public bool HasFailed(
        out object? item,
        out IReadOnlyList<ResultMessage> messages)
    {
        item = Item;
        messages = Messages;
        return IsSuccessful is false;
    }

    public bool HasNoItem(
        [NotNullWhen(false)]
        out object? item)
    {
        return HasFailed(out item) && item is null;
    }

    public bool HasNoItem(
        [NotNullWhen(false)]
        out object? item,
        out IReadOnlyList<ResultMessage> messages)
    {
        return HasFailed(out item, out messages) && item is null;
    }

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

    public static implicit operator int(
        Result result)
    {
        return result.IsSuccessful ? 0 : 1;
    }

    public static implicit operator Result(
        Exception exception)
    {
        return ErrorResult.FromErrorMessages(exception.ToString());
    }

    public static bool operator true(
        Result result)
    {
        return result.IsSuccessful;
    }

    public static bool operator false(
        Result result)
    {
        return result.IsSuccessful is false;
    }

    public static Result operator &(
        Result left,
        Result right)
    {
        return Combine(left, right);
    }

    public static Result operator |(
        Result left,
        Result right)
    {
        return Either(left, right);
    }
}
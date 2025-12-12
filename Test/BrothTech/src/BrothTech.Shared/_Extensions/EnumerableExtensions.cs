#pragma warning disable IDE0130 // Namespace does not match folder structure
using BrothTech.Shared.Contracts.Results;

namespace BrothTech.Shared;

public static class EnumerableExtensions
{
    public static Result AggregateResults<T>(
        this IEnumerable<T> enumerable,
        Func<T, Result> handler)
    {
        var aggregateResult = Result.Success;
        foreach (var element in enumerable)
        {
            aggregateResult &= handler(element);
            if (aggregateResult.IsSuccessful is false)
                return aggregateResult;
        }

        return aggregateResult;
    }

    public static async Task<Result> AggregateResultsAsync<T>(
        this IEnumerable<T> enumerable,
        Func<T, Task<Result>> handler)
    {
        var aggregateResult = Result.Success;
        foreach (var element in enumerable)
        {
            aggregateResult &= await handler(element);
            if (aggregateResult.IsSuccessful is false)
                return aggregateResult;
        }

        return aggregateResult;
    }

    public static async Task<Result> AggregateResultsAsync<T>(
        this IAsyncEnumerable<T> enumerable,
        Func<T, Task<Result>> handler)
    {
        var aggregateResult = Result.Success;
        await foreach (var element in enumerable)
        {
            aggregateResult &= await handler(element);
            if (aggregateResult.IsSuccessful is false)
                return aggregateResult;
        }

        return aggregateResult;
    }

    public static Result AggregateResults<TElement, TResultItem>(
        this IEnumerable<TElement> enumerable,
        Func<TElement, Result<TResultItem>> handler,
        Action<TResultItem>? onHasItem = null)
    {
        var aggregateResult = Result.Success;
        foreach (var element in enumerable)
        {
            var result = handler(element).OutWithItem(out var item);
            if ((aggregateResult &= result).IsSuccessful is false)
                return aggregateResult;

            onHasItem?.Invoke(item);
        }

        return aggregateResult;
    }

    public static async Task<Result> AggregateResultsAsync<TElement, TResultItem>(
        this IEnumerable<TElement> enumerable,
        Func<TElement, Task<Result<TResultItem>>> handler,
        Func<TResultItem, Task>? onHasItem = null)
    {
        var aggregateResult = Result.Success;
        foreach (var element in enumerable)
        {
            var result = (await handler(element)).OutWithItem(out var item);
            if ((aggregateResult &= result).IsSuccessful is false)
                return aggregateResult;

            await (onHasItem?.Invoke(item)).SafeAwait();
        }

        return aggregateResult;
    }

    public static async Task<Result> AggregateResultsAsync<TElement, TResultItem>(
        this IAsyncEnumerable<TElement> enumerable,
        Func<TElement, Task<Result<TResultItem>>> handler,
        Func<Result<TResultItem>, Task>? onHasItem = null)
    {
        var aggregateResult = Result.Success;
        await foreach (var element in enumerable)
        {
            var result = (await handler(element)).OutWithItem(out var item);
            if ((aggregateResult &= result).IsSuccessful is false)
                return aggregateResult;

            await (onHasItem?.Invoke(item)).SafeAwait();
        }

        return aggregateResult;
    }
}

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace BrothTech.Shared;

public static class TaskExtensions
{
    public static Task SafeAwait(
        this Task? task)
    {
        return task ?? Task.CompletedTask;
    }
}

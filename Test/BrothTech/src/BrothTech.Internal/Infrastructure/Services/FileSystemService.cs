using BrothTech.Shared.Contracts.Results;
using BrothTech.Shared.Contracts.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace BrothTech.Internal.Infrastructure.Services;

public class FileSystemService :
    IFileSystemService
{
    private static readonly Lazy<JsonSerializerOptions> _jsonSerializerOptions = new(() => new() { WriteIndented = true });

    public void EnsureDirectoryExists(
        string path)
    {
        if (DirectoryExists(path))
            return;

        Directory.CreateDirectory(path);
    }

    public Result<FileInfo> TryFindFile(
        string fileSearchPattern,
        string? directoryPath = null,
        bool shouldSearchRecursively = false)
    {
        var result = TryFindFiles(fileSearchPattern, directoryPath, shouldSearchRecursively);
        if (result.HasItem(out var files, out var messages))
            return files[0];

        return ErrorResult.FromMessages([.. messages]);
    }

    public Result<IReadOnlyList<FileInfo>> TryFindFiles(
        string searchPattern,
        string? directoryPath = null,
        bool shouldSearchRecursively = false)
    {
        var directory = new DirectoryInfo(directoryPath ?? GetCurrentDirectory());
        while (directory is not null)
        {
            if (GetFiles(directory, searchPattern) is { Length: > 0 } files)
                return files;

            if (shouldSearchRecursively is false)
                break;

            directory = directory.Parent;
        }

        return ErrorResult.FromMessages(("Unable to find directory with {pattern}", searchPattern));
    }

    public Result<string> TryReadFile(
        string path)
    {
        if (FileExists(path) is false)
            return ErrorResult.FromErrorMessages("File does not exist");

        return ReadAllText(path);
    }

    public Result<T> TryReadFile<T>(
        string path)
    {
        try
        {
            if (FileExists(path) is false)
                return ErrorResult.FromErrorMessages("File does not exist");

            using var stream = OpenReadStream(path);
            if (Deserialize<T>(stream) is not { } value)
                return ErrorResult.FromMessages(("Unable to read file to {typeName}", typeof(T).Name));

            return value;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }

    [ExcludeFromCodeCoverage(Justification = Passthrough)]
    public bool FileExists(
        string path)
    {
        return File.Exists(path);
    }

    public Result TryWriteFile(
        string path,
        string contents)
    {
        try
        {
            WriteAllText(path, contents);
            return Result.Success;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }

    public Result TryWriteFile<T>(
        string path,
        T value)
    {
        try
        {
            using var stream = OpenWriteStream(path);
            Serialize(stream, value);
            return Result.Success;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }

    [ExcludeFromCodeCoverage(Justification = Passthrough)]
    internal string GetCurrentDirectory()
    {
        return Directory.GetCurrentDirectory();
    }

    [ExcludeFromCodeCoverage(Justification = Passthrough)]
    internal bool DirectoryExists(
        string path)
    {
        return Directory.Exists(path);
    }

    [ExcludeFromCodeCoverage(Justification = Passthrough)]
    internal FileInfo[] GetFiles(
        DirectoryInfo directoryInfo,
        string searchPattern)
    {
        return directoryInfo.GetFiles(searchPattern);
    }

    [ExcludeFromCodeCoverage(Justification = Passthrough)]
    internal string ReadAllText(
        string path)
    {
        return File.ReadAllText(path, Encoding.UTF8);
    }

    [ExcludeFromCodeCoverage(Justification = Passthrough)]
    internal FileStream OpenReadStream(
        string path)
    {
        return File.OpenRead(path);
    }

    [ExcludeFromCodeCoverage(Justification = Passthrough)]
    internal void WriteAllText(
        string path,
        string contents)
    {
        File.WriteAllText(path, contents);
    }

    [ExcludeFromCodeCoverage(Justification = Passthrough)]
    internal FileStream OpenWriteStream(
        string path)
    {
        return File.OpenWrite(path);
    }

    [ExcludeFromCodeCoverage(Justification = Passthrough)]
    internal T? Deserialize<T>(
        FileStream stream)
    {
        return JsonSerializer.Deserialize<T>(stream);
    }

    [ExcludeFromCodeCoverage(Justification = Passthrough)]
    internal void Serialize<T>(
        FileStream stream,
        T value)
    {
        JsonSerializer.Serialize(stream, value, _jsonSerializerOptions.Value);
    }
}

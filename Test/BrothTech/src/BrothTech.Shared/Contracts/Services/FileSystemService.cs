using BrothTech.Shared.Contracts.Results;
using System.Xml.Linq;

namespace BrothTech.Shared.Contracts.Services;

public interface IFileSystemService
{
    void EnsureDirectoryExists(
        string path);

    Result<FileInfo> TryFindFile(
        string fileSearchPattern,
        string? directoryPath = null,
        bool shouldSearchRecursively = false);

    Result<IReadOnlyList<FileInfo>> TryFindFiles(
        string fileSearchPattern,
        string? directoryPath = null,
        bool shouldSearchRecursively = false);

    Result<string> TryReadFile(
        string path);

    Result<T> TryReadFile<T>(
        string path);

    bool FileExists(
        string path);

    Result TryWriteFile(
        string path,
        string contents);

    Result TryWriteFile<T>(
        string path,
        T value);

    Task<Result> TryUpdateXmlElementAsync(
        string filePath,
        Action<XElement> handler,
        string? elementPath = null,
        CancellationToken token = default);
}
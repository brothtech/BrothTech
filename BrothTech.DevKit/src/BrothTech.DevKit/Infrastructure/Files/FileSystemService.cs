using BrothTech.Contracts.Results;
using BrothTech.Infrastructure.DependencyInjection;

namespace BrothTech.DevKit.Infrastructure.Files;

public interface IFileSystemService
{
    void EnsureDirectoryExists(
        string path);

    Result<string> TryFindDirectory(
        string fileSearchPattern);
}

public class FileSystemService :
    IFileSystemService
{
    public void EnsureDirectoryExists(
        string path)
    {
        if (Directory.Exists(path) is true)
            return;

        Directory.CreateDirectory(path);
    }

    public Result<string> TryFindDirectory(
        string fileSearchPattern)
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory is not null)
        {
            if (directory.GetFiles(fileSearchPattern).Length > 0)
                return directory.FullName;

            directory = directory.Parent;
        }

        return ErrorResult.FromMessages(("Unable to find directory with {pattern}", fileSearchPattern));
    }
}

using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.Files;

namespace BrothTech.DevKit.DomainManagement.Services;

public interface IDomainService
{
    Result<string> TryGetRootDirectory();
}

public class DomainService(
    IFileSystemService fileSystemService) :
    IDomainService
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();

    public Result<string> TryGetRootDirectory()
    {
        return _fileSystemService.TryFindDirectory("*.Root.sln");
    }
}

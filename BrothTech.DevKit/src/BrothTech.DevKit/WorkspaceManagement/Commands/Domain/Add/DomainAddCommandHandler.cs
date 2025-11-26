using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.DevKit.WorkspaceManagement.Services;
using BrothTech.Infrastructure.DependencyInjection;

namespace BrothTech.DevKit.WorkspaceManagement.Commands.Domain.Add;

[ServiceDescriptor<ICommandHandler<DomainAddCommand, DomainAddCommandResult>, DomainAddCommandHandler>]
public class DomainAddCommandHandler(
    IFileSystemService fileSystemService,
    IWorkspaceManagementService domainService,
    IDotNetService dotNetService) :
    ICommandHandler<DomainAddCommand, DomainAddCommandResult>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly IWorkspaceManagementService _domainService = domainService.EnsureNotNull();
    private readonly IDotNetService _dotNetService = dotNetService.EnsureNotNull();

    public async Task<Result> TryHandleAsync(
        DomainAddCommandResult commandResult, 
        CancellationToken token)
    {
        var rootDirectory = _domainService.TryGetRootDirectory().Resolve().EnsureNotNull();
        return await TryCreateDomainSolutionAsync(rootDirectory, commandResult.Name, token) &&
               await TryCreateDomainProjectsAsync(rootDirectory, commandResult.Name, commandResult.PrimaryProjectType ?? DotNetProjectTemplate.ClassLib, token) &&
               await TryAddDomainProjectsToSolutionAsync(rootDirectory, commandResult.Name, token);
    }

    private async Task<Result> TryCreateDomainSolutionAsync(
        string rootDirectory,
        string domainName,
        CancellationToken token)
    {
        var path = @$"{rootDirectory}\{domainName}";
        _fileSystemService.EnsureDirectoryExists(path);
        return await _dotNetService.TryCreateSolutionAsync(domainName, path, token);
    }

    private async Task<Result> TryCreateDomainProjectsAsync(
        string rootDirectory,
        string domainName,
        DotNetProjectTemplate template,
        CancellationToken token)
    {
        var srcPath = @$"{rootDirectory}\{domainName}\src\{domainName}";
        _fileSystemService.EnsureDirectoryExists(srcPath);
        return await _dotNetService.TryCreateProject(domainName, template, srcPath, token) &&
               await _dotNetService.TryCreateProject($"{domainName}.Sandbox", template, @$"{srcPath}.Sandbox", token) &&
               await _dotNetService.TryAddProjectReference(
                   projectPath: @$"{srcPath}.Sandbox\{domainName}.Sandbox.csproj",
                   referencePath: @$"{srcPath}\{domainName}.csproj",
                   token: token);
    }

    private async Task<Result> TryAddDomainProjectsToSolutionAsync(
        string rootDirectory,
        string domainName,
        CancellationToken token)
    {
        var path = @$"{rootDirectory}\{domainName}";
        return await _dotNetService.TryAddProjectToSolution(
            solutionPath: @$"{path}\{domainName}.sln",
            projectPath: @$"{path}\src\{domainName}\{domainName}.csproj",
            token);
    }
}

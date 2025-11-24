using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.DomainManagement.Services;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.Infrastructure.DependencyInjection;
using System.CommandLine;

namespace BrothTech.DevKit.DomainManagement.Commands.Domain.Add;

[ServiceDescriptor<ICommandHandler<DomainAddCommand>, DomainAddCommandHandler>]
public class DomainAddCommandHandler(
    IFileSystemService fileSystemService,
    IDomainService domainService,
    IDotNetService dotNetService) :
    ICommandHandler<DomainAddCommand>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly IDomainService _domainService = domainService.EnsureNotNull();
    private readonly IDotNetService _dotNetService = dotNetService.EnsureNotNull();

    public async Task HandleAsync(
        DomainAddCommand command,
        ParseResult parseResult, 
        CancellationToken token)
    {
        var rootDirectory = _domainService.TryGetRootDirectory().Resolve().EnsureNotNull();
        var domainName = parseResult.GetRequiredValue(command.Name);
        var template = parseResult.GetRequiredValue(command.PrimaryProjectType);
        var result = await CreateDomainSolutionAsync(rootDirectory, domainName, token) &
                     await CreateDomainProjectsAsync(rootDirectory, domainName, template, token) &
                     await AddDomainProjectsToSolutionAsync(rootDirectory, domainName, token);
        _ = result.Resolve();
    }

    private async Task<Result> CreateDomainSolutionAsync(
        string rootDirectory,
        string domainName,
        CancellationToken token)
    {
        var path = @$"{rootDirectory}\{domainName}";
        _fileSystemService.EnsureDirectoryExists(path);
        return await _dotNetService.TryCreateSolutionAsync(domainName, path, token);
    }

    private async Task<Result> CreateDomainProjectsAsync(
        string rootDirectory,
        string domainName,
        DotNetProjectTemplate template,
        CancellationToken token)
    {
        var srcPath = @$"{rootDirectory}\{domainName}\src";
        _fileSystemService.EnsureDirectoryExists(srcPath);
        return await _dotNetService.TryCreateProject(domainName, template, srcPath, token) &
               await _dotNetService.TryCreateProject($"{domainName}.Sandbox", template, srcPath, token) &
               await _dotNetService.TryAddProjectReference(
                   projectPath: @$"{srcPath}\{domainName}.Sandbox\{domainName}.Sandbox.csproj",
                   referencePath: @$"{srcPath}\{domainName}\{domainName}.csproj",
                   token: token);
    }

    private async Task<Result> AddDomainProjectsToSolutionAsync(
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

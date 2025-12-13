using BrothTech.Shared.Contracts.Results;
using BrothTech.Shared.Contracts.Services;
using BrothTech.WorkspaceManagement.Shared.Contracts.Services;
using System.Xml.Linq;

namespace BrothTech.WorkspaceManagement.Internal.Infrastructure.Services;

internal class DotNetService(
    IFileSystemService fileSystemService,
    IProcessRunner processRunner) :
    IDotNetService
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly IProcessRunner _processRunner = processRunner.EnsureNotNull();

    public Task<Result> TryCreateSolutionAsync(
        string name,
        string directory,
        CancellationToken token)
    {
        _fileSystemService.EnsureDirectoryExists(directory);
        return _processRunner.TryRunAsync(
            fileName: "dotnet",
            token: token,
            "new",
            "sln",
            "-n",
            name,
            "-o",
            directory);
    }

    public Task<Result> TryAddProjectToSolution(
        string solutionPath,
        string projectPath,
        string solutionFolder,
        CancellationToken token)
    {
        return _processRunner.TryRunAsync(
            fileName: "dotnet",
            token: token,
            "sln",
            solutionPath,
            "add",
            projectPath,
            "-s",
            solutionFolder);
    }

    public Task<Result> TryCreateProject(
        string name,
        string template,
        string directory,
        CancellationToken token)
    {
        _fileSystemService.EnsureDirectoryExists(directory);
        return _processRunner.TryRunAsync(
            fileName: "dotnet",
            token: token,
            "new",
            template.ToString().ToLower(),
            "-n",
            name,
            "-o",
            directory,
            "-f",
            "net10.0");
    }

    public Task<Result> TryAddProjectReferenceAsync(
        string projectPath,
        string referencePath,
        CancellationToken token)
    {
        return _processRunner.TryRunAsync(
            fileName: "dotnet",
            token: token,
            "add",
            projectPath,
            "reference",
            referencePath);
    }

    public Task<Result> TryAddPackageReference(
        string projectPath,
        string packageName,
        string packageVersion,
        CancellationToken token)
    {
        return _processRunner.TryRunAsync(
            fileName: "dotnet",
            token: token,
            "package",
            "add",
            $"{packageName}@{packageVersion}",
            "--project",
            projectPath);
    }

    public async Task<Result> TrySetPropertiesAsync(
        string projectPath,
        CancellationToken token,
        params DotNetProjectProperty[] propertyGroups)
    {
        var elements = propertyGroups.Select(x => new XElement(x.Name, x.Value));
        return await _fileSystemService.TryUpdateXmlElementAsync(
            filePath: projectPath,
            handler: x => x.Add([.. elements]), 
            elementPath: "PropertyGroup", 
            token: token);
    }

    public async Task<Result> TryAddInternalVisiblityAsync(
        string projectPath,
        string assemblyName,
        CancellationToken token)
    {
        var attribute = new XAttribute("Include", assemblyName);
        var element = new XElement("InternalsVisibleTo", attribute);
        return await _fileSystemService.TryUpdateXmlElementAsync(
            filePath: projectPath,
            handler: x => x.Add(element),
            elementPath: "ItemGroup",
            token: token);
    }
}

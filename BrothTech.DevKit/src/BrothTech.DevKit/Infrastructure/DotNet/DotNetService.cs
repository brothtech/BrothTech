using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.Files;
using Microsoft.Extensions.Logging;
using System.CommandLine.Parsing;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace BrothTech.DevKit.Infrastructure.DotNet;

public interface IDotNetService
{
    Task<Result> TryCreateSolutionAsync(
        string name,
        string directory,
        CancellationToken token);

    Task<Result> TryAddProjectToSolution(
        string solutionPath,
        string projectPath,
        string solutionFolder,
        CancellationToken token);

    Task<Result> TryCreateProject(
        string name,
        DotNetProjectTemplate template,
        string directory,
        CancellationToken token);

    Task<Result> TryAddProjectReferenceAsync(
        string projectPath,
        string referencePath,
        CancellationToken token);

    Task<Result> TryAddPackageReference(
        string projectPath,
        string packageName,
        string packageVersion,
        CancellationToken token);

    Task<Result> TrySetPropertiesAsync(
        string projectPath,
        CancellationToken token,
        params DotNetProjectProperty[] propertyGroups);

    Task<Result> TryAddInternalVisiblityAsync(
        string projectPath,
        string assemblyName,
        CancellationToken token);
}

public class DotNetService(
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
        DotNetProjectTemplate template,
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
        return await TryUpdateXmlElementAsync(projectPath, x => x.Add([.. elements]), "PropertyGroup", token);
    }

    public async Task<Result> TryAddInternalVisiblityAsync(
        string projectPath,
        string assemblyName,
        CancellationToken token)
    {
        var attribute = new XAttribute("Include", assemblyName);
        var element = new XElement("InternalsVisibleTo", attribute);
        return await TryUpdateXmlElementAsync(projectPath, x => x.Add(element), "ItemGroup", token);
    }

    private async Task<Result> TryUpdateXmlElementAsync(
        string filePath,
        Action<XElement> handler,
        string? targetElementName = null,
        CancellationToken token = default)
    {
        try
        {
            using var stream = File.Open(filePath, FileMode.Open);
            var document = await XDocument.LoadAsync(stream, LoadOptions.None, token);
            var targetElement = GetOrCreateTargetElement(document, targetElementName);
            handler(targetElement);
            stream.SetLength(stream.Position = 0);
            await document.SaveAsync(stream, SaveOptions.None, token);
            return Result.Success;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }

    private XElement GetOrCreateTargetElement(
        XDocument document,
        string? targetElementName)
    {
        var root = document.Root.EnsureNotNull();
        if (targetElementName.IsNullOrWhiteSpace())
            return root;

        if (root.Element(targetElementName) is not { } targetElement)
            root.Add(targetElement = new XElement(targetElementName));

        return targetElement;
    }
}

public record DotNetProjectProperty(string Name, string Value);

public enum DotNetProjectTemplate
{
    None,
    ClassLib,
    Console
}

public interface IProcessRunner
{
    Task<Result> TryRunAsync(
        string fileName,
        CancellationToken token,
        params string[] args);
}

public class ProcessRunner :
    IProcessRunner
{
    public async Task<Result> TryRunAsync(
        string fileName,
        CancellationToken token,
        params string[] args)
    {
        var processInfo = GetProcessStartInfo(fileName, args);
        using var process = new Process { StartInfo = processInfo };

        process.Start();

        var stdOutput = await process.StandardOutput.ReadToEndAsync(token);
        var stdError = await process.StandardError.ReadToEndAsync(token);
        
        await process.WaitForExitAsync(token);

        var isSuccessful = process.ExitCode == 0;

        return new Result
        {
            IsSuccessful = isSuccessful,
            Messages = [new ResultMessage(
                Message: isSuccessful ? stdOutput : stdError,
                LogLevel: isSuccessful ? LogLevel.Trace : LogLevel.Error)]
        };
    }

    private ProcessStartInfo GetProcessStartInfo(
        string fileName,
        string[] args)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = fileName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var arg in args)
            processInfo.ArgumentList.Add(arg);

        return processInfo;
    }
}
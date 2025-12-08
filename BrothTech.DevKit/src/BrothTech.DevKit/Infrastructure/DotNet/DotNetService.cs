using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.Files;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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

    Task<Result> TryAddProjectReference(
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

    public Task<Result> TryAddProjectReference(
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
        try
        {
            using var stream = File.Open(projectPath, FileMode.Open);
            var document = await XDocument.LoadAsync(stream, LoadOptions.None, token);
            var root = document.Root.EnsureNotNull();
            if (root.Element("PropertyGroup") is not { } propertyGroup)
                root.Add(propertyGroup = new XElement("PropertyGroup"));

            propertyGroup.Add([.. propertyGroups.Select(x => new XElement(x.Name, x.Value))]);
            stream.SetLength(stream.Position = 0);
            await document.SaveAsync(stream, SaveOptions.None, token);
            return Result.Success;
        }
        catch (Exception exception)
        {
            return exception;
        }
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
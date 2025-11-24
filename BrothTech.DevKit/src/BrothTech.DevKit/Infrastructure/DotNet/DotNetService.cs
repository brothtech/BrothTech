using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.Files;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Xml.Linq;

namespace BrothTech.DevKit.Infrastructure.DotNet;

public interface IDotNetService
{
    Task<Result> TryCreateSolutionAsync(
        string name,
        string path,
        CancellationToken token);

    Task<Result> TryAddProjectToSolution(
        string solutionPath,
        string projectPath,
        CancellationToken token);

    Task<Result> TryCreateProject(
        string name,
        DotNetProjectTemplate template,
        string path,
        CancellationToken token);

    Task<Result> TryAddProjectReference(
        string projectPath,
        string referencePath,
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
        string path,
        CancellationToken token)
    {
        _fileSystemService.EnsureDirectoryExists(path);
        return _processRunner.TryRunAsync(
            fileName: "dotnet",
            token: token,
            "new",
            "sln",
            "-n",
            name);
    }
    public Task<Result> TryAddProjectToSolution(
        string solutionPath, 
        string projectPath,
        CancellationToken token)
    {
        return _processRunner.TryRunAsync(
            fileName: "dotnet",
            token: token,
            "sln",
            solutionPath,
            "add",
            projectPath);
    }

    public Task<Result> TryCreateProject(
        string name,
        DotNetProjectTemplate template,
        string path,
        CancellationToken token)
    {
        _fileSystemService.EnsureDirectoryExists(path);
        return _processRunner.TryRunAsync(
            fileName: "dotnet",
            token: token,
            "new",
            template.ToString().ToLower(),
            "-n",
            name,
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
}

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
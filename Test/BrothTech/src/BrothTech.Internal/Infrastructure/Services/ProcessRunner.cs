using BrothTech.Shared.Contracts.Results;
using BrothTech.Shared.Contracts.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BrothTech.Internal.Infrastructure.Services;

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
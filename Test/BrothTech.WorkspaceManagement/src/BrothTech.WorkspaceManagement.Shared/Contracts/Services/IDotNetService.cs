using BrothTech.Shared.Contracts.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Services;

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
        string template,
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

public record DotNetProjectProperty(string Name, string Value);
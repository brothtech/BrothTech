using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using BrothTech.WorkspaceManagement.Internal;
using BrothTech.WorkspaceManagement.Internal.Infrastructure.Services;
using BrothTech.WorkspaceManagement.Shared.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BrothTech.WorkspaceManagement.Infrastructure.DependencyInjection;

public class WorkspaceManagementServiceRegistration :
    DomainServicesRegistration
{
    protected override Type MarkerType => typeof(WorkspaceManagementMarker);

    protected override bool ShouldRegisterScannedService(
        Type type)
    {
        return type.IsAssignableTo(typeof(ICliCommandBuilder)) ||
               type.IsAssignableTo(typeof(ICliRequestHandler)) ||
               type.IsAssignableTo(typeof(ICliRequestValidator));
    }

    protected override void RegisterAdditionalServices(
        IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IWorkspaceService, WorkspaceService>();
        serviceCollection.AddSingleton<IDotNetService, DotNetService>();
    }
}

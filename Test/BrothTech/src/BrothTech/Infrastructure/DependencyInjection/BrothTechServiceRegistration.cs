using BrothTech.Internal.Infrastructure.DependencyInjection;
using BrothTech.Internal.Infrastructure.Services;
using BrothTech.Shared.Contracts.Services;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace BrothTech.Infrastructure.DependencyInjection;

public class BrothTechServiceRegistration :
    DomainServicesRegistration
{
    protected override Type MarkerType => typeof(BrothTechInternalMarker);

    protected override bool ShouldRegisterScannedService(
        Type type)
    {
        return false;
    }

    protected override void RegisterAdditionalServices(
        IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IFileSystemService, FileSystemService>();
        serviceCollection.AddSingleton<IProcessRunner, ProcessRunner>();
    }
}

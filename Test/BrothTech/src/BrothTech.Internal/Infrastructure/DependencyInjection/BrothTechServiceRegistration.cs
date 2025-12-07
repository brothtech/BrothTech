using BrothTech.Internal.Infrastructure.FileSystem;
using BrothTech.Shared.Contracts.FileSystem;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace BrothTech.Internal.Infrastructure.DependencyInjection;

public class BrothTechServiceRegistration :
    DomainServicesRegistration
{
    protected override Type MarkerType => typeof(BrothTechMarker);

    protected override bool ShouldRegisterScannedService(
        Type type)
    {
        return false;
    }

    protected override void RegisterAdditionalServices(
        IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IFileSystemService, FileSystemService>();
    }
}

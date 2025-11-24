using BrothTech.Cli.Shared.Contracts;
using BrothTech.DevKit.DomainManagement.Services;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace BrothTech.DevKit.Infrastructure.DependencyInjection;

public class DevKitServicesRegistration :
    DomainServicesRegistration
{
    protected override Type MarkerType => typeof(DevKitMarker);

    protected override bool ShouldRegisterService(
        Type type)
    {
        return type.IsAssignableTo(typeof(ICommandHandler)) ||
               type.IsAssignableTo(typeof(ICommandBuilder));
    }

    protected override void RegisterAdditionalServices(
        IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IFileSystemService, FileSystemService>();
        serviceCollection.AddSingleton<IProcessRunner, ProcessRunner>();
        serviceCollection.AddSingleton<IDotNetService, DotNetService>();
        serviceCollection.AddSingleton<IDomainService, DomainService>();
    }
}

using BrothTech.Cli.Commands.Services;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace BrothTech.Cli.Infrastructure.DependencyInjection;

public class CliServicesRegistration :
    DomainServicesRegistration
{
    protected override Type MarkerType => typeof(CliMarker);

    protected override bool ShouldRegisterService(
        Type type)
    {
        return type.IsAssignableTo(typeof(ICommandHandler)) ||
               type.IsAssignableTo(typeof(ICommandBuilder));
    }

    protected override void RegisterAdditionalServices(
        IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ICliCommandInvoker, CliCommandInvoker>();
    }
}

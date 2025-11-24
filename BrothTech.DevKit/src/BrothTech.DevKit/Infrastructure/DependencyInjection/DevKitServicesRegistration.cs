using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;

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
}

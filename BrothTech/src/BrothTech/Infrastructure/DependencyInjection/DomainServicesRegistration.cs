using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace BrothTech.Infrastructure.DependencyInjection;

public abstract class DomainServicesRegistration
{
    protected abstract Type MarkerType { get; }

    public void RegisterServices(
        ILoggerFactory loggerFactory,
        IServiceCollection serviceCollection)
    {
        ILogger? logger = null;
        try
        {
            logger = loggerFactory.CreateLogger(GetType());
            foreach (var serviceType in GetServiceTypes())
                RegisterService(serviceCollection, logger, serviceType);

            RegisterAdditionalServices(serviceCollection);
        }
        catch (Exception exception)
        {
            if (logger?.IsEnabled(LogLevel.Error) is true)
                logger.LogError(
                    exception: exception,
                    message: "An error occurred registering services in {registration}.",
                    GetType().Name);
        }
    }

    private void RegisterService(
        IServiceCollection serviceCollection,
        ILogger logger,
        Type serviceType)
    {
        try
        {
            serviceCollection.Add(serviceType.GetCustomAttribute<ServiceDescriptorAttribute>().EnsureNotNull());
        }
        catch (Exception exception)
        {
            if (logger.IsEnabled(LogLevel.Error))
                logger.LogError(
                    exception: exception,
                    message: "An error occurred registering {service}.",
                    serviceType.Name);
        }
    }

    private IEnumerable<Type> GetServiceTypes()
    {
        var @namespace = MarkerType.Namespace;
        if (@namespace.IsNullOrWhiteSpace())
            return [];

        return MarkerType.Assembly
            .GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Where(x => x.GetName()?.Name?.StartsWith(@namespace) ?? false)
            .SelectMany(x => x.GetTypes())
            .Concat(MarkerType.Assembly.GetTypes())
            .Where(ShouldRegisterService)
            .Where(x => x.IsDefined(typeof(ServiceDescriptorAttribute), true)).ToList();
    }

    protected abstract bool ShouldRegisterService(
        Type type);

    protected virtual void RegisterAdditionalServices(
        IServiceCollection serviceCollection)
    {
    }
}
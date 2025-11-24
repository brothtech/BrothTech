using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.Infrastructure;

public class EntryPoint
{
    private readonly Type[] _registrationTypes;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<EntryPoint> _logger;

    public EntryPoint(
        params Type[] registrationTypes)
    {
        _registrationTypes = registrationTypes.EnsureNotNull();
        _loggerFactory = LoggerFactory.Create(x => x.AddSimpleConsole());
        _logger = _loggerFactory.CreateLogger<EntryPoint>();
    }

    public virtual async Task<int> RunAsync<T1>(
        Func<T1, Task<int>> task)
        where T1 : class
    {
        try
        {
            var serviceProvider = BuildServiceProvider();
            var service1 = serviceProvider.GetRequiredService<T1>();
            return await task(service1);
        }
        catch (Exception exception)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(
                    exception: exception,
                    message: "An error occurred executing the entry point task.");
            return 1;
        }
    }

    public virtual IServiceProvider BuildServiceProvider()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(x => x.AddSimpleConsole());
        foreach (var type in _registrationTypes)
            ExecuteDomainServicesRegistration(serviceCollection, type);

        return serviceCollection.BuildServiceProvider();
    }

    protected virtual void ExecuteDomainServicesRegistration(
        IServiceCollection serviceCollection,
        Type type)
    {
        try
        {
            var servicesRegistration = (DomainServicesRegistration)Activator.CreateInstance(type).EnsureNotNull();
            servicesRegistration.RegisterServices(_loggerFactory, serviceCollection);
        }
        catch (Exception exception)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(
                    exception: exception,
                    message: "An error occurred executing {serviceRegistration}.",
                    type.Name);
        }
    }
}
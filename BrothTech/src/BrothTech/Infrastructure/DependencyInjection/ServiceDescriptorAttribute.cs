using Microsoft.Extensions.DependencyInjection;

namespace BrothTech.Infrastructure.DependencyInjection;

public class ServiceDescriptorAttribute<TService, TImplementation>(
    object? serviceKey = null,
    ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) :
    ServiceDescriptorAttribute(typeof(TService), serviceKey, typeof(TImplementation), serviceLifetime)
    where TService : class
    where TImplementation : class, TService
{
}

public class ServiceDescriptorAttribute<TService>(
    object? serviceKey = null,
    ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) :
    ServiceDescriptorAttribute(typeof(TService), serviceKey, serviceLifetime: serviceLifetime)
    where TService : class
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public abstract class ServiceDescriptorAttribute(
    Type serviceType,
    object? serviceKey = null,
    Type? implementationType = null,
    ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) :
    Attribute
{
    public Type ServiceType { get; } = serviceType.EnsureNotNull();
    public Type? ImplementationType { get; } = implementationType;
    public object? ServiceKey { get; } = serviceKey;
    public ServiceLifetime ServiceLifetime { get; } = serviceLifetime;

    public static implicit operator ServiceDescriptor(
        ServiceDescriptorAttribute attribute)
    {
        return new ServiceDescriptor(
            serviceType: attribute.ServiceType,
            serviceKey: attribute.ServiceKey,
            implementationType: attribute.ImplementationType ?? attribute.ServiceType,
            lifetime: attribute.ServiceLifetime);
    }
}
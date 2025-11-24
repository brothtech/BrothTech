using BrothTech.Sql.Database.Model;

namespace BrothTech.Sql.Database.Builders;

public abstract class SqlObjectBuilder<TInterface, TImplementation>
    where TInterface : ISqlObject
    where TImplementation : SqlObject, TInterface
{
    private TInterface? _result;
    
    protected List<Action<TImplementation>> BuildActions { get; } = [];

    public virtual TInterface Build()
    {
        if (_result is not null)
            return _result;
        var implementation = CreateImplementation();
        foreach (var buildAction in BuildActions)
            buildAction(implementation);
        
        return _result = implementation;
    }

    protected abstract TImplementation CreateImplementation();
}

public abstract class SqlObjectBuilder<TInterface, TImplementation, TParent, TParentInterface, TParentImplementation>(
    TParent parent) :
    SqlObjectBuilder<TInterface, TImplementation>
    where TInterface : ISqlObject
    where TImplementation : SqlObject, TInterface
    where TParentInterface : ISqlObject
    where TParentImplementation : SqlObject, TParentInterface
    where TParent : SqlObjectBuilder<TParentInterface, TParentImplementation>
{
    protected TParent Parent { get; } = parent;
    
    protected class DependencyMissingException(string name) :
        Exception($"Dependency {name} is missing.");
}
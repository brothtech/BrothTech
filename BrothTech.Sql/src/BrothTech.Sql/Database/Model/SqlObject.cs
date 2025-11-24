using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using SetWorks.Platform.Shared.CodeGeneration.Sql.Model.Validation;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface ISqlObject
{
    [JsonIgnore]
    ISqlObject Parent { get; }
    
    string Name { get; }

    IEnumerable<ISqlObject> Children { get; }

    ISqlObject? GetChild(
        string name);
}

public abstract class SqlObject :
    ISqlObject
{
    private SqlObject? _parent;
    private readonly Dictionary<string, ISqlObject> _children = [];

    [JsonIgnore]
    public SqlObject Parent
    {
        get => _parent ?? throw new SqlParentNotSetException(GetType());
        set
        {
            if (_parent is not null)
                throw new SqlParentAlreadySetException(GetType());
            ValidateParent(value);
            _parent = value;
        }
    }

    ISqlObject ISqlObject.Parent => Parent;

    protected virtual Type ParentType => throw new HasNoParentException(GetType());

    public IReadOnlyDictionary<string, ISqlObject> Children
    {
        get => _children;
        init => _children = value as Dictionary<string, ISqlObject> ?? throw new ChildrenTypeException();
    }

    IEnumerable<ISqlObject> ISqlObject.Children => Children.Values;

    protected virtual Type ChildType => throw new HasNoChildrenException(GetType());
    
    [SqlIdentifier]
    public required string Name { get; init; }

    private void ValidateParent(
        SqlObject? parent)
    {
        if (parent is null || parent.GetType().IsAssignableTo(ParentType) is false)
            throw new InvalidParentTypeException(GetType(), ParentType);
    }
    
    public ISqlObject? GetChild(
        string name)
    {
        return Children.GetValueOrDefault(name);
    }

    public void AddChild(
        SqlObject child)
    {
        ValidateChild(child);
        child.Parent = this;
        _children.Add(child.Name, child);
    }

    private void ValidateChild(
        SqlObject? child)
    {
        if (child is null || child.GetType().IsAssignableTo(ChildType) is false)
            throw new InvalidChildTypeException(GetType(), ChildType);
    }

    public override string ToString() => $"{Name}";

    private class SqlParentNotSetException(Type sqlObjectType) :
        Exception($"The parent of {sqlObjectType.Name} has not been set.");

    private class SqlParentAlreadySetException(Type sqlObjectType) :
        Exception($"The parent of {sqlObjectType.Name} has already been set.");

    private class InvalidParentTypeException(Type sqlObjectType, Type parentType) :
        Exception($"Parent of type {sqlObjectType.Name} must be of type {parentType.Name} and not null.");
    
    private class HasNoParentException(Type sqlObjectType) :
        Exception($"{sqlObjectType.Name} has no parent.");
    
    private class ChildrenTypeException() :
        TypeException(nameof(Children), typeof(Dictionary<string, ISqlObject>));

    private class InvalidChildTypeException(Type sqlObjectType, Type childType) :
        Exception($"Child of type {sqlObjectType.Name} must be of type {childType.Name} and not null.");

    private class HasNoChildrenException(Type sqlObjectType) :
        Exception($"{sqlObjectType.Name} has no children.");

    protected class TypeException(string memberName, Type type) :
        Exception($"{memberName} must be set to {type.Name}.");
}
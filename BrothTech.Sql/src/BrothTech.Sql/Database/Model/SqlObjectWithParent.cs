using System.Text.Json.Serialization;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface ISqlObjectWithParent<out TParent> :
    ISqlObject
    where TParent : class, ISqlObject
{
    [JsonIgnore]
    new TParent Parent { get; }
}

public class SqlObjectWithParent<TParent> :
    SqlObject,
    ISqlObjectWithParent<TParent>
    where TParent : class, ISqlObject
{
    protected override Type ParentType => typeof(TParent);

    protected TParent ParentInternal => Parent as TParent ?? throw new UnexpectedParentStateException(GetType());

    TParent ISqlObjectWithParent<TParent>.Parent => ParentInternal;

    private class UnexpectedParentStateException(Type sqlObjectType) :
        Exception($"Parent of type {sqlObjectType.Name} in an unexpected state.");
}
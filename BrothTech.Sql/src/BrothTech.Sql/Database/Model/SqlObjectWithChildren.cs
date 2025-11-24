namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface ISqlObjectWithChildren<out TChild> :
    ISqlObject
    where TChild : class, ISqlObject
{
    new IEnumerable<TChild> Children { get; }

    new TChild? GetChild(
        string name);
}

public class SqlObjectWithChildren<TChild> :
    SqlObject,
    ISqlObjectWithChildren<TChild>
    where TChild : class, ISqlObject
{
    protected override Type ChildType => typeof(TChild);

    protected IEnumerable<TChild> ChildrenInternal => Children.Values.Cast<TChild>();

    IEnumerable<TChild> ISqlObjectWithChildren<TChild>.Children => ChildrenInternal;

    protected TChild? GetChildInternal(string name) => GetChild(name) as TChild;

    TChild? ISqlObjectWithChildren<TChild>.GetChild(string name) => GetChildInternal(name);

    protected void AddChildInternal(TChild child) => AddChild((child as SqlObject)!);
}
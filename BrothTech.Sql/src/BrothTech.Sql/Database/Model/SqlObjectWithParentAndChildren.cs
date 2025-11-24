namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface ISqlObjectWithParentAndChildren<out TParent, out TChild> :
    ISqlObjectWithParent<TParent>,
    ISqlObjectWithChildren<TChild>
    where TParent : class, ISqlObject
    where TChild : class, ISqlObject
{
}

public class SqlObjectWithParentAndChildren<TParent, TChild> :
    SqlObjectWithParent<TParent>,
    ISqlObjectWithParentAndChildren<TParent, TChild> 
    where TParent : class, ISqlObject
    where TChild : class, ISqlObject
{
    protected override Type ChildType => typeof(TChild);

    protected IEnumerable<TChild> ChildrenInternal => Children.Values.Cast<TChild>();

    IEnumerable<TChild> ISqlObjectWithChildren<TChild>.Children => ChildrenInternal;

    protected TChild? GetChildInternal(string name) => GetChild(name) as TChild;

    TChild? ISqlObjectWithChildren<TChild>.GetChild(string name) => GetChildInternal(name);

    protected void AddChildInternal(TChild child) => AddChild((child as SqlObject)!);
}
namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public class ColumnReference :
    SqlObjectWithParent<IIndex>,
    IColumn
{
    private IColumn? _referencedColumn;
    
    public ITable Table => GetReferencedColumn().Table;
    ITable ISqlObjectWithParent<ITable>.Parent => GetReferencedColumn().Parent;
    public ColumnFlag Flags => GetReferencedColumn().Flags;
    public ISqlType Type => GetReferencedColumn().Type;
    public IIdentity? Identity => GetReferencedColumn().Identity;
    public IDefault? Default => GetReferencedColumn().Default;
    public int Sequence => GetReferencedColumn().Sequence;
    public ITable? ReferencedTable => GetReferencedColumn().ReferencedTable;
    public string Definition => GetReferencedColumn().Definition;
    
    protected IColumn GetReferencedColumn()
    {
        return _referencedColumn ??= ParentInternal.Table.GetChild(Name) ?? 
                                     throw new ColumnReferenceException(Name);
    }
    
    private class ColumnReferenceException(string name) : Exception($"Column reference {name} is invalid.");
}
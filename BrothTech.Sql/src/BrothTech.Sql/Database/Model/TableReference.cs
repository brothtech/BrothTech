using SetWorks.Platform.Shared.CodeGeneration.Sql.Model.Validation;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public class TableReference :
    SqlObjectWithParent<IColumn>,
    ITable
{
    private ITable? _referencedTable;
    
    [SqlIdentifier]
    public required string SchemaName { get; init; }
    public ISchema Schema => GetReferencedTable().Schema;
    ISchema ISqlObjectWithParent<ISchema>.Parent => GetReferencedTable().Parent;
    public IPrimaryKeyColumn PrimaryKey => GetReferencedTable().PrimaryKey;
    public ISystemVersioningColumns? SystemVersioningColumns => GetReferencedTable().SystemVersioningColumns;
    public IEnumerable<IIndex> Indices => GetReferencedTable().Indices;
    public IEnumerable<IColumn> Columns => GetReferencedTable().Columns;
    IEnumerable<IColumn> ISqlObjectWithChildren<IColumn>.Children => GetReferencedTable().Columns;
    
    IColumn? ISqlObjectWithChildren<IColumn>.GetChild(string name) => GetReferencedTable().GetChild(name);
    public IColumn? GetColumn(string name) => GetReferencedTable().GetColumn(name);
    protected ITable GetReferencedTable()
    {
        return _referencedTable ??= ParentInternal.Table.Schema.Database.GetSchema(SchemaName)?.GetTable(Name) ??
                                    throw new TableReferenceException(Name);
    }
    
    private class TableReferenceException(string name) : Exception($"Table reference {name} is invalid.");
}
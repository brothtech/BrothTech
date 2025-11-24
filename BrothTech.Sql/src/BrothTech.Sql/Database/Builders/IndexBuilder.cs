using BrothTech.Sql.Database.Model;

namespace BrothTech.Sql.Database.Builders;

public interface IRequireIndexColumn
{
    IndexBuilder WithIndexColumn(
        string columnName);
}

public interface IIndexBuilder :
    ITableBuilder
{
}

public class IndexBuilder(
    TableBuilder tableBuilder,
    string name,
    IndexFlag flags) :
    SqlObjectBuilder<IIndex, Index, TableBuilder, ITable, Table>(tableBuilder),
    IIndexBuilder,
    IRequireIndexColumn
{
    private readonly string _name = name;
    private readonly IndexFlag _flags = flags;
    
    protected override Index CreateImplementation()
    {
        return new Index
        {
            Name = _name,
            Flags = _flags
        };
    }

    public IDatabase BuildDatabase() => Parent.BuildDatabase();

    public SchemaBuilder WithSchema(string schemaName) => Parent.WithSchema(schemaName);

    public ISchema BuildSchema() => Parent.BuildSchema();

    public IRequirePrimaryKey WithTable(string tableName) => Parent.WithTable(tableName);

    public ITable BuildTable() => Parent.BuildTable();

    public TableBuilder WithSystemVersioningColumns(
        string historyTableName, 
        string startColumnName, 
        string endColumnName,
        int columnPrecision = 7, 
        bool isHidden = false)
    {
        return Parent.WithSystemVersioningColumns(
            historyTableName,
            startColumnName,
            endColumnName,
            columnPrecision,
            isHidden);
    }

    public IRequireType<IColumn, Column> WithColumn(string columnName) => Parent.WithColumn(columnName);

    public IRequireIndexColumn WithIndex(string indexName, IndexFlag flags) => Parent.WithIndex(indexName, flags);

    public IndexBuilder WithIndexColumn(
        string columnName)
    {
        BuildActions.Add(x =>
        {
            x.AddColumn(new IndexColumnReference
            {
                Name = columnName,
                IsIncluded = false
            });
        });
        return this;
    }

    public IndexBuilder WithIncludedIndexColumn(
        string columnName)
    {
        BuildActions.Add(x =>
        {
            x.AddColumn(new IndexColumnReference
            {
                Name = columnName,
                IsIncluded = true
            });
        });
        return this;
    }
}
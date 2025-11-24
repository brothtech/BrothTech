using BrothTech.Sql.Database.Model;

namespace BrothTech.Sql.Database.Builders;

public interface IRequirePrimaryKey
{
    IRequireType<PrimaryKeyColumn, PrimaryKeyColumn> WithPrimaryKey(
        string columnName,
        bool isClustered = true);
}

public interface ITableBuilder :
    ISchemaBuilder
{
    ITable BuildTable();
    
    TableBuilder WithSystemVersioningColumns(
        string historyTableName,
        string startColumnName,
        string endColumnName,
        int columnPrecision = 7,
        bool isHidden = false);

    IRequireIndexColumn WithIndex(
        string indexName,
        IndexFlag flags);


    IRequireType<IColumn, Column> WithColumn(
        string columnName);
}

public class TableBuilder(
    SchemaBuilder schemaBuilder,
    string name) :
    SqlObjectBuilder<ITable, Table, SchemaBuilder, ISchema, Schema>(schemaBuilder),
    ITableBuilder,
    IRequirePrimaryKey
{
    private readonly string _name = name;
    private Func<PrimaryKeyColumn>? _buildPrimaryKey;
    
    protected override Table CreateImplementation()
    {
        return new Table
        {
            Name = _name,
            PrimaryKey = _buildPrimaryKey?.Invoke() ?? throw new DependencyMissingException(nameof(Table.PrimaryKey))
        };
    }

    public IDatabase BuildDatabase() => Parent.BuildDatabase();

    public SchemaBuilder WithSchema(string schemaName) => Parent.WithSchema(schemaName);

    public ISchema BuildSchema() => Parent.BuildSchema();
    
    public IRequirePrimaryKey WithTable(string tableName) => Parent.WithTable(tableName);

    public ITable BuildTable() => Build();

    public IRequireType<PrimaryKeyColumn, PrimaryKeyColumn> WithPrimaryKey(
        string columnName,
        bool isClustered = true)
    {
        var columnBuilder = new PrimaryKeyColumnBuilder(this, columnName, isClustered);
        _buildPrimaryKey = () => columnBuilder.Build();
        return columnBuilder;
    }

    public TableBuilder WithSystemVersioningColumns(
        string historyTableName,
        string startColumnName,
        string endColumnName,
        int columnPrecision = 7,
        bool isHidden = false)
    {
        BuildActions.Add(x =>
        {
            x.SystemVersioningColumns = new SystemVersioningColumns
            {
                HistoryTableName = historyTableName,
                StartColumnName = startColumnName,
                EndColumnName = endColumnName,
                ColumnPrecision = columnPrecision,
                IsHidden = isHidden
            };
        });
        return this;
    }

    public IRequireIndexColumn WithIndex(
        string indexName,
        IndexFlag flags)
    {
        var indexBuilder = new IndexBuilder(this, indexName, flags);
        BuildActions.Add(x => x.AddIndex(indexBuilder.Build()));
        return indexBuilder;
    }

    public IRequireType<IColumn, Column> WithColumn(
        string columnName)
    {
        var columnBuilder = new ColumnBuilder(this, columnName);
        BuildActions.Add(x => x.AddColumn(columnBuilder.Build()));
        return columnBuilder;
    }
}
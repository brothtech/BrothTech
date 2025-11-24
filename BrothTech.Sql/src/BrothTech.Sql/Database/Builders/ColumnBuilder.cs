using BrothTech.Sql.Database.Model;

namespace BrothTech.Sql.Database.Builders;

public interface IColumnBuilder<TInterface, TImplementation> :
    ITableBuilder
    where TInterface : IColumn
    where TImplementation : Column, TInterface
{
    ColumnBuilder<TInterface, TImplementation> WithIdentity(
        int seed = 0,
        int increment = 1);

    ColumnBuilder<TInterface, TImplementation> WithDefault(
        string valueExpression,
        string? name = null);

    ColumnBuilder<TInterface, TImplementation> WithForeignKey(
        string schemaName,
        string tableName);
}

public interface IRequireType<TInterface, TImplementation>
    where TInterface : IColumn
    where TImplementation : Column, TInterface
{
    SqlTypeBuilder<TInterface, TImplementation> WithType(
        string typeName,
        bool isNullable = false,
        params ReadOnlySpan<int> argumentValues);
}

public abstract class ColumnBuilder<TInterface, TImplementation>(
    TableBuilder tableBuilder) :
    SqlObjectBuilder<TInterface, TImplementation, TableBuilder, ITable, Table>(tableBuilder),
    IColumnBuilder<TInterface, TImplementation>
    where TInterface : IColumn
    where TImplementation : Column, TInterface
{
    private Func<ISqlType>? _buildType;

    protected override TImplementation CreateImplementation()
    {
        var type = _buildType?.Invoke() ?? throw new DependencyMissingException(nameof(Column.Type));
        return CreateImplementation(type);
    }

    protected abstract TImplementation CreateImplementation(ISqlType type);

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
    
    public SqlTypeBuilder<TInterface, TImplementation> WithType(
        string typeName,
        bool isNullable = false,
        params ReadOnlySpan<int> argumentValues)
    {
        var sqlTypeBuilder = new SqlTypeBuilder<TInterface, TImplementation>(this, typeName, isNullable, argumentValues);
        _buildType = () => sqlTypeBuilder.Build();
        return sqlTypeBuilder;
    }

    public ColumnBuilder<TInterface, TImplementation> WithIdentity(
        int seed = 0,
        int increment = 1)
    {
        BuildActions.Add(x => x.Identity = new Identity { Seed = seed, Increment = increment });
        return this;
    }

    public ColumnBuilder<TInterface, TImplementation> WithDefault(
        string valueExpression,
        string? name = null)
    {
        BuildActions.Add(x => x.Default = new Default { ValueExpression = valueExpression, Name = name });
        return this;
    }

    public ColumnBuilder<TInterface, TImplementation> WithForeignKey(
        string schemaName,
        string tableName)
    {
        BuildActions.Add(x => x.ReferencedTable = new TableReference { SchemaName = schemaName, Name = tableName });
        return this;
    }
}

public class ColumnBuilder(
    TableBuilder tableBuilder,
    string name) :
    ColumnBuilder<IColumn, Column>(tableBuilder),
    IRequireType<IColumn, Column>
{
    private readonly string _name = name;

    protected override Column CreateImplementation(
        ISqlType type)
    {
        return new Column
        {
            Name = _name,
            Type = type
        };
    }
}

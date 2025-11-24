using BrothTech.Sql.Database.Model;

namespace BrothTech.Sql.Database.Builders;

public interface ISqlTypeBuilder<TParentInterface, TParentImplementation> :
    IColumnBuilder<TParentInterface, TParentImplementation>
    where TParentInterface : IColumn
    where TParentImplementation : Column, TParentInterface
{
}

public class SqlTypeBuilder<TParentInterface, TParentImplementation>(
    ColumnBuilder<TParentInterface, TParentImplementation> columnBuilder,
    string name,
    bool isNullable,
    ReadOnlySpan<int> argumentValues) :
    SqlObjectBuilder<ISqlType, SqlType, ColumnBuilder<TParentInterface, TParentImplementation>, TParentInterface, TParentImplementation>(columnBuilder),
    ISqlTypeBuilder<TParentInterface, TParentImplementation>
    where TParentInterface : IColumn
    where TParentImplementation : Column, TParentInterface
{
    private readonly string _name = name;
    private readonly bool _isNullable = isNullable;
    private readonly int[] _argumentValues = argumentValues.ToArray();
    
    protected override SqlType CreateImplementation()
    {
        WithTypeArguments(_argumentValues.AsSpan());
        return new SqlType
        {
            Name = _name,
            IsNullable = _isNullable
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

    public ColumnBuilder<TParentInterface, TParentImplementation> WithIdentity(int seed = 0, int increment = 1) => Parent.WithIdentity(seed, increment);

    public ColumnBuilder<TParentInterface, TParentImplementation> WithDefault(string valueExpression, string? name = null) => Parent.WithDefault(valueExpression, name);
    
    public ColumnBuilder<TParentInterface, TParentImplementation> WithForeignKey(string schemaName, string tableName) => Parent.WithForeignKey(schemaName, tableName);

    public SqlTypeBuilder<TParentInterface, TParentImplementation> WithTypeArguments(
        params ReadOnlySpan<int> argumentValues)
    {
        foreach (var argumentValue in argumentValues)
            _ = WithTypeArgument(argumentValue);

        return this;
    }

    public SqlTypeBuilder<TParentInterface, TParentImplementation> WithTypeArgument(
        int argumentValue)
    {
        BuildActions.Add(x => x.SqlTypeArguments.Add(new SqlTypeArgument { Value = argumentValue }));
        return this;
    }

    public SqlTypeBuilder<TParentInterface, TParentImplementation> WithMaxTypeArgument()
    {
        BuildActions.Add(x => x.SqlTypeArguments.Add(SqlTypeArgument.Max));
        return this;
    }
}
using BrothTech.Sql.Database.Model;

namespace BrothTech.Sql.Database.Builders;

public interface ISchemaBuilder :
    IDatabaseBuilder
{
    ISchema BuildSchema();
    
    IRequirePrimaryKey WithTable(
        string tableName);
}

public class SchemaBuilder(
    DatabaseBuilder databaseBuilder,
    string name) : 
    SqlObjectBuilder<ISchema, Schema, DatabaseBuilder, IDatabase, Database>(databaseBuilder), 
    ISchemaBuilder
{
    private readonly string _name = name;
    
    protected override Schema CreateImplementation()
    {
        return new Schema { Name = _name };
    }

    public IDatabase BuildDatabase() => Parent.BuildDatabase();

    public SchemaBuilder WithSchema(string schemaName) => Parent.WithSchema(schemaName);

    public ISchema BuildSchema() => Build();
    
    public IRequirePrimaryKey WithTable(
        string tableName)
    {
        var tableBuilder = new TableBuilder(this, tableName);
        BuildActions.Add(x => x.AddTable(tableBuilder.Build()));
        return tableBuilder;
    }
}
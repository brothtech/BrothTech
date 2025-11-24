using BrothTech.Sql.Database.Model;

namespace BrothTech.Sql.Database.Builders;

public interface IDatabaseBuilder
{
    IDatabase BuildDatabase();
    
    SchemaBuilder WithSchema(
        string schemaName);
}

public class DatabaseBuilder(
    string name) :
    SqlObjectBuilder<IDatabase, Database>,
    IDatabaseBuilder
{
    private readonly string _name = name;

    protected override Database CreateImplementation()
    {
        return new Database { Name = _name };
    }

    public IDatabase BuildDatabase() => Build();

    public SchemaBuilder WithSchema(
        string schemaName)
    {
        var schemaBuilder = new SchemaBuilder(this, schemaName);
        BuildActions.Add(x => x.AddSchema(schemaBuilder.Build()));
        return schemaBuilder;
    }
}
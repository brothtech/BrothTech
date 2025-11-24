using System.Diagnostics.CodeAnalysis;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface IDatabase : 
    ISqlObjectWithChildren<ISchema>
{
    IEnumerable<ISchema> Schemas { get; }
    
    ISchema? GetSchema(
        string name);
}

public class Database :
    SqlObjectWithChildren<ISchema>,
    IDatabase
{
    public IEnumerable<ISchema> Schemas => ChildrenInternal;

    public ISchema? GetSchema(string name) => GetChildInternal(name);

    public void AddSchema(ISchema schema) => AddChildInternal(schema);
}
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface ISchema :
    ISqlObjectWithParentAndChildren<IDatabase, ITable>
{
    [JsonIgnore]
    IDatabase Database { get; }
    
    IEnumerable<ITable> Tables { get; }
    
    ITable? GetTable(
        string name);
}

public class Schema :
    SqlObjectWithParentAndChildren<IDatabase, ITable>,
    ISchema
{
    public IDatabase Database => ParentInternal;

    public IEnumerable<ITable> Tables => ChildrenInternal;

    public ITable? GetTable(string name) => GetChildInternal(name);

    public void AddTable(ITable table) => AddChildInternal(table);
}
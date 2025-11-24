using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface ITable :
    ISqlObjectWithParentAndChildren<ISchema, IColumn>
{
    [JsonIgnore]
    ISchema Schema { get; }
    
    IPrimaryKeyColumn PrimaryKey { get; }

    ISystemVersioningColumns? SystemVersioningColumns { get; }

    IEnumerable<IColumn> Columns { get; }

    IEnumerable<IIndex> Indices { get; }
    
    IColumn? GetColumn(
        string name);
}

public class Table :
    SqlObjectWithParentAndChildren<ISchema, IColumn>,
    ITable
{
    private SystemVersioningColumns? _systemVersioningColumns;
    private List<IIndex> _indices = [];

    public ISchema Schema => ParentInternal;

    public required PrimaryKeyColumn PrimaryKey
    {
        get => Children.Values.OfType<PrimaryKeyColumn>().Single(x => x.Flags.HasFlag(ColumnFlag.PrimaryKey));
        init => AddChild(value);
    }

    IPrimaryKeyColumn ITable.PrimaryKey => PrimaryKey;

    public SystemVersioningColumns? SystemVersioningColumns
    {
        get => _systemVersioningColumns;
        set
        {
            _systemVersioningColumns = value;
            if (value is null)
                return;
            AddChild(value.StartColumn);
            AddChild(value.EndColumn);
        }
    }

    ISystemVersioningColumns? ITable.SystemVersioningColumns => SystemVersioningColumns;

    public IEnumerable<IIndex> Indices { get => _indices; init => _indices = value as List<IIndex> ?? throw new TypeException(nameof(Indices), typeof(List<IIndex>)); }

    public IEnumerable<IColumn> Columns => ChildrenInternal.OrderBy(x => x.Sequence);

    public IColumn? GetColumn(string name) => GetChildInternal(name);

    public void AddIndex(
        IIndex index)
    {
        var indexImplementation = index as Index ?? throw new TypeException(nameof(index), typeof(Index));
        indexImplementation.Parent = this;
        _indices.Add(indexImplementation);
    }

    public void AddColumn(IColumn column) => AddChildInternal(column);    
}
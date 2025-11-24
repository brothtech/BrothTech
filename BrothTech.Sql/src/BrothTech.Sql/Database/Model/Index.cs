using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

[Flags]
public enum IndexFlag
{
    None = 0,
    PrimaryKey = 1 << 0,
    Clustered = 1 << 1,
    Unique = 1 << 2
}

public interface IIndex :
    ISqlObjectWithParentAndChildren<ITable, IColumn>
{
    [JsonIgnore]
    ITable Table { get; }
    
    IndexFlag Flags { get; }

    IEnumerable<IColumn> Columns { get; }

    IEnumerable<IColumn> IncludedColumns { get; }

    IColumn? GetColumn(
        string name);
}

public class Index :
    SqlObjectWithParentAndChildren<ITable, IColumn>,
    IIndex,
    IValidatableObject
{
    public ITable Table => ParentInternal;

    public IndexFlag Flags { get; init; }

    protected override Type ChildType => typeof(IndexColumnReference);
    
    public IEnumerable<IColumn> IncludedColumns => ChildrenInternal.Cast<IndexColumnReference>().Where(x => x.IsIncluded).OrderBy(x => x.Sequence);

    public IEnumerable<IColumn> Columns => ChildrenInternal.Cast<IndexColumnReference>().Where(x => x.IsIncluded is false).OrderBy(x => x.Sequence);

    public IColumn? GetColumn(string name) => GetChildInternal(name);

    public void AddColumn(IColumn column) => AddChildInternal(column);
    
    public virtual IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        foreach (var validationResult in ValidateIncludedColumns())
            yield return validationResult;
    }

    //TODO: need validation that says cannot have included columns with no regular columns
    private IEnumerable<ValidationResult> ValidateIncludedColumns()
    {
        if (IncludedColumns.Any() is false)
            yield break;

        if (Flags.HasFlag(IndexFlag.Clustered))
            yield return new ValidationResult(
                $"No values can be present in {nameof(IncludedColumns)} when {nameof(Flags)} has the {IndexFlag.Clustered} flag.",
                [nameof(IncludedColumns), nameof(Flags)]);
    }
}
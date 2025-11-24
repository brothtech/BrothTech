using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ConsoleApp1;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

[Flags]
public enum ColumnFlag
{
    None = 0,
    PrimaryKey = 1 << 0,
    SystemVersioningStart = 1 << 1,
    SystemVersioningEnd = 1 << 2
}

public interface IColumn :
    ISqlObjectWithParent<ITable>
{
    [JsonIgnore]
    ITable Table { get; }
    
    ColumnFlag Flags { get; }
    
    ISqlType Type { get; }
    
    IIdentity? Identity { get; }
    
    IDefault? Default { get; }

    int Sequence { get; }

    ITable? ReferencedTable { get; }

    string Definition { get; }
}

public class Column :
    SqlObjectWithParent<ITable>,
    IColumn,
    IValidatableObject
{
    private int? _sequence;
    private TableReference? _referencedTable;
    
    public ITable Table => ParentInternal;
    
    public ColumnFlag Flags { get; init; }
    
    public required ISqlType Type { get; init; }
    
    public IIdentity? Identity { get; set; }
    
    public IDefault? Default { get; set; }
    
    public int Sequence { get => _sequence ??= Table.Columns.Count(); init => _sequence = value; }

    public TableReference? ReferencedTable
    {
        get => _referencedTable;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _referencedTable = value;
            _referencedTable.Parent = this;
        }
    }
    
    ITable? IColumn.ReferencedTable => ReferencedTable;

    public virtual string Definition => $"[{Name}] {Type} {Identity} {Default}";
    
    //TODO: Referenced table cannot be the table this column belongs to.
    public virtual IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        foreach (var validationResult in ValidateIdentity())
            yield return validationResult;
    }
    
    private IEnumerable<ValidationResult> ValidateIdentity()
    {
        if (Identity is null)
            yield break;
        
        if (Type.Name.NotIn(SqlType.Names.TinyInt, SqlType.Names.SmallInt, SqlType.Names.TinyInt, SqlType.Names.BigInt))
            yield return new ValidationResult(
                $"{nameof(Type)} must be {SqlType.Names.TinyInt}, {SqlType.Names.TinyInt}, {SqlType.Names.Int}, or {SqlType.Names.BigInt} when {Identity} is present.",
                [nameof(Type), nameof(Identity)]);

        if (Default is not null)
            yield return new ValidationResult(
                $"{nameof(Default)} cannot be present when {Identity} is present.",
                [nameof(Default)]);
    }
}

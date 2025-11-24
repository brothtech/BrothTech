using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface IPrimaryKeyColumn :
    IColumn
{
    bool IsClustered { get; }
}

public class PrimaryKeyColumn :
    Column,
    IPrimaryKeyColumn
{
    public bool IsClustered { get; set; }
    
    public PrimaryKeyColumn()
    {
        Flags = ColumnFlag.PrimaryKey;
    }

    public override IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        foreach (var validationResult in base.Validate(validationContext))
            yield return validationResult;
        foreach (var validationResult in ValidateType())
            yield return validationResult;
    }

    private IEnumerable<ValidationResult> ValidateType()
    {
        if (Type.IsNullable)
            yield return new ValidationResult(
                $"{nameof(Type)} must not be nullable on primary key columns.",
                [nameof(Type)]);
    }
}
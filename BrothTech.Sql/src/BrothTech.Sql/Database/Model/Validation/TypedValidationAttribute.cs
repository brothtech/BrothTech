using System.ComponentModel.DataAnnotations;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model.Validation;

[AttributeUsage(AttributeTargets.Property)]
public abstract class TypedValidationAttribute<T> :
    ValidationAttribute
{
    protected sealed override ValidationResult? IsValid(
        object? value, 
        ValidationContext validationContext)
    {
        return value switch
        {
            null => ValidationResult.Success,
            T tValue => IsValid(tValue, validationContext),
            _ => new ValidationResult($"[{validationContext.DisplayName}] is not of expected type [{typeof(T).Name}].")
        };
    }

    protected abstract ValidationResult? IsValid(
        T value,
        ValidationContext validationContext);
}
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model.Validation;

public partial class SqlIdentifierAttribute :
    TypedValidationAttribute<string>
{
    [GeneratedRegex(@"^(?:[^\]]|(?:\]\])+){1,128}$", RegexOptions.Singleline, 50)]
    private static partial Regex _validSqlIdentifierRegex { get; }
    
    protected override ValidationResult? IsValid(
        string value, 
        ValidationContext validationContext)
    {
        if (_validSqlIdentifierRegex.IsMatch(value))
            return ValidationResult.Success;

        return new ValidationResult($"[{validationContext.DisplayName}] is not a valid sql identifier.");
    }
}
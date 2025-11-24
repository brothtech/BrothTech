using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model.Validation;

public partial class SqlExpressionAttribute :
    TypedValidationAttribute<string>
{
    [GeneratedRegex(@"^;|--|/\*|\*/|\b(DROP|ALTER|CREATE|INSERT|UPDATE|DELETE|EXEC|EXECUTE|BEGIN|END|DECLARE|MERGE|GRANT|DENY|REVOKE|WAITFOR|KILL|OPENROWSET|OPENDATASOURCE|XP_)\b$", RegexOptions.Singleline, 50)]
    private static partial Regex _sqlExpressionBlacklistRegex { get; }

    protected override ValidationResult? IsValid(
        string value, 
        ValidationContext validationContext)
    {
        if (_sqlExpressionBlacklistRegex.IsMatch(value) is false) 
            return ValidationResult.Success;

        return new ValidationResult($"[{validationContext.DisplayName}] is not a valid sql expression.");
    }
}
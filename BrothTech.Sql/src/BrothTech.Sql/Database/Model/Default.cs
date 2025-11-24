using ConsoleApp1;
using SetWorks.Platform.Shared.CodeGeneration.Sql.Model.Validation;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface IDefault
{
    string? Name { get; }

    string ValueExpression { get; }
}

public class Default :
    IDefault
{
    [SqlIdentifier]
    public string? Name { get; init; }

    [SqlExpression]
    public required string ValueExpression { get; init; }

    public override string ToString()
    {
        return Name.IfNotWhiteSpace(x => $"CONSTRAINT [{x}] ") + $"DEFAULT ({ValueExpression})";
    }
}
namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public class IndexColumnReference :
    ColumnReference
{
    public required bool IsIncluded { get; init; }
}
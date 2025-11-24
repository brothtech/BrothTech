namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface IIdentity
{
    int Seed { get; }
    
    int Increment { get; }
}

public class Identity :
    IIdentity
{
    public required int Seed { get; init; }
    
    public required int Increment { get; init; }
    
    public override string ToString() => $"IDENTITY({Seed},{Increment})";
}
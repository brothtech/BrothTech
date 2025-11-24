namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface ISqlTypeArgument
{
    string Value { get; }
}

public class SqlTypeArgument :
    ISqlTypeArgument
{
    public static ISqlTypeArgument Max { get; } = new MaxSqlTypeArgument();

    public required int Value { get; set; }

    string ISqlTypeArgument.Value => Value.ToString();

    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator SqlTypeArgument(
        int value)
    {
        return new() { Value = value };
    }
    
    private class MaxSqlTypeArgument : 
        ISqlTypeArgument
    {
        public string Value => "MAX";

        public override string ToString()
        {
            return Value;
        }
    }
}
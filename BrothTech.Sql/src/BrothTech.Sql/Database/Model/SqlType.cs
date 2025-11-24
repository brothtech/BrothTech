#pragma warning disable CA2211
using System.Data;
using System.Diagnostics.CodeAnalysis;
using ConsoleApp1;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface ISqlType :
    ISqlObject
{
    IReadOnlyList<ISqlTypeArgument> SqlTypeArguments { get; }
    
    bool IsNullable { get; }
}

public class SqlType :
    SqlObject,
    ISqlType
{
    public List<ISqlTypeArgument> SqlTypeArguments { get; init; } = [];

    public bool IsNullable { get; init; }

    IReadOnlyList<ISqlTypeArgument> ISqlType.SqlTypeArguments => SqlTypeArguments;
    
    public SqlType()
    {
    }

    [SetsRequiredMembers]
    public SqlType(
        SqlDbType sqlDbType)
    {
        if (sqlDbType is SqlDbType.Timestamp or SqlDbType.Variant or SqlDbType.Udt or SqlDbType.Structured)
            throw new ArgumentOutOfRangeException(nameof(sqlDbType));
        
        Name = sqlDbType.ToString().ToUpper();
    }

    public override string ToString()
    {
        var nullability = $"{IsNullable.IfNot("NOT ")}NULL";
        return SqlTypeArguments.Count switch
        {
            >= 2 => $"{Name}({SqlTypeArguments[0]},{SqlTypeArguments[1]}) {nullability}",
            1 => $"{Name}({SqlTypeArguments[0]}) {nullability}",
            _ => $"{Name} {nullability}"
        };
    }

    public static implicit operator SqlType(
        SqlDbType sqlDbType)
    {
        return new SqlType(sqlDbType);
    }

    public class Names
    {
        public const string BigInt = "BIGINT";
        public const string Binary = "BINARY";
        public const string Bit = "BIT";
        public const string Char = "CHAR";
        public const string DateTime = "DATETIME";
        public const string Decimal = "DECIMAL";
        public const string Float = "FLOAT";
        public const string Image = "IMAGE";
        public const string Int = "INT";
        public const string Money = "MONEY";
        public const string NChar = "NCHAR";
        public const string NText = "NTEXT";
        public const string NVarChar = "NVARCHAR";
        public const string Real = "REAL";
        public const string UniqueIdentifier = "UNIQUEIDENTIFIER";
        public const string SmallDateTime = "SMALLDATETIME";
        public const string SmallInt = "SMALLINT";
        public const string SmallMoney = "SMALLMONEY";
        public const string Text = "TEXT";
        public const string TinyInt = "TINYINT";
        public const string VarBinary = "VARBINARY";
        public const string VarChar = "VARCHAR";
        public const string Xml = "XML";
        public const string Date = "DATE";
        public const string Time = "TIME";
        public const string DateTime2 = "DATETIME2";
        public const string DateTimeOffset = "DATETIMEOFFSET";
        public const string Json = "JSON";
    }
}
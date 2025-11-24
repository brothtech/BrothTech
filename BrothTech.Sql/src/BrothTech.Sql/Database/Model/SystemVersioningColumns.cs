using SetWorks.Platform.Shared.CodeGeneration.Sql.Model.Validation;
using System.ComponentModel.DataAnnotations;
using ConsoleApp1;

namespace SetWorks.Platform.Shared.CodeGeneration.Sql.Model;

public interface ISystemVersioningColumns
{
    string HistoryTableName { get; }

    public IColumn StartColumn { get; }

    public IColumn EndColumn { get; }
}

public class SystemVersioningColumns :
    ISystemVersioningColumns
{
    private Column? _startColumn;
    private Column? _endColumn;

    [Required, SqlIdentifier]
    public required string HistoryTableName { get; init; }

    [Required, SqlIdentifier]
    public required string StartColumnName { get; init; }

    [Required, SqlIdentifier]
    public required string EndColumnName { get; init; }

    public required int ColumnPrecision { get; init; }

    public required bool IsHidden { get; init; }

    public Column StartColumn => _startColumn ??= CreateSystemVersioningColumn(ColumnFlag.SystemVersioningStart);

    IColumn ISystemVersioningColumns.StartColumn => StartColumn;
    
    public Column EndColumn => _endColumn ??= CreateSystemVersioningColumn(ColumnFlag.SystemVersioningEnd);

    IColumn ISystemVersioningColumns.EndColumn => EndColumn;

    private SystemVersioningColumn CreateSystemVersioningColumn(
        ColumnFlag flag)
    {
        var name = flag is ColumnFlag.SystemVersioningStart ? StartColumnName : EndColumnName;
        var defaultValueExpression = flag is ColumnFlag.SystemVersioningStart ? "SYSUTCDATETIME()" : "CONVERT(DATETIME2(7), '9999-12-31 23:59:59.9999999')";
        return new SystemVersioningColumn
        {
            Name = name,
            IsHidden = IsHidden,
            Type = new SqlType
            {
                Name = SqlType.Names.DateTime2,
                SqlTypeArguments = [new SqlTypeArgument { Value = ColumnPrecision }]
            },
            Flags = ColumnFlag.SystemVersioningEnd,
            Default = new Default { ValueExpression = defaultValueExpression }
        };
    }

    private class SystemVersioningColumn :
        Column
    {
        public required bool IsHidden { get; init; }

        public override string Definition => $"[{Name}] {Type} GENERATED ALWAYS AS ROW START {IsHidden.If("HIDDEN ")}{Default}";
    }
}
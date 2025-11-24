#pragma warning disable CA1822
using SetWorks.Platform.Shared.CodeGeneration.Sql.Model;
using System.Data.Common;
using System.Text;
using ConsoleApp1;


namespace SetWorks.Platform.Shared.CodeGeneration.Sql;

public class DatabaseScriptBuilder()
{
    public string TryBuildScript(
        IDatabase database)
    {
        var context = new DatabaseScriptBuilderContext();
        ProcessDatabase(context, database);
        var preBuilder = context.PreBuilder.ToString();
        if (string.IsNullOrWhiteSpace(preBuilder) is false)
            context.Builder.AppendLine(preBuilder);
        
        var postBuilder = context.PostBuilder.ToString();
        if (string.IsNullOrWhiteSpace(postBuilder) is false)
            context.Builder.AppendLine(postBuilder);

        return context.Builder.ToString();
    }

    private void ProcessDatabase(
        DatabaseScriptBuilderContext context,
        IDatabase database)
    {
        foreach (var schema in database.Schemas)
            ProcessSchema(context, schema);
    }

    private void ProcessSchema(
        DatabaseScriptBuilderContext context,
        ISchema schema)
    {
        EnsureSchemaWillExist(context, schema);
        foreach (var table in schema.Tables)
            ProcessTable(context, table);
    }

    private void EnsureSchemaWillExist(
        DatabaseScriptBuilderContext context,
        ISchema schema)
    {
        var sql = $"""
                   IF NOT EXISTS (SELECT 1 FROM [sys].[schemas] WHERE [name] = N'{schema.Name}'
                        BEGIN
                            CREATE SCHEMA [{schema}];
                        END
                   """;
        context.Builder.AppendLine(sql);
    }
    
    private void ProcessTable(
        DatabaseScriptBuilderContext context,
        ITable table)
    {
        EnsureTableWillExist(context, table);
        foreach (var column in table.Columns.Where(x => x.Flags is ColumnFlag.None))
            ProcessColumn(context, column);
        
        if (table.SystemVersioningColumns is not null)
            EnsureSystemVersioningColumnsWillExist(context, table, table.SystemVersioningColumns);
        EnsureIndicesWillExist(context, table);
    }

    private void EnsureTableWillExist(
        DatabaseScriptBuilderContext context,
        ITable table)
    {
        var column = table.PrimaryKey;
        var clusterStatus = $"{column.IsClustered.IfNot("NON")}CLUSTERED";
        var sql = $"""
                   IF NOT EXISTS (SELECT 1 FROM [sys].[tables] WHERE SCHEMA_NAME([schema_id]) = N'{table.Schema}' AND [name] = N'{table}')
                        BEGIN
                            CREATE TABLE [{table.Schema}].[{table}]
                            (
                                [{column}] {column.Type} PRIMARY KEY {clusterStatus} {column.Identity} {column.Default}
                            );
                        END
                   """;
        context.Builder.AppendLine(sql);
    }

    private void ProcessColumn(
        DatabaseScriptBuilderContext context,
        IColumn column)
    {
        EnsureColumnWillExist(context, column);
        if (column.ReferencedTable is not null)
            EnsureForeignKeyWillExist(context, column, column.ReferencedTable);
    }

    private void EnsureColumnWillExist(
        DatabaseScriptBuilderContext context,
        IColumn column)
    {
        var sql = $"""
                   IF NOT EXISTS (SELECT    1
                                  FROM      [sys].[tables] as a
                                            INNER JOIN [sys].[columns] as b
                                                ON a.[object_id] = b.[object_id] AND
                                                b.[name] = N'{column}'
                                  WHERE     SCHEMA_NAME(a.[schema_id]) = N'{column.Table.Schema}' AND
                                            a.[name] = N'{column.Table}')
                       BEGIN
                           ALTER TABLE [{column.Table.Schema}].[{column.Table}] ADD
                               {column.Definition};
                       END 
                   """;
        context.Builder.AppendLine(sql);   
    }

    private void EnsureForeignKeyWillExist(
        DatabaseScriptBuilderContext context,
        IColumn column,
        ITable referencedTable)
    {
        var sql = $"""
                   IF NOT EXISTS (SELECT    1
                                  FROM      [sys].[tables] as a
                   			                INNER JOIN [sys].[columns] as b
                   			                    ON a.[object_id] = b.[object_id] AND
                   			                    b.[name] = N'{column}'
                   			                INNER JOIN [sys].[foreign_key_columns] as c
                   			                    ON a.[object_id] = c.[parent_object_id] AND
                   			                    b.[column_id] = c.[parent_column_id]
                   			                INNER JOIN [sys].[tables] as d
                   			                    ON c.[referenced_object_id] = d.[object_id] AND
                   			                    SCHEMA_NAME(d.[schema_id]) = N'{referencedTable.Schema}' AND
                   			                    d.[name] = N'{referencedTable}'
                   			                INNER JOIN [sys].[columns] as e
                   			                    ON d.[object_id] = e.[object_id] AND
                   			                    c.[referenced_column_id] = e.[column_id] AND
                   			                    e.[name] = N'{referencedTable.PrimaryKey}'
                                  WHERE	    SCHEMA_NAME(a.[schema_id]) = N'{column.Table.Schema}' AND
                   			                a.[name] = N'{column.Table}')
                        BEGIN
                            ALTER TABLE [{column.Table.Schema}].[{column.Table}] ADD FOREIGN KEY ([{column}])
                                REFERENCES [{referencedTable.Schema}].[{referencedTable}] ([{referencedTable.PrimaryKey}]);
                        END
                   """;
        context.PostBuilder.AppendLine(sql);
    }

    private void EnsureSystemVersioningColumnsWillExist(
        DatabaseScriptBuilderContext context,
        ITable table,
        ISystemVersioningColumns systemVersioningColumns)
    {
        var (startColumn, endColumn) = (systemVersioningColumns.StartColumn, systemVersioningColumns.EndColumn);
        var sql = $"""
                   IF NOT EXISTS (SELECT    1
                                  FROM      [sys].[tables] as a
                   			                INNER JOIN [sys].[columns] as b
                   				                ON a.[object_id] = b.[object_id] AND
                   				                b.[name] IN (N'{startColumn}', N'{endColumn}')
                                  WHERE	    SCHEMA_NAME(a.[schema_id]) = N'{table.Schema}' AND
                   			                a.[name] = N'{table}')
                        BEGIN
                            ALTER TABLE [{table.Schema}].[{table}] ADD
                                {startColumn.Definition},
                                {endColumn.Definition},
                                PERIOD FOR SYSTEM_TIME ([{startColumn}], [{endColumn}]);
                            ALTER TABLE [{table.Schema}].[{table}] SET
                                (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [{table.Schema}].[{systemVersioningColumns.HistoryTableName}]'));
                        END
                   """;
        context.Builder.AppendLine(sql);
    }

    private void EnsureIndicesWillExist(
        DatabaseScriptBuilderContext context,
        ITable table)
    {
        foreach (var index in table.Indices)
            EnsureIndexWillExist(context, index);
    }

    private void EnsureIndexWillExist(
        DatabaseScriptBuilderContext context,
        IIndex index)
    {
        var clusterStatus = $"{index.Flags.HasFlag(IndexFlag.Clustered).IfNot("NON")}CLUSTERED";
        var columns = string.Join(",\n             ", index.Columns.Select(x => $"[{x}]"));
        var includedColumns = string.Join(",\n             ", index.IncludedColumns.Select(x => $"[{x}]"));
        var sql = $"""
                   IF NOT EXISTS (SELECT    1
                                  FROM      [sys].[tables] as a
                                            INNER JOIN [sys].[indexes] as b
                                                ON a.[object_id] = b.[object_id] AND
                                                b.[name] = N'{index}'
                                  WHERE     SCHEMA_NAME(a.[schema_id]) = N'{index.Table.Schema}' AND
                                            a.[name] = N'{index.Table}')
                        BEGIN
                            CREATE {clusterStatus} INDEX [{index}] ON [{index.Table.Schema}].[{index.Table}]
                            (
                                {columns}
                            )
                            INCLUDE 
                            (
                                {includedColumns}
                            );
                        END
                   """;
        context.Builder.AppendLine(sql);
    }
    
    
}

public class DatabaseScriptBuilderContext
{
    public StringBuilder PreBuilder { get; } = new();
    public StringBuilder Builder { get; } = new();
    public StringBuilder PostBuilder { get; } = new();
}
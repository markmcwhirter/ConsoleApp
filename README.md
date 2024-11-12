DECLARE @SchemaName NVARCHAR(128) = 'YourSchemaName'; -- Replace with your schema name
DECLARE @TableName NVARCHAR(128);
DECLARE @TableScript NVARCHAR(MAX);

DECLARE table_cursor CURSOR FOR
SELECT t.name
FROM sys.tables t
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = @SchemaName;

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @TableScript = 'CREATE TABLE ' + QUOTENAME(@SchemaName) + '.' + QUOTENAME(@TableName) + ' (' + CHAR(13) +
    (
        SELECT STRING_AGG(
            '    ' + QUOTENAME(c.name) + ' ' +
            CASE 
                WHEN ty.name IN ('char', 'varchar', 'nchar', 'nvarchar') 
                    THEN ty.name + '(' + CASE WHEN c.max_length = -1 THEN 'MAX' ELSE CAST(c.max_length / CASE WHEN ty.name LIKE 'n%' THEN 2 ELSE 1 END AS VARCHAR) END + ')'
                WHEN ty.name IN ('decimal', 'numeric') 
                    THEN ty.name + '(' + CAST(c.precision AS VARCHAR) + ', ' + CAST(c.scale AS VARCHAR) + ')'
                ELSE ty.name
            END + 
            CASE WHEN c.is_nullable = 0 THEN ' NOT NULL' ELSE ' NULL' END + 
            CASE WHEN dc.definition IS NOT NULL THEN ' DEFAULT ' + dc.definition ELSE '' END +
            CASE WHEN ic.index_id IS NOT NULL THEN ' PRIMARY KEY' ELSE '' END
            , ',' + CHAR(13)
        ) WITHIN GROUP (ORDER BY c.column_id)
        FROM sys.columns c
        INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
        LEFT JOIN sys.default_constraints dc ON c.default_object_id = dc.object_id
        LEFT JOIN (
            SELECT ic.object_id, ic.index_id, ic.column_id
            FROM sys.index_columns ic
            JOIN sys.indexes i ON i.object_id = ic.object_id AND i.index_id = ic.index_id
            WHERE i.is_primary_key = 1
        ) ic ON c.object_id = ic.object_id AND c.column_id = ic.column_id
        WHERE c.object_id = OBJECT_ID(QUOTENAME(@SchemaName) + '.' + QUOTENAME(@TableName))
    ) + CHAR(13) + ');' + CHAR(13) + CHAR(13);

    PRINT @TableScript;
    
    FETCH NEXT FROM table_cursor INTO @TableName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

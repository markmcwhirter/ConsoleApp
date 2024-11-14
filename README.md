DECLARE @SchemaName NVARCHAR(128) = 'YourSchemaName';  -- Replace with your schema name
DECLARE @SQL NVARCHAR(MAX) = '';

-- Generate the create scripts for each view in the specified schema
SELECT @SQL = @SQL + '
IF OBJECT_ID(''' + QUOTENAME(@SchemaName) + '.' + QUOTENAME(v.name) + ''', ''V'') IS NOT NULL
    DROP VIEW ' + QUOTENAME(@SchemaName) + '.' + QUOTENAME(v.name) + ';
GO
' + m.definition + '
GO
'
FROM sys.views v
JOIN sys.sql_modules m ON v.object_id = m.object_id
WHERE SCHEMA_NAME(v.schema_id) = @SchemaName;

-- Print or execute the generated script
PRINT @SQL;
-- If the script is too large for PRINT, you can use SELECT
-- SELECT @SQL AS ScriptToCreateViews;

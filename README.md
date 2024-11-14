DECLARE @SQL NVARCHAR(MAX) = '';

-- Generate the create scripts for each view in the database
SELECT @SQL = @SQL + '
IF OBJECT_ID(''' + QUOTENAME(SCHEMA_NAME(v.schema_id)) + '.' + QUOTENAME(v.name) + ''', ''V'') IS NOT NULL
    DROP VIEW ' + QUOTENAME(SCHEMA_NAME(v.schema_id)) + '.' + QUOTENAME(v.name) + ';
GO
' + m.definition + '
GO
'
FROM sys.views v
JOIN sys.sql_modules m ON v.object_id = m.object_id;

-- Print or execute the generated script
PRINT @SQL;
-- If the script is too large for PRINT, you can use SELECT
-- SELECT @SQL AS ScriptToCreateAllViews;

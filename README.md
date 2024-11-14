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

EXEC sp_executesql @disableConstraintSql;

-- Delete tables in the specified order
DECLARE @deleteTableSql NVARCHAR(MAX) = '';
SELECT @deleteTableSql = @deleteTableSql + 'DROP TABLE IF EXISTS [' + TableName + '];' + CHAR(13)
FROM @tablesToDelete;

EXEC sp_executesql @deleteTableSql;

-- Re-enable foreign key constraints
DECLARE @enableConstraintSql NVARCHAR(MAX) = '';
SELECT @enableConstraintSql = @enableConstraintSql +
    'ALTER TABLE [' + SchemaName + '].[' + TableName + '] WITH CHECK CHECK CONSTRAINT [' + FKName + '];' + CHAR(13)
FROM @constraintsToDisable;

EXEC sp_executesql @enableConstraintSql;

PRINT 'Tables deleted and constraints re-enabled successfully.';

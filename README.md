-- Define the list of tables you want to delete
DECLARE @tablesToDelete TABLE (TableName NVARCHAR(255));
INSERT INTO @tablesToDelete (TableName) VALUES 
    ('Table1'), 
    ('Table2'), 
    ('Table3'); -- Add more tables as needed

-- Temporary table to hold foreign key constraint information
DECLARE @constraintsToDisable TABLE (
    FKName NVARCHAR(255),
    TableName NVARCHAR(255),
    SchemaName NVARCHAR(255)
);

-- Populate the temporary table with foreign key constraints for the target tables
INSERT INTO @constraintsToDisable (FKName, TableName, SchemaName)
SELECT fk.name AS FKName,
       OBJECT_NAME(fk.parent_object_id) AS TableName,
       s.name AS SchemaName
FROM sys.foreign_keys fk
JOIN sys.tables t ON fk.parent_object_id = t.object_id
JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE OBJECT_NAME(fk.referenced_object_id) IN (SELECT TableName FROM @tablesToDelete)
  OR OBJECT_NAME(fk.parent_object_id) IN (SELECT TableName FROM @tablesToDelete);

-- Disable foreign key constraints
DECLARE @disableConstraintSql NVARCHAR(MAX) = '';
SELECT @disableConstraintSql = @disableConstraintSql +
    'ALTER TABLE [' + SchemaName + '].[' + TableName + '] NOCHECK CONSTRAINT [' + FKName + '];' + CHAR(13)
FROM @constraintsToDisable;

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

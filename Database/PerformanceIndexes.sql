-- =============================================
-- SSIS Analytics Performance Optimization Indexes
-- These indexes will dramatically improve query performance
-- =============================================

USE [SSISDB]
GO

-- Index for filtering by package_name (Business Unit filtering)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Executions_PackageName_StartTime')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Executions_PackageName_StartTime
    ON [catalog].[executions] (package_name, start_time DESC)
    INCLUDE (status, end_time, folder_name, project_name);
    PRINT 'Created index: IX_Executions_PackageName_StartTime';
END
GO

-- Index for filtering by status and date range
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Executions_Status_StartTime')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Executions_Status_StartTime
    ON [catalog].[executions] (status, start_time DESC)
    INCLUDE (package_name, end_time, folder_name, project_name);
    PRINT 'Created index: IX_Executions_Status_StartTime';
END
GO

-- Index for date-based queries (trends, timeline)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Executions_StartTime')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Executions_StartTime
    ON [catalog].[executions] (start_time DESC)
    INCLUDE (package_name, status, end_time, execution_id);
    PRINT 'Created index: IX_Executions_StartTime';
END
GO

-- Index for error message queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EventMessages_OperationId_MessageType')
BEGIN
    CREATE NONCLUSTERED INDEX IX_EventMessages_OperationId_MessageType
    ON [catalog].[event_messages] (operation_id, message_type)
    INCLUDE (message_time, message, event_message_id);
    PRINT 'Created index: IX_EventMessages_OperationId_MessageType';
END
GO

-- Statistics update for better query plans
UPDATE STATISTICS [catalog].[executions] WITH FULLSCAN;
UPDATE STATISTICS [catalog].[event_messages] WITH FULLSCAN;
PRINT 'Updated statistics';
GO

PRINT '========================================';
PRINT 'Performance indexes created successfully!';
PRINT 'Query performance should be significantly improved.';
PRINT '========================================';

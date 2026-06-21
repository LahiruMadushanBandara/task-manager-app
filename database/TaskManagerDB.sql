
-- Default login: username=admin, password=Admin@123

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TaskManagerDB')
BEGIN
    CREATE DATABASE TaskManagerDB;
END
GO

USE TaskManagerDB;
GO

-- Users
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id          INT             IDENTITY(1,1) PRIMARY KEY,
        Username    NVARCHAR(100)   NOT NULL,
        PasswordHash NVARCHAR(255)  NOT NULL,
        Salt        NVARCHAR(50)    NOT NULL,
        CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT UQ_Users_Username UNIQUE (Username)
    );
END
GO

-- Tasks
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tasks')
BEGIN
    CREATE TABLE Tasks (
        Id          INT             IDENTITY(1,1) PRIMARY KEY,
        Title       NVARCHAR(200)   NOT NULL,
        Description NVARCHAR(1000)  NULL,
        IsCompleted BIT             NOT NULL DEFAULT 0,
        Priority    INT             NOT NULL DEFAULT 1, -- 0=Low 1=Medium 2=High
        DueDate     DATETIME2       NULL,
        CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        UserId      INT             NOT NULL,
        CONSTRAINT FK_Tasks_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
    );
END
GO

-- Indexes for common query patterns
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_UserId')
    CREATE INDEX IX_Tasks_UserId ON Tasks (UserId);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_IsCompleted')
    CREATE INDEX IX_Tasks_IsCompleted ON Tasks (IsCompleted);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_Priority')
    CREATE INDEX IX_Tasks_Priority ON Tasks (Priority);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_DueDate')
    CREATE INDEX IX_Tasks_DueDate ON Tasks (DueDate);
GO

-- add a default admin user
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, PasswordHash, Salt)
    VALUES (
        'admin',
        '50f31ff6ff7d3ef6a8b57d33797d87d2dd4d13191df74710d8f068a39b98ba76',
        'TaskManagerSalt2024'
    );
END
GO

-- add sample tasks for the admin user
DECLARE @AdminId INT = (SELECT Id FROM Users WHERE Username = 'admin');

IF NOT EXISTS (SELECT 1 FROM Tasks WHERE UserId = @AdminId)
BEGIN
    INSERT INTO Tasks (Title, Description, IsCompleted, Priority, DueDate, UpdatedAt, UserId)
    VALUES
        ('Task 01',    'Task 01 Desc',           1, 2, NULL,                          GETUTCDATE(), @AdminId),
        ('Task 02',       'Task 02 Desc',       1, 2, NULL,                          GETUTCDATE(), @AdminId),
        ('Task 03',           'Task 03 Desc',       0, 2, DATEADD(day,  3, GETUTCDATE()), GETUTCDATE(), @AdminId),
        ('Task 04',       'Task 04 Desc',         0, 2, DATEADD(day,  5, GETUTCDATE()), GETUTCDATE(), @AdminId),
        ('Task 05',             'Task 05 Desc',          0, 1, DATEADD(day,  7, GETUTCDATE()), GETUTCDATE(), @AdminId),
        ('Task 06',            'Task 06 Desc',   0, 0, DATEADD(day, 14, GETUTCDATE()), GETUTCDATE(), @AdminId);
END
GO

PRINT 'TaskManagerDB setup complete. Login: admin / Admin@123';
GO

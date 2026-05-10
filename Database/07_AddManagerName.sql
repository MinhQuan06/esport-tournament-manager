USE EsportTournamentDB;
GO

-- Thêm cột ManagerName (text input cho tên người quản lý đội)
IF COL_LENGTH('Team','ManagerName') IS NULL
    ALTER TABLE Team ADD ManagerName NVARCHAR(100) NULL;
GO

-- Backfill từ ManagerAccountID đã có
UPDATE t SET t.ManagerName = a.FullName
FROM Team t JOIN Account a ON t.ManagerAccountID = a.AccountID
WHERE t.ManagerName IS NULL;
GO

PRINT N'>>> Đã thêm cột ManagerName vào Team.';

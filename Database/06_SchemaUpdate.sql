-- ============================================================
-- SCHEMA UPDATE - Bổ sung trường để khớp với UI mockup
-- An toàn: chỉ ALTER TABLE thêm cột (không xóa/đổi cột cũ)
-- Chạy SAU khi đã chạy 01-05.
-- ============================================================
USE EsportTournamentDB;
GO

-- ============================================================
-- 1. Tournament: thêm GameType, Format
-- ============================================================
IF COL_LENGTH('Tournament','GameType') IS NULL
    ALTER TABLE Tournament ADD GameType NVARCHAR(50) NULL DEFAULT N'Khác';
GO
IF COL_LENGTH('Tournament','Format') IS NULL
    ALTER TABLE Tournament ADD Format NVARCHAR(50) NULL DEFAULT N'Round Robin';
GO

-- ============================================================
-- 2. Team: thêm ShortName, LogoColor, GameType, IsActive
-- ============================================================
IF COL_LENGTH('Team','ShortName') IS NULL
    ALTER TABLE Team ADD ShortName NVARCHAR(10) NULL;
GO
IF COL_LENGTH('Team','LogoColor') IS NULL
    ALTER TABLE Team ADD LogoColor VARCHAR(20) NULL DEFAULT '#3b82f6';
GO
IF COL_LENGTH('Team','GameType') IS NULL
    ALTER TABLE Team ADD GameType NVARCHAR(50) NULL;
GO
IF COL_LENGTH('Team','IsActive') IS NULL
    ALTER TABLE Team ADD IsActive BIT NOT NULL DEFAULT 1;
GO

-- ============================================================
-- 3. Player: thêm Nickname, Position, Country, BirthDate, IsActive
-- ============================================================
IF COL_LENGTH('Player','Nickname') IS NULL
    ALTER TABLE Player ADD Nickname NVARCHAR(50) NULL;
GO
IF COL_LENGTH('Player','Position') IS NULL
    ALTER TABLE Player ADD Position NVARCHAR(30) NULL;
GO
IF COL_LENGTH('Player','Country') IS NULL
    ALTER TABLE Player ADD Country NVARCHAR(50) NULL DEFAULT N'Việt Nam';
GO
IF COL_LENGTH('Player','BirthDate') IS NULL
    ALTER TABLE Player ADD BirthDate DATE NULL;
GO
IF COL_LENGTH('Player','IsActive') IS NULL
    ALTER TABLE Player ADD IsActive BIT NOT NULL DEFAULT 1;
GO

-- ============================================================
-- 4. Match: thêm RoundName, GroupName, MatchFormat
-- ============================================================
IF COL_LENGTH('Match','RoundName') IS NULL
    ALTER TABLE Match ADD RoundName NVARCHAR(50) NULL DEFAULT N'Vòng bảng';
GO
IF COL_LENGTH('Match','GroupName') IS NULL
    ALTER TABLE Match ADD GroupName NVARCHAR(20) NULL;
GO
IF COL_LENGTH('Match','MatchFormat') IS NULL
    ALTER TABLE Match ADD MatchFormat NVARCHAR(10) NULL DEFAULT N'BO3';
GO

-- ============================================================
-- 5. Account: thêm LastLoginAt
-- ============================================================
IF COL_LENGTH('Account','LastLoginAt') IS NULL
    ALTER TABLE Account ADD LastLoginAt DATETIME NULL;
GO

-- ============================================================
-- 6. Backfill dữ liệu mẫu cho các trường mới
-- ============================================================
UPDATE Tournament SET GameType = N'Valorant',     Format = N'Round Robin'
WHERE TournamentName = N'Valorant Champions';
UPDATE Tournament SET GameType = N'CS:GO',        Format = N'Single Elim.'
WHERE TournamentName = N'CS:GO Vietnam Open';
UPDATE Tournament SET GameType = N'League of Legends', Format = N'Double Elim.'
WHERE TournamentName = N'VCS Spring 2026';

UPDATE Team SET ShortName = N'SP',  LogoColor = '#3b82f6', GameType = N'Valorant'   WHERE TeamName = N'Saigon Phantoms';
UPDATE Team SET ShortName = N'HD',  LogoColor = '#ef4444', GameType = N'Valorant'   WHERE TeamName = N'Hanoi Dragons';
UPDATE Team SET ShortName = N'DH',  LogoColor = '#f59e0b', GameType = N'Valorant'   WHERE TeamName = N'Da Nang Hawks';
UPDATE Team SET ShortName = N'CT',  LogoColor = '#8b5cf6', GameType = N'Valorant'   WHERE TeamName = N'Can Tho Tigers';
UPDATE Team SET ShortName = N'NV',  LogoColor = '#10b981', GameType = N'CS:GO'      WHERE TeamName = N'Team Nova';
UPDATE Team SET ShortName = N'AL',  LogoColor = '#06b6d4', GameType = N'CS:GO'      WHERE TeamName = N'Team Alpha';

-- Backfill Nickname/Position/Country cho player đã có
UPDATE Player SET Nickname = REPLACE(REPLACE(PlayerName,' ','_'),'Player_','SP_'), Position = N'Duelist', Country = N'Việt Nam', BirthDate = '2002-01-01'
WHERE TeamID = (SELECT TeamID FROM Team WHERE TeamName = N'Saigon Phantoms') AND Nickname IS NULL;
UPDATE Player SET Nickname = REPLACE(REPLACE(PlayerName,' ','_'),'Player_','HD_'), Position = N'Sentinel', Country = N'Việt Nam', BirthDate = '2001-06-15'
WHERE TeamID = (SELECT TeamID FROM Team WHERE TeamName = N'Hanoi Dragons') AND Nickname IS NULL;

-- Match: backfill default
UPDATE Match SET RoundName = N'Vòng bảng', GroupName = N'A', MatchFormat = N'BO3'
WHERE RoundName IS NULL OR RoundName = N'';

PRINT N'>>> Schema update applied successfully.';
GO

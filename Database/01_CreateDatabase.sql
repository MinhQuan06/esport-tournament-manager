-- ============================================================
-- ESPORT TOURNAMENT MANAGEMENT - DATABASE SCRIPT
-- Source: Báo cáo CNPM - Đồ án quản lý giải đấu Esport
-- Target: SQL Server 2016+ (ADO.NET)
-- ============================================================

USE master;
GO

IF DB_ID(N'EsportTournamentDB') IS NOT NULL
BEGIN
    ALTER DATABASE EsportTournamentDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EsportTournamentDB;
END
GO

CREATE DATABASE EsportTournamentDB COLLATE Vietnamese_CI_AS;
GO

USE EsportTournamentDB;
GO

-- ============================================================
-- 1. TABLES
-- ============================================================

-- Bảng tài khoản (FR-ACC-01..06, BR-ACC-01..06)
CREATE TABLE Account (
    AccountID    INT IDENTITY(1,1) PRIMARY KEY,
    Username     VARCHAR(50)   NOT NULL UNIQUE,
    PasswordHash VARCHAR(255)  NOT NULL,
    Email        VARCHAR(100)  NOT NULL UNIQUE,
    FullName     NVARCHAR(100) NOT NULL,
    IsLocked     BIT           NOT NULL DEFAULT 0,
    CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng vai trò (FR-ROLE-01..06)
CREATE TABLE Role (
    RoleID      INT IDENTITY(1,1) PRIMARY KEY,
    RoleName    NVARCHAR(50)  NOT NULL UNIQUE,
    Description NVARCHAR(200)
);
GO

-- N:M Account <-> Role (FR-ROLE-04, BR-ROLE-04)
CREATE TABLE AccountRole (
    AccountID  INT NOT NULL,
    RoleID     INT NOT NULL,
    AssignedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_AccountRole PRIMARY KEY (AccountID, RoleID),
    CONSTRAINT FK_AR_Account  FOREIGN KEY (AccountID) REFERENCES Account(AccountID),
    CONSTRAINT FK_AR_Role     FOREIGN KEY (RoleID)    REFERENCES Role(RoleID)
);
GO

-- Bảng giải đấu (FR-TOUR-01..08)
CREATE TABLE Tournament (
    TournamentID   INT IDENTITY(1,1) PRIMARY KEY,
    TournamentName NVARCHAR(200) NOT NULL,
    StartDate      DATE          NOT NULL,
    EndDate        DATE          NOT NULL,
    Description    NVARCHAR(500),
    Status         NVARCHAR(50)  NOT NULL DEFAULT N'Chưa bắt đầu',
    CreatedAt      DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_Tour_Dates  CHECK (EndDate > StartDate),
    CONSTRAINT CK_Tour_Status CHECK (Status IN
        (N'Chưa bắt đầu', N'Đang diễn ra', N'Đã kết thúc'))
);
GO

-- Bảng đội (FR-TEAM-01..08)
CREATE TABLE Team (
    TeamID           INT IDENTITY(1,1) PRIMARY KEY,
    TournamentID     INT           NOT NULL,
    TeamName         NVARCHAR(100) NOT NULL,
    Description      NVARCHAR(300),
    ManagerAccountID INT NULL,
    CreatedAt        DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Team_Tournament FOREIGN KEY (TournamentID) REFERENCES Tournament(TournamentID),
    CONSTRAINT FK_Team_Manager    FOREIGN KEY (ManagerAccountID) REFERENCES Account(AccountID),
    CONSTRAINT UQ_Team_Name_Tour  UNIQUE (TournamentID, TeamName)
);
GO

-- Bảng người chơi (FR-PLAYER-01..09)
CREATE TABLE Player (
    PlayerID    INT IDENTITY(1,1) PRIMARY KEY,
    TeamID      INT           NOT NULL,
    PlayerName  NVARCHAR(100) NOT NULL,
    ContactInfo NVARCHAR(200),
    CreatedAt   DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Player_Team FOREIGN KEY (TeamID) REFERENCES Team(TeamID),
    CONSTRAINT UQ_Player_Team UNIQUE (TeamID, PlayerName)
);
GO

-- Bảng lịch thi đấu (FR-MATCH-01..10)
CREATE TABLE Match (
    MatchID      INT IDENTITY(1,1) PRIMARY KEY,
    TournamentID INT      NOT NULL,
    Team1ID      INT      NOT NULL,
    Team2ID      INT      NOT NULL,
    MatchTime    DATETIME NOT NULL,
    Status       NVARCHAR(50) NOT NULL DEFAULT N'Chưa diễn ra',
    CreatedAt    DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Match_Tour   FOREIGN KEY (TournamentID) REFERENCES Tournament(TournamentID),
    CONSTRAINT FK_Match_Team1  FOREIGN KEY (Team1ID)      REFERENCES Team(TeamID),
    CONSTRAINT FK_Match_Team2  FOREIGN KEY (Team2ID)      REFERENCES Team(TeamID),
    CONSTRAINT CK_Match_Teams  CHECK (Team1ID <> Team2ID),
    CONSTRAINT CK_Match_Status CHECK (Status IN
        (N'Chưa diễn ra', N'Đang diễn ra', N'Đã kết thúc'))
);
GO

-- Bảng kết quả (FR-RESULT-01..08)
CREATE TABLE MatchResult (
    ResultID     INT IDENTITY(1,1) PRIMARY KEY,
    MatchID      INT NOT NULL UNIQUE,
    WinnerTeamID INT NULL,
    ScoreTeam1   INT NOT NULL CHECK (ScoreTeam1 >= 0),
    ScoreTeam2   INT NOT NULL CHECK (ScoreTeam2 >= 0),
    IsConfirmed  BIT NOT NULL DEFAULT 0,
    RecordedAt   DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Result_Match  FOREIGN KEY (MatchID)      REFERENCES Match(MatchID),
    CONSTRAINT FK_Result_Winner FOREIGN KEY (WinnerTeamID) REFERENCES Team(TeamID)
);
GO

-- Bảng xếp hạng (FR-RANK-01..06)
CREATE TABLE Ranking (
    RankingID    INT IDENTITY(1,1) PRIMARY KEY,
    TournamentID INT NOT NULL,
    TeamID       INT NOT NULL,
    Wins         INT NOT NULL DEFAULT 0,
    Losses       INT NOT NULL DEFAULT 0,
    Draws        INT NOT NULL DEFAULT 0,
    Points       INT NOT NULL DEFAULT 0,
    Rank         INT NOT NULL DEFAULT 0,
    UpdatedAt    DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Rank_Tournament FOREIGN KEY (TournamentID) REFERENCES Tournament(TournamentID),
    CONSTRAINT FK_Rank_Team       FOREIGN KEY (TeamID)       REFERENCES Team(TeamID),
    CONSTRAINT UQ_Rank_Tour_Team  UNIQUE (TournamentID, TeamID)
);
GO

-- ============================================================
-- 2. ROLES MẶC ĐỊNH (BR-ROLE-02: không được xóa)
-- ============================================================
INSERT INTO Role (RoleName, Description) VALUES
    (N'Admin',       N'Quản trị toàn bộ hệ thống'),
    (N'TeamManager', N'Quản lý đội và người chơi'),
    (N'Viewer',      N'Xem thông tin giải đấu - mặc định khi tạo tài khoản');
GO

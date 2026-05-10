USE EsportTournamentDB;
GO

-- ============================================================
-- TRIGGERS
-- ============================================================

-- TR_PreventTeamOverflow: Giới hạn 10 player/đội (BR-PLAYER-02)
IF OBJECT_ID('dbo.TR_PreventTeamOverflow', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_PreventTeamOverflow;
GO
CREATE TRIGGER dbo.TR_PreventTeamOverflow
ON Player
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE dbo.FN_CountPlayersInTeam(i.TeamID) >= 10
    )
    BEGIN
        RAISERROR(N'Đội đã đủ 10 người chơi - không thể thêm. (BR-PLAYER-02)', 16, 1);
        RETURN;
    END
    INSERT INTO Player (TeamID, PlayerName, ContactInfo, CreatedAt)
    SELECT TeamID, PlayerName, ContactInfo, ISNULL(CreatedAt, GETDATE())
    FROM inserted;
END
GO

-- TR_PreventDuplicateMatch: Ngăn xung đột lịch (BR-MATCH-02)
IF OBJECT_ID('dbo.TR_PreventDuplicateMatch', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_PreventDuplicateMatch;
GO
CREATE TRIGGER dbo.TR_PreventDuplicateMatch
ON Match
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE dbo.FN_IsScheduleConflict(i.Team1ID, i.MatchTime, NULL) = 1
           OR dbo.FN_IsScheduleConflict(i.Team2ID, i.MatchTime, NULL) = 1
    )
    BEGIN
        RAISERROR(N'Một đội đã có trận đấu vào thời điểm này. (BR-MATCH-02)', 16, 1);
        RETURN;
    END
    INSERT INTO Match (TournamentID, Team1ID, Team2ID, MatchTime, Status, CreatedAt)
    SELECT TournamentID, Team1ID, Team2ID, MatchTime,
           ISNULL(Status, N'Chưa diễn ra'), ISNULL(CreatedAt, GETDATE())
    FROM inserted;
END
GO

-- TR_UpdateRankingAfterResult: Auto recalc BXH (FR-RANK-03)
IF OBJECT_ID('dbo.TR_UpdateRankingAfterResult', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_UpdateRankingAfterResult;
GO
CREATE TRIGGER dbo.TR_UpdateRankingAfterResult
ON MatchResult
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TournamentID INT;
    SELECT TOP 1 @TournamentID = m.TournamentID
    FROM Match m JOIN inserted i ON m.MatchID = i.MatchID;

    IF @TournamentID IS NOT NULL
        EXEC dbo.SP_RecalcRanking @TournamentID;
END
GO

-- TR_PreventDeleteConfirmedResult (FR-RESULT-07)
IF OBJECT_ID('dbo.TR_PreventDeleteConfirmedResult', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_PreventDeleteConfirmedResult;
GO
CREATE TRIGGER dbo.TR_PreventDeleteConfirmedResult
ON MatchResult
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted WHERE IsConfirmed = 1)
    BEGIN
        RAISERROR(N'Không thể xóa kết quả đã được xác nhận chính thức.', 16, 1);
        RETURN;
    END
    DELETE FROM MatchResult
    WHERE ResultID IN (SELECT ResultID FROM deleted);
END
GO

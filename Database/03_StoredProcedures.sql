USE EsportTournamentDB;
GO

-- ============================================================
-- STORED PROCEDURES
-- ============================================================

-- SP_RecalcRanking: Tính lại bảng xếp hạng (BR-RANK-01..03)
IF OBJECT_ID('dbo.SP_RecalcRanking', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_RecalcRanking;
GO
CREATE PROCEDURE dbo.SP_RecalcRanking
    @TournamentID INT
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Stats AS (
        SELECT
            t.TeamID,
            ISNULL(SUM(CASE
                WHEN mr.WinnerTeamID = t.TeamID THEN 3
                WHEN mr.WinnerTeamID IS NULL AND mr.MatchID IS NOT NULL THEN 1
                ELSE 0
            END), 0) AS Points,
            ISNULL(SUM(CASE WHEN mr.WinnerTeamID = t.TeamID THEN 1 ELSE 0 END), 0) AS Wins,
            ISNULL(SUM(CASE WHEN mr.WinnerTeamID IS NOT NULL
                             AND mr.WinnerTeamID <> t.TeamID THEN 1 ELSE 0 END), 0) AS Losses,
            ISNULL(SUM(CASE WHEN mr.WinnerTeamID IS NULL AND mr.MatchID IS NOT NULL
                            THEN 1 ELSE 0 END), 0) AS Draws
        FROM Team t
        LEFT JOIN Match m ON (m.Team1ID = t.TeamID OR m.Team2ID = t.TeamID)
                          AND m.TournamentID = @TournamentID
        LEFT JOIN MatchResult mr ON m.MatchID = mr.MatchID
        WHERE t.TournamentID = @TournamentID
        GROUP BY t.TeamID
    ),
    Ranked AS (
        SELECT TeamID, Points, Wins, Losses, Draws,
            RANK() OVER (
                ORDER BY Points DESC,
                         dbo.FN_GetGoalDifference(TeamID, @TournamentID) DESC,
                         TeamID ASC
            ) AS Rank
        FROM Stats
    )
    MERGE Ranking AS target
    USING Ranked AS source
        ON target.TeamID = source.TeamID
       AND target.TournamentID = @TournamentID
    WHEN MATCHED THEN
        UPDATE SET Points = source.Points, Wins = source.Wins,
                   Losses = source.Losses, Draws = source.Draws,
                   Rank = source.Rank, UpdatedAt = GETDATE()
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (TournamentID, TeamID, Points, Wins, Losses, Draws, Rank, UpdatedAt)
        VALUES (@TournamentID, source.TeamID, source.Points,
                source.Wins, source.Losses, source.Draws, source.Rank, GETDATE());
END
GO

-- SP_GetTournamentRanking: Lấy bảng xếp hạng giải (FR-RANK-02)
IF OBJECT_ID('dbo.SP_GetTournamentRanking', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_GetTournamentRanking;
GO
CREATE PROCEDURE dbo.SP_GetTournamentRanking
    @TournamentID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        r.Rank,
        r.TeamID,
        t.TeamName,
        (r.Wins + r.Losses + r.Draws) AS Played,
        r.Wins,
        r.Draws,
        r.Losses,
        r.Points,
        dbo.FN_GetGoalDifference(t.TeamID, @TournamentID) AS GoalDiff
    FROM Ranking r
    JOIN Team t ON r.TeamID = t.TeamID
    WHERE r.TournamentID = @TournamentID
    ORDER BY r.Rank ASC;
END
GO

-- SP_EnterMatchResult: Nhập kết quả trận đấu (FR-RESULT-01, BR-RESULT-03)
IF OBJECT_ID('dbo.SP_EnterMatchResult', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_EnterMatchResult;
GO
CREATE PROCEDURE dbo.SP_EnterMatchResult
    @MatchID INT,
    @ScoreTeam1 INT,
    @ScoreTeam2 INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Match WHERE MatchID = @MatchID)
        BEGIN
            RAISERROR(N'Trận đấu không tồn tại.', 16, 1);
            ROLLBACK; RETURN;
        END

        DECLARE @Team1ID INT, @Team2ID INT, @TournamentID INT, @CurStatus NVARCHAR(50);
        SELECT @Team1ID = Team1ID, @Team2ID = Team2ID,
               @TournamentID = TournamentID, @CurStatus = Status
        FROM Match WHERE MatchID = @MatchID;

        DECLARE @WinnerID INT = NULL;
        IF @ScoreTeam1 > @ScoreTeam2 SET @WinnerID = @Team1ID;
        IF @ScoreTeam2 > @ScoreTeam1 SET @WinnerID = @Team2ID;

        -- Upsert MatchResult (chỉ khi chưa Confirmed)
        IF EXISTS (SELECT 1 FROM MatchResult WHERE MatchID = @MatchID AND IsConfirmed = 1)
        BEGIN
            RAISERROR(N'Kết quả đã được xác nhận - không thể chỉnh sửa.', 16, 1);
            ROLLBACK; RETURN;
        END

        IF EXISTS (SELECT 1 FROM MatchResult WHERE MatchID = @MatchID)
            UPDATE MatchResult
               SET ScoreTeam1 = @ScoreTeam1, ScoreTeam2 = @ScoreTeam2,
                   WinnerTeamID = @WinnerID, RecordedAt = GETDATE()
             WHERE MatchID = @MatchID;
        ELSE
            INSERT INTO MatchResult (MatchID, WinnerTeamID, ScoreTeam1, ScoreTeam2, IsConfirmed, RecordedAt)
            VALUES (@MatchID, @WinnerID, @ScoreTeam1, @ScoreTeam2, 0, GETDATE());

        UPDATE Match SET Status = N'Đã kết thúc' WHERE MatchID = @MatchID;

        EXEC dbo.SP_RecalcRanking @TournamentID;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP_ConfirmMatchResult: Xác nhận kết quả (FR-RESULT-06, BR-RESULT-04)
IF OBJECT_ID('dbo.SP_ConfirmMatchResult', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_ConfirmMatchResult;
GO
CREATE PROCEDURE dbo.SP_ConfirmMatchResult
    @MatchID INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM MatchResult WHERE MatchID = @MatchID)
    BEGIN
        RAISERROR(N'Chưa có kết quả để xác nhận.', 16, 1);
        RETURN;
    END
    UPDATE MatchResult SET IsConfirmed = 1 WHERE MatchID = @MatchID;

    DECLARE @TournamentID INT;
    SELECT @TournamentID = TournamentID FROM Match WHERE MatchID = @MatchID;
    EXEC dbo.SP_RecalcRanking @TournamentID;
END
GO

-- SP_AssignRole: Gán vai trò (FR-ROLE-04, BR-ROLE-04)
IF OBJECT_ID('dbo.SP_AssignRole', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_AssignRole;
GO
CREATE PROCEDURE dbo.SP_AssignRole
    @AccountID INT,
    @RoleID INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM AccountRole
                    WHERE AccountID = @AccountID AND RoleID = @RoleID)
    BEGIN
        INSERT INTO AccountRole (AccountID, RoleID, AssignedAt)
        VALUES (@AccountID, @RoleID, GETDATE());
    END
END
GO

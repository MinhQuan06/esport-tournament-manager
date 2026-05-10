USE EsportTournamentDB;
GO

-- ============================================================
-- FUNCTIONS
-- ============================================================

-- FN_GetTeamPoints: Lấy tổng điểm của 1 đội (FR-RANK-01)
IF OBJECT_ID('dbo.FN_GetTeamPoints', 'FN') IS NOT NULL
    DROP FUNCTION dbo.FN_GetTeamPoints;
GO
CREATE FUNCTION dbo.FN_GetTeamPoints (@TeamID INT, @TournamentID INT)
RETURNS INT
AS
BEGIN
    DECLARE @Points INT;
    SELECT @Points = Points FROM Ranking
    WHERE TeamID = @TeamID AND TournamentID = @TournamentID;
    RETURN ISNULL(@Points, 0);
END
GO

-- FN_GetGoalDifference: Hiệu số bàn thắng/thua (BR-RANK-03)
IF OBJECT_ID('dbo.FN_GetGoalDifference', 'FN') IS NOT NULL
    DROP FUNCTION dbo.FN_GetGoalDifference;
GO
CREATE FUNCTION dbo.FN_GetGoalDifference (@TeamID INT, @TournamentID INT)
RETURNS INT
AS
BEGIN
    DECLARE @GoalFor INT, @GoalAgainst INT;
    SELECT @GoalFor = ISNULL(SUM(
        CASE WHEN m.Team1ID = @TeamID THEN mr.ScoreTeam1
             ELSE mr.ScoreTeam2 END), 0)
    FROM Match m JOIN MatchResult mr ON m.MatchID = mr.MatchID
    WHERE (m.Team1ID = @TeamID OR m.Team2ID = @TeamID)
      AND m.TournamentID = @TournamentID;

    SELECT @GoalAgainst = ISNULL(SUM(
        CASE WHEN m.Team1ID = @TeamID THEN mr.ScoreTeam2
             ELSE mr.ScoreTeam1 END), 0)
    FROM Match m JOIN MatchResult mr ON m.MatchID = mr.MatchID
    WHERE (m.Team1ID = @TeamID OR m.Team2ID = @TeamID)
      AND m.TournamentID = @TournamentID;

    RETURN @GoalFor - @GoalAgainst;
END
GO

-- FN_IsScheduleConflict: Kiểm tra xung đột lịch (BR-MATCH-02)
IF OBJECT_ID('dbo.FN_IsScheduleConflict', 'FN') IS NOT NULL
    DROP FUNCTION dbo.FN_IsScheduleConflict;
GO
CREATE FUNCTION dbo.FN_IsScheduleConflict
    (@TeamID INT, @MatchTime DATETIME, @ExcludeMatchID INT)
RETURNS BIT
AS
BEGIN
    DECLARE @Conflict BIT = 0;
    IF EXISTS (
        SELECT 1 FROM Match
        WHERE (Team1ID = @TeamID OR Team2ID = @TeamID)
          AND MatchTime = @MatchTime
          AND Status <> N'Đã kết thúc'
          AND (@ExcludeMatchID IS NULL OR MatchID <> @ExcludeMatchID)
    )
        SET @Conflict = 1;
    RETURN @Conflict;
END
GO

-- FN_CountPlayersInTeam: Đếm số player của đội (BR-PLAYER-02)
IF OBJECT_ID('dbo.FN_CountPlayersInTeam', 'FN') IS NOT NULL
    DROP FUNCTION dbo.FN_CountPlayersInTeam;
GO
CREATE FUNCTION dbo.FN_CountPlayersInTeam (@TeamID INT)
RETURNS INT
AS
BEGIN
    RETURN (SELECT COUNT(*) FROM Player WHERE TeamID = @TeamID);
END
GO

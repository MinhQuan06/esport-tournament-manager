USE EsportTournamentDB;
GO

/*
  Rich demo data for video presentation.
  Demo logins:
  - admin / admin123 / Admin
  - teammanager / teammanager123 / TeamManager
  This script is idempotent for the named demo tournaments below.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    IF OBJECT_ID('dbo.TR_PreventDeleteConfirmedResult', 'TR') IS NOT NULL
        DISABLE TRIGGER dbo.TR_PreventDeleteConfirmedResult ON dbo.MatchResult;
    IF OBJECT_ID('dbo.TR_PreventTeamOverflow', 'TR') IS NOT NULL
        DISABLE TRIGGER dbo.TR_PreventTeamOverflow ON dbo.Player;
    IF OBJECT_ID('dbo.TR_PreventDuplicateMatch', 'TR') IS NOT NULL
        DISABLE TRIGGER dbo.TR_PreventDuplicateMatch ON dbo.[Match];

    BEGIN TRANSACTION;

    DECLARE @DemoTournaments TABLE (TournamentName NVARCHAR(200) PRIMARY KEY);
    INSERT INTO @DemoTournaments (TournamentName) VALUES
        (N'VCS Spring 2026'),
        (N'CS:GO Vietnam Open'),
        (N'Valorant Champions'),
        (N'CS2 Vietnam Open 2026'),
        (N'Valorant Campus Cup 2026'),
        (N'Dota 2 Legacy Cup 2026');

    DELETE mr
    FROM dbo.MatchResult mr
    JOIN dbo.[Match] m ON mr.MatchID = m.MatchID
    JOIN dbo.Tournament t ON m.TournamentID = t.TournamentID
    JOIN @DemoTournaments d ON d.TournamentName = t.TournamentName;

    DELETE r
    FROM dbo.Ranking r
    JOIN dbo.Tournament t ON r.TournamentID = t.TournamentID
    JOIN @DemoTournaments d ON d.TournamentName = t.TournamentName;

    DELETE m
    FROM dbo.[Match] m
    JOIN dbo.Tournament t ON m.TournamentID = t.TournamentID
    JOIN @DemoTournaments d ON d.TournamentName = t.TournamentName;

    DELETE p
    FROM dbo.Player p
    JOIN dbo.Team tm ON p.TeamID = tm.TeamID
    JOIN dbo.Tournament t ON tm.TournamentID = t.TournamentID
    JOIN @DemoTournaments d ON d.TournamentName = t.TournamentName;

    DELETE tm
    FROM dbo.Team tm
    JOIN dbo.Tournament t ON tm.TournamentID = t.TournamentID
    JOIN @DemoTournaments d ON d.TournamentName = t.TournamentName;

    DELETE t
    FROM dbo.Tournament t
    JOIN @DemoTournaments d ON d.TournamentName = t.TournamentName;

    DECLARE @AdminHash VARCHAR(255) = '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9';
    DECLARE @TeamManagerHash VARCHAR(255) = '216800ed94384c085e9961d788ecd8f8f9bff6675630ea46c67033859887325c';

    MERGE dbo.Account AS target
    USING (VALUES
        ('admin',       @AdminHash,       'admin@esport.vn',       N'Quan tri vien'),
        ('teammanager', @TeamManagerHash, 'teammanager@esport.vn', N'Team Manager Demo')
    ) AS src(Username, PasswordHash, Email, FullName)
    ON target.Username = src.Username
    WHEN MATCHED THEN
        UPDATE SET PasswordHash = src.PasswordHash,
                   Email = src.Email,
                   FullName = src.FullName,
                   IsLocked = 0,
                   LastLoginAt = NULL
    WHEN NOT MATCHED THEN
        INSERT (Username, PasswordHash, Email, FullName, IsLocked, CreatedAt, LastLoginAt)
        VALUES (src.Username, src.PasswordHash, src.Email, src.FullName, 0, GETDATE(), NULL);

    -- Only the two default demo accounts can log in; old sample accounts are locked.
    UPDATE dbo.Account
    SET IsLocked = 1
    WHERE Username NOT IN ('admin', 'teammanager');

    DECLARE @AdminRole INT = (SELECT RoleID FROM dbo.Role WHERE RoleName = N'Admin');
    DECLARE @ManagerRole INT = (SELECT RoleID FROM dbo.Role WHERE RoleName = N'TeamManager');

    DELETE ar
    FROM dbo.AccountRole ar
    JOIN dbo.Account a ON a.AccountID = ar.AccountID
    WHERE a.Username IN ('admin', 'teammanager');

    INSERT INTO dbo.AccountRole (AccountID, RoleID)
    SELECT a.AccountID, @AdminRole
    FROM dbo.Account a
    WHERE a.Username = 'admin'
      AND @AdminRole IS NOT NULL;

    INSERT INTO dbo.AccountRole (AccountID, RoleID)
    SELECT a.AccountID, @ManagerRole
    FROM dbo.Account a
    WHERE a.Username = 'teammanager'
      AND @ManagerRole IS NOT NULL;

    INSERT INTO dbo.Tournament (TournamentName, StartDate, EndDate, Description, Status, GameType, Format, CreatedAt) VALUES
        (N'VCS Spring 2026', N'2026-05-13', N'2026-06-30', N'Giải Liên Minh Huyền Thoại mùa xuân với 6 đội tuyển mạnh nhất Việt Nam.', N'Đang diễn ra', N'League of Legends', N'Round Robin', N'2026-05-01T08:00:00'),
        (N'CS2 Vietnam Open 2026', N'2026-05-01', N'2026-05-25', N'Giải CS2 mở rộng toàn quốc, thi đấu BO3 theo nhánh thắng/thua.', N'Đang diễn ra', N'CS2', N'Double Elim.', N'2026-05-01T08:05:00'),
        (N'Valorant Campus Cup 2026', N'2026-05-18', N'2026-06-05', N'Giải Valorant sinh viên, dùng cho demo đăng ký và lịch thi đấu sắp diễn ra.', N'Chưa bắt đầu', N'Valorant', N'Single Elim.', N'2026-05-02T09:00:00'),
        (N'Dota 2 Legacy Cup 2026', N'2026-04-18', N'2026-04-30', N'Giải Dota 2 đã kết thúc, có kết quả và bảng xếp hạng hoàn chỉnh.', N'Đã kết thúc', N'Dota 2', N'Playoffs BO3', N'2026-04-01T08:00:00');

    DECLARE @M1 INT = (SELECT AccountID FROM dbo.Account WHERE Username = 'teammanager');
    DECLARE @M2 INT = @M1;
    DECLARE @M3 INT = @M1;

    DECLARE @TeamSeed TABLE (
        TournamentName NVARCHAR(200), TeamName NVARCHAR(100), ShortName NVARCHAR(10), LogoColor VARCHAR(20),
        GameType NVARCHAR(50), ManagerAccountID INT NULL, ManagerName NVARCHAR(100), Description NVARCHAR(300)
    );
    INSERT INTO @TeamSeed VALUES
        (N'VCS Spring 2026', N'GAM Esports', N'GAM', '#f97316', N'League of Legends', @M1, N'Trần Hữu Long', N'Đương kim vô địch VCS'),
        (N'VCS Spring 2026', N'Team Whales', N'TW', '#06b6d4', N'League of Legends', @M2, N'Nguyễn Minh Quân', N'Đội tuyển trẻ giàu tốc độ'),
        (N'VCS Spring 2026', N'Vikings Esports', N'VKE', '#ef4444', N'League of Legends', @M3, N'Phan Lam Trường', N'Ứng viên top 4 mùa giải'),
        (N'VCS Spring 2026', N'Saigon Buffalo', N'SGB', '#dc2626', N'League of Legends', @M1, N'Trần Hữu Long', N'Lối chơi giao tranh mạnh'),
        (N'VCS Spring 2026', N'Cerberus Esports', N'CES', '#8b5cf6', N'League of Legends', @M2, N'Nguyễn Minh Quân', N'Đội hình kinh nghiệm'),
        (N'VCS Spring 2026', N'MGN Blue Esports', N'MBE', '#3b82f6', N'League of Legends', @M3, N'Phan Lam Trường', N'Tân binh nhiều tiềm năng'),

        (N'CS2 Vietnam Open 2026', N'Saigon Aces', N'SA', '#f59e0b', N'CS2', @M1, N'Trần Hữu Long', N'Đội CS2 khu vực miền Nam'),
        (N'CS2 Vietnam Open 2026', N'Hanoi Reflex', N'HR', '#10b981', N'CS2', @M2, N'Nguyễn Minh Quân', N'Đội CS2 khu vực miền Bắc'),
        (N'CS2 Vietnam Open 2026', N'Danang Titans', N'DT', '#6366f1', N'CS2', @M3, N'Phan Lam Trường', N'Đại diện miền Trung'),
        (N'CS2 Vietnam Open 2026', N'Cantho Rush', N'CR', '#14b8a6', N'CS2', @M1, N'Trần Hữu Long', N'Lối chơi kiểm soát bản đồ'),

        (N'Valorant Campus Cup 2026', N'FPT Phoenix', N'FPT', '#f43f5e', N'Valorant', @M1, N'Trần Hữu Long', N'Đại diện FPT University'),
        (N'Valorant Campus Cup 2026', N'HCMUS Sentinels', N'HCM', '#22c55e', N'Valorant', @M2, N'Nguyễn Minh Quân', N'Đội tuyển sinh viên HCMUS'),
        (N'Valorant Campus Cup 2026', N'UIT Wolves', N'UIT', '#0ea5e9', N'Valorant', @M3, N'Phan Lam Trường', N'Đội tuyển UIT'),
        (N'Valorant Campus Cup 2026', N'HUTECH Storm', N'HUT', '#a855f7', N'Valorant', @M1, N'Trần Hữu Long', N'Đội tuyển HUTECH'),

        (N'Dota 2 Legacy Cup 2026', N'Lotus Gaming', N'LOT', '#84cc16', N'Dota 2', @M1, N'Trần Hữu Long', N'Nhà vô địch Legacy Cup'),
        (N'Dota 2 Legacy Cup 2026', N'Mekong Hydra', N'MKH', '#06b6d4', N'Dota 2', @M2, N'Nguyễn Minh Quân', N'Á quân Legacy Cup'),
        (N'Dota 2 Legacy Cup 2026', N'Imperial Foxes', N'FOX', '#fb7185', N'Dota 2', @M3, N'Phan Lam Trường', N'Top 3 Legacy Cup'),
        (N'Dota 2 Legacy Cup 2026', N'Hue Guardians', N'HUE', '#64748b', N'Dota 2', @M1, N'Trần Hữu Long', N'Đội tuyển khách mời');

    INSERT INTO dbo.Team (TournamentID, TeamName, Description, ManagerAccountID, ManagerName, ShortName, LogoColor, GameType, IsActive, CreatedAt)
    SELECT t.TournamentID, s.TeamName, s.Description, s.ManagerAccountID, s.ManagerName, s.ShortName, s.LogoColor, s.GameType, 1, GETDATE()
    FROM @TeamSeed s
    JOIN dbo.Tournament t ON t.TournamentName = s.TournamentName;

    DECLARE @PlayerSeed TABLE (
        TeamName NVARCHAR(100), PlayerName NVARCHAR(100), Nickname NVARCHAR(50), Position NVARCHAR(30), ContactInfo NVARCHAR(200), BirthDate DATE
    );
    INSERT INTO @PlayerSeed VALUES
        (N'GAM Esports', N'Lê Minh Khang', N'KhangTop', N'Top', N'khang@gam.vn', '2003-01-12'),
        (N'GAM Esports', N'Nguyễn Hải Nam', N'NamJungle', N'Jungle', N'nam@gam.vn', '2002-03-08'),
        (N'GAM Esports', N'Phạm Quốc An', N'AnMid', N'Mid', N'an@gam.vn', '2001-07-21'),
        (N'GAM Esports', N'Đỗ Gia Bảo', N'BaoADC', N'ADC', N'bao@gam.vn', '2003-10-01'),
        (N'GAM Esports', N'Vũ Thành Đạt', N'DatSupport', N'Support', N'dat@gam.vn', '2002-12-19'),
        (N'Team Whales', N'Trần Duy Anh', N'WhaleTop', N'Top', N'duyanh@tw.vn', '2002-04-11'),
        (N'Team Whales', N'Hoàng Minh Tú', N'TuJungle', N'Jungle', N'tu@tw.vn', '2001-02-16'),
        (N'Team Whales', N'Bùi Nhật Long', N'LongMid', N'Mid', N'long@tw.vn', '2003-09-05'),
        (N'Team Whales', N'Nguyễn Thành Tín', N'TinADC', N'ADC', N'tin@tw.vn', '2002-11-09'),
        (N'Team Whales', N'Lâm Hữu Phước', N'PhuocSP', N'Support', N'phuoc@tw.vn', '2001-06-22'),
        (N'Vikings Esports', N'Võ Anh Khoa', N'KhoaTop', N'Top', N'khoa@vke.vn', '2002-06-14'),
        (N'Vikings Esports', N'Nguyễn Tuấn Kiệt', N'KietJG', N'Jungle', N'kiet@vke.vn', '2003-08-30'),
        (N'Vikings Esports', N'Trương Huy Hoàng', N'HoangMid', N'Mid', N'hoang@vke.vn', '2001-12-04'),
        (N'Vikings Esports', N'Mai Đức Huy', N'HuyADC', N'ADC', N'huy@vke.vn', '2002-05-17'),
        (N'Vikings Esports', N'Phan Gia Hưng', N'HungSP', N'Support', N'hung@vke.vn', '2003-03-29'),
        (N'Saigon Buffalo', N'Đặng Minh Phát', N'PhatTop', N'Top', N'phat@sgb.vn', '2001-01-25'),
        (N'Saigon Buffalo', N'Ngô Hoàng Sơn', N'SonJG', N'Jungle', N'son@sgb.vn', '2002-08-18'),
        (N'Saigon Buffalo', N'Phan Nhật Minh', N'MinhMid', N'Mid', N'minh@sgb.vn', '2003-04-02'),
        (N'Saigon Buffalo', N'Trần Bảo Khánh', N'KhanhADC', N'ADC', N'khanh@sgb.vn', '2002-07-07'),
        (N'Saigon Buffalo', N'Lê Quang Vinh', N'VinhSP', N'Support', N'vinh@sgb.vn', '2001-11-11'),
        (N'Cerberus Esports', N'Nguyễn Phúc Lâm', N'LamTop', N'Top', N'lam@ces.vn', '2002-02-10'),
        (N'Cerberus Esports', N'Trịnh Quốc Huy', N'HuyJG', N'Jungle', N'huy@ces.vn', '2001-09-14'),
        (N'Cerberus Esports', N'Đào Nhật Tân', N'TanMid', N'Mid', N'tan@ces.vn', '2003-01-05'),
        (N'Cerberus Esports', N'Vũ Minh Trí', N'TriADC', N'ADC', N'tri@ces.vn', '2002-12-08'),
        (N'Cerberus Esports', N'Bùi Gia Minh', N'MinhSP', N'Support', N'giaminh@ces.vn', '2001-05-20'),
        (N'MGN Blue Esports', N'Hoàng Gia Huy', N'BlueTop', N'Top', N'giahuy@mbe.vn', '2003-06-06'),
        (N'MGN Blue Esports', N'Lê Tuấn Anh', N'BlueJG', N'Jungle', N'tuananh@mbe.vn', '2002-04-04'),
        (N'MGN Blue Esports', N'Nguyễn Hoàng Phúc', N'BlueMid', N'Mid', N'phuc@mbe.vn', '2001-10-10'),
        (N'MGN Blue Esports', N'Trần Minh Quân', N'BlueADC', N'ADC', N'quan@mbe.vn', '2002-09-09'),
        (N'MGN Blue Esports', N'Phạm Đức Toàn', N'BlueSP', N'Support', N'toan@mbe.vn', '2003-02-02');

    INSERT INTO @PlayerSeed
    SELECT ts.TeamName,
           CONCAT(N'Tuyển thủ ', v.NickSuffix, N' ', ts.ShortName),
           CONCAT(ts.ShortName, v.NickSuffix),
           v.Position,
           CONCAT(LOWER(ts.ShortName), v.NickSuffix, N'@demo.vn'),
           DATEADD(DAY, v.Seq * 37, '2001-01-01')
    FROM @TeamSeed ts
    CROSS JOIN (VALUES
        (1, N'One', N'IGL'), (2, N'Two', N'Entry'), (3, N'Three', N'Rifler'), (4, N'Four', N'Sniper'), (5, N'Five', N'Support')
    ) v(Seq, NickSuffix, Position)
    WHERE ts.TeamName NOT IN (N'GAM Esports', N'Team Whales', N'Vikings Esports', N'Saigon Buffalo', N'Cerberus Esports', N'MGN Blue Esports');

    INSERT INTO dbo.Player (TeamID, PlayerName, ContactInfo, Nickname, Position, Country, BirthDate, IsActive, CreatedAt)
    SELECT tm.TeamID, p.PlayerName, p.ContactInfo, p.Nickname, p.Position, N'Việt Nam', p.BirthDate, 1, GETDATE()
    FROM @PlayerSeed p
    JOIN dbo.Team tm ON tm.TeamName = p.TeamName;

    DECLARE @MatchSeed TABLE (
        TournamentName NVARCHAR(200), Team1Name NVARCHAR(100), Team2Name NVARCHAR(100), MatchTime DATETIME,
        Status NVARCHAR(50), RoundName NVARCHAR(50), GroupName NVARCHAR(20), MatchFormat NVARCHAR(10),
        ScoreTeam1 INT NULL, ScoreTeam2 INT NULL, IsConfirmed BIT NULL
    );
    INSERT INTO @MatchSeed VALUES
        (N'Dota 2 Legacy Cup 2026', N'Lotus Gaming', N'Hue Guardians', '2026-04-20T18:00:00', N'Đã kết thúc', N'Bán kết', N'A', N'BO3', 2, 0, 1),
        (N'Dota 2 Legacy Cup 2026', N'Mekong Hydra', N'Imperial Foxes', '2026-04-21T18:00:00', N'Đã kết thúc', N'Bán kết', N'A', N'BO3', 2, 1, 1),
        (N'Dota 2 Legacy Cup 2026', N'Imperial Foxes', N'Hue Guardians', '2026-04-27T17:00:00', N'Đã kết thúc', N'Tranh hạng 3', N'A', N'BO3', 2, 0, 1),
        (N'Dota 2 Legacy Cup 2026', N'Lotus Gaming', N'Mekong Hydra', '2026-04-30T19:00:00', N'Đã kết thúc', N'Chung kết', N'A', N'BO5', 3, 2, 1),

        (N'CS2 Vietnam Open 2026', N'Saigon Aces', N'Cantho Rush', '2026-05-03T15:00:00', N'Đã kết thúc', N'Vòng bảng', N'A', N'BO3', 13, 8, 1),
        (N'CS2 Vietnam Open 2026', N'Hanoi Reflex', N'Danang Titans', '2026-05-04T19:00:00', N'Đã kết thúc', N'Vòng bảng', N'A', N'BO3', 10, 13, 1),
        (N'CS2 Vietnam Open 2026', N'Saigon Aces', N'Hanoi Reflex', '2026-05-13T16:00:00', N'Đang diễn ra', N'Nhánh thắng', N'A', N'BO3', NULL, NULL, NULL),
        (N'CS2 Vietnam Open 2026', N'Danang Titans', N'Cantho Rush', '2026-05-15T19:30:00', N'Chưa diễn ra', N'Nhánh thắng', N'A', N'BO3', NULL, NULL, NULL),
        (N'CS2 Vietnam Open 2026', N'Saigon Aces', N'Danang Titans', '2026-05-17T20:00:00', N'Chưa diễn ra', N'Bán kết', N'A', N'BO3', NULL, NULL, NULL),

        (N'VCS Spring 2026', N'GAM Esports', N'Saigon Buffalo', '2026-05-04T18:00:00', N'Đã kết thúc', N'Vòng bảng', N'A', N'BO3', 2, 0, 1),
        (N'VCS Spring 2026', N'Team Whales', N'Cerberus Esports', '2026-05-05T20:00:00', N'Đã kết thúc', N'Vòng bảng', N'A', N'BO3', 1, 2, 1),
        (N'VCS Spring 2026', N'GAM Esports', N'MGN Blue Esports', '2026-05-07T19:00:00', N'Đã kết thúc', N'Vòng bảng', N'B', N'BO3', 2, 1, 1),
        (N'VCS Spring 2026', N'Vikings Esports', N'MGN Blue Esports', '2026-05-13T14:00:00', N'Đang diễn ra', N'Vòng bảng', N'B', N'BO3', NULL, NULL, NULL),
        (N'VCS Spring 2026', N'GAM Esports', N'Team Whales', '2026-05-13T19:30:00', N'Chưa diễn ra', N'Tâm điểm', N'A', N'BO5', NULL, NULL, NULL),
        (N'VCS Spring 2026', N'Saigon Buffalo', N'Cerberus Esports', '2026-05-14T18:00:00', N'Chưa diễn ra', N'Vòng bảng', N'A', N'BO3', NULL, NULL, NULL),
        (N'VCS Spring 2026', N'Vikings Esports', N'GAM Esports', '2026-05-16T20:00:00', N'Chưa diễn ra', N'Vòng bảng', N'B', N'BO3', NULL, NULL, NULL),
        (N'VCS Spring 2026', N'Team Whales', N'MGN Blue Esports', '2026-05-20T19:00:00', N'Chưa diễn ra', N'Vòng bảng', N'B', N'BO3', NULL, NULL, NULL),
        (N'VCS Spring 2026', N'Cerberus Esports', N'Vikings Esports', '2026-05-22T19:30:00', N'Chưa diễn ra', N'Vòng bảng', N'A', N'BO3', NULL, NULL, NULL),

        (N'Valorant Campus Cup 2026', N'FPT Phoenix', N'HCMUS Sentinels', '2026-05-18T18:00:00', N'Chưa diễn ra', N'Tứ kết', N'A', N'BO3', NULL, NULL, NULL),
        (N'Valorant Campus Cup 2026', N'UIT Wolves', N'HUTECH Storm', '2026-05-18T20:00:00', N'Chưa diễn ra', N'Tứ kết', N'B', N'BO3', NULL, NULL, NULL),
        (N'Valorant Campus Cup 2026', N'FPT Phoenix', N'UIT Wolves', '2026-05-21T19:30:00', N'Chưa diễn ra', N'Bán kết', N'A', N'BO3', NULL, NULL, NULL),
        (N'Valorant Campus Cup 2026', N'HCMUS Sentinels', N'HUTECH Storm', '2026-05-24T20:00:00', N'Chưa diễn ra', N'Chung kết', N'A', N'BO5', NULL, NULL, NULL);

    INSERT INTO dbo.[Match] (TournamentID, Team1ID, Team2ID, MatchTime, Status, RoundName, GroupName, MatchFormat, CreatedAt)
    SELECT tour.TournamentID, t1.TeamID, t2.TeamID, s.MatchTime, s.Status, s.RoundName, s.GroupName, s.MatchFormat, GETDATE()
    FROM @MatchSeed s
    JOIN dbo.Tournament tour ON tour.TournamentName = s.TournamentName
    JOIN dbo.Team t1 ON t1.TournamentID = tour.TournamentID AND t1.TeamName = s.Team1Name
    JOIN dbo.Team t2 ON t2.TournamentID = tour.TournamentID AND t2.TeamName = s.Team2Name;

    INSERT INTO dbo.MatchResult (MatchID, WinnerTeamID, ScoreTeam1, ScoreTeam2, IsConfirmed, RecordedAt)
    SELECT m.MatchID,
           CASE WHEN s.ScoreTeam1 > s.ScoreTeam2 THEN t1.TeamID
                WHEN s.ScoreTeam2 > s.ScoreTeam1 THEN t2.TeamID
                ELSE NULL END AS WinnerTeamID,
           s.ScoreTeam1,
           s.ScoreTeam2,
           ISNULL(s.IsConfirmed, 0),
           GETDATE()
    FROM @MatchSeed s
    JOIN dbo.Tournament tour ON tour.TournamentName = s.TournamentName
    JOIN dbo.Team t1 ON t1.TournamentID = tour.TournamentID AND t1.TeamName = s.Team1Name
    JOIN dbo.Team t2 ON t2.TournamentID = tour.TournamentID AND t2.TeamName = s.Team2Name
    JOIN dbo.[Match] m ON m.TournamentID = tour.TournamentID
                      AND m.Team1ID = t1.TeamID
                      AND m.Team2ID = t2.TeamID
                      AND m.MatchTime = s.MatchTime
    WHERE s.ScoreTeam1 IS NOT NULL AND s.ScoreTeam2 IS NOT NULL;

    DECLARE @TournamentID INT;
    DECLARE tournament_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT TournamentID FROM dbo.Tournament WHERE TournamentName IN (SELECT TournamentName FROM @DemoTournaments);
    OPEN tournament_cursor;
    FETCH NEXT FROM tournament_cursor INTO @TournamentID;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        EXEC dbo.SP_RecalcRanking @TournamentID;
        FETCH NEXT FROM tournament_cursor INTO @TournamentID;
    END
    CLOSE tournament_cursor;
    DEALLOCATE tournament_cursor;

    COMMIT TRANSACTION;

    IF OBJECT_ID('dbo.TR_PreventDeleteConfirmedResult', 'TR') IS NOT NULL
        ENABLE TRIGGER dbo.TR_PreventDeleteConfirmedResult ON dbo.MatchResult;
    IF OBJECT_ID('dbo.TR_PreventTeamOverflow', 'TR') IS NOT NULL
        ENABLE TRIGGER dbo.TR_PreventTeamOverflow ON dbo.Player;
    IF OBJECT_ID('dbo.TR_PreventDuplicateMatch', 'TR') IS NOT NULL
        ENABLE TRIGGER dbo.TR_PreventDuplicateMatch ON dbo.[Match];

    PRINT N'Demo video seed completed.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

    IF OBJECT_ID('dbo.TR_PreventDeleteConfirmedResult', 'TR') IS NOT NULL
        ENABLE TRIGGER dbo.TR_PreventDeleteConfirmedResult ON dbo.MatchResult;
    IF OBJECT_ID('dbo.TR_PreventTeamOverflow', 'TR') IS NOT NULL
        ENABLE TRIGGER dbo.TR_PreventTeamOverflow ON dbo.Player;
    IF OBJECT_ID('dbo.TR_PreventDuplicateMatch', 'TR') IS NOT NULL
        ENABLE TRIGGER dbo.TR_PreventDuplicateMatch ON dbo.[Match];

    THROW;
END CATCH;
GO

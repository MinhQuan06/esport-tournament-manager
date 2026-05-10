USE EsportTournamentDB;
GO

-- ============================================================
-- DỮ LIỆU MẪU
-- Mật khẩu mặc định: 123456 (SHA256 hash bên dưới)
-- SHA256('123456') = 8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92
-- ============================================================

-- Tài khoản mẫu (3 vai trò)
INSERT INTO Account (Username, PasswordHash, Email, FullName) VALUES
    (N'admin',     N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'admin@esport.vn',      N'Quản trị viên'),
    (N'manager1',  N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'manager1@esport.vn',   N'Trần Hữu Long'),
    (N'manager2',  N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'manager2@esport.vn',   N'Lý Khôi Nguyên'),
    (N'viewer1',   N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'viewer1@esport.vn',    N'Đỗ Minh Quân');
GO

-- Gán vai trò
INSERT INTO AccountRole (AccountID, RoleID)
SELECT a.AccountID, r.RoleID
FROM Account a JOIN Role r ON
    (a.Username = N'admin'    AND r.RoleName = N'Admin')      OR
    (a.Username = N'manager1' AND r.RoleName = N'TeamManager') OR
    (a.Username = N'manager2' AND r.RoleName = N'TeamManager') OR
    (a.Username = N'viewer1'  AND r.RoleName = N'Viewer');
-- Mọi tài khoản cũng có Viewer (BR-ACC-02)
INSERT INTO AccountRole (AccountID, RoleID)
SELECT a.AccountID, r.RoleID
FROM Account a JOIN Role r ON r.RoleName = N'Viewer'
WHERE NOT EXISTS (SELECT 1 FROM AccountRole ar
                  WHERE ar.AccountID = a.AccountID AND ar.RoleID = r.RoleID);
GO

-- Giải đấu mẫu
INSERT INTO Tournament (TournamentName, StartDate, EndDate, Description, Status) VALUES
    (N'VCS Spring 2026',     '2026-05-15', '2026-06-30', N'Vòng đấu mùa Xuân 2026 - LMHT', N'Chưa bắt đầu'),
    (N'CS:GO Vietnam Open',  '2026-04-01', '2026-05-01', N'Giải đấu CS:GO mở rộng',       N'Đang diễn ra'),
    (N'Valorant Champions',  '2026-01-10', '2026-02-15', N'Giải Valorant',                 N'Đã kết thúc');
GO

-- Đội mẫu (giải Valorant đã kết thúc - 4 đội)
DECLARE @T3 INT = (SELECT TournamentID FROM Tournament WHERE TournamentName = N'Valorant Champions');
DECLARE @M1 INT = (SELECT AccountID FROM Account WHERE Username = N'manager1');
DECLARE @M2 INT = (SELECT AccountID FROM Account WHERE Username = N'manager2');

INSERT INTO Team (TournamentID, TeamName, Description, ManagerAccountID) VALUES
    (@T3, N'Saigon Phantoms', N'Đại diện TP.HCM',  @M1),
    (@T3, N'Hanoi Dragons',   N'Đại diện Hà Nội',  @M2),
    (@T3, N'Da Nang Hawks',   N'Đại diện Đà Nẵng', NULL),
    (@T3, N'Can Tho Tigers',  N'Đại diện Cần Thơ', NULL);

DECLARE @T2 INT = (SELECT TournamentID FROM Tournament WHERE TournamentName = N'CS:GO Vietnam Open');
INSERT INTO Team (TournamentID, TeamName, Description, ManagerAccountID) VALUES
    (@T2, N'Team Nova',  N'Team CS:GO Nova',  @M1),
    (@T2, N'Team Alpha', N'Team CS:GO Alpha', @M2);
GO

-- Người chơi mẫu (đảm bảo không quá 10/đội)
DECLARE @TmA INT = (SELECT TeamID FROM Team WHERE TeamName = N'Saigon Phantoms');
DECLARE @TmB INT = (SELECT TeamID FROM Team WHERE TeamName = N'Hanoi Dragons');
INSERT INTO Player (TeamID, PlayerName, ContactInfo) VALUES
    (@TmA, N'Player A1', N'a1@phantom.vn'),
    (@TmA, N'Player A2', N'a2@phantom.vn'),
    (@TmA, N'Player A3', N'a3@phantom.vn'),
    (@TmA, N'Player A4', N'a4@phantom.vn'),
    (@TmA, N'Player A5', N'a5@phantom.vn'),
    (@TmB, N'Player B1', N'b1@dragon.vn'),
    (@TmB, N'Player B2', N'b2@dragon.vn'),
    (@TmB, N'Player B3', N'b3@dragon.vn');
GO

-- Lịch và kết quả mẫu cho giải đã kết thúc
DECLARE @T3 INT = (SELECT TournamentID FROM Tournament WHERE TournamentName = N'Valorant Champions');
DECLARE @SP INT = (SELECT TeamID FROM Team WHERE TeamName = N'Saigon Phantoms');
DECLARE @HD INT = (SELECT TeamID FROM Team WHERE TeamName = N'Hanoi Dragons');
DECLARE @DH INT = (SELECT TeamID FROM Team WHERE TeamName = N'Da Nang Hawks');
DECLARE @CT INT = (SELECT TeamID FROM Team WHERE TeamName = N'Can Tho Tigers');

INSERT INTO Match (TournamentID, Team1ID, Team2ID, MatchTime, Status) VALUES
    (@T3, @SP, @HD, '2026-01-15 19:00', N'Đã kết thúc'),
    (@T3, @DH, @CT, '2026-01-16 19:00', N'Đã kết thúc'),
    (@T3, @SP, @DH, '2026-01-22 19:00', N'Đã kết thúc'),
    (@T3, @HD, @CT, '2026-01-23 19:00', N'Đã kết thúc'),
    (@T3, @SP, @CT, '2026-01-29 19:00', N'Đã kết thúc'),
    (@T3, @HD, @DH, '2026-01-30 19:00', N'Đã kết thúc');

-- Nhập kết quả qua SP để tự cập nhật BXH
DECLARE @MID INT;
SELECT TOP 1 @MID = MatchID FROM Match WHERE Team1ID = @SP AND Team2ID = @HD AND TournamentID = @T3;
EXEC dbo.SP_EnterMatchResult @MID, 13, 8;
EXEC dbo.SP_ConfirmMatchResult @MID;

SELECT TOP 1 @MID = MatchID FROM Match WHERE Team1ID = @DH AND Team2ID = @CT AND TournamentID = @T3;
EXEC dbo.SP_EnterMatchResult @MID, 13, 11;
EXEC dbo.SP_ConfirmMatchResult @MID;

SELECT TOP 1 @MID = MatchID FROM Match WHERE Team1ID = @SP AND Team2ID = @DH AND TournamentID = @T3;
EXEC dbo.SP_EnterMatchResult @MID, 10, 13;
EXEC dbo.SP_ConfirmMatchResult @MID;

SELECT TOP 1 @MID = MatchID FROM Match WHERE Team1ID = @HD AND Team2ID = @CT AND TournamentID = @T3;
EXEC dbo.SP_EnterMatchResult @MID, 13, 13;  -- Hòa
EXEC dbo.SP_ConfirmMatchResult @MID;

SELECT TOP 1 @MID = MatchID FROM Match WHERE Team1ID = @SP AND Team2ID = @CT AND TournamentID = @T3;
EXEC dbo.SP_EnterMatchResult @MID, 13, 5;
EXEC dbo.SP_ConfirmMatchResult @MID;

SELECT TOP 1 @MID = MatchID FROM Match WHERE Team1ID = @HD AND Team2ID = @DH AND TournamentID = @T3;
EXEC dbo.SP_EnterMatchResult @MID, 13, 9;
EXEC dbo.SP_ConfirmMatchResult @MID;
GO

PRINT N'>>> Seed data đã được nạp thành công.';
PRINT N'>>> Tài khoản mẫu (mật khẩu: 123456):';
PRINT N'    - admin / manager1 / manager2 / viewer1';
GO

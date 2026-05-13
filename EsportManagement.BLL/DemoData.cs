using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using EsportManagement.DTO;

namespace EsportManagement.BLL
{
    internal static class DemoData
    {
        public static bool IsDatabaseUnavailable(Exception ex)
        {
            var enabled = Environment.GetEnvironmentVariable("ESPORT_ENABLE_DEMO_FALLBACK");
            if (!string.Equals(enabled, "1", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(enabled, "true", StringComparison.OrdinalIgnoreCase))
                return false;

            while (ex != null)
            {
                if (ex is SqlException) return true;
                if (ex is InvalidOperationException &&
                    ex.Message.IndexOf("connection string", StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
                ex = ex.InnerException;
            }
            return false;
        }

        public static Account Login(string username, string passwordHash)
        {
            foreach (var account in Accounts())
            {
                if (string.Equals(account.Username, username, StringComparison.OrdinalIgnoreCase) &&
                    account.PasswordHash == passwordHash &&
                    !account.IsLocked)
                {
                    account.LastLoginAt = DateTime.Now;
                    return account;
                }
            }
            return null;
        }

        public static List<Account> Accounts()
        {
            return new List<Account>
            {
                Account(1, "admin", PasswordHasher.Hash("admin123"), "admin@esport.vn", "Demo Admin", "Admin"),
                Account(2, "teammanager", PasswordHasher.Hash("teammanager123"), "teammanager@esport.vn", "Demo Team Manager", "TeamManager")
            };
        }

        public static Account AccountById(int id)
        {
            foreach (var account in Accounts())
                if (account.AccountID == id) return account;
            return null;
        }

        public static List<Role> Roles()
        {
            return new List<Role>
            {
                new Role { RoleID = 1, RoleName = "Admin", Description = "System administrator" },
                new Role { RoleID = 2, RoleName = "TeamManager", Description = "Team manager" },
                new Role { RoleID = 3, RoleName = "Viewer", Description = "Viewer" }
            };
        }

        public static Role RoleById(int id)
        {
            foreach (var role in Roles())
                if (role.RoleID == id) return role;
            return null;
        }

        public static Role RoleByName(string name)
        {
            foreach (var role in Roles())
                if (string.Equals(role.RoleName, name, StringComparison.OrdinalIgnoreCase)) return role;
            return null;
        }

        public static List<Tournament> Tournaments()
        {
            var now = DateTime.Now;
            return new List<Tournament>
            {
                new Tournament
                {
                    TournamentID = 1,
                    TournamentName = "Valorant Spring Cup",
                    StartDate = now.AddDays(-3),
                    EndDate = now.AddDays(4),
                    Description = "Demo tournament when SQL Server is unavailable.",
                    Status = TournamentStatus.DangDienRa,
                    CreatedAt = now.AddDays(-10),
                    GameType = "Valorant",
                    Format = "Group Stage",
                    TeamCount = 4,
                    MatchCount = 3
                },
                new Tournament
                {
                    TournamentID = 2,
                    TournamentName = "League of Legends Campus",
                    StartDate = now.AddDays(7),
                    EndDate = now.AddDays(14),
                    Description = "Demo upcoming tournament.",
                    Status = TournamentStatus.ChuaBatDau,
                    CreatedAt = now.AddDays(-2),
                    GameType = "League of Legends",
                    Format = "Single Elim",
                    TeamCount = 2,
                    MatchCount = 1
                },
                new Tournament
                {
                    TournamentID = 3,
                    TournamentName = "CS2 Night League",
                    StartDate = now.AddDays(-20),
                    EndDate = now.AddDays(-12),
                    Description = "Demo completed tournament.",
                    Status = TournamentStatus.DaKetThuc,
                    CreatedAt = now.AddMonths(-1),
                    GameType = "CS2",
                    Format = "Double Elim",
                    TeamCount = 2,
                    MatchCount = 1
                }
            };
        }

        public static Tournament TournamentById(int id)
        {
            foreach (var tournament in Tournaments())
                if (tournament.TournamentID == id) return tournament;
            return null;
        }

        public static List<Tournament> SearchTournaments(string keyword, string status, string gameType)
        {
            var result = new List<Tournament>();
            foreach (var tournament in Tournaments())
            {
                if (!string.IsNullOrEmpty(keyword) &&
                    tournament.TournamentName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                if (!string.IsNullOrEmpty(status) && StatusHelper.ToDb(tournament.Status) != status)
                    continue;
                if (!string.IsNullOrEmpty(gameType) &&
                    !string.Equals(tournament.GameType, gameType, StringComparison.OrdinalIgnoreCase))
                    continue;
                result.Add(tournament);
            }
            return result;
        }

        public static List<Team> Teams()
        {
            var now = DateTime.Now;
            return new List<Team>
            {
                Team(1, 1, "Saigon Wolves", "SGW", "#ef4444", "Valorant", "Demo Manager 1", 2, 3, 1, 1),
                Team(2, 1, "Hanoi Phoenix", "HNP", "#f59e0b", "Valorant", "Demo Manager 2", 2, 2, 2, 2),
                Team(3, 1, "Danang Storm", "DNS", "#06b6d4", "Valorant", "Demo Manager 1", 2, 1, 3, 3),
                Team(4, 1, "Cantho Titans", "CTT", "#8b5cf6", "Valorant", "Demo Manager 2", 2, 0, 4, 4),
                Team(5, 2, "TDT Dragons", "TDT", "#3b82f6", "League of Legends", "Demo Manager 1", 2, 0, 0, 0),
                Team(6, 2, "UIT Knights", "UIT", "#22c55e", "League of Legends", "Demo Manager 2", 2, 0, 0, 0),
                Team(7, 3, "Night Owls", "NOW", "#f97316", "CS2", "Demo Manager 1", 2, 1, 1, 1),
                Team(8, 3, "Pixel Hunters", "PXH", "#14b8a6", "CS2", "Demo Manager 2", 2, 0, 1, 2)
            };
        }

        public static List<Team> TeamsByTournament(int tournamentId)
        {
            var result = new List<Team>();
            foreach (var team in Teams())
                if (team.TournamentID == tournamentId) result.Add(team);
            return result;
        }

        public static List<Team> TeamsByManager(int managerAccountId)
        {
            var result = new List<Team>();
            foreach (var team in Teams())
                if (team.ManagerAccountID == managerAccountId) result.Add(team);
            return result;
        }

        public static List<Team> MyTeamsByGame(int managerAccountId, string gameType)
        {
            var result = new List<Team>();
            foreach (var team in TeamsByManager(managerAccountId))
            {
                if (string.IsNullOrEmpty(gameType) ||
                    string.Equals(team.GameType, gameType, StringComparison.OrdinalIgnoreCase))
                    result.Add(team);
            }
            return result;
        }

        public static Team TeamById(int id)
        {
            foreach (var team in Teams())
                if (team.TeamID == id) return team;
            return null;
        }

        public static List<Player> Players()
        {
            var now = DateTime.Now;
            return new List<Player>
            {
                Player(1, 1, "Nguyen Van A", "WolfA", "Duelist", "Saigon Wolves", "SGW", "#ef4444"),
                Player(2, 1, "Tran Van B", "WolfB", "Controller", "Saigon Wolves", "SGW", "#ef4444"),
                Player(3, 2, "Le Van C", "PhoenixC", "Sentinel", "Hanoi Phoenix", "HNP", "#f59e0b"),
                Player(4, 2, "Pham Van D", "PhoenixD", "Initiator", "Hanoi Phoenix", "HNP", "#f59e0b"),
                Player(5, 5, "Do Van E", "DragonE", "Mid", "TDT Dragons", "TDT", "#3b82f6"),
                Player(6, 6, "Vo Van F", "KnightF", "Jungle", "UIT Knights", "UIT", "#22c55e")
            };
        }

        public static List<Player> PlayersByTeam(int teamId)
        {
            var result = new List<Player>();
            foreach (var player in Players())
                if (player.TeamID == teamId) result.Add(player);
            return result;
        }

        public static List<Player> SearchPlayers(string keyword, int? teamId, string position)
        {
            var result = new List<Player>();
            foreach (var player in Players())
            {
                if (teamId.HasValue && player.TeamID != teamId.Value) continue;
                if (!string.IsNullOrEmpty(position) &&
                    !string.Equals(player.Position, position, StringComparison.OrdinalIgnoreCase)) continue;
                if (!string.IsNullOrEmpty(keyword) &&
                    player.PlayerName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) < 0 &&
                    player.Nickname.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                result.Add(player);
            }
            return result;
        }

        public static Player PlayerById(int id)
        {
            foreach (var player in Players())
                if (player.PlayerID == id) return player;
            return null;
        }

        public static List<Match> MatchesByTournament(int tournamentId)
        {
            var result = new List<Match>();
            foreach (var match in Matches())
                if (match.TournamentID == tournamentId) result.Add(match);
            return result;
        }

        public static Match MatchById(int id)
        {
            foreach (var match in Matches())
                if (match.MatchID == id) return match;
            return null;
        }

        public static List<Match> UpcomingMatches(int days)
        {
            var result = new List<Match>();
            var from = DateTime.Now.AddDays(-days);
            foreach (var match in Matches())
                if (match.MatchTime >= from) result.Add(match);
            return result;
        }

        public static List<Ranking> RankingsByTournament(int tournamentId)
        {
            var result = new List<Ranking>();
            var rank = 1;
            foreach (var team in TeamsByTournament(tournamentId))
            {
                result.Add(new Ranking
                {
                    Rank = rank,
                    TournamentID = tournamentId,
                    TeamID = team.TeamID,
                    TeamName = team.TeamName,
                    Wins = team.Wins,
                    Draws = 0,
                    Losses = team.Losses,
                    Points = team.Wins * 3,
                    GoalDiff = team.Wins - team.Losses,
                    UpdatedAt = DateTime.Now
                });
                rank++;
            }
            return result;
        }

        private static Account Account(int id, string username, string hash, string email, string name, string role)
        {
            var account = new Account
            {
                AccountID = id,
                Username = username,
                PasswordHash = hash,
                Email = email,
                FullName = name,
                IsLocked = false,
                CreatedAt = DateTime.Now.AddDays(-30)
            };
            account.Roles.Add(role);
            return account;
        }

        private static Team Team(int id, int tournamentId, string name, string shortName, string color,
            string gameType, string managerName, int playerCount, int wins, int losses, int rank)
        {
            var tournament = TournamentById(tournamentId);
            return new Team
            {
                TeamID = id,
                TournamentID = tournamentId,
                TeamName = name,
                ShortName = shortName,
                LogoColor = color,
                GameType = gameType,
                ManagerName = managerName,
                ManagerAccountID = managerName.IndexOf("1", StringComparison.Ordinal) >= 0 ? (int?)2 : 3,
                CreatedAt = DateTime.Now.AddDays(-7),
                IsActive = true,
                TournamentName = tournament == null ? "" : tournament.TournamentName,
                PlayerCount = playerCount,
                Wins = wins,
                Losses = losses,
                Rank = rank
            };
        }

        private static Player Player(int id, int teamId, string name, string nick, string position,
            string teamName, string teamShort, string teamColor)
        {
            return new Player
            {
                PlayerID = id,
                TeamID = teamId,
                PlayerName = name,
                Nickname = nick,
                Position = position,
                Country = "Vietnam",
                ContactInfo = nick.ToLower() + "@demo.local",
                CreatedAt = DateTime.Now.AddDays(-5),
                IsActive = true,
                TeamName = teamName,
                TeamShortName = teamShort,
                TeamLogoColor = teamColor
            };
        }

        private static List<Match> Matches()
        {
            var now = DateTime.Now;
            return new List<Match>
            {
                Match(1, 1, 1, 2, now.AddDays(-1), MatchStatus.DaKetThuc, "Group A", "BO3", 13, 9, 1),
                Match(2, 1, 3, 4, now.AddHours(6), MatchStatus.ChuaDienRa, "Group A", "BO3", null, null, null),
                Match(3, 1, 1, 3, now.AddDays(1), MatchStatus.ChuaDienRa, "Semi Final", "BO3", null, null, null),
                Match(4, 2, 5, 6, now.AddDays(8), MatchStatus.ChuaDienRa, "Final", "BO5", null, null, null),
                Match(5, 3, 7, 8, now.AddDays(-15), MatchStatus.DaKetThuc, "Final", "BO3", 16, 13, 7)
            };
        }

        private static Match Match(int id, int tournamentId, int team1Id, int team2Id, DateTime time,
            MatchStatus status, string round, string format, int? score1, int? score2, int? winnerId)
        {
            var t1 = TeamById(team1Id);
            var t2 = TeamById(team2Id);
            var winner = winnerId.HasValue ? TeamById(winnerId.Value) : null;
            return new Match
            {
                MatchID = id,
                TournamentID = tournamentId,
                Team1ID = team1Id,
                Team2ID = team2Id,
                MatchTime = time,
                Status = status,
                CreatedAt = DateTime.Now.AddDays(-3),
                RoundName = round,
                GroupName = "A",
                MatchFormat = format,
                Team1Name = t1 == null ? "" : t1.TeamName,
                Team2Name = t2 == null ? "" : t2.TeamName,
                Team1Short = t1 == null ? "" : t1.ShortName,
                Team2Short = t2 == null ? "" : t2.ShortName,
                Team1Color = t1 == null ? "#3b82f6" : t1.LogoColor,
                Team2Color = t2 == null ? "#ef4444" : t2.LogoColor,
                ScoreTeam1 = score1,
                ScoreTeam2 = score2,
                WinnerTeamID = winnerId,
                WinnerName = winner == null ? null : winner.TeamName,
                IsConfirmed = score1.HasValue
            };
        }
    }
}

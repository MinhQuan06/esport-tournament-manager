# Esport Tournament Management - ASP.NET Web Forms (3 lớp)

Đồ án CNPM - **Phần mềm Quản lý Giải đấu Esport** xây dựng theo kiến trúc 3 lớp:
- **DTO** (Entity classes) - dữ liệu thuần
- **DAL** (Data Access Layer) - ADO.NET với `SqlConnection`/`SqlCommand`
- **BLL** (Business Logic Layer) - kiểm tra nghiệp vụ, gọi DAL
- **GUI** (Web Forms) - giao diện ASP.NET Web Forms + Bootstrap 5



---

## 1. Yêu cầu hệ thống

| Thành phần | Phiên bản đề xuất |
|---|---|
| Visual Studio | 2019 / 2022 (workload "ASP.NET and web development") |
| .NET Framework | 4.7.2 |
| SQL Server | 2016+ (Express / Developer / LocalDB đều dùng được) |
| SSMS | 18+ (để chạy script SQL) |
| Trình duyệt | Chrome / Edge mới nhất |

---

## 2. Cài đặt CSDL

1. Mở **SQL Server Management Studio (SSMS)**, kết nối tới SQL Server của bạn (vd: `.\SQLEXPRESS`).
2. Mở và chạy **TUẦN TỰ** các file trong thư mục `Database/`:
   ```
   01_CreateDatabase.sql     -- Tạo DB + tables + 3 role mặc định
   02_Functions.sql          -- 4 function (Goal Diff, Schedule Conflict, ...)
   03_StoredProcedures.sql   -- SP_RecalcRanking, SP_EnterMatchResult, ...
   04_Triggers.sql           -- Trigger giới hạn 10 player, auto recalc BXH
   05_SeedData.sql           -- Dữ liệu mẫu + 4 tài khoản test
   ```


---

## 3. Cấu hình Connection String

Mở `EsportManagement.GUI/Web.config`, đổi `Server=` sao cho khớp với SQL Server của bạn:

```xml
<connectionStrings>
  <add name="EsportDB"
       connectionString="Server=.\SQLEXPRESS;Database=EsportTournamentDB;Integrated Security=True;TrustServerCertificate=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

## 5. Thêm Project Reference cho website (LẦN ĐẦU MỞ DỰ ÁN)

Vì `EsportManagement.GUI` là **Web Site Project** (không phải Web Application), Visual Studio cần được chỉ tay "lấy DLL từ đâu":

1. Trong Solution Explorer, **right-click** project `EsportManagement.GUI` → **Add** → **Reference...**
2. Chọn tab **Projects** (bên trái) → tick chọn cả 3:
   - `EsportManagement.DTO`
   - `EsportManagement.DAL`
   - `EsportManagement.BLL`
3. Bấm **OK**.
4. Build Solution (Ctrl+Shift+B). Visual Studio sẽ tự copy 3 file `.dll` vào `EsportManagement.GUI/Bin/`.

Sau bước này, F5 chạy bình thường.

> **Nếu gặp lỗi "type không tồn tại" khi build lần đầu**: chỉ cần build solution thêm 1 lần nữa - VS đôi khi cần 2 lần build để giải quyết dependency của Web Site.

---

## 4. Build và chạy

1. Mở `EsportManagement.sln` bằng Visual Studio.
2. **Build Solution** (Ctrl+Shift+B). Lần đầu VS có thể yêu cầu restore NuGet (không cần, dự án chỉ dùng .NET Framework refs sẵn có).
3. Chuột phải project **`EsportManagement.GUI`** -> **Set as Startup Project**.
4. Nhấn **F5** (Debug) hoặc **Ctrl+F5** (Run).
5. Trình duyệt sẽ tự mở `http://localhost:xxxxx/Default.aspx`.

---



---

## 6. Tác giả

- Đỗ Minh Quân - 52400030
- Lý Khôi Nguyên
- Trần Hữu Long

Đại học Tôn Đức Thắng - Khoa CNTT - Đồ án Công nghệ phần mềm 2026.

---

## 5. Cấu trúc dự án

```
EsportManagement/
├─ EsportManagement.sln
├─ Database/
│  ├─ 00_RunAll.sql
│  ├─ 01_CreateDatabase.sql       (8 bảng + 3 role mặc định)
│  ├─ 02_Functions.sql            (FN_GetGoalDifference, FN_IsScheduleConflict, ...)
│  ├─ 03_StoredProcedures.sql     (SP_RecalcRanking, SP_EnterMatchResult, ...)
│  ├─ 04_Triggers.sql             (TR_PreventTeamOverflow, TR_UpdateRankingAfterResult, ...)
│  └─ 05_SeedData.sql             (4 account + 3 tournament + 6 team + matches)
│
├─ EsportManagement.DTO/          (Class library)
│  ├─ Account.cs, Role.cs, AccountRole.cs
│  ├─ Tournament.cs, Team.cs, Player.cs
│  ├─ Match.cs, MatchResult.cs, Ranking.cs
│  └─ Enums.cs                    (TournamentStatus, MatchStatus, StatusHelper)
│
├─ EsportManagement.DAL/          (Class library, ref DTO)
│  ├─ DBHelper.cs                 (kết nối + ExecuteScalar/NonQuery/DataTable)
│  ├─ AccountDAL.cs, RoleDAL.cs
│  ├─ TournamentDAL.cs, TeamDAL.cs, PlayerDAL.cs
│  ├─ MatchDAL.cs, MatchResultDAL.cs (gọi SP)
│  └─ RankingDAL.cs               (gọi SP_GetTournamentRanking)
│
├─ EsportManagement.BLL/          (Class library, ref DTO + DAL)
│  ├─ PasswordHasher.cs           (SHA256)
│  ├─ AccountBLL.cs, RoleBLL.cs
│  ├─ TournamentBLL.cs            (validate ngày, status)
│  ├─ TeamBLL.cs                  (BR-TEAM-01,02; chống trùng tên)
│  ├─ PlayerBLL.cs                (BR-PLAYER-02 max 10)
│  ├─ MatchBLL.cs                 (FR-MATCH-06 đội phải khác nhau)
│  ├─ MatchResultBLL.cs
│  └─ RankingBLL.cs
│
└─ EsportManagement.GUI/          (Web Forms ASP.NET 4.7.2)
   ├─ Web.config
   ├─ Site.Master + Site.Master.cs (layout chung, navbar phân quyền)
   ├─ Login.aspx, Register.aspx, Default.aspx (dashboard)
   ├─ App_Code/AuthHelper.cs      (kiểm tra Session quyền)
   ├─ Content/Site.css            (style tùy chỉnh)
   ├─ Public/                     (trang công khai - viewer)
   │  ├─ Tournaments.aspx         (danh sách + tìm kiếm/lọc)
   │  ├─ Schedule.aspx            (lịch thi đấu)
   │  └─ Ranking.aspx             (BXH highlight top 3)
   ├─ Admin/                      (yêu cầu role Admin)
   │  ├─ Tournaments.aspx, Teams.aspx, Players.aspx
   │  ├─ Matches.aspx, Results.aspx (nhập/xác nhận kết quả)
   │  ├─ Accounts.aspx (khóa, reset pwd, gán manager)
   │  └─ Roles.aspx
   └─ Manager/                    (yêu cầu role TeamManager)
      ├─ MyTeams.aspx, MyPlayers.aspx
      └─ JoinTournament.aspx
```

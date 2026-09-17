using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CampusActivitiesManager.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Data
{
    public class AccountRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(ILogger<AccountRepository> logger)
        {
            _logger = logger;
        }

        private async Task Init()
        {
            if (_hasBeenInitialized)
                return;

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            try
            {
                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Account (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    StudentCode TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    PhoneNumber TEXT,
                    AvatarUrl TEXT,
                    Status INTEGER NOT NULL,
                    Role INTEGER NOT NULL,
                    ClassName TEXT NOT NULL,
                    AcademicYear TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL
                );";
                await createTableCmd.ExecuteNonQueryAsync();

                // Check if we have data, if not insert realistic seed accounts
                var countCmd = connection.CreateCommand();
                countCmd.CommandText = "SELECT COUNT(*) FROM Account;";
                var count = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

                if (count == 0)
                {
                    await SeedInitialAccountsAsync(connection);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating Account table or seeding data");
                throw;
            }

            _hasBeenInitialized = true;
        }

        private async Task SeedInitialAccountsAsync(SqliteConnection connection)
        {
            var seedAccounts = new List<Account>
            {
                new() { StudentCode = "20110001", FullName = "Nguyễn An Cương", Email = "cuong.na20@campus.edu.vn", PhoneNumber = "0987123456", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.LopTruong, ClassName = "D20CNTT1", AcademicYear = "K20" },
                new() { StudentCode = "20110002", FullName = "Trần Thị Mai Hương", Email = "huong.ttm20@campus.edu.vn", PhoneNumber = "0912345678", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.BanChuNhiem, ClassName = "D20CNTT1", AcademicYear = "K20" },
                new() { StudentCode = "20110003", FullName = "Lê Hoàng Long", Email = "long.lh20@campus.edu.vn", PhoneNumber = "0978901234", AvatarUrl = "", Status = AccountStatus.BaoLuu, Role = AccountRole.SinhVien, ClassName = "D20CNTT2", AcademicYear = "K20" },
                new() { StudentCode = "21110015", FullName = "Phạm Minh Đức", Email = "duc.pm21@campus.edu.vn", PhoneNumber = "0934567890", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.SinhVien, ClassName = "D21KHMT1", AcademicYear = "K21" },
                new() { StudentCode = "21110022", FullName = "Đỗ Thị Quỳnh Trang", Email = "trang.dtq21@campus.edu.vn", PhoneNumber = "0945678901", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.LopTruong, ClassName = "D21KTPM1", AcademicYear = "K21" },
                new() { StudentCode = "21110045", FullName = "Vũ Quốc Bảo", Email = "bao.vq21@campus.edu.vn", PhoneNumber = "0956789012", AvatarUrl = "", Status = AccountStatus.BiKhoa, Role = AccountRole.SinhVien, ClassName = "D21KHMT2", AcademicYear = "K21" },
                new() { StudentCode = "22110008", FullName = "Bùi Tuấn Anh", Email = "anh.bt22@campus.edu.vn", PhoneNumber = "0967890123", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.SinhVien, ClassName = "D22HTTT1", AcademicYear = "K22" },
                new() { StudentCode = "22110034", FullName = "Phan Hải Yến", Email = "yen.ph22@campus.edu.vn", PhoneNumber = "0978901235", AvatarUrl = "", Status = AccountStatus.BaoLuu, Role = AccountRole.SinhVien, ClassName = "D22ATTT1", AcademicYear = "K22" },
                new() { StudentCode = "22110056", FullName = "Trịnh Đình Khang", Email = "khang.td22@campus.edu.vn", PhoneNumber = "0989012345", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.BanChuNhiem, ClassName = "D22HTTT2", AcademicYear = "K22" },
                new() { StudentCode = "23110012", FullName = "Đặng Kim Oanh", Email = "oanh.dk23@campus.edu.vn", PhoneNumber = "0901234567", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.SinhVien, ClassName = "D23CNTT1", AcademicYear = "K23" },
                new() { StudentCode = "23110029", FullName = "Mai Văn Nam", Email = "nam.mv23@campus.edu.vn", PhoneNumber = "0912345679", AvatarUrl = "", Status = AccountStatus.BiKhoa, Role = AccountRole.SinhVien, ClassName = "D23KHMT1", AcademicYear = "K23" },
                new() { StudentCode = "23110078", FullName = "Hoàng Lan Anh", Email = "anh.hl23@campus.edu.vn", PhoneNumber = "0923456780", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.SinhVien, ClassName = "D23KTPM1", AcademicYear = "K23" },
                new() { StudentCode = "GV001", FullName = "TS. Nguyễn Thanh Sơn", Email = "son.nt@campus.edu.vn", PhoneNumber = "0934567891", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.GiangVien, ClassName = "Khoa CNTT", AcademicYear = "Cán bộ" },
                new() { StudentCode = "GV002", FullName = "ThS. Lê Thu Hà", Email = "ha.lt@campus.edu.vn", PhoneNumber = "0945678902", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.GiangVien, ClassName = "Khoa KHMT", AcademicYear = "Cán bộ" },
                new() { StudentCode = "ADM01", FullName = "Quản Trị Viên Hệ Thống", Email = "admin@campus.edu.vn", PhoneNumber = "0900000000", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.Admin, ClassName = "Phòng Đào Tạo", AcademicYear = "Quản trị" }
            };

            foreach (var acc in seedAccounts)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO Account (StudentCode, FullName, Email, PhoneNumber, AvatarUrl, Status, Role, ClassName, AcademicYear, CreatedAt)
                VALUES (@StudentCode, @FullName, @Email, @PhoneNumber, @AvatarUrl, @Status, @Role, @ClassName, @AcademicYear, @CreatedAt);";

                cmd.Parameters.AddWithValue("@StudentCode", acc.StudentCode);
                cmd.Parameters.AddWithValue("@FullName", acc.FullName);
                cmd.Parameters.AddWithValue("@Email", acc.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", acc.PhoneNumber);
                cmd.Parameters.AddWithValue("@AvatarUrl", acc.AvatarUrl);
                cmd.Parameters.AddWithValue("@Status", (int)acc.Status);
                cmd.Parameters.AddWithValue("@Role", (int)acc.Role);
                cmd.Parameters.AddWithValue("@ClassName", acc.ClassName);
                cmd.Parameters.AddWithValue("@AcademicYear", acc.AcademicYear);
                cmd.Parameters.AddWithValue("@CreatedAt", acc.CreatedAt.ToString("o"));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<(List<Account> Items, int TotalCount, int FilteredCount)> GetPagedAsync(int page, int pageSize, AccountFilterCriteria? criteria)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var whereClauses = new List<string>();
            var parameters = new List<SqliteParameter>();

            if (criteria != null)
            {
                // AC 1.2: Search by FullName, StudentCode, Email
                if (!string.IsNullOrWhiteSpace(criteria.SearchText))
                {
                    var searchPattern = $"%{criteria.SearchText.Trim()}%";
                    whereClauses.Add("(FullName LIKE @SearchText OR StudentCode LIKE @SearchText OR Email LIKE @SearchText)");
                    parameters.Add(new SqliteParameter("@SearchText", searchPattern));
                }

                // AC 1.3: Quick Filter by Status (Đang học, Bảo lưu, Bị khóa)
                if (criteria.SelectedStatus.HasValue)
                {
                    whereClauses.Add("Status = @Status");
                    parameters.Add(new SqliteParameter("@Status", (int)criteria.SelectedStatus.Value));
                }

                // AC 1.3: Advanced Filter by Class
                if (!string.IsNullOrWhiteSpace(criteria.SelectedClass) && criteria.SelectedClass != "Tất cả")
                {
                    whereClauses.Add("ClassName = @ClassName");
                    parameters.Add(new SqliteParameter("@ClassName", criteria.SelectedClass));
                }

                // AC 1.3: Advanced Filter by Academic Year
                if (!string.IsNullOrWhiteSpace(criteria.SelectedAcademicYear) && criteria.SelectedAcademicYear != "Tất cả")
                {
                    whereClauses.Add("AcademicYear = @AcademicYear");
                    parameters.Add(new SqliteParameter("@AcademicYear", criteria.SelectedAcademicYear));
                }

                // AC 1.3: Advanced Filter by Role
                if (criteria.SelectedRole.HasValue)
                {
                    whereClauses.Add("Role = @Role");
                    parameters.Add(new SqliteParameter("@Role", (int)criteria.SelectedRole.Value));
                }
            }

            var whereString = whereClauses.Count > 0 ? " WHERE " + string.Join(" AND ", whereClauses) : "";

            // Total count in database
            var totalCmd = connection.CreateCommand();
            totalCmd.CommandText = "SELECT COUNT(*) FROM Account;";
            var totalCount = Convert.ToInt32(await totalCmd.ExecuteScalarAsync());

            // Filtered count
            var countCmd = connection.CreateCommand();
            countCmd.CommandText = $"SELECT COUNT(*) FROM Account{whereString};";
            foreach (var p in parameters)
            {
                countCmd.Parameters.Add(new SqliteParameter(p.ParameterName, p.Value));
            }
            var filteredCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

            // Paged items
            var offset = Math.Max(0, (page - 1) * pageSize);
            var queryCmd = connection.CreateCommand();
            queryCmd.CommandText = $@"
                SELECT ID, StudentCode, FullName, Email, PhoneNumber, AvatarUrl, Status, Role, ClassName, AcademicYear, CreatedAt
                FROM Account
                {whereString}
                ORDER BY ID DESC
                LIMIT @Limit OFFSET @Offset;";

            foreach (var p in parameters)
            {
                queryCmd.Parameters.Add(new SqliteParameter(p.ParameterName, p.Value));
            }
            queryCmd.Parameters.AddWithValue("@Limit", pageSize);
            queryCmd.Parameters.AddWithValue("@Offset", offset);

            var items = new List<Account>();
            await using var reader = await queryCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new Account
                {
                    ID = reader.GetInt32(0),
                    StudentCode = reader.GetString(1),
                    FullName = reader.GetString(2),
                    Email = reader.GetString(3),
                    PhoneNumber = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    AvatarUrl = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Status = (AccountStatus)reader.GetInt32(6),
                    Role = (AccountRole)reader.GetInt32(7),
                    ClassName = reader.GetString(8),
                    AcademicYear = reader.GetString(9),
                    CreatedAt = DateTime.TryParse(reader.GetString(10), out var dt) ? dt : DateTime.Now
                });
            }

            return (items, totalCount, filteredCount);
        }

        public async Task<(int Total, int DangHocCount, int BaoLuuCount, int BiKhoaCount)> GetStatusCountsAsync(string? searchText = null)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var where = "";
            var searchParam = "";
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                where = " WHERE (FullName LIKE @Search OR StudentCode LIKE @Search OR Email LIKE @Search)";
                searchParam = $"%{searchText.Trim()}%";
            }

            var cmd = connection.CreateCommand();
            cmd.CommandText = $@"
                SELECT 
                    COUNT(*) as Total,
                    SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END) as DangHoc,
                    SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) as BaoLuu,
                    SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) as BiKhoa
                FROM Account{where};";

            if (!string.IsNullOrEmpty(searchParam))
            {
                cmd.Parameters.AddWithValue("@Search", searchParam);
            }

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int total = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                int dangHoc = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                int baoLuu = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                int biKhoa = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                return (total, dangHoc, baoLuu, biKhoa);
            }

            return (0, 0, 0, 0);
        }

        public async Task<List<string>> GetUniqueClassesAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT ClassName FROM Account WHERE ClassName != '' ORDER BY ClassName;";

            var list = new List<string>();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(reader.GetString(0));
            }
            return list;
        }

        public async Task<List<string>> GetUniqueAcademicYearsAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT AcademicYear FROM Account WHERE AcademicYear != '' ORDER BY AcademicYear;";

            var list = new List<string>();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(reader.GetString(0));
            }
            return list;
        }

        public async Task<Account?> GetAsync(int id)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT ID, StudentCode, FullName, Email, PhoneNumber, AvatarUrl, Status, Role, ClassName, AcademicYear, CreatedAt
                FROM Account
                WHERE ID = @id;";
            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Account
                {
                    ID = reader.GetInt32(0),
                    StudentCode = reader.GetString(1),
                    FullName = reader.GetString(2),
                    Email = reader.GetString(3),
                    PhoneNumber = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    AvatarUrl = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Status = (AccountStatus)reader.GetInt32(6),
                    Role = (AccountRole)reader.GetInt32(7),
                    ClassName = reader.GetString(8),
                    AcademicYear = reader.GetString(9),
                    CreatedAt = DateTime.TryParse(reader.GetString(10), out var dt) ? dt : DateTime.Now
                };
            }

            return null;
        }

        public async Task<int> SaveItemAsync(Account item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            if (item.ID == 0)
            {
                cmd.CommandText = @"
                INSERT INTO Account (StudentCode, FullName, Email, PhoneNumber, AvatarUrl, Status, Role, ClassName, AcademicYear, CreatedAt)
                VALUES (@StudentCode, @FullName, @Email, @PhoneNumber, @AvatarUrl, @Status, @Role, @ClassName, @AcademicYear, @CreatedAt);
                SELECT last_insert_rowid();";
            }
            else
            {
                cmd.CommandText = @"
                UPDATE Account
                SET StudentCode = @StudentCode, FullName = @FullName, Email = @Email, PhoneNumber = @PhoneNumber,
                    AvatarUrl = @AvatarUrl, Status = @Status, Role = @Role, ClassName = @ClassName, AcademicYear = @AcademicYear
                WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("@ID", item.ID);
            }

            cmd.Parameters.AddWithValue("@StudentCode", item.StudentCode);
            cmd.Parameters.AddWithValue("@FullName", item.FullName);
            cmd.Parameters.AddWithValue("@Email", item.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", item.PhoneNumber);
            cmd.Parameters.AddWithValue("@AvatarUrl", item.AvatarUrl);
            cmd.Parameters.AddWithValue("@Status", (int)item.Status);
            cmd.Parameters.AddWithValue("@Role", (int)item.Role);
            cmd.Parameters.AddWithValue("@ClassName", item.ClassName);
            cmd.Parameters.AddWithValue("@AcademicYear", item.AcademicYear);
            cmd.Parameters.AddWithValue("@CreatedAt", item.CreatedAt.ToString("o"));

            var result = await cmd.ExecuteScalarAsync();
            if (item.ID == 0)
            {
                item.ID = Convert.ToInt32(result);
            }

            return item.ID;
        }

        public async Task<int> UpdateStatusAsync(int id, AccountStatus status)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Account SET Status = @Status WHERE ID = @ID;";
            cmd.Parameters.AddWithValue("@Status", (int)status);
            cmd.Parameters.AddWithValue("@ID", id);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteItemAsync(Account item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Account WHERE ID = @ID;";
            cmd.Parameters.AddWithValue("@ID", item.ID);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task DropTableAsync()
        {
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DROP TABLE IF EXISTS Account;";
            await cmd.ExecuteNonQueryAsync();

            _hasBeenInitialized = false;
        }
    }
}

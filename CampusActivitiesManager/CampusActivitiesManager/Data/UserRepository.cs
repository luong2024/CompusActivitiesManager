using CampusActivitiesManager.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Data
{
    /// <summary>
    /// Repository class for managing Users and Role-Based Access Control in SQLite Database.
    /// Uses parameterized queries to prevent SQL Injection (NFR-01).
    /// </summary>
    public class UserRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ILogger<UserRepository> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Initializes the database connection, creates the User table if it does not exist,
        /// and seeds default users across all roles if table is empty.
        /// </summary>
        public async Task Init()
        {
            if (_hasBeenInitialized)
                return;

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            try
            {
                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS User (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT,
                        FullName TEXT NOT NULL,
                        Email TEXT NOT NULL,
                        Role TEXT NOT NULL DEFAULT 'User',
                        PhoneNumber TEXT,
                        Department TEXT,
                        IsActive INTEGER NOT NULL DEFAULT 1
                    );";
                await createTableCmd.ExecuteNonQueryAsync();

                // Check if user table is empty, if so seed initial users
                var countCmd = connection.CreateCommand();
                countCmd.CommandText = "SELECT COUNT(*) FROM User;";
                var userCount = Convert.ToInt64(await countCmd.ExecuteScalarAsync());

                if (userCount == 0)
                {
                    await SeedDefaultUsersInternalAsync(connection);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating User table");
                throw;
            }

            _hasBeenInitialized = true;
        }

        private async Task SeedDefaultUsersInternalAsync(SqliteConnection connection)
        {
            var seedUsers = new[]
            {
                new { Username = "admin", Password = "123", FullName = "Nguyễn Văn Quản Trị (Admin)", Email = "admin@campus.edu.vn", Role = "Admin", Phone = "0901234567", Dept = "Ban Giám hiệu & CNTT", Active = 1 },
                new { Username = "manager", Password = "123", FullName = "Lê Hoàng Quản Lý (Manager)", Email = "manager@campus.edu.vn", Role = "Manager", Phone = "0912345678", Dept = "Đoàn Thanh niên - Hội Sinh viên", Active = 1 },
                new { Username = "student1", Password = "123", FullName = "Trần Minh Đức (Student)", Email = "duc.tm@sinhvien.campus.edu.vn", Role = "User", Phone = "0987654321", Dept = "Khoa Công nghệ Thông tin", Active = 1 },
                new { Username = "student2", Password = "123", FullName = "Phạm Thu Hương (Student)", Email = "huong.pt@sinhvien.campus.edu.vn", Role = "User", Phone = "0976543210", Dept = "Khoa Kinh tế & Quản trị", Active = 1 },
                new { Username = "guest", Password = "123", FullName = "Khách Tham Quan (Guest)", Email = "guest@campus.edu.vn", Role = "Guest", Phone = "0934567890", Dept = "Khách vãng lai", Active = 1 }
            };

            foreach (var u in seedUsers)
            {
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = @"
                    INSERT INTO User (Username, PasswordHash, FullName, Email, Role, PhoneNumber, Department, IsActive)
                    VALUES (@Username, @PasswordHash, @FullName, @Email, @Role, @PhoneNumber, @Department, @IsActive);";
                insertCmd.Parameters.AddWithValue("@Username", u.Username);
                insertCmd.Parameters.AddWithValue("@PasswordHash", u.Password);
                insertCmd.Parameters.AddWithValue("@FullName", u.FullName);
                insertCmd.Parameters.AddWithValue("@Email", u.Email);
                insertCmd.Parameters.AddWithValue("@Role", u.Role);
                insertCmd.Parameters.AddWithValue("@PhoneNumber", u.Phone);
                insertCmd.Parameters.AddWithValue("@Department", u.Dept);
                insertCmd.Parameters.AddWithValue("@IsActive", u.Active);
                await insertCmd.ExecuteNonQueryAsync();
            }
        }

        public async Task SeedDefaultUsersAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();
            await SeedDefaultUsersInternalAsync(connection);
        }

        /// <summary>
        /// Retrieves all users from SQLite Database.
        /// </summary>
        public async Task<List<User>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT ID, Username, PasswordHash, FullName, Email, Role, PhoneNumber, Department, IsActive FROM User ORDER BY ID ASC";
            var users = new List<User>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(MapUserFromReader(reader));
            }

            return users;
        }

        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        public async Task<User?> GetAsync(int id)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT ID, Username, PasswordHash, FullName, Email, Role, PhoneNumber, Department, IsActive FROM User WHERE ID = @id";
            selectCmd.Parameters.AddWithValue("@id", id);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapUserFromReader(reader);
            }

            return null;
        }

        /// <summary>
        /// Retrieves a user by Username.
        /// </summary>
        public async Task<User?> GetByUsernameAsync(string username)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT ID, Username, PasswordHash, FullName, Email, Role, PhoneNumber, Department, IsActive FROM User WHERE Username = @username";
            selectCmd.Parameters.AddWithValue("@username", username);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapUserFromReader(reader);
            }

            return null;
        }

        /// <summary>
        /// Updates the role of a user in SQLite Database.
        /// </summary>
        public async Task<bool> UpdateRoleAsync(int id, Role role)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var updateCmd = connection.CreateCommand();
            updateCmd.CommandText = "UPDATE User SET Role = @Role WHERE ID = @ID";
            updateCmd.Parameters.AddWithValue("@Role", role.ToString());
            updateCmd.Parameters.AddWithValue("@ID", id);

            var rowsAffected = await updateCmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        /// <summary>
        /// Saves or updates a user in the database.
        /// </summary>
        public async Task<int> SaveItemAsync(User item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();
            if (item.IntId == 0)
            {
                saveCmd.CommandText = @"
                    INSERT INTO User (Username, PasswordHash, FullName, Email, Role, PhoneNumber, Department, IsActive)
                    VALUES (@Username, @PasswordHash, @FullName, @Email, @Role, @PhoneNumber, @Department, @IsActive);
                    SELECT last_insert_rowid();";
            }
            else
            {
                saveCmd.CommandText = @"
                    UPDATE User 
                    SET Username = @Username, PasswordHash = @PasswordHash, FullName = @FullName, 
                        Email = @Email, Role = @Role, PhoneNumber = @PhoneNumber, 
                        Department = @Department, IsActive = @IsActive
                    WHERE ID = @ID";
                saveCmd.Parameters.AddWithValue("@ID", item.IntId);
            }

            saveCmd.Parameters.AddWithValue("@Username", item.Username ?? string.Empty);
            saveCmd.Parameters.AddWithValue("@PasswordHash", item.PasswordHash ?? string.Empty);
            saveCmd.Parameters.AddWithValue("@FullName", item.FullName ?? string.Empty);
            saveCmd.Parameters.AddWithValue("@Email", item.Email ?? string.Empty);
            saveCmd.Parameters.AddWithValue("@Role", item.Role.ToString());
            saveCmd.Parameters.AddWithValue("@PhoneNumber", item.PhoneNumber ?? string.Empty);
            saveCmd.Parameters.AddWithValue("@Department", item.Department ?? string.Empty);
            saveCmd.Parameters.AddWithValue("@IsActive", item.IsActive ? 1 : 0);

            var result = await saveCmd.ExecuteScalarAsync();
            if (item.IntId == 0 && result != null)
            {
                item.IntId = Convert.ToInt32(result);
            }

            return item.IntId;
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        public async Task<bool> DeleteItemAsync(int id)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM User WHERE ID = @id";
            deleteCmd.Parameters.AddWithValue("@id", id);

            var rows = await deleteCmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        /// <summary>
        /// Toggles active status of a user (Khóa / Mở tài khoản).
        /// </summary>
        public async Task<bool> ToggleStatusAsync(int id)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var toggleCmd = connection.CreateCommand();
            toggleCmd.CommandText = "UPDATE User SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE ID = @id";
            toggleCmd.Parameters.AddWithValue("@id", id);

            var rows = await toggleCmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        /// <summary>
        /// Drops the User table.
        /// </summary>
        public async Task DropTableAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var dropCmd = connection.CreateCommand();
            dropCmd.CommandText = "DROP TABLE IF EXISTS User";
            await dropCmd.ExecuteNonQueryAsync();
            _hasBeenInitialized = false;
        }

        private static User MapUserFromReader(SqliteDataReader reader)
        {
            var roleStr = reader.IsDBNull(5) ? "User" : reader.GetString(5);
            if (!Enum.TryParse<Role>(roleStr, true, out var role))
            {
                role = Role.User;
            }

            return new User
            {
                Id = reader.GetInt32(0).ToString(),
                Username = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                PasswordHash = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                FullName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Email = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Role = role,
                PhoneNumber = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                Department = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                IsActive = !reader.IsDBNull(8) && reader.GetInt32(8) == 1
            };
        }
    }
}

using CampusActivitiesManager.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Data
{
    /// <summary>
    /// Repository class for managing users and role assignments in the SQLite database.
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
        /// Initializes the database connection and creates the User table if it does not exist.
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
                    FullName TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Role TEXT NOT NULL,
                    PhoneNumber TEXT,
                    Department TEXT
                );";
                await createTableCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating User table");
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        public async Task<List<User>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT ID, Username, FullName, Email, Role, PhoneNumber, Department FROM User ORDER BY ID ASC";
            var users = new List<User>();

            try
            {
                await using var reader = await selectCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        ID = reader.GetInt32(0),
                        Username = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        FullName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                        Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        Role = reader.IsDBNull(4) ? UserRoles.Student : reader.GetString(4),
                        PhoneNumber = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                        Department = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching user list");
                throw;
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
            selectCmd.CommandText = "SELECT ID, Username, FullName, Email, Role, PhoneNumber, Department FROM User WHERE ID = @id";
            selectCmd.Parameters.AddWithValue("@id", id);

            try
            {
                await using var reader = await selectCmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        ID = reader.GetInt32(0),
                        Username = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        FullName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                        Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        Role = reader.IsDBNull(4) ? UserRoles.Student : reader.GetString(4),
                        PhoneNumber = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                        Department = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching user by ID {ID}", id);
                throw;
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
            selectCmd.CommandText = "SELECT ID, Username, FullName, Email, Role, PhoneNumber, Department FROM User WHERE Username = @username COLLATE NOCASE";
            selectCmd.Parameters.AddWithValue("@username", username);

            try
            {
                await using var reader = await selectCmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        ID = reader.GetInt32(0),
                        Username = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        FullName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                        Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        Role = reader.IsDBNull(4) ? UserRoles.Student : reader.GetString(4),
                        PhoneNumber = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                        Department = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching user by username {Username}", username);
                throw;
            }

            return null;
        }

        /// <summary>
        /// Updates the role/permissions for a user in the database.
        /// Fulfills the Acceptance Criteria: Hệ thống cho phép tài khoản Admin cập nhật vai trò/quyền hạn cho tài khoản khác và lưu thành công vào CSDL.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="newRole">The new role to assign (Admin, Manager, Student).</param>
        public async Task<bool> UpdateRoleAsync(int userId, string newRole)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var updateCmd = connection.CreateCommand();
            updateCmd.CommandText = "UPDATE User SET Role = @role WHERE ID = @id";
            updateCmd.Parameters.AddWithValue("@role", newRole);
            updateCmd.Parameters.AddWithValue("@id", userId);

            try
            {
                int rowsAffected = await updateCmd.ExecuteNonQueryAsync();
                _logger.LogInformation("Updated role of User ID {UserID} to {NewRole}. Rows affected: {Rows}", userId, newRole, rowsAffected);
                return rowsAffected > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating role for User ID {UserID}", userId);
                throw;
            }
        }

        /// <summary>
        /// Saves or updates a user item in the database.
        /// </summary>
        public async Task SaveItemAsync(User item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();
            if (item.ID == 0)
            {
                saveCmd.CommandText = @"
                INSERT INTO User (Username, FullName, Email, Role, PhoneNumber, Department)
                VALUES (@username, @fullname, @email, @role, @phone, @department);
                SELECT last_insert_rowid();";
            }
            else
            {
                saveCmd.CommandText = @"
                UPDATE User 
                SET Username = @username, FullName = @fullname, Email = @email, Role = @role, PhoneNumber = @phone, Department = @department
                WHERE ID = @id;";
                saveCmd.Parameters.AddWithValue("@id", item.ID);
            }

            saveCmd.Parameters.AddWithValue("@username", item.Username);
            saveCmd.Parameters.AddWithValue("@fullname", item.FullName);
            saveCmd.Parameters.AddWithValue("@email", item.Email);
            saveCmd.Parameters.AddWithValue("@role", item.Role);
            saveCmd.Parameters.AddWithValue("@phone", item.PhoneNumber ?? string.Empty);
            saveCmd.Parameters.AddWithValue("@department", item.Department ?? string.Empty);

            try
            {
                if (item.ID == 0)
                {
                    var result = await saveCmd.ExecuteScalarAsync();
                    if (result is not null && int.TryParse(result.ToString(), out int newId))
                    {
                        item.ID = newId;
                    }
                }
                else
                {
                    await saveCmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving User {Username}", item.Username);
                throw;
            }
        }

        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        public async Task DeleteItemAsync(User item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM User WHERE ID = @id";
            deleteCmd.Parameters.AddWithValue("@id", item.ID);

            try
            {
                await deleteCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting User ID {ID}", item.ID);
                throw;
            }
        }

        /// <summary>
        /// Drops the User table.
        /// </summary>
        public async Task DropTableAsync()
        {
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var dropTableCmd = connection.CreateCommand();
            dropTableCmd.CommandText = "DROP TABLE IF EXISTS User";

            try
            {
                await dropTableCmd.ExecuteNonQueryAsync();
                _hasBeenInitialized = false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error dropping User table");
                throw;
            }
        }

        /// <summary>
        /// Seeds default users representing all 3 roles (Admin, Manager, Student) if none exist.
        /// </summary>
        public async Task SeedDefaultUsersAsync()
        {
            await Init();
            var existingUsers = await ListAsync();
            if (existingUsers.Count > 0)
                return;

            var defaultUsers = new List<User>
            {
                new User
                {
                    Username = "admin",
                    FullName = "Nguyễn Văn An",
                    Email = "admin@campus.edu.vn",
                    Role = UserRoles.Admin,
                    PhoneNumber = "0901234567",
                    Department = "Ban Quản Trị Hệ Thống"
                },
                new User
                {
                    Username = "manager1",
                    FullName = "Trần Thị Bình",
                    Email = "binh.tran@campus.edu.vn",
                    Role = UserRoles.Manager,
                    PhoneNumber = "0912345678",
                    Department = "Đoàn Thanh Niên & Hội Sinh Viên"
                },
                new User
                {
                    Username = "student1",
                    FullName = "Lê Hoàng Cường",
                    Email = "cuong.le@student.campus.edu.vn",
                    Role = UserRoles.Student,
                    PhoneNumber = "0923456789",
                    Department = "Khoa Công Nghệ Thông Tin - K21"
                },
                new User
                {
                    Username = "student2",
                    FullName = "Phạm Minh Đức",
                    Email = "duc.pham@student.campus.edu.vn",
                    Role = UserRoles.Student,
                    PhoneNumber = "0934567890",
                    Department = "Khoa Kinh Tế Đối Ngoại - K22"
                }
            };

            foreach (var user in defaultUsers)
            {
                await SaveItemAsync(user);
            }

            _logger.LogInformation("Seeded default users successfully.");
        }
    }
}

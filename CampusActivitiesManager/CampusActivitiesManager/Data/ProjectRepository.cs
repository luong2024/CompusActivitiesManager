using CampusActivitiesManager.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Data
{
    /// <summary>
    /// Repository class for managing projects in the local SQLite database.
    /// </summary>
    public class ProjectRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly TaskRepository _taskRepository;
        private readonly TagRepository _tagRepository;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectRepository"/> class.
        /// </summary>
        public ProjectRepository(TaskRepository taskRepository, TagRepository tagRepository, ILogger<ProjectRepository> logger)
        {
            _taskRepository = taskRepository;
            _tagRepository = tagRepository;
            _logger = logger;
        }

        /// <summary>
        /// Initializes the database connection and creates the Project table if it does not exist.
        /// </summary>
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
            CREATE TABLE IF NOT EXISTS Project (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                Icon TEXT,
                CategoryID INTEGER NOT NULL
            );";
                await createTableCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating Project table");
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves a list of all projects from the database.
        /// </summary>
        public async Task<List<Project>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM Project";
            var projects = new List<Project>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                projects.Add(new Project
                {
                    ID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    Icon = reader.GetString(3),
                    CategoryID = reader.GetInt32(4)
                });
            }

            foreach (var project in projects)
            {
                project.Tags = await _tagRepository.ListAsync(project.ID);
                project.Tasks = await _taskRepository.ListAsync(project.ID);
            }

            return projects;
        }

        /// <summary>
        /// Retrieves a specific project by its ID.
        /// </summary>
        public async Task<Project?> GetAsync(int id)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM Project WHERE ID = @ID";
            selectCmd.Parameters.AddWithValue("@ID", id);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var project = new Project
                {
                    ID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    Icon = reader.GetString(3),
                    CategoryID = reader.GetInt32(4)
                };

                project.Tags = await _tagRepository.ListAsync(project.ID);
                project.Tasks = await _taskRepository.ListAsync(project.ID);

                return project;
            }

            return null;
        }

        /// <summary>
        /// Saves a project to the local database.
        /// </summary>
        public async Task<int> SaveItemAsync(Project item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();
            if (item.ID == 0)
            {
                saveCmd.CommandText = @"
                INSERT INTO Project (Name, Description, Icon, CategoryID)
                VALUES (@Name, @Description, @Icon, @CategoryID);
                SELECT last_insert_rowid();";
            }
            else
            {
                saveCmd.CommandText = @"
                UPDATE Project
                SET Name = @Name, Description = @Description, Icon = @Icon, CategoryID = @CategoryID
                WHERE ID = @ID";
                saveCmd.Parameters.AddWithValue("@ID", item.ID);
            }

            saveCmd.Parameters.AddWithValue("@Name", item.Name);
            saveCmd.Parameters.AddWithValue("@Description", item.Description ?? string.Empty);
            saveCmd.Parameters.AddWithValue("@Icon", item.Icon ?? "📅");
            saveCmd.Parameters.AddWithValue("@CategoryID", item.CategoryID);

            var dbResult = await saveCmd.ExecuteScalarAsync();
            if (item.ID == 0)
            {
                item.ID = Convert.ToInt32(dbResult);
            }

            return item.ID;
        }

        /// <summary>
        /// Deletes a project from the database.
        /// </summary>
        public async Task<int> DeleteItemAsync(Project item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM Project WHERE ID = @ID";
            deleteCmd.Parameters.AddWithValue("@ID", item.ID);

            return await deleteCmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Deletes all projects from the database.
        /// </summary>
        public async Task<int> DropTableAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM Project";

            return await deleteCmd.ExecuteNonQueryAsync();
        }
    }
}

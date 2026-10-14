using CampusActivitiesManager.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Data
{
    /// <summary>
    /// Repository class for managing categories in the database.
    /// </summary>
    public class CategoryRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public CategoryRepository(ILogger<CategoryRepository> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Initializes the database connection and creates the Category table if it does not exist.
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
            CREATE TABLE IF NOT EXISTS Category (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Color TEXT NOT NULL
            );";
                await createTableCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating Category table");
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves a list of all categories from the database.
        /// </summary>
        /// <returns>A list of <see cref="Category"/> objects.</returns>
        public async Task<List<Category>> ListAsync()
        {
            try
            {
                using var httpClient = CampusActivitiesManager.Services.HttpHelper.CreateApiClient();
                var response = await httpClient.GetAsync(Constants.CategoriesApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = await System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync<CampusActivitiesManager.Services.ApiResponse<List<Category>>>(response.Content, options);
                    if (result != null && result.Success && result.Data != null)
                        return result.Data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error fetching categories from API, falling back to SQLite");
            }

            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM Category";
            var categories = new List<Category>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                categories.Add(new Category
                {
                    ID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Color = reader.GetString(2)
                });
            }

            return categories;
        }

        /// <summary>
        /// Retrieves a specific category by its ID.
        /// </summary>
        public async Task<Category?> GetAsync(int id)
        {
            try
            {
                using var httpClient = CampusActivitiesManager.Services.HttpHelper.CreateApiClient();
                var response = await httpClient.GetAsync($"{Constants.CategoriesApiUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = await System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync<CampusActivitiesManager.Services.ApiResponse<Category>>(response.Content, options);
                    if (result != null && result.Success && result.Data != null)
                        return result.Data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error fetching category by ID from API, falling back to SQLite");
            }

            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM Category WHERE ID = @id";
            selectCmd.Parameters.AddWithValue("@id", id);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Category
                {
                    ID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Color = reader.GetString(2)
                };
            }

            return null;
        }

        /// <summary>
        /// Saves a category to the Firebase API and database.
        /// </summary>
        public async Task<int> SaveItemAsync(Category item)
        {
            try
            {
                using var httpClient = CampusActivitiesManager.Services.HttpHelper.CreateApiClient();
                var payload = new
                {
                    title = item.Title,
                    color = string.IsNullOrWhiteSpace(item.Color) ? "#2563EB" : item.Color
                };

                HttpResponseMessage response;
                if (item.ID == 0)
                {
                    response = await System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync(httpClient, Constants.CategoriesApiUrl, payload);
                }
                else
                {
                    response = await System.Net.Http.Json.HttpClientJsonExtensions.PutAsJsonAsync(httpClient, $"{Constants.CategoriesApiUrl}/{item.ID}", payload);
                }

                if (response.IsSuccessStatusCode)
                {
                    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = await System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync<CampusActivitiesManager.Services.ApiResponse<Category>>(response.Content, options);
                    if (result?.Data != null && result.Data.ID > 0)
                    {
                        item.ID = result.Data.ID;
                        return item.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error saving category to API, falling back to SQLite");
            }

            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();
            if (item.ID == 0)
            {
                saveCmd.CommandText = @"
                INSERT INTO Category (Title, Color)
                VALUES (@Title, @Color);
                SELECT last_insert_rowid();";
            }
            else
            {
                saveCmd.CommandText = @"
                UPDATE Category SET Title = @Title, Color = @Color
                WHERE ID = @ID";
                saveCmd.Parameters.AddWithValue("@ID", item.ID);
            }

            saveCmd.Parameters.AddWithValue("@Title", item.Title);
            saveCmd.Parameters.AddWithValue("@Color", item.Color);

            var dbResult = await saveCmd.ExecuteScalarAsync();
            if (item.ID == 0)
            {
                item.ID = Convert.ToInt32(dbResult);
            }

            return item.ID;
        }

        /// <summary>
        /// Deletes a category from the Firebase API and database.
        /// </summary>
        public async Task<int> DeleteItemAsync(Category item)
        {
            try
            {
                using var httpClient = CampusActivitiesManager.Services.HttpHelper.CreateApiClient();
                var response = await httpClient.DeleteAsync($"{Constants.CategoriesApiUrl}/{item.ID}");
                if (response.IsSuccessStatusCode)
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error deleting category from API, falling back to SQLite");
            }

            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM Category WHERE ID = @id";
            deleteCmd.Parameters.AddWithValue("@id", item.ID);

            return await deleteCmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Drops the Category table from the database.
        /// </summary>
        public async Task DropTableAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var dropTableCmd = connection.CreateCommand();
            dropTableCmd.CommandText = "DROP TABLE IF EXISTS Category";

            await dropTableCmd.ExecuteNonQueryAsync();
            _hasBeenInitialized = false;
        }
    }
}
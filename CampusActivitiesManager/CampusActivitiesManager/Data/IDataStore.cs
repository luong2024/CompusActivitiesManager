namespace CampusActivitiesManager.Data
{
    /// <summary>
    /// Generic Interface định nghĩa các thao tác CRUDL cơ bản với cơ sở dữ liệu.
    /// </summary>
    /// <typeparam name="T">Kiểu thực thể dữ liệu</typeparam>
    public interface IDataStore<T>
    {
        Task<List<T>> GetItemsAsync();
        Task<T?> GetItemAsync(string id);
        Task<int> SaveItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
    }
}

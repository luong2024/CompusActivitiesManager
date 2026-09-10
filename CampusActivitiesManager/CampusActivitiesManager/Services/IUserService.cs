using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampusActivitiesManager.Services
{
    /// <summary>
    /// Generic Interface định nghĩa các thao tác CRUD bất đồng bộ cho User / Account
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu thực thể (ví dụ: Account)</typeparam>
    public interface IUserService<T> where T : class
    {
        /// <summary>
        /// Lấy toàn bộ danh sách thực thể từ API: GET /api/accounts
        /// </summary>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Lấy chi tiết một thực thể theo ID từ API: GET /api/accounts/{id}
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Thêm mới một thực thể qua API: POST /api/accounts
        /// </summary>
        Task<T> CreateAsync(T item);

        /// <summary>
        /// Cập nhật thông tin thực thể qua API: PUT /api/accounts/{id}
        /// </summary>
        Task<bool> UpdateAsync(int id, T item);

        /// <summary>
        /// Xóa một thực thể qua API: DELETE /api/accounts/{id}
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}

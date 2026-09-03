using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    /// <summary>
    /// Service hiện thực hóa IUserService, IDataStore phục vụ quản lý người dùng và phân quyền RBAC.
    /// </summary>
    public class UserService : IUserService, IDataStore<User>
    {
        private readonly UserRepository _userRepository;
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<UserService> _logger;

        public UserService(UserRepository userRepository, ModalErrorHandler errorHandler, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _errorHandler = errorHandler;
            _logger = logger;
        }

        public async Task<List<User>> GetUsersListAsync()
        {
            try
            {
                return await _userRepository.ListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách người dùng");
                _errorHandler.HandleError(ex);
                return [];
            }
        }

        public async Task<List<User>> GetItemsAsync() => await GetUsersListAsync();

        public async Task<User?> GetUserByIdAsync(string id)
        {
            try
            {
                if (int.TryParse(id, out var intId))
                {
                    return await _userRepository.GetAsync(intId);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin người dùng {Id}", id);
                _errorHandler.HandleError(ex);
                return null;
            }
        }

        public async Task<User?> GetItemAsync(string id) => await GetUserByIdAsync(id);

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _userRepository.GetByUsernameAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm người dùng theo username {Username}", username);
                _errorHandler.HandleError(ex);
                return null;
            }
        }

        public async Task<bool> UpdateUserRoleAsync(string id, Role newRole)
        {
            try
            {
                if (int.TryParse(id, out var intId))
                {
                    return await _userRepository.UpdateRoleAsync(intId, newRole);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật vai trò cho người dùng {Id}", id);
                _errorHandler.HandleError(ex);
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                if (int.TryParse(id, out var intId))
                {
                    return await _userRepository.DeleteItemAsync(intId);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa người dùng {Id}", id);
                _errorHandler.HandleError(ex);
                return false;
            }
        }

        public async Task<bool> DeleteItemAsync(string id) => await DeleteUserAsync(id);

        public async Task<int> SaveUserAsync(User user)
        {
            try
            {
                return await _userRepository.SaveItemAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu người dùng {Username}", user?.Username);
                _errorHandler.HandleError(ex);
                return 0;
            }
        }

        public async Task<int> SaveItemAsync(User item) => await SaveUserAsync(item);

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var id = await _userRepository.SaveItemAsync(user);
                return id > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật người dùng {Username}", user?.Username);
                _errorHandler.HandleError(ex);
                return false;
            }
        }

        public async Task<bool> UpdateItemAsync(User item) => await UpdateUserAsync(item);

        public async Task<bool> ToggleUserStatusAsync(string id)
        {
            try
            {
                if (int.TryParse(id, out var intId))
                {
                    return await _userRepository.ToggleStatusAsync(intId);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi trạng thái người dùng {Id}", id);
                _errorHandler.HandleError(ex);
                return false;
            }
        }

        public async Task SeedDefaultUsersAsync()
        {
            try
            {
                await _userRepository.SeedDefaultUsersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi nạp dữ liệu người dùng mẫu");
                _errorHandler.HandleError(ex);
            }
        }
    }
}

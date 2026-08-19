using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    public interface IAuthService
    {
        User? CurrentUser { get; }
        bool IsAdmin { get; }
        bool IsManager { get; }
        bool IsStudent { get; }
        bool CanManageRoles { get; }
        bool CanManageProjects { get; }
        bool CanManageMeta { get; }

        event EventHandler? CurrentUserChanged;

        Task InitializeAsync();
        Task SwitchUserAsync(User user);
        Task<bool> LoginAsync(string username);
        Task RefreshCurrentUserAsync();
    }

    public class AuthService : IAuthService
    {
        private const string CurrentUserPrefKey = "current_logged_in_username";
        private readonly UserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;
        private User? _currentUser;

        public User? CurrentUser => _currentUser;

        public bool IsAdmin => _currentUser?.IsAdmin ?? false;
        public bool IsManager => _currentUser?.IsManager ?? false;
        public bool IsStudent => _currentUser?.IsStudent ?? false;

        public bool CanManageRoles => IsAdmin;
        public bool CanManageProjects => IsAdmin || IsManager;
        public bool CanManageMeta => IsAdmin;

        public event EventHandler? CurrentUserChanged;

        public AuthService(UserRepository userRepository, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _userRepository.SeedDefaultUsersAsync();

                string savedUsername = Preferences.Default.Get(CurrentUserPrefKey, "admin");
                var user = await _userRepository.GetByUsernameAsync(savedUsername);

                if (user == null)
                {
                    var allUsers = await _userRepository.ListAsync();
                    user = allUsers.FirstOrDefault(u => u.Role == UserRoles.Admin) ?? allUsers.FirstOrDefault();
                }

                _currentUser = user;
                CurrentUserChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error initializing AuthService");
            }
        }

        public async Task SwitchUserAsync(User user)
        {
            if (user == null) return;

            // Fetch fresh copy from database
            var freshUser = await _userRepository.GetAsync(user.ID) ?? user;
            _currentUser = freshUser;
            Preferences.Default.Set(CurrentUserPrefKey, freshUser.Username);

            CurrentUserChanged?.Invoke(this, EventArgs.Empty);
            _logger.LogInformation("Switched current user to {Username} with Role: {Role}", freshUser.Username, freshUser.Role);
        }

        public async Task<bool> LoginAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user != null)
            {
                await SwitchUserAsync(user);
                return true;
            }
            return false;
        }

        public async Task RefreshCurrentUserAsync()
        {
            if (_currentUser != null)
            {
                var refreshed = await _userRepository.GetAsync(_currentUser.ID);
                if (refreshed != null)
                {
                    _currentUser = refreshed;
                    CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}

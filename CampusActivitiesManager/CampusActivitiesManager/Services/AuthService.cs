using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using Microsoft.Maui.Storage;

namespace CampusActivitiesManager.Services
{
    public class AuthService
    {
        private const string TokenKey = "auth_jwt_token";
        private const string UserKey = "auth_current_user";
        private const string RememberMeKey = "auth_remember_me";

        private readonly AccountRepository _accountRepository;
        private Account? _currentUser;

        public Account? CurrentUser => _currentUser;
        public bool IsLoggedIn => _currentUser != null;

        public AuthService(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<bool> CheckAutoLoginAsync()
        {
            try
            {
                var rememberMe = await SecureStorage.GetAsync(RememberMeKey);
                if (rememberMe == "true")
                {
                    var userJson = await SecureStorage.GetAsync(UserKey);
                    if (!string.IsNullOrEmpty(userJson))
                    {
                        var user = JsonSerializer.Deserialize<Account>(userJson);
                        if (user != null)
                        {
                            // Verify user is not locked in database
                            var dbUser = await _accountRepository.GetAsync(user.ID);
                            if (dbUser != null && dbUser.Status != AccountStatus.BiKhoa)
                            {
                                _currentUser = dbUser;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Fallback if secure storage error
            }
            return false;
        }

        public async Task<(bool Success, string Message, Account? User)> LoginAsync(string usernameOrEmail, string password, bool rememberMe = false)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                return (false, "Vui lòng nhập đầy đủ tên đăng nhập/email và mật khẩu.", null);
            }

            // Look up account in database
            var (accounts, _, _) = await _accountRepository.GetPagedAsync(1, 100, new AccountFilterCriteria
            {
                SearchText = usernameOrEmail.Trim()
            });

            var account = accounts.Find(a =>
                string.Equals(a.Email, usernameOrEmail.Trim(), StringComparison.OrdinalIgnoreCase) ||
                string.Equals(a.StudentCode, usernameOrEmail.Trim(), StringComparison.OrdinalIgnoreCase));

            // If account doesn't exist yet, for demo/seed ease if user is typing admin or student:
            if (account == null)
            {
                if (usernameOrEmail.Trim().ToLower() == "admin" || usernameOrEmail.Trim().ToLower() == "admin@campus.edu.vn")
                {
                    account = new Account
                    {
                        FullName = "Quản Trị Viên Hệ Thống",
                        StudentCode = "ADMIN01",
                        Email = "admin@campus.edu.vn",
                        Role = AccountRole.Admin,
                        Status = AccountStatus.DangHoc,
                        ClassName = "Phòng Quản trị",
                        AcademicYear = "BGH"
                    };
                    await _accountRepository.SaveItemAsync(account);
                }
                else if (usernameOrEmail.Trim().ToLower() == "student" || 
                         usernameOrEmail.Trim().ToLower() == "cuong.na@campus.edu.vn" || 
                         usernameOrEmail.Trim().ToLower() == "student@campus.edu.vn" ||
                         usernameOrEmail.Trim().ToLower() == "cuong.na" ||
                         usernameOrEmail.Trim().ToLower() == "b20dccn001")
                {
                    account = new Account
                    {
                        FullName = "Nguyễn An Cương",
                        StudentCode = "B20DCCN001",
                        Email = "cuong.na@campus.edu.vn",
                        PhoneNumber = "0987123456",
                        Role = AccountRole.LopTruong,
                        Status = AccountStatus.DangHoc,
                        ClassName = "D20CNTT1",
                        AcademicYear = "K20",
                        TrainingPoints = 92
                    };
                    await _accountRepository.SaveItemAsync(account);
                }
                else
                {
                    return (false, "Tên đăng nhập hoặc mật khẩu không chính xác.", null);
                }
            }

            // Check if account is locked (AC 28.4.5 & AC 05.4.3)
            if (account.Status == AccountStatus.BiKhoa)
            {
                return (false, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Quản trị viên để được hỗ trợ.", null);
            }

            _currentUser = account;

            // Generate mock JWT token
            string token = GenerateJwtToken(account);
            await SecureStorage.SetAsync(TokenKey, token);

            if (rememberMe)
            {
                await SecureStorage.SetAsync(RememberMeKey, "true");
                await SecureStorage.SetAsync(UserKey, JsonSerializer.Serialize(account));
            }
            else
            {
                SecureStorage.Remove(RememberMeKey);
                SecureStorage.Remove(UserKey);
            }

            return (true, "Đăng nhập thành công!", account);
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string fullName, string email, string password, string className = "D20CNTT1")
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return (false, "Họ và tên không được để trống.");

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return (false, "Địa chỉ email không đúng định dạng.");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                return (false, "Mật khẩu phải có độ dài tối thiểu 6 ký tự.");

            // Check if email already exists
            var (existing, _, count) = await _accountRepository.GetPagedAsync(1, 10, new AccountFilterCriteria
            {
                SearchText = email.Trim()
            });

            if (existing.Exists(a => string.Equals(a.Email, email.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Địa chỉ Email này đã được đăng ký trong hệ thống.");
            }

            // Generate Student code from email prefix or timestamp
            string studentCode = email.Split('@')[0].ToUpper();
            if (studentCode.Length > 10) studentCode = studentCode[..10];

            var newAccount = new Account
            {
                FullName = fullName.Trim(),
                Email = email.Trim().ToLower(),
                StudentCode = studentCode,
                ClassName = className,
                AcademicYear = "K20",
                Status = AccountStatus.DangHoc,
                Role = AccountRole.SinhVien,
                CreatedAt = DateTime.Now
            };

            await _accountRepository.SaveItemAsync(newAccount);
            return (true, "Đăng ký tài khoản thành công! Vui lòng đăng nhập.");
        }

        public async Task LogoutAsync()
        {
            _currentUser = null;
            SecureStorage.Remove(TokenKey);
            SecureStorage.Remove(UserKey);
            SecureStorage.Remove(RememberMeKey);
            await Task.CompletedTask;
        }

        public bool CanLockAccount(Account targetAccount)
        {
            // AC 05.4.3: Admin cannot lock their own account
            if (_currentUser != null && _currentUser.ID == targetAccount.ID)
            {
                return false;
            }
            return true;
        }

        private static string GenerateJwtToken(Account account)
        {
            var header = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"alg\":\"HS256\",\"typ\":\"JWT\"}"));
            var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                $"{{\"id\":{account.ID},\"email\":\"{account.Email}\",\"role\":\"{account.Role}\",\"exp\":{DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds()}}}"
            ));
            var signature = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes($"{header}.{payload}.campus_secret_key")));
            return $"{header}.{payload}.{signature}";
        }
    }
}

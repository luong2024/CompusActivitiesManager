using CampusActivitiesManager.Api.Models;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CampusActivitiesManager.Api.Services
{
    public class FirebaseAccountService : IFirebaseAccountService
    {
        private readonly FirebaseAuth? _firebaseAuth;
        private readonly FirestoreDb? _firestoreDb;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FirebaseAccountService> _logger;

        public FirebaseAccountService(IConfiguration configuration, ILogger<FirebaseAccountService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            try
            {
                _firebaseAuth = FirebaseAuth.DefaultInstance;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "FirebaseAuth.DefaultInstance could not be obtained.");
                _firebaseAuth = null;
            }

            try
            {
                string projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT") ?? "campusacmanage";
                _firestoreDb = FirestoreDb.Create(projectId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "FirestoreDb could not be initialized.");
                _firestoreDb = null;
            }
        }

        public async Task<UserAccountDto> CreateAccountAsync(CreateAccountRequest request)
        {
            if (_firebaseAuth == null)
            {
                throw new InvalidOperationException("Firebase Auth is not initialized.");
            }

            var userArgs = new UserRecordArgs
            {
                Email = request.Email,
                Password = request.Password,
                DisplayName = request.FullName,
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber
            };

            UserRecord userRecord = await _firebaseAuth.CreateUserAsync(userArgs);

            if (_firestoreDb != null)
            {
                try
                {
                    DocumentReference docRef = _firestoreDb.Collection("users").Document(userRecord.Uid);
                    await docRef.SetAsync(new
                    {
                        email = request.Email,
                        fullName = request.FullName,
                        role = request.Role,
                        phoneNumber = request.PhoneNumber,
                        studentCode = request.StudentCode,
                        isActive = true,
                        isLocked = false,
                        createdAt = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to persist user profile in Firestore for UID {Uid}", userRecord.Uid);
                }
            }

            return new UserAccountDto
            {
                Id = userRecord.Uid,
                Email = request.Email,
                FullName = request.FullName,
                Role = request.Role,
                PhoneNumber = request.PhoneNumber,
                StudentCode = request.StudentCode,
                IsDisabled = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
        }

        public async Task<UserAccountDto?> GetAccountByIdAsync(string id)
        {
            if (_firebaseAuth == null) return null;

            try
            {
                UserRecord user = await _firebaseAuth.GetUserAsync(id);
                string role = "Student";
                string? studentCode = null;
                string? avatarUrl = null;
                bool isActive = !user.Disabled;

                if (_firestoreDb != null)
                {
                    try
                    {
                        var doc = await _firestoreDb.Collection("users").Document(id).GetSnapshotAsync();
                        if (doc.Exists)
                        {
                            var dict = doc.ToDictionary();
                            if (dict.TryGetValue("role", out var r) && r != null) role = r.ToString()!;
                            if (dict.TryGetValue("studentCode", out var sc) && sc != null) studentCode = sc.ToString();
                            if (dict.TryGetValue("avatarUrl", out var av) && av != null) avatarUrl = av.ToString();
                            if (dict.TryGetValue("isActive", out var act) && act is bool b) isActive = b;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not load Firestore metadata for UID {Uid}", id);
                    }
                }

                return new UserAccountDto
                {
                    Id = user.Uid,
                    Email = user.Email,
                    FullName = user.DisplayName ?? string.Empty,
                    Role = role,
                    PhoneNumber = user.PhoneNumber,
                    StudentCode = studentCode,
                    AvatarUrl = avatarUrl,
                    IsDisabled = user.Disabled,
                    IsActive = isActive && !user.Disabled,
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return null;
            }
        }

        public async Task<UserAccountDto?> GetAccountByEmailAsync(string email)
        {
            // MOCK ADMIN ACCOUNT FOR TESTING
            if (email.Equals("admin@campus.edu.vn", StringComparison.OrdinalIgnoreCase))
            {
                return new UserAccountDto
                {
                    Id = "mock-admin-id-12345",
                    Email = "admin@campus.edu.vn",
                    FullName = "Nguyễn Văn Quản Trị (Admin)",
                    Role = "Admin",
                    IsActive = true,
                    IsDisabled = false,
                    PhoneNumber = "0901234567",
                    CreatedAt = DateTime.UtcNow.ToString("o")
                };
            }

            if (_firebaseAuth == null) return null;

            try
            {
                UserRecord user = await _firebaseAuth.GetUserByEmailAsync(email);
                return await GetAccountByIdAsync(user.Uid);
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return null;
            }
        }

        public async Task<List<UserAccountDto>> GetAllAccountsAsync()
        {
            var users = new List<UserAccountDto>();
            if (_firestoreDb == null) return users;

            try
            {
                CollectionReference usersRef = _firestoreDb.Collection("users");
                QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
                foreach (var doc in snapshot.Documents)
                {
                    var dict = doc.ToDictionary();
                    bool isActive = !dict.ContainsKey("isActive") || Convert.ToBoolean(dict["isActive"]);
                    bool isLocked = dict.ContainsKey("isLocked") && Convert.ToBoolean(dict["isLocked"]);

                    users.Add(new UserAccountDto
                    {
                        Id = doc.Id,
                        Email = dict.ContainsKey("email") ? dict["email"]?.ToString() ?? "" : "",
                        FullName = dict.ContainsKey("fullName") ? dict["fullName"]?.ToString() ?? "" : "",
                        Role = dict.ContainsKey("role") ? dict["role"]?.ToString() ?? "Student" : "Student",
                        PhoneNumber = dict.ContainsKey("phoneNumber") ? dict["phoneNumber"]?.ToString() : null,
                        StudentCode = dict.ContainsKey("studentCode") ? dict["studentCode"]?.ToString() : null,
                        AvatarUrl = dict.ContainsKey("avatarUrl") ? dict["avatarUrl"]?.ToString() : null,
                        IsDisabled = isLocked,
                        IsActive = isActive && !isLocked,
                        CreatedAt = dict.ContainsKey("createdAt") ? dict["createdAt"]?.ToString() ?? "" : ""
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all accounts from Firestore");
            }

            return users;
        }

        public async Task<UserAccountDto> UpdateAccountAsync(string id, UpdateAccountRequest request)
        {
            if (_firebaseAuth == null)
            {
                throw new InvalidOperationException("Firebase Auth is not initialized.");
            }

            UserRecord existingUser = await _firebaseAuth.GetUserAsync(id);

            var userArgs = new UserRecordArgs { Uid = id };
            if (!string.IsNullOrEmpty(request.FullName))
            {
                userArgs.DisplayName = request.FullName;
            }
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                userArgs.PhoneNumber = request.PhoneNumber;
            }

            UserRecord updatedUser = await _firebaseAuth.UpdateUserAsync(userArgs);

            if (_firestoreDb != null)
            {
                try
                {
                    DocumentReference docRef = _firestoreDb.Collection("users").Document(id);
                    var updates = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(request.FullName)) updates["fullName"] = request.FullName;
                    if (!string.IsNullOrEmpty(request.PhoneNumber)) updates["phoneNumber"] = request.PhoneNumber;
                    if (!string.IsNullOrEmpty(request.AvatarUrl)) updates["avatarUrl"] = request.AvatarUrl;
                    if (!string.IsNullOrEmpty(request.Role)) updates["role"] = request.Role;
                    updates["updatedAt"] = DateTime.UtcNow;

                    await docRef.SetAsync(updates, SetOptions.MergeAll);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating Firestore document for UID {Uid}", id);
                }
            }

            return new UserAccountDto
            {
                Id = updatedUser.Uid,
                Email = updatedUser.Email,
                FullName = request.FullName ?? existingUser.DisplayName ?? string.Empty,
                Role = request.Role ?? "Student",
                PhoneNumber = request.PhoneNumber ?? updatedUser.PhoneNumber,
                AvatarUrl = request.AvatarUrl,
                IsDisabled = updatedUser.Disabled,
                IsActive = !updatedUser.Disabled,
                UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
        }

        public async Task<AccountStatusResponse> SetAccountLockStatusAsync(string id, bool isLocked)
        {
            if (_firebaseAuth == null)
            {
                throw new InvalidOperationException("Firebase Auth is not initialized.");
            }

            // AC 35.1.3: Throws FirebaseAuthException if user does not exist (handled by caller as 404)
            UserRecord existingUser = await _firebaseAuth.GetUserAsync(id);

            // Update Firebase Auth Disabled property
            var userArgs = new UserRecordArgs
            {
                Uid = id,
                Disabled = isLocked
            };

            UserRecord updatedUser = await _firebaseAuth.UpdateUserAsync(userArgs);

            // AC 35.1.4: Update Firestore document
            if (_firestoreDb != null)
            {
                try
                {
                    DocumentReference docRef = _firestoreDb.Collection("users").Document(id);
                    await docRef.SetAsync(new
                    {
                        isActive = !isLocked,
                        isLocked = isLocked,
                        updatedAt = DateTime.UtcNow
                    }, SetOptions.MergeAll);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating Firestore lock status for UID {Uid}", id);
                }
            }

            return new AccountStatusResponse
            {
                Id = updatedUser.Uid,
                IsLocked = isLocked,
                IsActive = !isLocked,
                Message = isLocked ? "Account locked successfully" : "Account unlocked successfully",
                UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
        }

        public async Task<LoginResponse?> AuthenticateAsync(string email, string password)
        {
            var user = await GetAccountByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            // Note: In client applications, Firebase Auth signs in via client SDK.
            // On API, if account is locked, we return user with IsActive = false / IsLocked = true
            // so Controller can reject with 403.
            string token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Token = token,
                IsActive = user.IsActive && !user.IsDisabled
            };
        }

        private string GenerateJwtToken(UserAccountDto user)
        {
            string secretKey = _configuration["Jwt:SecretKey"] ?? "CampusActivitiesManager_DefaultSecretKey_ForTokenGeneration_2026_!@#";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("role", user.Role),
                new Claim("isActive", (user.IsActive && !user.IsDisabled).ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "CampusActivitiesManager.Api",
                audience: _configuration["Jwt:Audience"] ?? "CampusActivitiesManager.Client",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusActivitiesManager.Api.Models;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CampusActivitiesManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private static readonly object _lock = new();
        private static readonly List<Account> _accounts = new()
        {
            new() { ID = 1, StudentCode = "20110001", FullName = "Nguyễn An Cương", Email = "cuong.na20@campus.edu.vn", PhoneNumber = "0987123456", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.LopTruong, ClassName = "D20CNTT1", AcademicYear = "K20", TrainingPoints = 90 },
            new() { ID = 2, StudentCode = "20110002", FullName = "Trần Thị Mai Hương", Email = "huong.ttm20@campus.edu.vn", PhoneNumber = "0912345678", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.BanChuNhiem, ClassName = "D20CNTT1", AcademicYear = "K20", TrainingPoints = 88 },
            new() { ID = 3, StudentCode = "20110003", FullName = "Lê Hoàng Long", Email = "long.lh20@campus.edu.vn", PhoneNumber = "0978901234", AvatarUrl = "", Status = AccountStatus.BaoLuu, Role = AccountRole.SinhVien, ClassName = "D20CNTT2", AcademicYear = "K20", TrainingPoints = 75 },
            new() { ID = 4, StudentCode = "21110015", FullName = "Phạm Minh Đức", Email = "duc.pm21@campus.edu.vn", PhoneNumber = "0934567890", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.SinhVien, ClassName = "D21KHMT1", AcademicYear = "K21", TrainingPoints = 82 },
            new() { ID = 5, StudentCode = "21110022", FullName = "Đỗ Thị Quỳnh Trang", Email = "trang.dtq21@campus.edu.vn", PhoneNumber = "0945678901", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.LopTruong, ClassName = "D21KTPM1", AcademicYear = "K21", TrainingPoints = 94 },
            new() { ID = 6, StudentCode = "21110045", FullName = "Vũ Quốc Bảo", Email = "bao.vq21@campus.edu.vn", PhoneNumber = "0956789012", AvatarUrl = "", Status = AccountStatus.BiKhoa, Role = AccountRole.SinhVien, ClassName = "D21KHMT2", AcademicYear = "K21", TrainingPoints = 50 },
            new() { ID = 7, StudentCode = "22110008", FullName = "Bùi Tuấn Anh", Email = "anh.bt22@campus.edu.vn", PhoneNumber = "0967890123", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.SinhVien, ClassName = "D22HTTT1", AcademicYear = "K22", TrainingPoints = 85 },
            new() { ID = 8, StudentCode = "22110034", FullName = "Phan Hải Yến", Email = "yen.ph22@campus.edu.vn", PhoneNumber = "0978901235", AvatarUrl = "", Status = AccountStatus.BaoLuu, Role = AccountRole.SinhVien, ClassName = "D22ATTT1", AcademicYear = "K22", TrainingPoints = 72 },
            new() { ID = 9, StudentCode = "22110056", FullName = "Trịnh Đình Khang", Email = "khang.td22@campus.edu.vn", PhoneNumber = "0989012345", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.BanChuNhiem, ClassName = "D22HTTT2", AcademicYear = "K22", TrainingPoints = 86 },
            new() { ID = 10, StudentCode = "23110012", FullName = "Đặng Kim Oanh", Email = "oanh.dk23@campus.edu.vn", PhoneNumber = "0901234567", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.SinhVien, ClassName = "D23CNTT1", AcademicYear = "K23", TrainingPoints = 80 },
            new() { ID = 11, StudentCode = "23110029", FullName = "Mai Văn Nam", Email = "nam.mv23@campus.edu.vn", PhoneNumber = "0912345679", AvatarUrl = "", Status = AccountStatus.BiKhoa, Role = AccountRole.SinhVien, ClassName = "D23KHMT1", AcademicYear = "K23", TrainingPoints = 45 },
            new() { ID = 12, StudentCode = "23110078", FullName = "Hoàng Lan Anh", Email = "anh.hl23@campus.edu.vn", PhoneNumber = "0923456780", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.SinhVien, ClassName = "D23KTPM1", AcademicYear = "K23", TrainingPoints = 91 },
            new() { ID = 13, StudentCode = "GV001", FullName = "TS. Nguyễn Thanh Sơn", Email = "son.nt@campus.edu.vn", PhoneNumber = "0934567891", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.GiangVien, ClassName = "Khoa CNTT", AcademicYear = "Cán bộ", TrainingPoints = 100 },
            new() { ID = 14, StudentCode = "GV002", FullName = "ThS. Lê Thu Hà", Email = "ha.lt@campus.edu.vn", PhoneNumber = "0945678902", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.GiangVien, ClassName = "Khoa KHMT", AcademicYear = "Cán bộ", TrainingPoints = 100 },
            new() { ID = 15, StudentCode = "ADM01", FullName = "Quản Trị Viên Hệ Thống", Email = "admin@campus.edu.vn", PhoneNumber = "0900000000", AvatarUrl = "", Status = AccountStatus.DangHoc, Role = AccountRole.Admin, ClassName = "Phòng Đào Tạo", AcademicYear = "Quản trị", TrainingPoints = 100 }
        };

        private readonly FirebaseAuth? _firebaseAuth;

        public AccountsController()
        {
            try
            {
                _firebaseAuth = FirebaseAuth.DefaultInstance;
            }
            catch
            {
                // Firebase optional
            }
        }

        /// <summary>
        /// GET: /api/accounts
        /// Lấy toàn bộ danh sách tài khoản, có hỗ trợ tìm kiếm và lọc trạng thái
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Account>> GetAccounts([FromQuery] string? search = null, [FromQuery] int? status = null)
        {
            lock (_lock)
            {
                var query = _accounts.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.Trim();
                    query = query.Where(a =>
                        a.FullName.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                        a.StudentCode.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                        a.Email.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                        a.ClassName.Contains(s, StringComparison.OrdinalIgnoreCase));
                }

                if (status.HasValue)
                {
                    query = query.Where(a => (int)a.Status == status.Value);
                }

                return Ok(query.OrderByDescending(a => a.ID).ToList());
            }
        }

        /// <summary>
        /// GET: /api/accounts/{id:int}
        /// Lấy thông tin chi tiết 1 tài khoản theo ID số
        /// </summary>
        [HttpGet("{id:int}")]
        public ActionResult<Account> GetAccount(int id)
        {
            lock (_lock)
            {
                var account = _accounts.FirstOrDefault(a => a.ID == id);
                if (account == null)
                {
                    return NotFound(new { message = $"Không tìm thấy tài khoản có ID = {id}." });
                }
                return Ok(account);
            }
        }

        /// <summary>
        /// POST: /api/accounts
        /// Thêm mới 1 tài khoản
        /// </summary>
        [HttpPost]
        public ActionResult<Account> CreateAccount([FromBody] Account newAccount)
        {
            if (newAccount == null)
            {
                return BadRequest("Dữ liệu tài khoản không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(newAccount.FullName))
            {
                return BadRequest("Họ và tên không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(newAccount.StudentCode))
            {
                return BadRequest("Mã sinh viên / cán bộ không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(newAccount.Email) || !newAccount.Email.Contains("@"))
            {
                return BadRequest("Địa chỉ Email không đúng định dạng.");
            }

            lock (_lock)
            {
                if (_accounts.Any(a => string.Equals(a.StudentCode, newAccount.StudentCode, StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest($"Mã sinh viên/cán bộ '{newAccount.StudentCode}' đã tồn tại trong hệ thống.");
                }

                int nextId = _accounts.Count > 0 ? _accounts.Max(a => a.ID) + 1 : 1;
                newAccount.ID = nextId;
                newAccount.CreatedAt = DateTime.Now;
                _accounts.Add(newAccount);

                return CreatedAtAction(nameof(GetAccount), new { id = newAccount.ID }, newAccount);
            }
        }

        /// <summary>
        /// PUT: /api/accounts/{id:int}
        /// Cập nhật thông tin / trạng thái tài khoản
        /// </summary>
        [HttpPut("{id:int}")]
        public IActionResult UpdateAccount(int id, [FromBody] Account updated)
        {
            if (updated == null)
            {
                return BadRequest("Dữ liệu cập nhật không hợp lệ.");
            }

            lock (_lock)
            {
                var existing = _accounts.FirstOrDefault(a => a.ID == id);
                if (existing == null)
                {
                    return NotFound(new { message = $"Không tìm thấy tài khoản có ID = {id}." });
                }

                if (!string.IsNullOrWhiteSpace(updated.FullName))
                    existing.FullName = updated.FullName.Trim();

                if (!string.IsNullOrWhiteSpace(updated.Email))
                    existing.Email = updated.Email.Trim();

                if (!string.IsNullOrWhiteSpace(updated.PhoneNumber))
                    existing.PhoneNumber = updated.PhoneNumber.Trim();

                if (!string.IsNullOrWhiteSpace(updated.ClassName))
                    existing.ClassName = updated.ClassName.Trim();

                if (!string.IsNullOrWhiteSpace(updated.AcademicYear))
                    existing.AcademicYear = updated.AcademicYear.Trim();

                existing.Status = updated.Status;
                existing.Role = updated.Role;

                return NoContent();
            }
        }

        /// <summary>
        /// DELETE: /api/accounts/{id:int}
        /// Xóa 1 tài khoản
        /// </summary>
        [HttpDelete("{id:int}")]
        public IActionResult DeleteAccount(int id)
        {
            lock (_lock)
            {
                var existing = _accounts.FirstOrDefault(a => a.ID == id);
                if (existing == null)
                {
                    return NotFound(new { message = $"Không tìm thấy tài khoản có ID = {id}." });
                }

                _accounts.Remove(existing);
                return NoContent();
            }
        }

        // ==========================================
        // Firebase compatibility endpoints (if configured)
        // ==========================================

        [HttpPost("firebase")]
        public async Task<IActionResult> CreateFirebaseAccount([FromBody] CreateAccountRequest request)
        {
            if (_firebaseAuth == null)
            {
                return BadRequest("Firebase is not configured on this server instance.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Validation failed", Errors = ModelState });
            }

            try
            {
                var userArgs = new UserRecordArgs
                {
                    Email = request.Email,
                    Password = request.Password,
                    DisplayName = request.DisplayName
                };

                UserRecord userRecord = await _firebaseAuth.CreateUserAsync(userArgs);

                return Ok(new { Message = "Account created successfully in Firebase", Uid = userRecord.Uid });
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(new { Message = "Failed to create account in Firebase", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred", Details = ex.Message });
            }
        }

        [HttpGet("firebase/{uid}")]
        public async Task<IActionResult> GetFirebaseAccount(string uid)
        {
            if (_firebaseAuth == null)
            {
                return BadRequest("Firebase is not configured on this server instance.");
            }

            try
            {
                UserRecord userRecord = await _firebaseAuth.GetUserAsync(uid);
                return Ok(new
                {
                    Uid = userRecord.Uid,
                    Email = userRecord.Email,
                    DisplayName = userRecord.DisplayName
                });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                return NotFound(new { Message = "Account not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred", Details = ex.Message });
            }
        }
    }
}

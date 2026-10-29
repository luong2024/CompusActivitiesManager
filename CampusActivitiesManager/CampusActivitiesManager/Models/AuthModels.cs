using System.Text.Json.Serialization;

namespace CampusActivitiesManager.Models
{
    public class AuthLoginResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Bản ghi yêu cầu cấp lại mật khẩu gửi tới Ban Quản Trị (Admin).
    /// Khi Admin xác nhận, mật khẩu tài khoản được đặt về mặc định của vai trò:
    /// Admin: Admin@123, Manager: Manager@123, Lecturer: Gv@123, Secretary: Bithu@123, Student: Student@123.
    /// </summary>
    public class PasswordResetRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "RST-" + Guid.NewGuid().ToString("N")[..6].ToUpper();

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public Role Role { get; set; } = Role.User;

        [JsonPropertyName("requestedAt")]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// "Pending" (Chờ xử lý), "Resolved" (Đã đặt lại mật khẩu), "Rejected" (Từ chối)
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = "Pending";

        [JsonPropertyName("defaultPasswordAssigned")]
        public string DefaultPasswordAssigned { get; set; } = string.Empty;

        [JsonPropertyName("resolvedAt")]
        public DateTime? ResolvedAt { get; set; }

        [JsonPropertyName("resolvedBy")]
        public string? ResolvedBy { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonIgnore]
        public bool IsPending => string.Equals(Status, "Pending", StringComparison.OrdinalIgnoreCase);

        [JsonIgnore]
        public string StatusDisplay => Status switch
        {
            "Pending" => "Chờ Admin xử lý ⏳",
            "Resolved" => "Đã reset mật khẩu ✅",
            "Rejected" => "Đã từ chối ❌",
            _ => Status
        };

        [JsonIgnore]
        public string StatusColor => Status switch
        {
            "Pending" => "#F59E0B",
            "Resolved" => "#10B981",
            "Rejected" => "#EF4444",
            _ => "#6B7280"
        };

        [JsonIgnore]
        public string FormattedTime => RequestedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
    }

    /// <summary>
    /// Nhật ký quản lý điểm rèn luyện (cộng/trừ điểm thủ công bởi Ban tổ chức, Cố vấn hoặc Admin)
    /// </summary>
    public class PointAuditRecord
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "AUD-" + Guid.NewGuid().ToString("N")[..6].ToUpper();

        [JsonPropertyName("studentCode")]
        public string StudentCode { get; set; } = string.Empty;

        [JsonPropertyName("studentName")]
        public string StudentName { get; set; } = string.Empty;

        /// <summary>
        /// Số điểm cộng (+) hoặc trừ (-)
        /// </summary>
        [JsonPropertyName("delta")]
        public int Delta { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;

        [JsonPropertyName("operatorUsername")]
        public string OperatorUsername { get; set; } = string.Empty;

        [JsonPropertyName("operatorName")]
        public string OperatorName { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public string FormattedTime => Timestamp.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        [JsonIgnore]
        public string DeltaDisplay => Delta >= 0 ? $"+{Delta} ĐRL" : $"{Delta} ĐRL";

        [JsonIgnore]
        public string DeltaColor => Delta >= 0 ? "#16A34A" : "#DC2626";

        [JsonIgnore]
        public string TypeBadge => Delta >= 0 ? "Khen thưởng / Cộng điểm 🎉" : "Kỷ luật / Trừ điểm ⚠️";
    }
}

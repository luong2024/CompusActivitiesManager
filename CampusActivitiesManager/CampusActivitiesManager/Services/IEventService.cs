using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.Services
{
    public interface IEventService
    {
        Task<List<CampusEvent>> GetEventsAsync(bool includePending = false);
        Task<List<CampusEvent>> GetEventsForUserAsync(User? user);
        Task<CampusEvent?> GetEventByIdAsync(int id);
        Task<bool> CreateEventAsync(CampusEvent evt);
        Task<bool> UpdateEventAsync(CampusEvent evt);
        Task<bool> ApproveEventAsync(int id, bool approve, string? approvedBy = "Admin", string? reason = null);
        Task<bool> DeleteEventAsync(int id);
        Task<List<EventCategoryDto>> GetCategoriesAsync();

        // Đăng ký tham gia & Hủy đăng ký
        Task<(bool Success, string Message, string Status)> RegisterUserForEventAsync(int eventId, User user);
        Task<(bool Success, string Message)> CancelRegistrationAsync(int eventId, string studentCodeOrId);
        Task<bool> ApproveRegistrationAsync(int eventId, string registrationId, bool approve);
    }
}

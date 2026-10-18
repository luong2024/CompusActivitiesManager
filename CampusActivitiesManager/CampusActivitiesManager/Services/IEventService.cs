using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.Services
{
    public interface IEventService
    {
        Task<List<CampusEvent>> GetEventsAsync(bool includePending = false);
        Task<CampusEvent?> GetEventByIdAsync(int id);
        Task<bool> CreateEventAsync(CampusEvent evt);
        Task<bool> UpdateEventAsync(CampusEvent evt);
        Task<bool> ApproveEventAsync(int id, bool approve, string? approvedBy = "Admin", string? reason = null);
        Task<bool> DeleteEventAsync(int id);
        Task<List<EventCategoryDto>> GetCategoriesAsync();
    }
}

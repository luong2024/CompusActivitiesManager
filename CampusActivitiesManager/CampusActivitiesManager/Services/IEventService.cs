using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.Services
{
    public interface IEventService
    {
        Task<List<CampusEvent>> GetEventsAsync();
        Task<CampusEvent?> GetEventByIdAsync(int id);
        Task<bool> CreateEventAsync(CampusEvent evt);
        Task<bool> UpdateEventAsync(CampusEvent evt);
        Task<bool> DeleteEventAsync(int id);
        Task<List<EventCategoryDto>> GetCategoriesAsync();
    }
}

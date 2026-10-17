using CampusActivitiesManager.Api.Models;

namespace CampusActivitiesManager.Api.Services
{
    public interface IFirebaseEventService
    {
        Task<List<EventDto>> GetAllEventsAsync(int limit = 20, int? afterId = null);
        Task<EventDto?> GetEventByIdAsync(int id);
        Task<EventDto> CreateEventAsync(CreateEventRequest request);
        Task<EventDto?> UpdateEventAsync(int id, UpdateEventRequest request);
        Task<bool> DeleteEventAsync(int id);

        Task<List<EventCategoryDto>> GetAllCategoriesAsync();
        Task<EventCategoryDto?> GetCategoryByIdAsync(int id);
        Task<EventCategoryDto> CreateCategoryAsync(CreateCategoryRequest request);
        Task<EventCategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
        Task<bool> DeleteCategoryAsync(int id);
    }
}

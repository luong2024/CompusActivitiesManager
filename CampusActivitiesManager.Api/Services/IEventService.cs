using CampusActivitiesManager.Api.Models;

namespace CampusActivitiesManager.Api.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetEventsAsync(
            int? categoryId = null,
            bool? hasCategory = null,
            string? search = null,
            string? status = null);

        Task<EventDto?> GetEventByIdAsync(int id);

        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();

        Task<CategoryDto?> GetCategoryByIdAsync(int id);
    }
}

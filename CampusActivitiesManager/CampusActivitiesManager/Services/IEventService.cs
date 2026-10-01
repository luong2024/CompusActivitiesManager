using CampusActivitiesManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampusActivitiesManager.Services
{
    public interface IEventService
    {
        Task<List<EventDto>> GetEventsAsync();
        Task<EventDto?> GetEventByIdAsync(int id);
    }
}

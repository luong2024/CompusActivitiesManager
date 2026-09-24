using System.Net.Http.Json;
using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.Services;

public class EventService
{
    private readonly HttpClient _httpClient;

    public EventService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
    }

    public async Task<List<EventDto>> GetEventsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<EventDto>>("api/v1/events") ?? new List<EventDto>();
    }

    public async Task<EventDto> GetEventAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<EventDto>($"api/v1/events/{id}");
    }

    public async Task CreateEventAsync(EventDto eventDto)
    {
        await _httpClient.PostAsJsonAsync("api/v1/events", eventDto);
    }

    public async Task UpdateEventAsync(EventDto eventDto)
    {
        await _httpClient.PutAsJsonAsync($"api/v1/events/{eventDto.Id}", eventDto);
    }

    public async Task DeleteEventAsync(int id)
    {
        await _httpClient.DeleteAsync($"api/v1/events/{id}");
    }
}

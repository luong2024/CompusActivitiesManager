using System.Net.Http.Json;
using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.Services;

public class CategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
    }

    public async Task<List<EventCategoryDto>> GetCategoriesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<EventCategoryDto>>("api/v1/categories") ?? new List<EventCategoryDto>();
    }

    public async Task<EventCategoryDto> GetCategoryAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<EventCategoryDto>($"api/v1/categories/{id}");
    }

    public async Task CreateCategoryAsync(EventCategoryDto categoryDto)
    {
        await _httpClient.PostAsJsonAsync("api/v1/categories", categoryDto);
    }

    public async Task UpdateCategoryAsync(EventCategoryDto categoryDto)
    {
        await _httpClient.PutAsJsonAsync($"api/v1/categories/{categoryDto.Id}", categoryDto);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await _httpClient.DeleteAsync($"api/v1/categories/{id}");
    }
}

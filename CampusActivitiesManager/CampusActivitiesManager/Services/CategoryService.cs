using System.Net.Http.Json;
using CampusActivitiesManager.Models;

namespace CampusActivitiesManager.Services;

public class CategoryService
{
    private readonly HttpClient _httpClient;

    private readonly string _baseUrl = DeviceInfo.Platform == DevicePlatform.Android 
        ? "https://10.0.2.2:7258/api/v1/categories" 
        : "https://localhost:7258/api/v1/categories";

    public CategoryService()
    {
        _httpClient = new HttpClient(HttpHelper.GetInsecureHandler());
    }

    public async Task<List<EventCategoryDto>> GetCategoriesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<EventCategoryDto>>(_baseUrl) ?? new List<EventCategoryDto>();
    }

    public async Task<EventCategoryDto> GetCategoryAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<EventCategoryDto>($"{_baseUrl}/{id}");
    }

    public async Task CreateCategoryAsync(EventCategoryDto categoryDto)
    {
        await _httpClient.PostAsJsonAsync(_baseUrl, categoryDto);
    }

    public async Task UpdateCategoryAsync(EventCategoryDto categoryDto)
    {
        await _httpClient.PutAsJsonAsync($"{_baseUrl}/{categoryDto.ID}", categoryDto);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
    }
}

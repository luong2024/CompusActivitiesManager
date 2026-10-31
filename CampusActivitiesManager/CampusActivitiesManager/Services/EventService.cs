using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CampusActivitiesManager.Models;
using Microsoft.Extensions.Logging;

namespace CampusActivitiesManager.Services
{
    public class EventService : IEventService
    {
        private readonly HttpClient _httpClient;
        private readonly ModalErrorHandler _errorHandler;
        private readonly ILogger<EventService> _logger;
        
        private readonly string _baseUrl = DeviceInfo.Platform == DevicePlatform.Android 
            ? "https://10.0.2.2:7258/api/v1/events" 
            : "https://localhost:7258/api/v1/events";

        public EventService(ModalErrorHandler errorHandler, ILogger<EventService> logger)
        {
            _httpClient = new HttpClient(HttpHelper.GetInsecureHandler());
            _errorHandler = errorHandler;
            _logger = logger;
        }

        public async Task<List<EventDto>> GetEventsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<EventDto>>>(options);
                    if (result != null && result.Success && result.Data != null)
                    {
                        return result.Data;
                    }
                }
                return new List<EventDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi l?y danh sách s? ki?n");
                _errorHandler.HandleError(ex);
                return new List<EventDto>();
            }
        }

        public async Task<EventDto> GetEventByIdAsync(int id)
        {
            var events = await GetEventsAsync();
            return events.FirstOrDefault(e => e.ID == id);
        }
    }
}

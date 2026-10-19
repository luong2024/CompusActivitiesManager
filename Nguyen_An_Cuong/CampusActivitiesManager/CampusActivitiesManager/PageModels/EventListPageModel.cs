using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class EventListPageModel : ObservableObject
    {
        private readonly IEventService _eventService;

        [ObservableProperty]
        private ObservableCollection<EventDto> _events = new();

        [ObservableProperty]
        private bool _isRefreshing;

        public EventListPageModel(IEventService eventService)
        {
            _eventService = eventService;
        }

        [RelayCommand]
        private async Task LoadEventsAsync()
        {
            if (IsRefreshing) return;
            IsRefreshing = true;
            try
            {
                var events = await _eventService.GetEventsAsync();
                Events.Clear();
                foreach (var e in events)
                {
                    Events.Add(e);
                }
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task RefreshEventsAsync()
        {
            try
            {
                var events = await _eventService.GetEventsAsync();
                Events.Clear();
                foreach (var e in events)
                {
                    Events.Add(e);
                }
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task Appearing()
        {
            await LoadEventsAsync();
        }

        [RelayCommand]
        private Task NavigateToEvent(EventDto eventDto)
        {
            if (eventDto == null) return Task.CompletedTask;
            return Shell.Current.GoToAsync($"eventdetails?id={eventDto.ID}", new Dictionary<string, object>
            {
                { "Event", eventDto }
            });
        }
    }
}

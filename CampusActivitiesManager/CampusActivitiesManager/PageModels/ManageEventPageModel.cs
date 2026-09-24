using System.Collections.ObjectModel;
using System.Windows.Input;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace CampusActivitiesManager.PageModels;

public class ManageEventPageModel : BaseViewModel
{
    private readonly EventService _eventService;

    public ObservableCollection<EventDto> Events { get; set; } = new();

    public ICommand RefreshCommand { get; }

    public ManageEventPageModel(EventService eventService)
    {
        _eventService = eventService;
        RefreshCommand = new Command(async () => await LoadEventsAsync());
    }

    public async Task LoadEventsAsync()
    {
        var events = await _eventService.GetEventsAsync();
        Events.Clear();
        foreach (var ev in events)
        {
            Events.Add(ev);
        }
    }
}

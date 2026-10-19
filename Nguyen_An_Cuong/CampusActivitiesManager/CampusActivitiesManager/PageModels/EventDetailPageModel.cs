using CampusActivitiesManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;

namespace CampusActivitiesManager.PageModels
{
    [QueryProperty(nameof(EventDto), "Event")]
    public partial class EventDetailPageModel : ObservableObject
    {
        [ObservableProperty]
        private EventDto _eventDto;

        public EventDetailPageModel()
        {
        }
    }
}

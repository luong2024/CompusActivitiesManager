using CampusActivitiesManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;

namespace CampusActivitiesManager.PageModels
{
    [QueryProperty(nameof(CampusEvent), "Event")]
    public partial class EventDetailPageModel : ObservableObject
    {
        [ObservableProperty]
        private CampusEvent _campusEvent = new();

        public EventDetailPageModel()
        {
        }
    }
}

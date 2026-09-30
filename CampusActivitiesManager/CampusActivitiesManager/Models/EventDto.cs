using System;

namespace CampusActivitiesManager.Models
{
    public class EventDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public EventCategoryDto Category { get; set; }
    }
}

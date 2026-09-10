using System.Text.Json.Serialization;
using CampusActivitiesManager.Models;

[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(ProjectTask))]
[JsonSerializable(typeof(ProjectsJson))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(Tag))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(Role))]
[JsonSerializable(typeof(List<User>))]
public partial class JsonContext : JsonSerializerContext
{
}
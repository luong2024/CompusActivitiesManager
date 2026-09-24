using System.Collections.ObjectModel;
using System.Windows.Input;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace CampusActivitiesManager.PageModels;

public class ManageCategoryPageModel : BaseViewModel
{
    private readonly CategoryService _categoryService;

    public ObservableCollection<EventCategoryDto> Categories { get; set; } = new();

    public ICommand RefreshCommand { get; }

    public ManageCategoryPageModel(CategoryService categoryService)
    {
        _categoryService = categoryService;
        RefreshCommand = new Command(async () => await LoadCategoriesAsync());
    }

    public async Task LoadCategoriesAsync()
    {
        var categories = await _categoryService.GetCategoriesAsync();
        Categories.Clear();
        foreach (var cat in categories)
        {
            Categories.Add(cat);
        }
    }
}

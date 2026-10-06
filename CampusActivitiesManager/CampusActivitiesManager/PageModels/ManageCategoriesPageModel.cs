using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace CampusActivitiesManager.PageModels
{
    public partial class ManageCategoriesPageModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<Category> _categories = new();

        [ObservableProperty]
        private string _newCategoryName = string.Empty;

        public ManageCategoriesPageModel()
        {
            Title = "Quản lý Danh mục";
            LoadCategories();
        }

        private void LoadCategories()
        {
            Categories.Add(new Category { ID = 1, Title = "Học thuật", Color = "#2563EB" });
            Categories.Add(new Category { ID = 2, Title = "Thể thao", Color = "#059669" });
            Categories.Add(new Category { ID = 3, Title = "Tình nguyện", Color = "#DC2626" });
            Categories.Add(new Category { ID = 4, Title = "Kỹ năng", Color = "#7C3AED" });
        }

        [RelayCommand]
        private async Task AddCategoryAsync()
        {
            if (string.IsNullOrWhiteSpace(NewCategoryName))
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Lỗi", "Vui lòng nhập tên danh mục", "OK");
                return;
            }

            Categories.Add(new Category
            {
                ID = Categories.Count + 1,
                Title = NewCategoryName,
                Color = "#6B7280"
            });

            NewCategoryName = string.Empty;
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Thành công", "Đã thêm danh mục mới!", "OK");
        }

        [RelayCommand]
        private async Task DeleteCategoryAsync(Category category)
        {
            if (category == null || Shell.Current == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Xóa danh mục", $"Bạn có chắc muốn xóa '{category.Title}'?", "Có", "Không");
            if (confirm)
            {
                Categories.Remove(category);
            }
        }
    }
}

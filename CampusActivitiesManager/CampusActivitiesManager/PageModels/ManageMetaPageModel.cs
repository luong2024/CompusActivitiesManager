using System.Collections.ObjectModel;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class ManageMetaPageModel : ObservableObject
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly TagRepository _tagRepository;
        private readonly SeedDataService _seedDataService;
        private readonly IAuthService _authService;

        [ObservableProperty]
        private ObservableCollection<Category> _categories = [];

        [ObservableProperty]
        private ObservableCollection<Tag> _tags = [];

        [ObservableProperty]
        private bool _isAdmin;

        public ManageMetaPageModel(CategoryRepository categoryRepository, TagRepository tagRepository, SeedDataService seedDataService, IAuthService authService)
        {
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _seedDataService = seedDataService;
            _authService = authService;
            _isAdmin = _authService.IsAdmin;
        }

        private async Task LoadData()
        {
            IsAdmin = _authService.IsAdmin;
            var categoriesList = await _categoryRepository.ListAsync();
            Categories = new ObservableCollection<Category>(categoriesList);
            var tagsList = await _tagRepository.ListAsync();
            Tags = new ObservableCollection<Tag>(tagsList);
        }

        [RelayCommand]
        private Task Appearing()
            => LoadData();

        [RelayCommand]
        private async Task SaveCategories()
        {
            if (!_authService.IsAdmin)
            {
                await AppShell.DisplaySnackbarAsync("Chỉ Quản trị viên (Admin) mới có quyền lưu cấu hình danh mục!");
                return;
            }

            foreach (var category in Categories)
            {
                await _categoryRepository.SaveItemAsync(category);
            }

            await AppShell.DisplayToastAsync("Categories saved");
        }

        [RelayCommand]
        private async Task DeleteCategory(Category category)
        {
            if (!_authService.IsAdmin)
            {
                await AppShell.DisplaySnackbarAsync("Chỉ Quản trị viên (Admin) mới có quyền xóa danh mục!");
                return;
            }

            Categories.Remove(category);
            await _categoryRepository.DeleteItemAsync(category);
            await AppShell.DisplayToastAsync("Category deleted");
        }

        [RelayCommand]
        private async Task AddCategory()
        {
            if (!_authService.IsAdmin)
            {
                await AppShell.DisplaySnackbarAsync("Chỉ Quản trị viên (Admin) mới có quyền thêm danh mục!");
                return;
            }

            var category = new Category();
            Categories.Add(category);
            await _categoryRepository.SaveItemAsync(category);
            await AppShell.DisplayToastAsync("Category added");
        }

        [RelayCommand]
        private async Task SaveTags()
        {
            if (!_authService.IsAdmin)
            {
                await AppShell.DisplaySnackbarAsync("Chỉ Quản trị viên (Admin) mới có quyền lưu thẻ!");
                return;
            }

            foreach (var tag in Tags)
            {
                await _tagRepository.SaveItemAsync(tag);
            }

            await AppShell.DisplayToastAsync("Tags saved");
        }

        [RelayCommand]
        private async Task DeleteTag(Tag tag)
        {
            if (!_authService.IsAdmin)
            {
                await AppShell.DisplaySnackbarAsync("Chỉ Quản trị viên (Admin) mới có quyền xóa thẻ!");
                return;
            }

            Tags.Remove(tag);
            await _tagRepository.DeleteItemAsync(tag);
            await AppShell.DisplayToastAsync("Tag deleted");
        }

        [RelayCommand]
        private async Task AddTag()
        {
            if (!_authService.IsAdmin)
            {
                await AppShell.DisplaySnackbarAsync("Chỉ Quản trị viên (Admin) mới có quyền thêm thẻ!");
                return;
            }

            var tag = new Tag();
            Tags.Add(tag);
            await _tagRepository.SaveItemAsync(tag);
            await AppShell.DisplayToastAsync("Tag added");
        }

        [RelayCommand]
        private async Task Reset()
        {
            if (!_authService.IsAdmin)
            {
                await AppShell.DisplaySnackbarAsync("Chỉ Quản trị viên (Admin) mới có quyền đặt lại dữ liệu mẫu!");
                return;
            }

            Preferences.Default.Remove("is_campus_activities_seeded_v3");
            Preferences.Default.Remove("is_seeded");
            await _seedDataService.LoadSeedDataAsync();
            Preferences.Default.Set("is_campus_activities_seeded_v3", true);
            await Shell.Current.GoToAsync("//main");
        }
    }
}

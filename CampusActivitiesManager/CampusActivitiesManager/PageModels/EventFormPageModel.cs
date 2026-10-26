using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CampusActivitiesManager.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CampusActivitiesManager.PageModels
{
    /// <summary>
    /// ViewModel cho form Tạo mới và Chỉnh sửa sự kiện (Task T13.2 / US13).
    /// Kế thừa từ BaseViewModel, cung cấp validation, quản lý danh mục và hỗ trợ QueryProperty cho chế độ chỉnh sửa.
    /// </summary>
    [QueryProperty(nameof(EventId), "id")]
    [QueryProperty(nameof(EventId), "EventId")]
    [QueryProperty(nameof(EventItem), "Event")]
    [QueryProperty(nameof(EventItem), "event")]
    [QueryProperty(nameof(ProjectItem), "project")]
    public partial class EventFormPageModel : BaseViewModel, IQueryAttributable
    {
        private readonly CategoryRepository? _categoryRepository;
        private readonly ProjectRepository? _projectRepository;
        private readonly ILogger<EventFormPageModel>? _logger;
        private readonly ModalErrorHandler? _errorHandler;

        // Lưu ý: Thuộc tính Title đại diện cho tiêu đề sự kiện, được kế thừa từ BaseViewModel (hỗ trợ INotifyPropertyChanged).

        [ObservableProperty]
        private int _eventId;

        [ObservableProperty]
        private Category? _category;

        [ObservableProperty]
        private List<Category> _categories = [];

        [ObservableProperty]
        private DateTime _startDate = DateTime.Today;

        [ObservableProperty]
        private TimeSpan _startTime = new TimeSpan(8, 0, 0);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        [ObservableProperty]
        private TimeSpan _endTime = new TimeSpan(11, 30, 0);

        [ObservableProperty]
        private string _location = string.Empty;

        [ObservableProperty]
        private int? _maxParticipants = 100;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PageTitle))]
        [NotifyPropertyChangedFor(nameof(FormTitle))]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        private bool _isEditMode;

        [ObservableProperty]
        private object? _eventItem;

        [ObservableProperty]
        private Project? _projectItem;

        /// <summary>
        /// Tiêu đề trang hiển thị linh hoạt theo chế độ Thêm mới hoặc Chỉnh sửa.
        /// </summary>
        public string PageTitle => IsEditMode ? "Chỉnh sửa sự kiện" : "Tạo mới sự kiện";

        public string FormTitle => PageTitle;

        public string SaveButtonText => IsEditMode ? "Lưu thay đổi" : "Tạo sự kiện";

        public EventFormPageModel()
        {
            Title = string.Empty;
            InitDefaultCategories();
        }

        public EventFormPageModel(
            CategoryRepository? categoryRepository,
            ProjectRepository? projectRepository,
            ILogger<EventFormPageModel>? logger,
            ModalErrorHandler? errorHandler)
        {
            _categoryRepository = categoryRepository;
            _projectRepository = projectRepository;
            _logger = logger;
            _errorHandler = errorHandler;

            Title = string.Empty;
            LoadCategoriesAsync().FireAndForgetSafeAsync(_errorHandler);
        }

        private void InitDefaultCategories()
        {
            Categories = new List<Category>
            {
                new Category { ID = 1, Title = "Học thuật", Color = "#2563EB" },
                new Category { ID = 2, Title = "Thể thao", Color = "#059669" },
                new Category { ID = 3, Title = "Tình nguyện", Color = "#DC2626" },
                new Category { ID = 4, Title = "Kỹ năng", Color = "#7C3AED" },
                new Category { ID = 5, Title = "Văn hóa nghệ thuật", Color = "#D97706" }
            };
            Category = Categories.FirstOrDefault();
        }

        public async Task LoadCategoriesAsync()
        {
            try
            {
                List<Category>? list = null;
                if (_categoryRepository != null)
                {
                    list = await _categoryRepository.ListAsync();
                }

                if (list != null && list.Count > 0)
                {
                    Categories = list;
                }
                else
                {
                    InitDefaultCategories();
                }

                if (Category == null && Categories.Count > 0)
                {
                    Category = Categories[0];
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi tải danh mục sự kiện trong EventFormPageModel");
                InitDefaultCategories();
            }
        }

        async partial void OnEventIdChanged(int value)
        {
            if (value > 0)
            {
                await LoadEventDataAsync(value);
            }
        }

        partial void OnEventItemChanged(object? value)
        {
            if (value is Project project)
            {
                PopulateFromProject(project);
            }
            else if (value != null)
            {
                PopulateFromObject(value);
            }
        }

        partial void OnProjectItemChanged(Project? value)
        {
            if (value != null)
            {
                PopulateFromProject(value);
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query == null || query.Count == 0)
                return;

            // 1. Nhận trực tiếp đối tượng sự kiện
            if (query.TryGetValue("Event", out var evObj) || query.TryGetValue("event", out evObj) || query.TryGetValue("item", out evObj))
            {
                if (evObj is Project project)
                {
                    PopulateFromProject(project);
                    return;
                }
                else if (evObj != null)
                {
                    PopulateFromObject(evObj);
                    return;
                }
            }

            if (query.TryGetValue("project", out var projObj) && projObj is Project p)
            {
                PopulateFromProject(p);
                return;
            }

            // 2. Nhận ID sự kiện để truy vấn dữ liệu
            if (query.TryGetValue("id", out var idVal) || query.TryGetValue("EventId", out idVal) || query.TryGetValue("eventId", out idVal))
            {
                if (int.TryParse(idVal?.ToString(), out int parsedId) && parsedId > 0)
                {
                    EventId = parsedId;
                    LoadEventDataAsync(parsedId).FireAndForgetSafeAsync(_errorHandler);
                    return;
                }
            }

            // 3. Nhận các tham số riêng lẻ qua query URL
            if (query.TryGetValue("title", out var titleVal) || query.TryGetValue("Title", out titleVal))
            {
                Title = titleVal?.ToString() ?? string.Empty;
                IsEditMode = true;
            }

            if (query.TryGetValue("location", out var locVal) || query.TryGetValue("Location", out locVal))
            {
                Location = locVal?.ToString() ?? string.Empty;
            }

            if (query.TryGetValue("description", out var descVal) || query.TryGetValue("Description", out descVal))
            {
                Description = descVal?.ToString() ?? string.Empty;
            }

            if (query.TryGetValue("maxParticipants", out var maxVal) || query.TryGetValue("MaxParticipants", out maxVal))
            {
                if (int.TryParse(maxVal?.ToString(), out int max))
                    MaxParticipants = max;
            }

            if (query.TryGetValue("startDate", out var sdVal) || query.TryGetValue("StartDate", out sdVal))
            {
                if (DateTime.TryParse(sdVal?.ToString(), out var sd))
                    StartDate = sd.Date;
            }

            if (query.TryGetValue("startTime", out var stVal) || query.TryGetValue("StartTime", out stVal))
            {
                if (TimeSpan.TryParse(stVal?.ToString(), out var st))
                    StartTime = st;
            }

            if (query.TryGetValue("endDate", out var edVal) || query.TryGetValue("EndDate", out edVal))
            {
                if (DateTime.TryParse(edVal?.ToString(), out var ed))
                    EndDate = ed.Date;
            }

            if (query.TryGetValue("endTime", out var etVal) || query.TryGetValue("EndTime", out etVal))
            {
                if (TimeSpan.TryParse(etVal?.ToString(), out var et))
                    EndTime = et;
            }

            if (query.TryGetValue("isEdit", out var editVal) || query.TryGetValue("isEditMode", out editVal) || query.TryGetValue("IsEditMode", out editVal))
            {
                if (bool.TryParse(editVal?.ToString(), out bool isEdit))
                    IsEditMode = isEdit;
            }

            if (query.TryGetValue("category", out var catVal) || query.TryGetValue("Category", out catVal))
            {
                if (catVal is Category cat)
                {
                    Category = cat;
                }
                else if (catVal is string catTitle && !string.IsNullOrWhiteSpace(catTitle))
                {
                    SetCategoryByTitle(catTitle);
                }
            }
        }

        private async Task LoadEventDataAsync(int id)
        {
            try
            {
                IsBusy = true;
                if (Categories == null || Categories.Count == 0)
                {
                    await LoadCategoriesAsync();
                }

                if (_projectRepository != null)
                {
                    var project = await _projectRepository.GetAsync(id);
                    if (project != null)
                    {
                        PopulateFromProject(project);
                        return;
                    }
                }

                IsEditMode = true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi tải thông tin sự kiện ID {Id}", id);
                _errorHandler?.HandleError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void PopulateFromProject(Project project)
        {
            if (project == null) return;

            EventId = project.ID;
            Title = project.Name;
            Description = project.Description;
            IsEditMode = true;

            if (Categories != null && Categories.Count > 0)
            {
                Category = Categories.FirstOrDefault(c => c.ID == project.CategoryID)
                           ?? Categories.FirstOrDefault(c => string.Equals(c.Title, project.CategoryTitle, StringComparison.OrdinalIgnoreCase))
                           ?? Categories.FirstOrDefault();
            }
        }

        private void PopulateFromObject(object obj)
        {
            try
            {
                var type = obj.GetType();
                var idProp = type.GetProperty("ID") ?? type.GetProperty("Id") ?? type.GetProperty("EventId");
                if (idProp != null && int.TryParse(idProp.GetValue(obj)?.ToString(), out int id))
                {
                    EventId = id;
                }

                var nameProp = type.GetProperty("Title") ?? type.GetProperty("Name");
                if (nameProp != null)
                {
                    Title = nameProp.GetValue(obj)?.ToString() ?? string.Empty;
                }

                var descProp = type.GetProperty("Description");
                if (descProp != null)
                {
                    Description = descProp.GetValue(obj)?.ToString() ?? string.Empty;
                }

                var locProp = type.GetProperty("Location");
                if (locProp != null)
                {
                    Location = locProp.GetValue(obj)?.ToString() ?? string.Empty;
                }

                var maxProp = type.GetProperty("MaxParticipants");
                if (maxProp != null && int.TryParse(maxProp.GetValue(obj)?.ToString(), out int max))
                {
                    MaxParticipants = max;
                }

                var catProp = type.GetProperty("Category");
                if (catProp != null)
                {
                    var catVal = catProp.GetValue(obj);
                    if (catVal is Category category)
                        Category = category;
                    else if (catVal is string catName)
                        SetCategoryByTitle(catName);
                }

                IsEditMode = true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi trích xuất thông tin sự kiện từ đối tượng động");
            }
        }

        private void SetCategoryByTitle(string title)
        {
            if (Categories != null && Categories.Count > 0)
            {
                var matched = Categories.FirstOrDefault(c => string.Equals(c.Title, title, StringComparison.OrdinalIgnoreCase));
                if (matched != null)
                {
                    Category = matched;
                }
            }
        }

        /// <summary>
        /// Xử lý lưu sự kiện với kiểm tra validation cơ bản, ghi log debug và thông báo thành công.
        /// </summary>
        [RelayCommand]
        public async Task Save()
        {
            // Validation 1: Kiểm tra không để trống Title
            if (string.IsNullOrWhiteSpace(Title))
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Lỗi hợp lệ", "Vui lòng nhập tiêu đề sự kiện.", "Đóng");
                }
                return;
            }

            // Validation 2: Ngày kết thúc phải sau ngày bắt đầu
            var startDateTime = StartDate.Date + StartTime;
            var endDateTime = EndDate.Date + EndTime;

            if (endDateTime <= startDateTime)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Lỗi hợp lệ", "Thời gian kết thúc phải sau thời gian bắt đầu.", "Đóng");
                }
                return;
            }

            try
            {
                IsBusy = true;

                // Ghi log debug thông tin sự kiện
                string logMessage = $"[SaveEvent] Chế độ: {(IsEditMode ? "Chỉnh sửa" : "Tạo mới")}, " +
                                    $"ID: {EventId}, Title: '{Title}', Category: '{Category?.Title}', " +
                                    $"Bắt đầu: {startDateTime:yyyy-MM-dd HH:mm}, Kết thúc: {endDateTime:yyyy-MM-dd HH:mm}, " +
                                    $"Địa điểm: '{Location}', Số lượng tối đa: {MaxParticipants}, Mô tả: '{Description}'";

                Debug.WriteLine(logMessage);
                _logger?.LogInformation("{LogMessage}", logMessage);

                // Hiển thị DisplayAlert thông báo thành công
                string successTitle = "Thành công";
                string successMessage = IsEditMode
                    ? $"Cập nhật sự kiện '{Title}' thành công!"
                    : $"Tạo mới sự kiện '{Title}' thành công!";

                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(successTitle, successMessage, "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi xảy ra trong quá trình lưu sự kiện");
                _errorHandler?.HandleError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Lệnh hủy bỏ, quay trở về trang trước đó.
        /// </summary>
        [RelayCommand]
        public async Task Cancel()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}

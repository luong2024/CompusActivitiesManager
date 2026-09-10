#nullable disable
using System.Collections.ObjectModel;
using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CampusActivitiesManager.PageModels
{
    public partial class MainPageModel : ObservableObject, IProjectTaskPageModel
    {
        private bool _isNavigatedTo;
        private bool _dataLoaded;
        private readonly ProjectRepository _projectRepository;
        private readonly TaskRepository _taskRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly ModalErrorHandler _errorHandler;
        private readonly SeedDataService _seedDataService;
        private readonly AuthService _authService;

        [ObservableProperty]
        private Account _currentStudent;

        [ObservableProperty]
        private string _studentGreeting = "Xin chào Sinh viên 👋";

        [ObservableProperty]
        private int _upcomingActivitiesCount = 4;

        [ObservableProperty]
        private int _registeredActivitiesCount = 2;

        [ObservableProperty]
        private int _completedTasksCount = 0;

        [ObservableProperty]
        private bool _isQrModalVisible = false;

        [ObservableProperty]
        private string _selectedActivityForQr = "Hội thảo Công nghệ Thông tin 2026";

        [ObservableProperty]
        private string _qrTicketCode = "CAMPUS-TICKET-2026-B20DCCN001";

        [ObservableProperty]
        private List<CategoryChartData> _todoCategoryData = [];

        [ObservableProperty]
        private List<Brush> _todoCategoryColors = [];

        [ObservableProperty]
        private List<ProjectTask> _tasks = [];

        [ObservableProperty]
        private List<Project> _projects = [];

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _today = DateTime.Now.ToString("dddd, dd/MM/yyyy");

        public bool HasCompletedTasks => Tasks?.Any(t => t.IsCompleted) ?? false;

        public MainPageModel(
            SeedDataService seedDataService,
            ProjectRepository projectRepository,
            TaskRepository taskRepository,
            CategoryRepository categoryRepository,
            ModalErrorHandler errorHandler,
            AuthService authService)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _categoryRepository = categoryRepository;
            _errorHandler = errorHandler;
            _seedDataService = seedDataService;
            _authService = authService;

            UpdateStudentProfile();
        }

        private void UpdateStudentProfile()
        {
            if (_authService != null && _authService.CurrentUser != null)
            {
                CurrentStudent = _authService.CurrentUser;
            }
            else
            {
                // Default fallback student account
                CurrentStudent = new Account
                {
                    FullName = "Nguyễn An Cương",
                    StudentCode = "B20DCCN001",
                    Email = "cuong.na@campus.edu.vn",
                    PhoneNumber = "0987123456",
                    ClassName = "D20CNTT1",
                    AcademicYear = "K20",
                    Role = AccountRole.LopTruong,
                    Status = AccountStatus.DangHoc,
                    TrainingPoints = 92
                };
            }

            StudentGreeting = $"Xin chào, {CurrentStudent.FullName} 👋";
            QrTicketCode = $"CAMPUS-TK-{CurrentStudent.StudentCode}-{(DateTime.Now.Ticks % 100000):D5}";
        }

        private async Task LoadData()
        {
            try
            {
                IsBusy = true;
                UpdateStudentProfile();

                Projects = await _projectRepository.ListAsync();
                UpcomingActivitiesCount = Projects.Count;

                var chartData = new List<CategoryChartData>();
                var chartColors = new List<Brush>();

                var categories = await _categoryRepository.ListAsync();
                foreach (var category in categories)
                {
                    chartColors.Add(category.ColorBrush);
                    var ps = Projects.Where(p => p.CategoryID == category.ID).ToList();
                    int tasksCount = ps.SelectMany(p => p.Tasks).Count();
                    chartData.Add(new(category.Title, tasksCount));
                }

                TodoCategoryData = chartData;
                TodoCategoryColors = chartColors;

                Tasks = await _taskRepository.ListAsync();
                CompletedTasksCount = Tasks.Count(t => t.IsCompleted);
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(HasCompletedTasks));
            }
        }

        private async Task InitData(SeedDataService seedDataService)
        {
            bool isSeeded = Preferences.Default.ContainsKey("is_seeded_v10");
            if (!isSeeded)
            {
                await seedDataService.LoadSeedDataAsync();
                Preferences.Default.Set("is_seeded_v10", true);
            }
            await Refresh();
        }

        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                IsRefreshing = true;
                await LoadData();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private void NavigatedTo() => _isNavigatedTo = true;

        [RelayCommand]
        private void NavigatedFrom() => _isNavigatedTo = false;

        [RelayCommand]
        private async Task Appearing()
        {
            if (!_dataLoaded)
            {
                await InitData(_seedDataService);
                _dataLoaded = true;
                await Refresh();
            }
            else if (!_isNavigatedTo)
            {
                await Refresh();
            }
        }

        [RelayCommand]
        private Task TaskCompleted(ProjectTask task)
        {
            OnPropertyChanged(nameof(HasCompletedTasks));
            CompletedTasksCount = Tasks.Count(t => t.IsCompleted);
            return _taskRepository.SaveItemAsync(task);
        }

        [RelayCommand]
        private Task AddTask() => Shell.Current.GoToAsync("task");

        [RelayCommand]
        private Task NavigateToProject(Project project) => Shell.Current.GoToAsync($"project?id={project.ID}");

        [RelayCommand]
        private Task NavigateToTask(ProjectTask task) => Shell.Current.GoToAsync($"task?id={task.ID}");

        // US 17 & US 01: Quick Register for Campus Activity
        [RelayCommand]
        private async Task RegisterActivity(Project project)
        {
            if (project == null) return;

            RegisteredActivitiesCount++;
            await Shell.Current.DisplayAlert(
                "Đăng ký thành công 🎉",
                $"Bạn đã đăng ký tham gia sự kiện: {project.Name}.\n" +
                $"Thời gian: Sắp diễn ra\n" +
                $"Vé điện tử QR của bạn đã sẵn sàng trong mục 'Vé QR'!",
                "Xem vé ngay"
            );
            SelectedActivityForQr = project.Name;
            ShowQrTicket();
        }

        // Show QR Attendance Ticket Modal (US 17)
        [RelayCommand]
        private void ShowQrTicket()
        {
            IsQrModalVisible = true;
        }

        [RelayCommand]
        private void CloseQrTicket()
        {
            IsQrModalVisible = false;
        }

        // Student Logout
        [RelayCommand]
        private async Task Logout()
        {
            bool confirm = await Shell.Current.DisplayAlert("Đăng xuất", "Bạn có chắc chắn muốn đăng xuất khỏi tài khoản Sinh viên?", "Đăng xuất", "Hủy");
            if (confirm)
            {
                if (_authService != null)
                {
                    await _authService.LogoutAsync();
                }
                await Shell.Current.GoToAsync("//login");
            }
        }

        [RelayCommand]
        private async Task CleanTasks()
        {
            var completedTasks = Tasks.Where(t => t.IsCompleted).ToList();
            foreach (var task in completedTasks)
            {
                await _taskRepository.DeleteItemAsync(task);
                Tasks.Remove(task);
            }

            OnPropertyChanged(nameof(HasCompletedTasks));
            Tasks = new(Tasks);
            CompletedTasksCount = 0;
            await Shell.Current.DisplayAlert("Thông báo", "Đã dọn dẹp các nhiệm vụ hoàn thành!", "Đóng");
        }
    }
}
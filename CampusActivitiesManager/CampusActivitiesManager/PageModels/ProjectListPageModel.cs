#nullable disable
using CampusActivitiesManager.Data;
using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class ProjectListPageModel : ObservableObject
    {
        private readonly ProjectRepository _projectRepository;
        private readonly IAuthService _authService;

        [ObservableProperty]
        private List<Project> _projects = [];

        [ObservableProperty]
        private bool _canManageProjects;

        public ProjectListPageModel(ProjectRepository projectRepository, IAuthService authService)
        {
            _projectRepository = projectRepository;
            _authService = authService;
            _canManageProjects = _authService.CanManageProjects;
        }

        [RelayCommand]
        private async Task Appearing()
        {
            CanManageProjects = _authService.CanManageProjects;
            Projects = await _projectRepository.ListAsync();
        }

        [RelayCommand]
        Task NavigateToProject(Project project)
            => Shell.Current.GoToAsync($"project?id={project.ID}");

        [RelayCommand]
        async Task AddProject()
        {
            if (!_authService.CanManageProjects)
            {
                await AppShell.DisplaySnackbarAsync("Tài khoản Sinh viên (Student) không có quyền tạo mới dự án!");
                return;
            }

            await Shell.Current.GoToAsync($"project");
        }
    }
}
using CampusActivitiesManager.BusinessLogic.Services;
using CampusActivitiesManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class CreateUserViewModel : BaseViewModel
    {
        private readonly IUserService<User> _userService;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private Role _selectedRole = Role.User;

        [ObservableProperty]
        private List<Role> _availableRoles = [Role.Admin, Role.Manager, Role.Lecturer, Role.Secretary, Role.User, Role.Guest];

        [ObservableProperty]
        private List<ClassRoom> _availableClasses = [];

        [ObservableProperty]
        private ClassRoom? _selectedClass;

        [ObservableProperty]
        private string _customClassName = string.Empty;

        public CreateUserViewModel(IUserService<User> userService, ModalErrorHandler errorHandler)
        {
            _userService = userService;
            _errorHandler = errorHandler;
            Title = "Thêm tài khoản";
            _ = LoadClassesAsync();
        }

        public async Task LoadClassesAsync()
        {
            try
            {
                AvailableClasses = await _userService.GetClassesAsync();
                SelectedClass = AvailableClasses.FirstOrDefault();
            }
            catch { }
        }

        [RelayCommand]
        public async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(FullName))
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Lỗi", "Vui lòng nhập đầy đủ Email và Họ tên.", "Đóng");
                }
                return;
            }

            try
            {
                IsBusy = true;
                string chosenClass = !string.IsNullOrWhiteSpace(CustomClassName)
                    ? CustomClassName.Trim()
                    : (SelectedClass?.ClassName ?? "D20CNTT1");

                var user = new User
                {
                    Email = Email.Trim(),
                    FullName = FullName.Trim(),
                    Username = string.IsNullOrWhiteSpace(Username) ? Email.Split('@')[0].Trim() : Username.Trim(),
                    PhoneNumber = PhoneNumber?.Trim() ?? string.Empty,
                    Role = SelectedRole,
                    ClassName = chosenClass,
                    AssignedClass = (SelectedRole == Role.Lecturer || SelectedRole == Role.Manager) ? chosenClass : string.Empty,
                    ManagedClasses = (SelectedRole == Role.Lecturer || SelectedRole == Role.Manager) ? chosenClass : string.Empty,
                    IsSecretary = (SelectedRole == Role.Secretary),
                    TrainingPoints = 0,
                    IsActive = true
                };

                var result = await _userService.SaveUserAsync(user);
                if (result > 0)
                {
                    await AppShell.DisplayToastAsync("Tài khoản đã được tạo thành công.");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.DisplayAlert("Lỗi", "Không thể tạo tài khoản. Có thể Email đã tồn tại hoặc Role không hợp lệ.", "Đóng");
                    }
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

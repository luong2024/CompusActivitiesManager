using CampusActivitiesManager.Models;
using CampusActivitiesManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    public partial class ProfilePageModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authService;
        private readonly ModalErrorHandler _errorHandler;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private string _avatarUrl = string.Empty;

        public ProfilePageModel(IUserService userService, IAuthenticationService authService, ModalErrorHandler errorHandler)
        {
            _userService = userService;
            _authService = authService;
            _errorHandler = errorHandler;
            Title = "Thông tin cá nhân";
        }

        [RelayCommand]
        public async Task LoadProfileAsync()
        {
            var user = _authService.CurrentUser;
            if (user != null)
            {
                FullName = user.FullName;
                PhoneNumber = user.PhoneNumber;
            }
            await Task.CompletedTask;
        }

        [RelayCommand]
        private async Task UpdateProfileAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                
                var request = new UpdateProfileRequest
                {
                    FullName = FullName,
                    PhoneNumber = PhoneNumber,
                    AvatarUrl = AvatarUrl
                };

                var result = await _userService.UpdateProfileAsync(request);
                
                if (result)
                {
                    await Shell.Current.DisplayAlert("Thành công", "C?p nh?t thông tin thành công", "OK");
                    await _authService.RefreshCurrentUserAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("L?i", "Không th? c?p nh?t thông tin", "OK");
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
    }
}

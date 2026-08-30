using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusActivitiesManager.PageModels
{
    /// <summary>
    /// ViewModel cho trang thông báo từ chối truy cập AccessDeniedPage khi người dùng không đủ quyền.
    /// </summary>
    public partial class AccessDeniedViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _reason = "Bạn không có quyền truy cập trang quản trị hoặc thực hiện tác vụ này!";

        public AccessDeniedViewModel()
        {
            Title = "Từ chối truy cập";
        }

        [RelayCommand]
        public async Task GoHome()
        {
            await Shell.Current.GoToAsync("//main");
        }
    }
}

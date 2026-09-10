using CommunityToolkit.Mvvm.ComponentModel;

namespace CampusActivitiesManager.PageModels
{
    /// <summary>
    /// Lớp cơ sở BaseViewModel triển khai INotifyPropertyChanged thông qua ObservableObject,
    /// cung cấp cơ chế SetProperty, quản lý trạng thái IsBusy và Title cho các ViewModel và Model.
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        public bool IsNotBusy => !IsBusy;
    }
}

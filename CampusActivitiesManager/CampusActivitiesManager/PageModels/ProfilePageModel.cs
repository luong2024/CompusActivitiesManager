using CampusActivitiesManager.BusinessLogic.Interfaces;

using CampusActivitiesManager.BusinessLogic.Services;

using CampusActivitiesManager.Models;



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







        [ObservableProperty]



        private string _username = string.Empty;







        [ObservableProperty]



        private string _email = string.Empty;







        [ObservableProperty]



        private string _roleDisplay = "Sinh viên";







        [ObservableProperty]



        private string _className = "D20CNTT1";







        [ObservableProperty]



        private int _trainingPoints = 0;







        private readonly IFirebaseSyncService? _firebaseSync;



        public ProfilePageModel(

            IUserService userService,

            IAuthenticationService authService,

            ModalErrorHandler errorHandler,

            IFirebaseSyncService? firebaseSync = null)

        {

            _userService = userService;

            _authService = authService;

            _errorHandler = errorHandler;

            _firebaseSync = firebaseSync;

            Title = "Thông tin cá nhân";

        }



        [RelayCommand]

        private async Task SyncFirebaseDataAsync()

        {

            if (_firebaseSync == null) return;

            try

            {

                IsBusy = true;

                string action = await Shell.Current.DisplayActionSheet(

                    "Đồng bộ Firebase Cloud ☁️",

                    "Đóng",

                    null,

                    "⬆️ Sao lưu toàn bộ lên Firebase (Backup)",

                    "⬇️ Khôi phục dữ liệu từ Firebase (Restore)",

                    "⚙️ Cấu hình URL Firebase Realtime DB");



                if (action == "⬆️ Sao lưu toàn bộ lên Firebase (Backup)")

                {

                    var users = await _userService.GetUsersListAsync();

                    var classes = await _userService.GetClassesAsync();

                    var ok1 = await _firebaseSync.PushUsersAsync(users);

                    var ok2 = await _firebaseSync.PushClassesAsync(classes);

                    if (ok1 && ok2)

                    {

                        await Shell.Current.DisplayAlert("Thành công 🎉", "Đã sao lưu toàn bộ dữ liệu người dùng và lớp học lên Firebase Cloud Database!", "Đóng");

                    }

                    else

                    {

                        await Shell.Current.DisplayAlert("Thông báo ⚠️", "Đã gửi dữ liệu tới Firebase. Vui lòng kiểm tra quyền truy cập (Rules) hoặc kết nối mạng.", "Đóng");

                    }

                }

                else if (action == "⬇️ Khôi phục dữ liệu từ Firebase (Restore)")

                {

                    var cloudUsers = await _firebaseSync.PullUsersAsync();

                    var cloudClasses = await _firebaseSync.PullClassesAsync();

                    if (cloudUsers != null && cloudUsers.Count > 0)

                    {

                        foreach (var u in cloudUsers) await _userService.SaveUserAsync(u);

                        await Shell.Current.DisplayAlert("Thành công 🎉", $"Đã khôi phục thành công {cloudUsers.Count} tài khoản từ Firebase Cloud Database!", "Đóng");

                        await LoadProfileAsync();

                    }

                    else

                    {

                        await Shell.Current.DisplayAlert("Thông báo", "Chưa có dữ liệu nào trên Firebase Cloud hoặc URL chưa chính xác.", "Đóng");

                    }

                }

                else if (action == "⚙️ Cấu hình URL Firebase Realtime DB")

                {

                    string currentUrl = _firebaseSync.DatabaseUrl;

                    string newUrl = await Shell.Current.DisplayPromptAsync(

                        "Cấu hình Firebase Realtime DB ☁️",

                        "Nhập URL cơ sở dữ liệu Firebase (ví dụ: https://your-project-default-rtdb.firebaseio.com/):",

                        "Lưu",

                        "Hủy",

                        currentUrl);



                    if (!string.IsNullOrWhiteSpace(newUrl))

                    {

                        _firebaseSync.DatabaseUrl = newUrl.Trim();

                        await Shell.Current.DisplayAlert("Đã lưu 💾", $"Đã cập nhật URL Firebase: {_firebaseSync.DatabaseUrl}", "OK");

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



        public async Task LoadProfileAsync()



        {



            var user = _authService.CurrentUser;



            if (user != null)



            {



                FullName = user.FullName;



                PhoneNumber = user.PhoneNumber;



                Username = user.Username;



                Email = user.Email;



                ClassName = user.ClassName ?? "D20CNTT1";



                TrainingPoints = user.TrainingPoints;



                RoleDisplay = user.Role switch



                {



                    Role.Admin => "Quản trị viên (Admin)",



                    Role.Manager => "Ban tổ chức / Quản lý sự kiện",



                    Role.Lecturer => "Giảng viên / Cố vấn",



                    Role.Secretary => "Bí thư chi đoàn lớp",



                    _ => "Sinh viên (User)"



                };



            }



            await Task.CompletedTask;



        }







        [RelayCommand]



        public void ToggleMenu()



        {



            if (Shell.Current != null)



            {



                Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;



            }



        }







        [RelayCommand]



        public async Task LogoutAsync()



        {



            if (Shell.Current == null) return;







            bool confirm = await Shell.Current.DisplayAlert(



                "Đăng xuất",



                "Bạn có chắc chắn muốn đăng xuất khỏi ứng dụng Campus Activities Manager?",



                "Đăng xuất",



                "Hủy");







            if (!confirm) return;







            _authService.Logout();



            await Shell.Current.GoToAsync("//login");



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



                    

                    Email = _authService.CurrentUser?.Email,



                    FullName = FullName,



                    PhoneNumber = PhoneNumber,



                    AvatarUrl = AvatarUrl



                };







                var result = await _userService.UpdateProfileAsync(request);



                if (_authService.CurrentUser != null)



                {



                    _authService.CurrentUser.FullName = FullName;



                    _authService.CurrentUser.PhoneNumber = PhoneNumber;



                    var accountUpdated = await _userService.UpdateUserAsync(_authService.CurrentUser);



                    result = result || accountUpdated;



                }



                



                if (result)



                {



                    await Shell.Current.DisplayAlert("Thành công", "Cập nhật thông tin thành công", "OK");



                    await _authService.RefreshCurrentUserAsync();



                }



                else



                {



                    await Shell.Current.DisplayAlert("Lỗi", "Không thể cập nhật thông tin", "OK");



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




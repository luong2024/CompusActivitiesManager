using CampusActivitiesManager.PageModels;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace CampusActivitiesManager.Pages
{
    public partial class QrScannerPage : ContentPage
    {
        private readonly QrScannerViewModel _viewModel;
        private CameraBarcodeReaderView? _cameraView;

        public QrScannerPage(QrScannerViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(QrScannerViewModel.IsCameraPermissionGranted))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_viewModel.IsCameraPermissionGranted)
                    {
                        StartCamera();
                    }
                    else
                    {
                        StopCamera();
                    }
                });
            }
            else if (e.PropertyName == nameof(QrScannerViewModel.IsTorchOn) && _cameraView != null)
            {
                _cameraView.IsTorchOn = _viewModel.IsTorchOn;
            }
            else if (e.PropertyName == nameof(QrScannerViewModel.SelectedCameraLocation) && _cameraView != null)
            {
                _cameraView.CameraLocation = _viewModel.SelectedCameraLocation;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // 1. Luôn ưu tiên kiểm tra/xin quyền Camera và kích hoạt máy ảnh ngay
            await _viewModel.InitializeCameraPermissionAsync();

            if (_viewModel.IsCameraPermissionGranted)
            {
                StartCamera();
            }

            // 2. Tải danh sách sự kiện song song trong nền, không làm gián đoạn máy ảnh
            _ = _viewModel.AppearingAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StopCamera();
            _viewModel.IsTorchOn = false;
            _viewModel.TorchButtonText = "⚡ Bật Flash";
        }

        private void StartCamera()
        {
            if (CameraContainer == null) return;

            StopCamera();

            try
            {
                _cameraView = new CameraBarcodeReaderView
                {
                    WidthRequest = 256,
                    HeightRequest = 256,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    CameraLocation = _viewModel.SelectedCameraLocation,
                    IsTorchOn = _viewModel.IsTorchOn,
                    IsDetecting = true,
                    Options = new BarcodeReaderOptions
                    {
                        Formats = BarcodeFormats.All,
                        AutoRotate = true,
                        Multiple = false
                    }
                };

                _cameraView.BarcodesDetected += OnBarcodesDetected;

                // Thêm CameraView vào vị trí đầu tiên (index 0) để 4 góc ngắm và nhãn hướng dẫn nổi lên trên
                CameraContainer.Children.Insert(0, _cameraView);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khởi động Camera: {ex.Message}");
            }
        }

        private void StopCamera()
        {
            if (_cameraView != null)
            {
                try
                {
                    _cameraView.IsDetecting = false;
                    _cameraView.BarcodesDetected -= OnBarcodesDetected;
                    if (CameraContainer != null && CameraContainer.Children.Contains(_cameraView))
                    {
                        CameraContainer.Children.Remove(_cameraView);
                    }
                }
                catch { }
                _cameraView = null;
            }
        }

        private void OnBarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
        {
            var firstBarcode = e.Results?.FirstOrDefault()?.Value;
            if (!string.IsNullOrWhiteSpace(firstBarcode))
            {
                Dispatcher.Dispatch(async () =>
                {
                    await _viewModel.ProcessScannedBarcodeAsync(firstBarcode);
                });
            }
        }
    }
}

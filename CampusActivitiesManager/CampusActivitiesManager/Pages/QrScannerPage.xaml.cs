using CampusActivitiesManager.PageModels;
using ZXing.Net.Maui;

namespace CampusActivitiesManager.Pages
{
    public partial class QrScannerPage : ContentPage
    {
        private readonly QrScannerViewModel _viewModel;

        public QrScannerPage(QrScannerViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.AppearingAsync();
            await _viewModel.InitializeCameraPermissionAsync();
            if (BarcodeReader != null && _viewModel.IsCameraPermissionGranted)
            {
                BarcodeReader.IsDetecting = true;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (BarcodeReader != null)
            {
                BarcodeReader.IsDetecting = false;
            }
            _viewModel.IsTorchOn = false;
            _viewModel.TorchButtonText = "⚡ Bật Flash";
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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PatternMatchingTool.Data;

namespace PatternMatchingTool.ViewModel
{
    public partial class AutoViewVM : ObservableObject
    {
        [ObservableProperty]
        private Bitmap displayBitmap;

        [ObservableProperty]
        private bool isCameraConnected = false;

        [ObservableProperty]
        private bool isServerConnected = false;

        public AutoViewVM()
        {

        }

        [RelayCommand]
        private void Start()
        {
            var pDocument = Document.GetDocument;

            pDocument.SetRunMode(Define.RunMode.RUN_MODE_IDLE);
        }

        [RelayCommand]
        private void Stop()
        {
            var pDocument = Document.GetDocument;

            pDocument.SetRunMode(Define.RunMode.RUN_MODE_STOP);
        }

        private void OnFrameReceived(Bitmap bmp)
        {
            DisplayBitmap = bmp;
        }


        [RelayCommand]
        //임시 트리거 테스트용
        private void Trigger()
        {
            var pDocument = Document.GetDocument;
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.Trigger();
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvCamCtrl.NET;
using PatternMatchingTool.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace PatternMatchingTool.ViewModel
{
    public partial class SettingCameraPageVM : ObservableObject
    {
        private Config.CameraParameter m_objCameraParameter;

        [ObservableProperty]
        private List<string> cameraList;

        [ObservableProperty]
        private int selectedCameraIndex = -1;

        [ObservableProperty]
        private float exposureTime = 0;

        [ObservableProperty]
        private float gain = 0;

        [ObservableProperty]
        private float frameRate = 0;

        [ObservableProperty]
        private bool isTriggerMode = false;

        [ObservableProperty]
        private bool isCameraOpen = false;

        [ObservableProperty]
        private string cameraMode = "Continuous";

        //[ObservableProperty]
        //private ICogImage displayImage;

        [ObservableProperty]
        private Bitmap displayBitmap;

        public SettingCameraPageVM()
        {
            var pDocument = Document.GetDocument;

        }


        [RelayCommand]
        private void SearchCamera()
        {
            var pDocument = Document.GetDocument;

            CameraList = new List<string>();
            CameraList = pDocument.m_objProcessMain.m_objProcessCameraManager.SearchDevice();

            m_objCameraParameter = new Config.CameraParameter();
            m_objCameraParameter = pDocument.m_objConfig.GetCameraParameter();
        }

        [RelayCommand]
        private void OpenCamera()
        {
            var pDocument = Document.GetDocument;

            IsCameraOpen = pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OpenDevice(SelectedCameraIndex);

            if(false == IsCameraOpen)
            {
                return;
            }

            SetCameraMode(IsTriggerMode);
            GetParameters();

            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OnFrameReceived += OnFrameReceived;
        }

        [RelayCommand]
        private void CloseCamera()
        {
            var pDocument = Document.GetDocument;
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.CloseDevice();
            IsCameraOpen = false;
        }

        [RelayCommand]
        private void StartGrab()
        {
            var pDocument = Document.GetDocument;
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.StartGrab();
        }

        [RelayCommand]
        private void StopGrab()
        {
            var pDocument = Document.GetDocument;
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.StopGrab();
        }

        [RelayCommand]
        private void SetCameraMode(bool bMode)
        {
            if (true == bMode)
            {
                var pDocument = Document.GetDocument;
                pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.SetTriggerMode(true);
                CameraMode = "Trigger";
            }
            else
            {
                var pDocument = Document.GetDocument;
                pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.SetTriggerMode(false);
                CameraMode = "Continous";
            }
        }

        [RelayCommand]
        private void TriggerOnce()
        {
            var pDocument = Document.GetDocument;
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.Trigger();
        }

        [RelayCommand]
        private void GetParameters()
        {
            var pDocument = Document.GetDocument;
            ExposureTime = pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.GetExposureTime();
            Gain = pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.GetGain();
            FrameRate = pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.GetFrameRate();
        }

        [RelayCommand]
        private void SetParameters()
        {
            var pDocument = Document.GetDocument;
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.SetExposureTime(ExposureTime);
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.SetGain(Gain);
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.SetFrameRate(FrameRate);
        }

        [RelayCommand]
        private void SaveCameraParameters()
        {
            var pDocument = Document.GetDocument;
            MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(pDocument.m_objProcessMain.m_objProcessCameraManager.m_stDeviceList.pDeviceInfo[SelectedCameraIndex], typeof(MyCamera.MV_CC_DEVICE_INFO));


            // CameraIP, SerialNumber 추출 (GigE 카메라)
            if (MyCamera.MV_GIGE_DEVICE == device.nTLayerType)
            {
                MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                m_objCameraParameter.iCameraIP = (int)gigeInfo.nCurrentIp;
                m_objCameraParameter.strCameraSerialNumber = gigeInfo.chSerialNumber;
            }

            m_objCameraParameter.fExposureTime = ExposureTime;
            m_objCameraParameter.fGain = Gain;
            m_objCameraParameter.fFrameRate = FrameRate;

            pDocument.m_objConfig.SaveCameraParameter(m_objCameraParameter);
        }
        partial void OnIsTriggerModeChanged(bool value)
        {
            SetCameraMode(value);
        }

        private void OnFrameReceived(Bitmap bmp)
        {
            DisplayBitmap = bmp;

            //=============================================================================
            //DisplayImage = new CogImage8Grey(bmp);
            //var hBitmap = bmp.GetHbitmap();
            //try
            //{
            //    var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //    bitmapSource.Freeze();  // 스레드 안전 처리
            //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        DisplayBitmap = bitmapSource;
            //    });

            //}
            //finally
            //{
            //    Gdi32.DeleteObject(hBitmap);
            //    bmp.Dispose();
            //}
            //=============================================================================
        }
    }
}

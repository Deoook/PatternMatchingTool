using Cognex.VisionPro;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvCamCtrl.NET;
using PatternMatchingTool.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PatternMatchingTool.ViewModel
{
    public partial class SettingCameraPageVM : ObservableObject
    {
        [ObservableProperty]
        private List<string> cameraList;

        [ObservableProperty]
        private int selectedCameraIndex = -1;

        //[ObservableProperty]
        //private ICogImage displayImage;

        [ObservableProperty]
        private Bitmap displayBitmap;

        public SettingCameraPageVM()
        {
               
        }

        [RelayCommand]
        private void SearchCamera()
        {
            var pDocument = Document.GetDocument;

            CameraList = new List<string>();
            CameraList = pDocument.m_objProcessMain.m_objProcessCameraManager.SearchDevice();
        }

        [RelayCommand]
        private void OpenCamera()
        {
            var pDocument = Document.GetDocument;

            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OpenDevice(selectedCameraIndex);
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OnFrameReceived += OnFrameReceived;
        }

        [RelayCommand]
        private void CloseCamera()
        {
            var pDocument = Document.GetDocument;
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.CloseDevice();
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


        private BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            var hBitmap = bitmap.GetHbitmap();
                bitmap.Dispose();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            }
            finally
            {
                Gdi32.DeleteObject(hBitmap);
            }
        }
    }

    public static class Gdi32
    {
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}

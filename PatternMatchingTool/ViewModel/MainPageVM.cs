using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PatternMatchingTool.Data;
using System.Windows;

namespace PatternMatchingTool.ViewModel
{
    public partial class MainViewVM : ObservableObject
    {
        [ObservableProperty]
        private string time = "";

        private System.Windows.Forms.Timer timerWatch;

        public MainViewVM()
        {
            //TODO Return False 면 Error 처리 후 프로그램 종료 필요
            Initialize();
        }

        public bool Initialize()
        {
            var pDocument = Document.GetDocument;
            bool bReturn = false;
            do
            {
                if (false == pDocument.Initialize())
                    break;

                timerWatch = new System.Windows.Forms.Timer();
                timerWatch.Tick += TimerWatch_Tick;
                timerWatch.Start();

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public void Deinitiailize()
        {
            var pDocument = Document.GetDocument;
            pDocument.Deinitialize();
        }

        [RelayCommand]
        private void DragMove(Window window)
        {
            window?.DragMove();
        }

        [RelayCommand]
        private void Minimize(Window window)
        {
            if (window != null)
                window.WindowState = WindowState.Minimized;
        }

        [RelayCommand]
        private void Maximize(Window window)
        {
            if (window != null)
            {
                window.WindowState = window.WindowState == WindowState.Normal
                                     ? WindowState.Maximized
                                     : WindowState.Normal;
            }
        }

        [RelayCommand]
        private void ExitProgram()
        {
            Deinitiailize();
            Environment.Exit(0);
        }

        private void TimerWatch_Tick(object sender, EventArgs e)
        {
            Time = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}

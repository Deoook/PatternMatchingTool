using PatternMatchingTool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PatternMatchingTool.View
{
    /// <summary>
    /// SettingPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingPage : Page
    {
        private bool IsOpened = false;
        public SettingPage()
        {
            InitializeComponent();
            var pDocument = Document.GetDocument;
            this.DataContext = pDocument.SettingVM;
            Loaded += SettingPage_Loaded;
        }

        private void SettingPage_Loaded(object sender, RoutedEventArgs e)
        {
            //처음 Load 될 때만 카메라 설정 페이지로 이동
            if (IsOpened == true)
                return;

            SettingNavigationView.Navigate(typeof(View.SettingCameraPage));
            IsOpened = true;
        }
    }
}

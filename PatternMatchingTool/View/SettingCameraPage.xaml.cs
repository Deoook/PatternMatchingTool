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
    /// SettingCameraPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingCameraPage : Page
    {
        private bool IsOpened = false;
        public SettingCameraPage()
        {
            InitializeComponent();
            var pDocument = Document.GetDocument;
            this.DataContext = pDocument.SettingCameraVM;
            Loaded += SettingCameraPage_Loaded;
        }

        private void SettingCameraPage_Loaded(object sender, RoutedEventArgs e)
        {
            //처음 Load 될 때만 카메라 검색 실행
            if (IsOpened == true)
                return;

            var vm = DataContext as ViewModel.SettingCameraPageVM;
            vm.SearchCameraCommand.Execute(null);
            IsOpened = true;
        }
    }
}

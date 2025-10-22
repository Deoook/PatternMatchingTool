using Cognex.VisionPro;
using PatternMatchingTool.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// SettingPatternPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingPatternPage : Page
    {
        public SettingPatternPage()
        {
            InitializeComponent();
            var pDocument = Document.GetDocument;
            this.DataContext = pDocument.SettingPatternVM;
            this.Loaded += SettingPatternPage_Loaded;
        }

        private void SettingPatternPage_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"CogDisplay1 is null? {CogDisplay1 == null}");
            Debug.WriteLine($"FormsHost Size: {formsHost.ActualWidth} x {formsHost.ActualHeight}");

            try
            {
                CogDisplay1.AutoFit = true;
                CogDisplay1.BackColor = System.Drawing.Color.DarkGray;

                // 테스트용 더미 이미지 생성
                CreateTestImage();

                CogDisplay1.Refresh();
                Debug.WriteLine("CogDisplay initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CogDisplay Error: {ex.Message}");
            }
        }

        private void CreateTestImage()
        {
            // 640x480 회색조 테스트 이미지 생성
            CogImage8Grey testImage = new CogImage8Grey();


            // CogDisplay에 표시
            CogDisplay1.Image = testImage;
            CogDisplay1.Fit(true);
        }
    }
}
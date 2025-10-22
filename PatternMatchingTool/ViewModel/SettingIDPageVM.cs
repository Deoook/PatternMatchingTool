using Cognex.VisionPro;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PatternMatchingTool.Data;
using PatternMatchingTool.Model;
using PatternMatchingTool.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PatternMatchingTool.Config;

namespace PatternMatchingTool.ViewModel
{
    public partial class SettingIDPageVM : ObservableObject
    {
        private Config.RecipeParameter m_objRecipeParameter;

        [ObservableProperty]
        private bool isTryHarder = false;

        [ObservableProperty]
        private bool isTryInverted = false;

        [ObservableProperty]
        private bool isTryAutoRotae = false;


        [ObservableProperty]
        private string barcodeID = string.Empty;

        [ObservableProperty]
        private Rectangle barcodeRect = new Rectangle();

        public SettingIDPageVM()
        {
            var pDocument = Document.GetDocument;
            m_objRecipeParameter = new Config.RecipeParameter();
            m_objRecipeParameter = pDocument.m_objConfig.GetRecipeParameter();

            IsTryHarder = m_objRecipeParameter.bTryHarder;
            IsTryInverted = m_objRecipeParameter.bTryInverted;
            IsTryAutoRotae = m_objRecipeParameter.bTryAutoRotae;
        }

        [ObservableProperty]
        private Bitmap displayBitmap;

        [RelayCommand]
        private void LoadImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                Bitmap bmp = new Bitmap(dlg.FileName);
                DisplayBitmap = bmp;
                bmp.Dispose();
            }
        }

        /// <summary>
        /// 바로 이전에 촬영했던 이미지 가져오기
        /// </summary>
        [RelayCommand]
        private void GrabImage()
        {
            var pDocument = Document.GetDocument;
            DisplayBitmap = pDocument.AutoVM.DisplayBitmap;
        }

        /// <summary>
        /// 이미지 새로 촬영 후에 가져오기
        /// </summary>
        [RelayCommand]
        private void GrabCamera()
        {
            var pDocument = Document.GetDocument;
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OnFrameReceived += OnFrameReceived;

            // 카메라 열고 Trigger 날리기
            pDocument.m_objProcessMain.m_objProcessCameraManager.Start();
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.Trigger();

            // 찍는 동안 200ms Delay
            Thread.Sleep(200);

            // 찍은 후 카메라 닫기
            pDocument.m_objProcessMain.m_objProcessCameraManager.Stop();
        }

        private void OnFrameReceived(Bitmap bmp)
        {
            var pDocument = Document.GetDocument;
            DisplayBitmap = bmp;
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OnFrameReceived -= OnFrameReceived;
        }

        [RelayCommand]
        private void TestRun()
        {
            ProcessID objProcessID = new ProcessID();
            OutputID output = objProcessID.GetID(DisplayBitmap, isTryHarder, IsTryInverted, IsTryAutoRotae);
            BarcodeID = output.strBarcode;

            // 찾은 바코드 Rect가 있을 경우
            if(0 < output.objRect.Width)
            {
                BarcodeRect = new Rectangle();
                BarcodeRect = output.objRect;
            }
        }

        [RelayCommand]
        private void SaveIDParameters()
        {
            var pDocument = Document.GetDocument;

            m_objRecipeParameter.bTryHarder = IsTryHarder;
            m_objRecipeParameter.bTryInverted = IsTryInverted;
            m_objRecipeParameter.bTryAutoRotae = IsTryAutoRotae;

            pDocument.m_objConfig.SaveRecipeParameter(m_objRecipeParameter);
        }

    }
}

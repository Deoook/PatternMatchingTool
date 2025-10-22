using Cognex.VisionPro;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PatternMatchingTool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatchingTool.ViewModel
{
    public partial class SettingPatternPageVM : ObservableObject
    {
        [ObservableProperty]
        private Define.PatternType patternType = Define.PatternType.PATTERN_TYPE_ID;

        [ObservableProperty]
        private List<string> patternTypes = new List<string>
        {
            "ID", "Pattern Matching"
        };

        public SettingPatternPageVM()
        {
            if (false == Initialize())
                return;
        }

        public bool Initialize()
        {
            bool bReturn = false;
            do
            {

                bReturn = true;
            } while (false);

            return bReturn;
        }

        [RelayCommand]
        private void LoadImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                Bitmap bmp = new Bitmap(dlg.FileName);
                ICogImage img = new CogImage8Grey(bmp);
            }
        }


    }
}

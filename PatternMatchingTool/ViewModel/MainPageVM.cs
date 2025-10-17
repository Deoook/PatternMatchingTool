using PatternMatchingTool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatchingTool.ViewModel
{
    public partial class MainViewVM
    {
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

                bReturn = true;
            } while (false);

            return bReturn;
        }
    }
}

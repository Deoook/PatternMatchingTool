using PatternMatchingTool.Model;
using ZXing;

namespace PatternMatchingTool.Process
{
    public class ProcessID
    {
        BarcodeReader m_objBarcodeReader = null;
        public ProcessID()
        {
            m_objBarcodeReader = new BarcodeReader();
        }

        public OutputID GetID(Bitmap bmp, bool bTryHarder = false, bool bTryInverted = false, bool bAutoRotate = false)
        {
            var output = new OutputID();

            do
            {
                m_objBarcodeReader.Options.TryHarder = bTryHarder;
                m_objBarcodeReader.Options.TryInverted = bTryInverted;
                m_objBarcodeReader.AutoRotate = bAutoRotate;

                // UTF-8로 고정
                m_objBarcodeReader.Options.CharacterSet = "UTF-8";

                var result = m_objBarcodeReader.Decode(bmp);

                // 검색한게 없으면 Break.
                if (null == result)
                    break;

                output.strBarcode = result.Text;

                var pts = result.ResultPoints;

                if (pts != null && pts.Length >= 2)
                {
                    int minX = (int)pts.Min(p => p.X);
                    int maxX = (int)pts.Max(p => p.X);
                    int minY = (int)pts.Min(p => p.Y);
                    int maxY = (int)pts.Max(p => p.Y);

                    output.objRect = new System.Drawing.Rectangle(
                        minX,
                        minY,
                        maxX - minX,
                        maxY - minY
                    );
                }

            } while (false);

            return output;
        }
    }
}

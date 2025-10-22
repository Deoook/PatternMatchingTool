using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatchingTool.Model
{
    public class OutputID
    {
        public string strBarcode;
        public Rectangle objRect;

        public OutputID()
        {
            strBarcode = string.Empty;
            objRect = new Rectangle();
        }
    }
}

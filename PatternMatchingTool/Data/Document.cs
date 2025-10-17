using PatternMatchingTool.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PatternMatchingTool.Data
{
    public class Document
    {
        private static Document m_objDocument = null;
        public ProcessMain m_objProcessMain = null;
        public Document()
        {
            
        }

        ~Document()
        {

        }

        public static Document GetDocument
        {
            get
            {
                if (null == m_objDocument)
                {
                    m_objDocument = new Document();
                }
                return m_objDocument;
            }
        }

        public bool Initialize()
        {
            bool bReturn = false;
            do
            {
                m_objProcessMain = new ProcessMain();
                if (false == m_objProcessMain.Initialize())
                    break;

                bReturn = true;
            } while (false);
            return bReturn;
        }
    }
}

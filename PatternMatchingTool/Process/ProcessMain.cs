using PatternMatchingTool.Device.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatchingTool.Process
{
    public class ProcessMain
    {
        public ProcessCameraManager m_objProcessCameraManager = null;

        public ProcessMain()
        {
        }

        public bool Initialize()
        {
            bool bReturn = false;

            do
            {
                m_objProcessCameraManager = new ProcessCameraManager();
                if (false == m_objProcessCameraManager.Initialize())
                    break;

                bReturn = true;
            } while (false);
            return bReturn; 
        }
    }
}

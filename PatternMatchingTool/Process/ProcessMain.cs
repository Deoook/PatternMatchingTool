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
        public ProcessServerManager m_objServerManager = null;
        public ProcessManager m_objProcessManager = null;

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

                m_objServerManager = new ProcessServerManager();
                if(false == m_objServerManager.Initialize())
                    break;

                m_objProcessManager = new ProcessManager();
                if (false == m_objProcessManager.Initialize())
                    break;

                bReturn = true;
            } while (false);
            return bReturn; 
        }

        public void Deinitialize()
        {
            m_objProcessCameraManager.Deinitialize();
            m_objProcessManager.Deinitialize();
        }
    }
}

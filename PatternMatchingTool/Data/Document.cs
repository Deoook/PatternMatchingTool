using PatternMatchingTool.Process;
using PatternMatchingTool.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PatternMatchingTool.Data
{
    public class Document
    {
        public AutoViewVM AutoVM = null;
        public SettingPageVM SettingVM = null;
        public SettingCameraPageVM SettingCameraVM = null;
        public SettingPatternPageVM SettingPatternVM = null;
        public ConfigPageVM ConfigVM = null;

        private static Document m_objDocument = null;
        public ProcessMain m_objProcessMain = null;
        public Config m_objConfig = null;
        public Define.RunMode eRunMode;
        public Define.Trigger eTrigger;

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
                //Page View Model Initialize
                if (false == InitializeVM())
                    break;

                //Config Initialize
                m_objConfig = new Config();
                if (false == m_objConfig.Initialize())
                    break;



                //Process Initialize
                m_objProcessMain = new ProcessMain();
                if (false == m_objProcessMain.Initialize())
                    break;

                //초기엔 정지 상태로
                eRunMode = Define.RunMode.RUN_MODE_STOP;
                eTrigger = Define.Trigger.TRIGGER_OFF;

                bReturn = true;
            } while (false);
            return bReturn;
        }

        public bool InitializeVM()
        {
            bool bReturn = false;
            do
            {
                AutoVM = new AutoViewVM();
                SettingVM = new SettingPageVM();
                SettingCameraVM = new SettingCameraPageVM();
                SettingPatternVM = new SettingPatternPageVM();
                ConfigVM = new ConfigPageVM();

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public void Deinitialize()
        {
            m_objProcessMain.Deinitialize();
        }

        public Define.RunMode GetRunMode()
        {
            return eRunMode;
        }

        public void SetRunMode(Define.RunMode mode)
        {
            eRunMode = mode;
        }

        public Define.Trigger GetTrigger()
        {
            return eTrigger;
        }

        public void SetTrigger(Define.Trigger trigger)
        {
            eTrigger = trigger;
        }
    }
}

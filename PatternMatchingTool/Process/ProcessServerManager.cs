using PatternMatchingTool.Data;
using PatternMatchingTool.Device.Communication;

namespace PatternMatchingTool.Process
{
    public class ProcessServerManager
    {
        public DeviceCommunication m_objDeviceCommunication;

        public ProcessServerManager()
        {
        }

        public bool Initialize()
        {
            bool bReturn = false;
            do
            {
                string ip = "192.168.1.20";
                int port = 5050;

                m_objDeviceCommunication = new DeviceCommunication(new DeviceCommunicationTCPClient(ip, port, true) as DeviceCommunicationAbstract);
                m_objDeviceCommunication.DataReceived += DataReceived;

                Connect();

                bReturn = true;
            } while (false);
            return bReturn;
        }

        private void DataReceived(object? sender, string e)
        {
            var pDocument = Document.GetDocument;

            do
            {
                Console.WriteLine($"Data Received: {e}");

                // 현재 검사 준비 완료 아니면 Break.
                if (Define.RunMode.RUN_MODE_RUNNING != pDocument.GetRunMode())
                    break;

                if (e == "1")
                {
                    //그냥 찍기만
                    pDocument.SetTrigger(Define.Trigger.TRIGGER_ON);
                }
                else if (e == "2")
                {
                    pDocument.SetTrigger(Define.Trigger.TRIGGER_ID);
                }
                else if (e == "3")
                {
                    pDocument.SetTrigger(Define.Trigger.TRIGGER_PATTERN_MATCHING);
                }
            } while (false);
        }

        private void Connect()
        {
            var pDocument = Document.GetDocument;
            pDocument.AutoVM.IsServerConnected = m_objDeviceCommunication.connect();
        }
    }
}

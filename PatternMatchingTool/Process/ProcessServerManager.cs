using PatternMatchingTool.Communication;
using PatternMatchingTool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //수신 데이터 처리
            Console.WriteLine($"Data Received: {e}");

            if (e == "1")
            {
                //그냥 찍기만
                pDocument.SetTrigger(Define.Trigger.TRIGGER_ON);
            }
            else if (e == "2") 
            {
                pDocument.SetTrigger(Define.Trigger.TRIGGER_ID);
            }
            else if(e == "3")
            {
                pDocument.SetTrigger(Define.Trigger.TRIGGER_PATTERN_MATCHING);
            }
        }

        private void Connect()
        {
            var pDocument = Document.GetDocument;
            pDocument.AutoVM.IsServerConnected = m_objDeviceCommunication.connect();
        }
    }
}

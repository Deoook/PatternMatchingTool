using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatchingTool.Communication
{
    public abstract class DeviceCommunicationAbstract
    {
        public abstract event EventHandler<string> DataReceived;
        public abstract event EventHandler Connected;
        public abstract event EventHandler Disconnected;
        public abstract event EventHandler<string> ErrorOccurred;

        public abstract bool Connect();
        public abstract void Disconnect();
        public abstract bool IsConnected();

        public abstract bool SendData(string strData);
    }
}

namespace PatternMatchingTool.Device.Communication
{
    public class DeviceCommunication
    {
        private DeviceCommunicationAbstract m_objDeviceCommunication;

        public DeviceCommunication(DeviceCommunicationAbstract objDeviceCommunication)
        {
            m_objDeviceCommunication = objDeviceCommunication;
        }
        public event EventHandler<string> DataReceived
        {
            add { m_objDeviceCommunication.DataReceived += value; }
            remove { m_objDeviceCommunication.DataReceived -= value; }
        }

        public event EventHandler Connected
        {
            add { m_objDeviceCommunication.Connected += value; }
            remove { m_objDeviceCommunication.Connected -= value; }
        }

        public event EventHandler Disconnected
        {
            add { m_objDeviceCommunication.Disconnected += value; }
            remove { m_objDeviceCommunication.Disconnected -= value; }
        }

        public event EventHandler<string> ErrorOccurred
        {
            add { m_objDeviceCommunication.ErrorOccurred += value; }
            remove { m_objDeviceCommunication.ErrorOccurred -= value; }
        }

        public bool connect()
        {
            return m_objDeviceCommunication.Connect();
        }

        public void disconnect()
        {
            m_objDeviceCommunication.Disconnect();
        }

        public bool isConnected()
        {
            return m_objDeviceCommunication.IsConnected();
        }

        public bool sendData(string strData)
        {
            return m_objDeviceCommunication.SendData(strData);
        }
    }
}

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatternMatchingTool.Communication
{
    public class DeviceCommunicationTCPClient : DeviceCommunicationAbstract
    {
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private string _ipAddress;
        private int _port;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _autoReconnect = false;
        private int _reconnectDelay = 3000; // 3초

        public override event EventHandler<string> DataReceived;
        public override event EventHandler Connected;
        public override event EventHandler Disconnected;
        public override event EventHandler<string> ErrorOccurred;

        public bool AutoReconnect
        {
            get => _autoReconnect;
            set => _autoReconnect = value;
        }

        public int ReconnectDelay
        {
            get => _reconnectDelay;
            set => _reconnectDelay = value;
        }

        public DeviceCommunicationTCPClient(string ipAddress, int port, bool autoReconnect = false)
        {
            _ipAddress = ipAddress;
            _port = port;
            _autoReconnect = autoReconnect;
        }

        public override bool Connect()
        {
            try
            {
                if (IsConnected())
                {
                    return true;
                }

                _tcpClient = new TcpClient();
                _tcpClient.ReceiveTimeout = 5000;
                _tcpClient.SendTimeout = 5000;

                var result = _tcpClient.BeginConnect(_ipAddress, _port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));

                if (!success)
                {
                    throw new Exception("연결 시간 초과");
                }

                _tcpClient.EndConnect(result);
                _networkStream = _tcpClient.GetStream();

                _cancellationTokenSource = new CancellationTokenSource();
                Task.Run(() => ReceiveDataAsync(_cancellationTokenSource.Token));

                Connected?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"연결 실패: {ex.Message}");

                if (_autoReconnect)
                {
                    Task.Delay(_reconnectDelay).ContinueWith(_ => Connect());
                }

                return false;
            }
        }

        public override void Disconnect()
        {
            try
            {
                _autoReconnect = false;
                _cancellationTokenSource?.Cancel();

                _networkStream?.Close();
                _networkStream?.Dispose();

                _tcpClient?.Close();
                _tcpClient?.Dispose();

                _networkStream = null;
                _tcpClient = null;

                Disconnected?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"연결 해제 실패: {ex.Message}");
            }
        }

        public override bool IsConnected()
        {
            try
            {
                if (_tcpClient == null || !_tcpClient.Connected)
                    return false;

                // 실제 연결 상태 확인
                if (_tcpClient.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buff = new byte[1];
                    if (_tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool SendData(string strData)
        {
            try
            {
                if (!IsConnected())
                {
                    ErrorOccurred?.Invoke(this, "연결되지 않았습니다.");
                    return false;
                }

                byte[] data = Encoding.UTF8.GetBytes(strData);
                _networkStream.Write(data, 0, data.Length);
                _networkStream.Flush();

                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"데이터 전송 실패: {ex.Message}");

                if (_autoReconnect)
                {
                    Disconnect();
                    Task.Delay(_reconnectDelay).ContinueWith(_ => Connect());
                }

                return false;
            }
        }

        public bool SendDataWithTerminator(string strData, string terminator = "\r\n")
        {
            return SendData(strData + terminator);
        }

        private async Task ReceiveDataAsync(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[4096];

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!IsConnected())
                    {
                        if (_autoReconnect)
                        {
                            await Task.Delay(_reconnectDelay, cancellationToken);
                            Connect();
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (_networkStream != null && _networkStream.DataAvailable)
                    {
                        int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                        if (bytesRead > 0)
                        {
                            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            DataReceived?.Invoke(this, receivedData);
                        }
                        else
                        {
                            // 연결 끊김
                            Disconnect();

                            if (_autoReconnect)
                            {
                                await Task.Delay(_reconnectDelay, cancellationToken);
                                Connect();
                            }
                        }
                    }

                    await Task.Delay(10, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // 정상 종료
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"수신 오류: {ex.Message}");

                if (_autoReconnect && !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(_reconnectDelay, cancellationToken);
                    Connect();
                }
            }
        }
    }
}
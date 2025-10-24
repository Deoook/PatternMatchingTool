using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatternMatchingTool.Device.Communication
{
    public class DeviceCommunicationTCPServer : DeviceCommunicationAbstract
    {
        private TcpListener _tcpListener;
        private List<ClientConnection> _clients;
        private CancellationTokenSource _cancellationTokenSource;
        private int _port;
        private bool _isRunning = false;

        public override event EventHandler<string> DataReceived;
        public override event EventHandler<string> ErrorOccurred;
        public override event EventHandler Connected;
        public override event EventHandler Disconnected;

        public int ConnectedClientCount => _clients?.Count ?? 0;

        public DeviceCommunicationTCPServer(int port)
        {
            _port = port;
            _clients = new List<ClientConnection>();
        }

        public override bool Connect()
        {
            try
            {
                if (_isRunning)
                {
                    return true;
                }

                _tcpListener = new TcpListener(IPAddress.Any, _port);
                _tcpListener.Start();

                _isRunning = true;
                _cancellationTokenSource = new CancellationTokenSource();

                // 클라이언트 접속 대기 시작
                Task.Run(() => AcceptClientsAsync(_cancellationTokenSource.Token));


                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"서버 시작 실패: {ex.Message}");
                return false;
            }
        }

        public override void Disconnect()
        {
            try
            {
                _isRunning = false;
                _cancellationTokenSource?.Cancel();

                // 모든 클라이언트 연결 종료
                lock (_clients)
                {
                    foreach (var client in _clients.ToList())
                    {
                        DisconnectClient(client);
                    }
                    _clients.Clear();
                }

                _tcpListener?.Stop();
                _tcpListener = null;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"서버 종료 실패: {ex.Message}");
            }
        }

        public override bool IsConnected()
        {
            return _isRunning;
        }

        public override bool SendData(string strData)
        {
            // 모든 연결된 클라이언트에게 전송
            return SendDataToAll(strData);
        }

        public bool SendDataToAll(string strData)
        {
            bool allSuccess = true;

            lock (_clients)
            {
                foreach (var client in _clients.ToList())
                {
                    if (!SendDataToClient(client, strData))
                    {
                        allSuccess = false;
                    }
                }
            }

            return allSuccess;
        }

        public bool SendDataToClient(string clientId, string strData)
        {
            lock (_clients)
            {
                var client = _clients.FirstOrDefault(c => c.Id == clientId);
                if (client != null)
                {
                    return SendDataToClient(client, strData);
                }
            }

            return false;
        }

        private bool SendDataToClient(ClientConnection client, string strData)
        {
            try
            {
                if (client.Stream == null || !client.Stream.CanWrite)
                {
                    return false;
                }

                byte[] data = Encoding.UTF8.GetBytes(strData);
                client.Stream.Write(data, 0, data.Length);
                client.Stream.Flush();

                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"데이터 전송 실패 [{client.Id}]: {ex.Message}");
                DisconnectClient(client);
                return false;
            }
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && _isRunning)
                {
                    if (_tcpListener.Pending())
                    {
                        var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                        var clientConnection = new ClientConnection
                        {
                            Id = Guid.NewGuid().ToString(),
                            TcpClient = tcpClient,
                            Stream = tcpClient.GetStream(),
                            ConnectedTime = DateTime.Now
                        };

                        lock (_clients)
                        {
                            _clients.Add(clientConnection);
                        }

                        var endpoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                        string clientInfo = $"{clientConnection.Id} ({endpoint?.Address}:{endpoint?.Port})";

                        // 이거 String으로 바꿔서 보내야하나?
                        Connected?.Invoke(this, EventArgs.Empty);

                        // 클라이언트 데이터 수신 시작
                        Task.Run(() => ReceiveDataFromClientAsync(clientConnection, cancellationToken));
                    }

                    await Task.Delay(100, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // 정상 종료
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"클라이언트 수락 오류: {ex.Message}");
            }
        }

        private async Task ReceiveDataFromClientAsync(ClientConnection client, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[4096];

            try
            {
                while (!cancellationToken.IsCancellationRequested && _isRunning)
                {
                    if (!IsClientConnected(client))
                    {
                        break;
                    }

                    if (client.Stream.DataAvailable)
                    {
                        int bytesRead = await client.Stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                        if (bytesRead > 0)
                        {
                            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            DataReceived?.Invoke(this, $"[{client.Id}] {receivedData}");
                        }
                        else
                        {
                            // 연결 끊김
                            break;
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
                ErrorOccurred?.Invoke(this, $"수신 오류 [{client.Id}]: {ex.Message}");
            }
            finally
            {
                DisconnectClient(client);
            }
        }

        private bool IsClientConnected(ClientConnection client)
        {
            try
            {
                if (client.TcpClient == null || !client.TcpClient.Connected)
                    return false;

                if (client.TcpClient.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buff = new byte[1];
                    if (client.TcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
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

        private void DisconnectClient(ClientConnection client)
        {
            try
            {
                lock (_clients)
                {
                    if (_clients.Contains(client))
                    {
                        _clients.Remove(client);

                        client.Stream?.Close();
                        client.Stream?.Dispose();

                        client.TcpClient?.Close();
                        client.TcpClient?.Dispose();

                        Disconnected?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"클라이언트 연결 해제 실패 [{client.Id}]: {ex.Message}");
            }
        }

        public List<string> GetConnectedClientIds()
        {
            lock (_clients)
            {
                return _clients.Select(c => c.Id).ToList();
            }
        }

        private class ClientConnection
        {
            public string Id { get; set; }
            public TcpClient TcpClient { get; set; }
            public NetworkStream Stream { get; set; }
            public DateTime ConnectedTime { get; set; }
        }
    }
}
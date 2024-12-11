using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using Lab5.Network.Common;

public class NetTcpClient : IDisposable
{
    private TcpClient? _tcpClient;
    private readonly IPAddress _serverIp;
    private readonly int _serverPort;

    public NetTcpClient(Uri serverAddress)
    {
        ServerAddress = serverAddress;

        _serverIp = IPAddress.Parse(serverAddress.Host);
        _serverPort = serverAddress.Port;
    }

    public async Task ConnectAsync() 
    {
        _tcpClient = new TcpClient();
        await _tcpClient.ConnectAsync(_serverIp, _serverPort);
    }

    public async Task<Command?> SendAsync(Command command)
    {
        var stream = _tcpClient!.GetStream();

        var dataBytes = command.SerializeCommand();

        await stream.WriteAsync(dataBytes);
        
        var response = new List<byte>();
        int bytesRead = 255;
        while ((bytesRead = stream.ReadByte()) != Const.ETX)
        {
            // добавляем в буфер
            response.Add((byte)bytesRead);
        }

        var responseBytes = response.ToArray();
        
        if (responseBytes.TryDeserializeCommand(out var responseCommand)) 
        {
            return responseCommand;
        }

        return null;
    }



    public void Dispose()
    {
        _tcpClient?.Close();
        _tcpClient?.Dispose();
    }

    public Uri ServerAddress { get; }
}
using Lab5.Network.Common;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class NetUdpClient : IDisposable
{
    private readonly UdpClient _udpClient;
    private readonly IPEndPoint _endpoint;

    public NetUdpClient(Uri serverAddress)
    {
        ServerAddress = serverAddress;

        _endpoint = new IPEndPoint(IPAddress.Parse(serverAddress.Host), serverAddress.Port);
        _udpClient = new UdpClient();
    }

    public async Task SendAsync(Command command)
    {
        var dataBytes = command.SerializeCommand();
        await _udpClient.SendAsync(dataBytes, dataBytes.Length, _endpoint);
    }

    public async Task<UdpReceiveResult> ReceiveAsync()
    {
        return await _udpClient.ReceiveAsync(); // Получаем данные асинхронно
    }

    public void Dispose()
    {
        _udpClient.Close();
        _udpClient.Dispose();
    }

    public Uri ServerAddress { get; }
}

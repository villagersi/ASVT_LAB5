// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using Lab5.Network.Common;

public abstract class UdpServerBase
{
    private readonly UdpClient _udpServer;

    public UdpServerBase(Uri listenAddress)
    {
        ListenAddress = listenAddress;

        _udpServer = new UdpClient(listenAddress.Port);
    }

    public Uri ListenAddress { get; }

    public void Start()
    {

        Console.WriteLine("Сервер запущен. Ожидание запросов... ");

        var clientTask = Task.Run(ProcessClientAsync);
    }

    // обрабатываем клиент
    private async Task ProcessClientAsync()
    {
        while (true)
        {
            var result = await _udpServer.ReceiveAsync();
            // буфер для входящих данных
            var request = new List<byte>();
            byte[] data = result.Buffer;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != Const.ETX)
                {
                    // добавляем в буфер
                    request.Add(data[i]);
                }
                else
                {
                    break;
                }
            }

            request.Add(Const.ETX);

            var requestArray = request.ToArray();

            if (!requestArray.TryDeserializeCommand(out var command))
            {
                continue;
            }

            await ProcessCommandAsync(command!);
            // отправляем ответ клиенту
            //await SendOkResponseAsync((byte)command!.Code);
            //request.Clear();
        }
    }

    protected abstract Task ProcessCommandAsync(Command command);
}

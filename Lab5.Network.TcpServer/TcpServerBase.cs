// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Lab5.Network.Common;

public abstract class TcpServerBase
{
    private readonly TcpListener _tcpListener;

    public TcpServerBase(Uri listenAddress)
    {
        ListenAddress = listenAddress;

        var address = IPAddress.Parse(listenAddress.Host);
        _tcpListener = new TcpListener(address, listenAddress.Port);
    }

    public Uri ListenAddress { get; }

    public async Task StartAsync()
    {
        try
        {
            _tcpListener.Start();    // запускаем сервер
            Console.WriteLine("Сервер запущен. Ожидание подключений... ");

            while (true)
            {
                // получаем подключение в виде TcpClient
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();

                // создаем новую задачу для обслуживания нового клиента
                var clientTask = Task.Run(async () => await ProcessClientAsync(tcpClient));
                // вместо задач можно использовать стандартный Thread
                // new Thread(async ()=>await ProcessClientAsync(tcpClient)).Start();
            }
        }
        finally
        {
            _tcpListener.Stop();
        }
    }

    // обрабатываем клиент
    private async Task ProcessClientAsync(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();
        // буфер для входящих данных
        var request = new List<byte>();
        int bytesRead = 255;
        while (true)
        {
            // считываем данные до конечного символа
            while ((bytesRead = stream.ReadByte()) != Const.ETX)
            {
                // добавляем в буфер
                request.Add((byte)bytesRead);
            }

            request.Add(Const.ETX);

            var requestArray = request.ToArray();

            if (requestArray[0] == Const.EOT)
            {
                await SendOkResponseAsync(0xff, stream);
                break;
            }

            if (!requestArray.TryDeserializeCommand( out var command)) {
                await SendErrorResponseAsync(stream, "wrong message");
                break;
            }

            var responseCommand = await ProcessCommandAsync(command!);
            await SendCommandResponseAsync(responseCommand!, stream);
            // отправляем ответ клиенту
            //await SendOkResponseAsync(command!.Code, stream);
            request.Clear();
        }
        request.Clear();
        tcpClient.Close();
    }

    protected abstract Task<Command> ProcessCommandAsync(Command command);

    protected static async Task SendCommandResponseAsync(Command command, NetworkStream stream)
    {
        var bytesToResponse = command.SerializeCommand();
        // отправляем ответ клиенту
        await stream.WriteAsync(bytesToResponse);
    }

    protected static async Task SendOkResponseAsync(byte code, NetworkStream stream)
    {
        var okResponse = new Command()
        {
            Code = code,
            Arguments = new Dictionary<string, object?>()
            {
                ["Ok"] = true
            }
        };

        await SendCommandResponseAsync(okResponse, stream);
    }

    protected static async Task SendErrorResponseAsync(NetworkStream stream, string errorMessage)
    {
        var errorCommand = new Command()
        {
            Code = 0,
            Arguments = new Dictionary<string, object?>()
            {
                ["Error"] = errorMessage
            }
        };
        await SendCommandResponseAsync(errorCommand, stream);
    }
}
